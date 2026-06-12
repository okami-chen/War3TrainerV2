using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace War3Trainer
{
    public partial class MainForm : Form
    {
        private GameContext _currentGameContext;
        private GameTrainer _mainTrainer;
        private Form _abilityForm;
        private readonly Random _abilityGroupDelayRandom = new Random();
        private volatile bool _abilityGroupCommandRunning;

        public MainForm()
        {
            InitializeComponent();
            InitializeAbilityOptions();
            InitializeAbilityWindow();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.EnterDebugMode();
            }
            catch
            {
                ReportEnterDebugFailure();
                return;
            }

            FindGame();
        }

        /************************************************************************/
        /* Main functions                                                       */
        /************************************************************************/
        private void FindGame()
        {
            bool isRecognized = false;
            try
            {
                _currentGameContext = GameContext.FindGameRunning("war3", "game.dll");
                if (_currentGameContext == null)
                {
                    // netease war3 platform(dz.163.com)
                    _currentGameContext = GameContext.FindGameRunning("dzwar3", "game.dll");
                }
                if (_currentGameContext != null)
                {
                    // Game online
                    ReportVersionOk(_currentGameContext.ProcessId, _currentGameContext.ProcessVersion);

                    // Get a new trainer
                    GetAllObject();

                    isRecognized = true;
                }
                else
                {
                    // Game offline
                    ReportNoGameFoundFailure();
                }
            }
            catch (UnkonwnGameVersionExpection ex)
            {
                // Unknown game version
                _currentGameContext = null;
                ReportVersionFailure(ex.ProcessId, ex.GameVersion);
            }
            catch (WindowsApi.BadProcessIdException ex)
            {
                this._currentGameContext = null;
                ReportProcessIdFailure(ex.ProcessId);
            }
            catch (Exception ex)
            {
                // Why here?
                _currentGameContext = null;
                ReportUnknownFailure(ex.Message);
            }

            // Enable buttons
            if (isRecognized)
            {
                viewFunctions.Enabled = true;
                viewData.Enabled = true;
                grpAbilityCommand.Enabled = true;
                toolStripButton2.Enabled = true;
                toolStripButton1.Enabled = true;
            }
            else
            {
                viewFunctions.Enabled = false;
                viewData.Enabled = false;
                grpAbilityCommand.Enabled = false;
                toolStripButton2.Enabled = false;
                toolStripButton1.Enabled = false;
            }
        }

        private void GetAllObject()
        {
            // Check paramters
            if (_currentGameContext == null)
                return;

            // Get a new trainer
            _mainTrainer = new GameTrainer(_currentGameContext);

            // Create function tree
            viewFunctions.Nodes.Clear();
            foreach (ITrainerNode currentFunction in _mainTrainer.GetFunctionList())
            {
                if (currentFunction.NodeType == TrainerNodeType.Introduction)
                    continue;

                TreeNode[] parentNodes = viewFunctions.Nodes.Find(currentFunction.ParentIndex.ToString(), true);
                TreeNodeCollection parentTree;
                if (parentNodes.Length < 1)
                    parentTree = viewFunctions.Nodes;
                else
                    parentTree = parentNodes[0].Nodes;

                parentTree.Add(
                    currentFunction.NodeIndex.ToString(),
                    currentFunction.NodeTypeName)
                    .Tag = currentFunction;
            }
            viewFunctions.ExpandAll();

            // Switch to page 1
            TreeNode[] introductionNodes = viewFunctions.Nodes.Find("1", true);
            if (introductionNodes.Length > 0)
            {
                viewFunctions.SelectedNode = introductionNodes[0];
                SelectFunction(introductionNodes[0]);
            }
            UpdateNodeIcons(viewFunctions.Nodes);
        }
        private void UpdateNodeIcons(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Nodes.Count > 0)
                {
                    node.ImageIndex = 0;
                    node.SelectedImageIndex = 0;
                }
                else
                {
                    node.ImageIndex = 1;
                    node.SelectedImageIndex = 1;
                }
                if (node.Nodes.Count > 0)
                {
                    UpdateNodeIcons(node.Nodes);
                }
            }
        }
        // Re-query specific tree-node by FunctionListNode
        private void RefreshSelectedObject(ITrainerNode currentFunction)
        {
            TreeNode[] currentNodes = viewFunctions.Nodes.Find(currentFunction.NodeIndex.ToString(), true);
            TreeNode currentTree;
            if (currentNodes.Length < 1)
                return;
            else
                currentTree = currentNodes[0];

            currentTree.Text = currentFunction.NodeTypeName;
        }

        private void SelectFunction(TreeNode functionNode)
        {
            if (functionNode == null)
                return;
            ITrainerNode node = functionNode.Tag as ITrainerNode;
            if (node == null)
                return;

            FillAddressList(node.NodeIndex);
        }

        private void FillAddressList(int functionNodeId)
        {
            // To set the right window
            viewData.Items.Clear();
            foreach (IAddressNode addressLine in _mainTrainer.GetAddressList())
            {
                if (addressLine.ParentIndex != functionNodeId)
                    continue;

                viewData.Items.Add(new ListViewItem(
                    new string[]
                    {
                        addressLine.Caption,    // Caption
                        "",                     // Original value
                        ""                      // Modified value
                    }));
                viewData.Items[viewData.Items.Count - 1].Tag = addressLine;
            }

            // To get memory content
            using (WindowsApi.ProcessMemory mem = new WindowsApi.ProcessMemory(_currentGameContext.ProcessId))
            {
                foreach (ListViewItem currentItem in viewData.Items)
                {
                    IAddressNode addressLine = currentItem.Tag as IAddressNode;
                    if (addressLine == null)
                        continue;

                    Object itemValue;
                    switch (addressLine.ValueType)
                    {
                        case AddressListValueType.Integer:
                            itemValue = mem.ReadInt32((IntPtr)addressLine.Address)
                                / addressLine.ValueScale;
                            break;
                        case AddressListValueType.Float:
                            itemValue = mem.ReadFloat((IntPtr)addressLine.Address)
                                / addressLine.ValueScale;
                            break;
                        case AddressListValueType.Char4:
                            itemValue = mem.ReadChar4((IntPtr)addressLine.Address);
                            break;
                        default:
                            itemValue = "";
                            break;
                    }
                    currentItem.SubItems[1].Text = itemValue.ToString();
                    currentItem.ImageIndex = 2;
                }
            }
        }

        // To apply the modifications
        private void ApplyModify()
        {
            using (WindowsApi.ProcessMemory mem = new WindowsApi.ProcessMemory(_currentGameContext.ProcessId))
            {
                foreach (ListViewItem currentItem in viewData.Items)
                {
                    string itemValueString = currentItem.SubItems[2].Text;
                    if (String.IsNullOrEmpty(itemValueString))
                    {
                        // Not modified
                        continue;
                    }

                    IAddressNode addressLine = currentItem.Tag as IAddressNode;
                    if (addressLine == null)
                        continue;

                    switch (addressLine.ValueType)
                    {
                        case AddressListValueType.Integer:
                            Int32 intValue;
                            if (!Int32.TryParse(itemValueString, out intValue))
                                intValue = 0;
                            intValue = unchecked(intValue * addressLine.ValueScale);
                            mem.WriteInt32((IntPtr)addressLine.Address, intValue);
                            break;
                        case AddressListValueType.Float:
                            float floatValue;
                            if (!float.TryParse(itemValueString, out floatValue))
                                floatValue = 0;
                            floatValue = unchecked(floatValue * addressLine.ValueScale);
                            mem.WriteFloat((IntPtr)addressLine.Address, floatValue);
                            if ((addressLine.Caption == "HP - 回复率" || addressLine.Caption == "MP - 回复率") && floatValue != 0)
                                mem.WriteInt32((IntPtr)unchecked(addressLine.Address - 4), 1);
                            break;
                        case AddressListValueType.Char4:
                            mem.WriteChar4((IntPtr)addressLine.Address, itemValueString);
                            break;
                    }
                    currentItem.SubItems[2].Text = "";
                }
            }
        }

        /************************************************************************/
        /* Exception UI                                                         */
        /************************************************************************/
        private void ReportEnterDebugFailure()
        {
            labGameScanState.Text = "请以管理员身份运行";
        }

        private void ReportNoGameFoundFailure()
        {
            labGameScanState.Text = "游戏未运行，运行游戏后单击“查找游戏”";
        }

        private void ReportUnknownFailure(string message)
        {
            labGameScanState.Text = "发生未知错误：" + message;
        }

        private void ReportProcessIdFailure(int processId)
        {
            labGameScanState.Text = "错误的进程ID："
                + processId.ToString();
        }

        private void ReportVersionFailure(int processId, string version)
        {
            labGameScanState.Text = "游戏已运行，但版本（"
                + version
                + "）不被支持";
        }

        private void ReportVersionOk(int processId, string version)
        {
            labGameScanState.Text = "游戏已运行("
                + processId.ToString()
                + ")，版本："
                + version
                + "（支持）";
        }

        /************************************************************************/
        /* GUI                                                                  */
        /************************************************************************/
        private void MenuHelpAbout_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("Warcraft III 内存修改器"
                 + Application.ProductVersion + System.Environment.NewLine
                 + System.Environment.NewLine
                 + "暴徒修改：https://github.com/Hooliby/War3Trainer" + System.Environment.NewLine
                 + "",
                 "War3Trainer",
                 MessageBoxButtons.OKCancel,
                 MessageBoxIcon.Information);

            if (r == DialogResult.OK)
            {
                try
                {
                    Process.Start("https://github.com/Hooliby/War3Trainer");
                }
                catch { }
            }

        }

        private void MenuFileExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menuAbility_Click(object sender, EventArgs e)
        {
            ShowAbilityWindow();
        }

        private void InitializeAbilityWindow()
        {
            _abilityForm = new Form();
            _abilityForm.Text = "技能";
            _abilityForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            _abilityForm.MaximizeBox = false;
            _abilityForm.MinimizeBox = false;
            _abilityForm.ShowInTaskbar = false;
            _abilityForm.StartPosition = FormStartPosition.Manual;
            _abilityForm.ClientSize = new Size(grpAbilityCommand.Width, grpAbilityCommand.Height);
            _abilityForm.FormClosing += abilityForm_FormClosing;

            grpAbilityCommand.Location = new Point(0, 0);
            grpAbilityCommand.Dock = DockStyle.Fill;
            _abilityForm.Controls.Add(grpAbilityCommand);
        }

        private void ShowAbilityWindow()
        {
            if (_abilityForm == null || _abilityForm.IsDisposed)
                InitializeAbilityWindow();

            if (_abilityForm.Visible)
            {
                _abilityForm.Activate();
                return;
            }

            Point location = this.PointToScreen(new Point(this.Width - _abilityForm.Width - 16, toolContainer.Bottom + 16));
            _abilityForm.Location = location;
            _abilityForm.Show(this);
        }

        private void abilityForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing)
                return;

            e.Cancel = true;
            _abilityForm.Hide();
        }

        private void cmdScanGame_Click(object sender, EventArgs e)
        {
            FindGame();
        }

        private void viewFunctions_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            // Check whether modification is not saved
            bool isSaved = true;
            foreach (ListViewItem currentItem in viewData.Items)
            {
                if (!String.IsNullOrEmpty(currentItem.SubItems[2].Text))
                {
                    isSaved = false;
                    break;
                }
            }

            // Save all if not saved
            if (!isSaved)
            {
                toolStripButton1_Click(this, null);
            }

            // Select another function
            try
            {
                SelectFunction(e.Node);
            }
            catch (WindowsApi.BadProcessIdException ex)
            {
                ReportProcessIdFailure(ex.ProcessId);
            }
        }

        private enum RightFunction
        {
            Empty,
            Introduction,
            EditTable,
        }

        //////////////////////////////////////////////////////////////////////////       
        // Make the ListView editable
        private void ReplaceInputTextbox()
        {
            if (viewData.SelectedItems.Count < 1)
                return;
            ListViewItem currentItem = viewData.SelectedItems[0];

            txtInput.Location = new Point(
                viewData.Columns[0].Width + viewData.Columns[1].Width,
                viewData.Top + currentItem.Position.Y + 2);
            txtInput.Width = viewData.Columns[2].Width + 2;
        }

        private void cmdAddAbility_Click(object sender, EventArgs e)
        {
            ExecuteAbilityUiCommand(true);
        }

        private void cmdRemoveAbility_Click(object sender, EventArgs e)
        {
            ExecuteAbilityUiCommand(false);
        }

        private void cmdAddTalent_Click(object sender, EventArgs e)
        {
            ExecuteTalentUiCommand(true);
        }

        private void cmdRemoveTalent_Click(object sender, EventArgs e)
        {
            ExecuteTalentUiCommand(false);
        }

        private void cmdAddGroupAbility_Click(object sender, EventArgs e)
        {
            ExecuteAbilityGroupUiCommand("技能", true);
        }

        private void cmdRemoveGroupAbility_Click(object sender, EventArgs e)
        {
            ExecuteAbilityGroupUiCommand("技能", false);
        }

        private void cmdAddGroupTalent_Click(object sender, EventArgs e)
        {
            ExecuteAbilityGroupUiCommand("天赋", true);
        }

        private void cmdRemoveGroupTalent_Click(object sender, EventArgs e)
        {
            ExecuteAbilityGroupUiCommand("天赋", false);
        }

        private void cboAbilityGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSelectedAbilityGroupItems();
        }

        private void ExecuteAbilityUiCommand(bool addAbility)
        {
            if (_currentGameContext == null)
            {
                labAbilityCommandState.Text = "游戏未运行";
                return;
            }

            string abilityId = GetSelectedAbilityId();
            if (!IsValidAbilityId(abilityId))
            {
                labAbilityCommandState.Text = "技能ID必须是4位ASCII字符";
                cboAbilityId.Focus();
                return;
            }

            int abilityLevel = Decimal.ToInt32(numAbilityLevel.Value);
            try
            {
                int unitCount = ExecuteJassAbilityCommand(abilityId, abilityLevel, addAbility);
                labAbilityCommandState.Text = addAbility
                    ? "已添加技能到" + unitCount.ToString() + "个单位"
                    : "已删除" + unitCount.ToString() + "个单位的技能";
            }
            catch (NotSupportedException ex)
            {
                labAbilityCommandState.Text = "当前版本未配置JASS函数地址";
                MessageBox.Show(
                    ex.Message,
                    "JASS技能",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (WindowsApi.BadProcessIdException ex)
            {
                labAbilityCommandState.Text = "游戏进程不可用";
                ReportProcessIdFailure(ex.ProcessId);
            }
            catch (InvalidOperationException ex)
            {
                labAbilityCommandState.Text = ex.Message;
                MessageBox.Show(
                    ex.Message,
                    "JASS技能",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void ExecuteTalentUiCommand(bool addAbility)
        {
            if (_currentGameContext == null)
            {
                labAbilityCommandState.Text = "游戏未运行";
                return;
            }

            string abilityId = GetSelectedAbilityId();
            if (!IsValidAbilityId(abilityId))
            {
                labAbilityCommandState.Text = "技能ID必须是4位ASCII字符";
                cboAbilityId.Focus();
                return;
            }

            int abilityLevel = Decimal.ToInt32(numAbilityLevel.Value);
            try
            {
                int unitCount = ExecuteJassAbilityCommand(abilityId, abilityLevel, addAbility);
                labAbilityCommandState.Text = addAbility
                    ? "已添加天赋到" + unitCount.ToString() + "个单位"
                    : "已删除" + unitCount.ToString() + "个单位的天赋";
            }
            catch (NotSupportedException ex)
            {
                labAbilityCommandState.Text = "当前版本未配置JASS函数地址";
                MessageBox.Show(
                    ex.Message,
                    "JASS天赋",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (WindowsApi.BadProcessIdException ex)
            {
                labAbilityCommandState.Text = "游戏进程不可用";
                ReportProcessIdFailure(ex.ProcessId);
            }
            catch (InvalidOperationException ex)
            {
                labAbilityCommandState.Text = ex.Message;
                MessageBox.Show(
                    ex.Message,
                    "JASS天赋",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void ExecuteAbilityGroupUiCommand(string targetName, bool addAbility)
        {
            if (_abilityGroupCommandRunning)
            {
                labAbilityCommandState.Text = "分组命令正在执行";
                return;
            }

            if (_currentGameContext == null)
            {
                labAbilityCommandState.Text = "游戏未运行";
                return;
            }

            AbilityGroup group = GetSelectedAbilityGroup();
            if (group == null || group.Abilities.Count == 0)
            {
                labAbilityCommandState.Text = "请选择分组";
                cboAbilityGroup.Focus();
                return;
            }

            int abilityLevel = Decimal.ToInt32(numAbilityLevel.Value);
            string groupName = group.Name;
            List<AbilityOption> abilities = group.Abilities
                .Select(ability => new AbilityOption(ability.Name, ability.Id))
                .ToList();

            SetAbilityGroupCommandRunning(true);
            labAbilityCommandState.Text = addAbility
                ? "正在添加" + groupName + targetName + "..."
                : "正在删除" + groupName + targetName + "...";

            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    int unitCount = ExecuteJassAbilityGroupCommand(abilities, abilityLevel, addAbility);

                    RunOnUiThread(delegate
                    {
                        labAbilityCommandState.Text = addAbility
                            ? "已添加" + groupName + "的" + abilities.Count.ToString() + "个" + targetName + "到" + unitCount.ToString() + "个单位"
                            : "已删除" + unitCount.ToString() + "个单位的" + groupName + targetName;
                    });
                }
                catch (NotSupportedException ex)
                {
                    RunOnUiThread(delegate
                    {
                        labAbilityCommandState.Text = "当前版本未配置JASS函数地址";
                        MessageBox.Show(
                            ex.Message,
                            "JASS技能",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    });
                }
                catch (WindowsApi.BadProcessIdException ex)
                {
                    RunOnUiThread(delegate
                    {
                        labAbilityCommandState.Text = "游戏进程不可用";
                        ReportProcessIdFailure(ex.ProcessId);
                    });
                }
                catch (InvalidOperationException ex)
                {
                    RunOnUiThread(delegate
                    {
                        labAbilityCommandState.Text = ex.Message;
                        MessageBox.Show(
                            ex.Message,
                            "JASS技能",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    });
                }
                catch (Exception ex)
                {
                    RunOnUiThread(delegate
                    {
                        labAbilityCommandState.Text = "分组命令执行失败";
                        MessageBox.Show(
                            ex.Message,
                            "JASS技能",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    });
                }
                finally
                {
                    RunOnUiThread(delegate
                    {
                        SetAbilityGroupCommandRunning(false);
                    });
                }
            });
        }

        private void SetAbilityGroupCommandRunning(bool running)
        {
            _abilityGroupCommandRunning = running;

            bool enabled = !running && _currentGameContext != null;
            cmdAddGroupAbility.Enabled = enabled;
            cmdRemoveGroupAbility.Enabled = enabled;
            cmdAddGroupTalent.Enabled = enabled;
            cmdRemoveGroupTalent.Enabled = enabled;
            cboAbilityGroup.Enabled = !running;
            cmdScanGame.Enabled = !running;
        }

        private void RunOnUiThread(Action action)
        {
            if (IsDisposed)
                return;

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(action);
                }
                catch (InvalidOperationException)
                {
                }
                return;
            }

            action();
        }

        private static bool IsValidAbilityId(string abilityId)
        {
            if (abilityId.Length != 4)
                return false;

            foreach (char c in abilityId)
            {
                if (c < 0x20 || c > 0x7E)
                    return false;
            }
            return true;
        }

        private void InitializeAbilityOptions()
        {
            cboAbilityId.DisplayMember = "Name";
            cboAbilityId.ValueMember = "Id";
            cboAbilityGroup.DisplayMember = "Name";

            string skillsFilePath = GetConfigFilePath("skills.ini");
            EnsureDefaultSkillsFile(skillsFilePath);
            LoadAbilityOptions(skillsFilePath);

            if (cboAbilityId.Items.Count > 0)
                cboAbilityId.SelectedIndex = 0;

            string customFilePath = GetConfigFilePath("custom.ini");
            EnsureDefaultCustomFile(customFilePath);
            LoadAbilityGroups(customFilePath);

            if (cboAbilityGroup.Items.Count > 0)
                cboAbilityGroup.SelectedIndex = 0;
        }

        private string GetConfigFilePath(string fileName)
        {
            string currentDirectoryPath = Path.Combine(Environment.CurrentDirectory, fileName);
            if (File.Exists(currentDirectoryPath))
                return currentDirectoryPath;

            string startupPath = Path.Combine(Application.StartupPath, fileName);
            if (File.Exists(startupPath))
                return startupPath;

            return currentDirectoryPath;
        }

        private void EnsureDefaultSkillsFile(string skillsFilePath)
        {
            if (File.Exists(skillsFilePath))
                return;

            File.WriteAllLines(
                skillsFilePath,
                new string[]
                {
                    "AHbz 再起",
                    "AHba 魅心"
                });
        }

        private void EnsureDefaultCustomFile(string customFilePath)
        {
            if (File.Exists(customFilePath))
                return;

            File.WriteAllLines(
                customFilePath,
                new string[]
                {
                    "[全肉]",
                    "A08R 洞察",
                    "A09E 神佑",
                    "A05H 不屈",
                    "A03V 金刚",
                    "",
                    "[法师]",
                    "A08R 洞察",
                    "A0BS 业炎",
                    "A05I 法神"
                });
        }

        private void LoadAbilityOptions(string skillsFilePath)
        {
            cboAbilityId.Items.Clear();

            foreach (string rawLine in File.ReadAllLines(skillsFilePath))
            {
                string line = rawLine.Trim();
                if (String.IsNullOrEmpty(line) || line.StartsWith("#"))
                    continue;

                string[] parts = line.Split(new char[] { ' ', '\t' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2 || !IsValidAbilityId(parts[0]))
                    continue;

                cboAbilityId.Items.Add(new AbilityOption(parts[1].Trim(), parts[0]));
            }
        }

        private void LoadAbilityGroups(string customFilePath)
        {
            cboAbilityGroup.Items.Clear();
            lstAbilityGroupItems.Items.Clear();

            AbilityGroup currentGroup = null;
            foreach (string rawLine in File.ReadAllLines(customFilePath))
            {
                string line = rawLine.Trim();
                if (String.IsNullOrEmpty(line) || line.StartsWith("#") || line.StartsWith(";"))
                    continue;

                if (line.StartsWith("[") && line.EndsWith("]") && line.Length > 2)
                {
                    currentGroup = new AbilityGroup(line.Substring(1, line.Length - 2).Trim());
                    cboAbilityGroup.Items.Add(currentGroup);
                    continue;
                }

                if (currentGroup == null)
                    continue;

                string[] parts = line.Split(new char[] { ' ', '\t' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2 || !IsValidAbilityId(parts[0]))
                    continue;

                currentGroup.Abilities.Add(new AbilityOption(parts[1].Trim(), parts[0]));
            }
        }

        private void LoadSelectedAbilityGroupItems()
        {
            lstAbilityGroupItems.Items.Clear();

            AbilityGroup group = GetSelectedAbilityGroup();
            if (group == null)
                return;

            foreach (AbilityOption ability in group.Abilities)
                lstAbilityGroupItems.Items.Add(ability);
        }

        private string GetSelectedAbilityId()
        {
            AbilityOption selectedAbility = cboAbilityId.SelectedItem as AbilityOption;
            if (selectedAbility == null)
                return String.Empty;

            return selectedAbility.Id;
        }

        private AbilityGroup GetSelectedAbilityGroup()
        {
            return cboAbilityGroup.SelectedItem as AbilityGroup;
        }

        private sealed class AbilityOption
        {
            public string Name { get; private set; }
            public string Id { get; private set; }

            public AbilityOption(string name, string id)
            {
                Name = name;
                Id = id;
            }

            public override string ToString()
            {
                return Id + " " + Name;
            }
        }

        private sealed class AbilityGroup
        {
            public string Name { get; private set; }
            public List<AbilityOption> Abilities { get; private set; }

            public AbilityGroup(string name)
            {
                Name = name;
                Abilities = new List<AbilityOption>();
            }

            public override string ToString()
            {
                return Name;
            }
        }

        private int ExecuteJassAbilityCommand(string abilityId, int abilityLevel, bool addAbility)
        {
            EnsureJassAbilityFunctionAddress(addAbility);

            List<UInt32> selectedUnits = GetSelectedUnitAddresses();
            if (selectedUnits.Count == 0)
                throw new InvalidOperationException("没有选中的单位");

            UInt32 abilityIdValue = AbilityIdToUInt32(abilityId);
            using (WindowsApi.ProcessMemory mem = new WindowsApi.ProcessMemory(_currentGameContext.ProcessId))
            {
                foreach (UInt32 unitAddress in selectedUnits)
                {
                    UInt32 jassHandle;
                    if (!TryGetJassUnitHandle(mem, unitAddress, out jassHandle))
                        throw new InvalidOperationException("无法获取选中单位的JASS handle");

                    if (addAbility)
                    {
                        ExecuteJassAddAbility(mem, jassHandle, abilityIdValue);
                        ExecuteJassSetAbilityLevel(mem, jassHandle, abilityIdValue, abilityLevel);
                    }
                    else
                    {
                        ExecuteJassRemoveAbility(mem, jassHandle, abilityIdValue);
                    }
                }
            }

            return selectedUnits.Count;
        }

        private int ExecuteJassAbilityGroupCommand(
            IList<AbilityOption> abilities,
            int abilityLevel,
            bool addAbility)
        {
            EnsureJassAbilityFunctionAddress(addAbility);

            List<UInt32> selectedUnits = GetSelectedUnitAddresses();
            if (selectedUnits.Count == 0)
                throw new InvalidOperationException("没有选中的单位");

            using (WindowsApi.ProcessMemory mem = new WindowsApi.ProcessMemory(_currentGameContext.ProcessId))
            {
                List<UInt32> unitHandles = new List<UInt32>();
                foreach (UInt32 unitAddress in selectedUnits)
                {
                    UInt32 jassHandle;
                    if (!TryGetJassUnitHandle(mem, unitAddress, out jassHandle))
                        throw new InvalidOperationException("无法获取选中单位的JASS handle");

                    unitHandles.Add(jassHandle);
                }

                foreach (UInt32 jassHandle in unitHandles)
                {
                    foreach (AbilityOption ability in abilities)
                    {
                        UInt32 abilityIdValue = AbilityIdToUInt32(ability.Id);
                        if (addAbility)
                        {
                            ExecuteJassAddAbility(mem, jassHandle, abilityIdValue);
                            ExecuteJassSetAbilityLevel(mem, jassHandle, abilityIdValue, abilityLevel);
                        }
                        else
                        {
                            ExecuteJassRemoveAbility(mem, jassHandle, abilityIdValue);
                        }

                        DelayAfterAbilityGroupCommand();
                    }
                }
            }

            return selectedUnits.Count;
        }

        private void DelayAfterAbilityGroupCommand()
        {
            int delayMilliseconds;
            lock (_abilityGroupDelayRandom)
            {
                delayMilliseconds = _abilityGroupDelayRandom.Next(10, 301);
            }

            Thread.Sleep(delayMilliseconds);
        }

        private void EnsureJassAbilityFunctionAddress(bool addAbility)
        {
            if (_currentGameContext.JassStateGlobalAddress == 0
                || _currentGameContext.JassGetManagerAddress == 0
                || (_currentGameContext.JassGetAgentByObjectAddress == 0
                    && _currentGameContext.JassHandleToUnitAddress == 0))
            {
                throw new NotSupportedException(
                    "当前游戏版本没有配置JASS handle相关函数地址。\r\n\r\n"
                    + "请在GameContext.GetAbilityFunctionAddress()里填写当前版本game.dll对应地址后再使用。");
            }

            if (addAbility)
            {
                if (_currentGameContext.JassUnitAddAbilityAddress == 0
                    || _currentGameContext.JassSetUnitAbilityLevelAddress == 0)
                {
                    throw new NotSupportedException(
                        "当前游戏版本没有配置完整的JASS添加/设置天赋函数地址。\r\n\r\n"
                        + "请在GameContext.GetAbilityFunctionAddress()里填写当前版本game.dll对应地址后再使用。");
                }
            }
            else
            {
                if (_currentGameContext.JassUnitRemoveAbilityAddress == 0)
                {
                    throw new NotSupportedException(
                        "当前游戏版本没有配置完整的JASS删除天赋函数地址。\r\n\r\n"
                        + "请在GameContext.GetAbilityFunctionAddress()里填写当前版本game.dll对应地址后再使用。");
                }
            }
        }

        private List<UInt32> GetSelectedUnitAddresses()
        {
            List<UInt32> selectedUnits = new List<UInt32>();
            using (WindowsApi.ProcessMemory mem = new WindowsApi.ProcessMemory(_currentGameContext.ProcessId))
            {
                UInt32 selectedUnitList = mem.ReadUInt32((IntPtr)_currentGameContext.UnitListAddress);
                if (selectedUnitList == 0)
                    return selectedUnits;

                UInt16 a2 = mem.ReadUInt16((IntPtr)unchecked(selectedUnitList + 0x28));
                UInt32 listAddress = mem.ReadUInt32((IntPtr)unchecked(selectedUnitList + 0x58 + 4 * a2));
                listAddress = mem.ReadUInt32((IntPtr)unchecked(listAddress + 0x34));
                if (listAddress == 0)
                    return selectedUnits;

                UInt32 nextNode = mem.ReadUInt32((IntPtr)unchecked(listAddress + 0x1F0));
                UInt32 listLength = mem.ReadUInt32((IntPtr)unchecked(listAddress + 0x1F8));
                for (UInt32 itemIndex = 0; itemIndex < listLength; itemIndex++)
                {
                    if (nextNode == 0)
                        break;

                    UInt32 unitAddress = mem.ReadUInt32((IntPtr)unchecked(nextNode + 8));
                    if (unitAddress != 0)
                        selectedUnits.Add(unitAddress);

                    nextNode = mem.ReadUInt32((IntPtr)unchecked(nextNode + 0));
                }
            }
            return selectedUnits;
        }

        private static UInt32 AbilityIdToUInt32(string abilityId)
        {
            return unchecked(
                ((UInt32)(byte)abilityId[0] << 24)
                | ((UInt32)(byte)abilityId[1] << 16)
                | ((UInt32)(byte)abilityId[2] << 8)
                | (UInt32)(byte)abilityId[3]);
        }

        private bool TryGetJassUnitHandle(
            WindowsApi.ProcessMemory mem,
            UInt32 unitAddress,
            out UInt32 jassHandle)
        {
            jassHandle = 0;
            if (_currentGameContext.JassStateGlobalAddress == 0
                || _currentGameContext.JassGetManagerAddress == 0)
                return false;

            if (_currentGameContext.JassGetAgentByObjectAddress == 0)
                return TryGetJassUnitHandleByRemoteScan(mem, unitAddress, out jassHandle);

            RemoteCodeBuilder code = new RemoteCodeBuilder();
            code.MovEcxFromAddress(_currentGameContext.JassStateGlobalAddress);
            code.MovEax(_currentGameContext.JassGetManagerAddress);
            code.CallEax();
            code.Ret();

            UInt32 jassManager = mem.ExecuteRemoteCode(code.ToArray());
            if (jassManager == 0)
                return false;

            code = new RemoteCodeBuilder();
            code.Push(unitAddress);
            code.MovEcx(jassManager);
            code.MovEax(_currentGameContext.JassGetAgentByObjectAddress);
            code.CallEax();
            code.Ret();

            UInt32 jassAgent = mem.ExecuteRemoteCode(code.ToArray());
            if (jassAgent == 0)
                return false;

            UInt32 handleTable = mem.ReadUInt32((IntPtr)unchecked(jassManager + 0x19C));
            if (handleTable == 0)
                return false;

            const UInt32 firstJassHandle = 0x100000;
            const UInt32 maxHandlesToScan = 0x200000;
            const int entrySize = 12;
            const int entriesPerChunk = 4096;
            const int objectOffsetInEntry = 4;

            byte[] targetBytes = BitConverter.GetBytes(jassAgent);
            for (UInt32 startIndex = 0; startIndex < maxHandlesToScan; startIndex += (UInt32)entriesPerChunk)
            {
                UInt32 remainingEntries = maxHandlesToScan - startIndex;
                int entriesThisChunk = remainingEntries > entriesPerChunk
                    ? entriesPerChunk
                    : (int)remainingEntries;
                int bytesRead;
                byte[] tableBytes = mem.ReadBytes(
                    (IntPtr)unchecked(handleTable + startIndex * entrySize),
                    entriesThisChunk * entrySize,
                    out bytesRead);

                if (bytesRead < entrySize)
                    break;

                int readableEntries = bytesRead / entrySize;
                for (int entryIndex = 0; entryIndex < readableEntries; entryIndex++)
                {
                    int objectOffset = entryIndex * entrySize + objectOffsetInEntry;
                    if (tableBytes[objectOffset] == targetBytes[0]
                        && tableBytes[objectOffset + 1] == targetBytes[1]
                        && tableBytes[objectOffset + 2] == targetBytes[2]
                        && tableBytes[objectOffset + 3] == targetBytes[3])
                    {
                        jassHandle = firstJassHandle + startIndex + (UInt32)entryIndex;
                        return true;
                    }
                }
            }

            return false;
        }

        private bool TryGetJassUnitHandleByRemoteScan(
            WindowsApi.ProcessMemory mem,
            UInt32 unitAddress,
            out UInt32 jassHandle)
        {
            jassHandle = 0;
            if (_currentGameContext.JassHandleToUnitAddress == 0)
                return false;

            RemoteCodeBuilder code = new RemoteCodeBuilder();
            code.MovEcxFromAddress(_currentGameContext.JassStateGlobalAddress);
            code.MovEax(_currentGameContext.JassGetManagerAddress);
            code.CallEax();
            code.Ret();

            UInt32 jassManager = mem.ExecuteRemoteCode(code.ToArray());
            if (jassManager == 0)
                return false;

            UInt32 handleTable = mem.ReadUInt32((IntPtr)unchecked(jassManager + 0x19C));
            if (handleTable == 0)
                return false;

            const UInt32 firstJassHandle = 0x100000;
            const UInt32 maxHandlesToScan = 0x200000;
            const int entrySize = 12;
            const int entriesPerChunk = 4096;
            const int objectOffsetInEntry = 4;

            for (UInt32 startIndex = 0; startIndex < maxHandlesToScan; startIndex += (UInt32)entriesPerChunk)
            {
                UInt32 remainingEntries = maxHandlesToScan - startIndex;
                int entriesThisChunk = remainingEntries > entriesPerChunk
                    ? entriesPerChunk
                    : (int)remainingEntries;
                int bytesRead;
                byte[] tableBytes = mem.ReadBytes(
                    (IntPtr)unchecked(handleTable + startIndex * entrySize),
                    entriesThisChunk * entrySize,
                    out bytesRead);

                if (bytesRead < entrySize)
                    break;

                int readableEntries = bytesRead / entrySize;
                for (int entryIndex = 0; entryIndex < readableEntries; entryIndex++)
                {
                    int objectOffset = entryIndex * entrySize + objectOffsetInEntry;
                    UInt32 jassAgent = BitConverter.ToUInt32(tableBytes, objectOffset);
                    if (jassAgent == 0)
                        continue;

                    UInt32 currentHandle = firstJassHandle + startIndex + (UInt32)entryIndex;
                    UInt32 currentUnit = ExecuteJassHandleToUnit(mem, currentHandle);
                    if (currentUnit == unitAddress)
                    {
                        jassHandle = currentHandle;
                        return true;
                    }
                }
            }

            return false;
        }

        private UInt32 ExecuteJassHandleToUnit(
            WindowsApi.ProcessMemory mem,
            UInt32 jassHandle)
        {
            RemoteCodeBuilder code = new RemoteCodeBuilder();
            code.MovEcx(jassHandle);
            code.MovEax(_currentGameContext.JassHandleToUnitAddress);
            code.CallEax();
            code.Ret();
            return mem.ExecuteRemoteCode(code.ToArray());
        }

        private void ExecuteJassAddAbility(
            WindowsApi.ProcessMemory mem,
            UInt32 jassHandle,
            UInt32 abilityId)
        {
            RemoteCodeBuilder code = new RemoteCodeBuilder();
            code.Push(abilityId);
            code.Push(jassHandle);
            code.MovEax(_currentGameContext.JassUnitAddAbilityAddress);
            code.CallEax();
            code.AddEsp(8);
            code.Ret();
            mem.ExecuteRemoteCode(code.ToArray());
        }

        private void ExecuteJassRemoveAbility(
            WindowsApi.ProcessMemory mem,
            UInt32 jassHandle,
            UInt32 abilityId)
        {
            RemoteCodeBuilder code = new RemoteCodeBuilder();
            code.Push(abilityId);
            code.Push(jassHandle);
            code.MovEax(_currentGameContext.JassUnitRemoveAbilityAddress);
            code.CallEax();
            code.AddEsp(8);
            code.Ret();
            mem.ExecuteRemoteCode(code.ToArray());
        }

        private void ExecuteJassSetAbilityLevel(
            WindowsApi.ProcessMemory mem,
            UInt32 jassHandle,
            UInt32 abilityId,
            int abilityLevel)
        {
            if (abilityLevel < 1)
                abilityLevel = 1;

            RemoteCodeBuilder code = new RemoteCodeBuilder();
            code.Push(unchecked((UInt32)abilityLevel));
            code.Push(abilityId);
            code.Push(jassHandle);
            code.MovEax(_currentGameContext.JassSetUnitAbilityLevelAddress);
            code.CallEax();
            code.AddEsp(12);
            code.Ret();
            mem.ExecuteRemoteCode(code.ToArray());
        }

        private sealed class RemoteCodeBuilder
        {
            private readonly List<byte> _code = new List<byte>();
            private readonly Dictionary<string, int> _labels = new Dictionary<string, int>();
            private readonly List<JumpPatch> _jumps = new List<JumpPatch>();

            public void Mark(string label)
            {
                _labels[label] = _code.Count;
            }

            public byte[] ToArray()
            {
                foreach (JumpPatch jump in _jumps)
                {
                    int target;
                    if (!_labels.TryGetValue(jump.Label, out target))
                        throw new InvalidOperationException("Missing remote code label: " + jump.Label);

                    int relative = target - (jump.Offset + 4);
                    byte[] bytes = BitConverter.GetBytes(relative);
                    for (int i = 0; i < 4; i++)
                        _code[jump.Offset + i] = bytes[i];
                }
                return _code.ToArray();
            }

            public void Push(UInt32 value) { AddByte(0x68); AddUInt32(value); }
            public void PushByte(byte value) { AddByte(0x6A); AddByte(value); }
            public void PushEax() { AddByte(0x50); }
            public void MovEax(UInt32 value) { AddByte(0xB8); AddUInt32(value); }
            public void MovEbx(UInt32 value) { AddByte(0xBB); AddUInt32(value); }
            public void MovEcx(UInt32 value) { AddByte(0xB9); AddUInt32(value); }
            public void MovEdx(UInt32 value) { AddByte(0xBA); AddUInt32(value); }
            public void MovEcxFromAddress(UInt32 address) { AddBytes(0x8B, 0x0D); AddUInt32(address); }
            public void MovEsiEax() { AddBytes(0x8B, 0xF0); }
            public void MovEdiEax() { AddBytes(0x8B, 0xF8); }
            public void MovEbxEax() { AddBytes(0x8B, 0xD8); }
            public void MovEcxEdi() { AddBytes(0x8B, 0xCF); }
            public void MovEaxFromEdi() { AddBytes(0x8B, 0x07); }
            public void MovEdxFromEdi() { AddBytes(0x8B, 0x17); }
            public void MovEsiFromEdi(byte offset) { AddBytes(0x8B, 0x77, offset); }
            public void IncEsi() { AddByte(0x46); }
            public void DecEsi() { AddByte(0x4E); }
            public void CmpEbxEax() { AddBytes(0x3B, 0xD8); }
            public void CmpEsiEbx() { AddBytes(0x3B, 0xF3); }
            public void TestEaxEax() { AddBytes(0x85, 0xC0); }
            public void TestEsiEsi() { AddBytes(0x85, 0xF6); }
            public void CallEax() { AddBytes(0xFF, 0xD0); }
            public void CallDwordEax(UInt32 offset) { AddBytes(0xFF, 0x90); AddUInt32(offset); }
            public void CallDwordEdx(UInt32 offset) { AddBytes(0xFF, 0x92); AddUInt32(offset); }
            public void AddEsp(byte value) { AddBytes(0x83, 0xC4, value); }
            public void Ret() { AddByte(0xC3); }
            public void Jmp(string label) { AddByte(0xE9); AddJump(label); }
            public void Jz(string label) { AddBytes(0x0F, 0x84); AddJump(label); }
            public void Jnz(string label) { AddBytes(0x0F, 0x85); AddJump(label); }
            public void Jl(string label) { AddBytes(0x0F, 0x8C); AddJump(label); }
            public void Jle(string label) { AddBytes(0x0F, 0x8E); AddJump(label); }
            public void Jg(string label) { AddBytes(0x0F, 0x8F); AddJump(label); }
            public void Jge(string label) { AddBytes(0x0F, 0x8D); AddJump(label); }

            private void AddJump(string label)
            {
                _jumps.Add(new JumpPatch(_code.Count, label));
                AddUInt32(0);
            }

            private void AddByte(byte value)
            {
                _code.Add(value);
            }

            private void AddBytes(params byte[] bytes)
            {
                _code.AddRange(bytes);
            }

            private void AddUInt32(UInt32 value)
            {
                _code.AddRange(BitConverter.GetBytes(value));
            }

            private sealed class JumpPatch
            {
                public int Offset { get; private set; }
                public string Label { get; private set; }

                public JumpPatch(int offset, string label)
                {
                    Offset = offset;
                    Label = label;
                }
            }
        }

        private void viewData_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch ((Keys)e.KeyChar)
            {
                case Keys.Enter:
                    viewData_MouseUp(sender, null);
                    e.Handled = true;
                    break;
            }
        }
        private void viewData_MouseUp(object sender, MouseEventArgs e)
        {
            //Get item
            if (viewData.SelectedItems.Count < 1) return;
            ListViewItem currentItem = viewData.SelectedItems[0];

            //Determine the content of edit box
            ReplaceInputTextbox();
            txtInput.Tag = currentItem;

            int textToEdit = string.IsNullOrEmpty(currentItem.SubItems[2].Text) ? 1 : 2;
            string originalText = currentItem.SubItems[textToEdit].Text;
            string itemName = currentItem.SubItems[0].Text;

            txtInput.Text = CalculateInputValue(itemName, originalText);
            //txtInput.Text = currentItem.SubItems[textToEdit].Text;

            //Enable editing
            txtInput.Visible = true;
            txtInput.Focus();
            txtInput.Select(0, 0);  // Cancel select all
        }

        private string CalculateInputValue(string itemName, string originalText)
        {
            if (itemName == "攻击① - 间隔") return "0.01";
            if (itemName.Contains("金币") || itemName.Contains("木材")) return "900000";
            if (itemName.Contains("最大人口")) return "100";

            int multiplier = xToolStripMenuItem1.Checked ? 2 :
                xToolStripMenuItem2.Checked ? 3 :
                toolStripMenuItem7.Checked ? 4 :
                xToolStripMenuItem3.Checked ? 5 : 1;

            if (multiplier > 1 && double.TryParse(originalText, out double val))
            {
                return ((int)Math.Round(val, MidpointRounding.AwayFromZero) * multiplier).ToString();
            }

            return originalText;
        }


        private void viewData_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            ReplaceInputTextbox();

        }

        private void viewData_Scrolling(object sender, EventArgs e)
        {
            viewData.Focus();
        }

        private void txtInput_Leave(object sender, EventArgs e)
        {
            txtInput.Visible = false;
            ListViewItem currentItem = txtInput.Tag as ListViewItem;
            if (currentItem == null)
                return;

            if (currentItem.SubItems[1].Text != txtInput.Text)
                currentItem.SubItems[2].Text = txtInput.Text;
            else
                currentItem.SubItems[2].Text = "";
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    CommitEditAndMoveNext(sender, 1);
                    e.Handled = true;
                    break;
                case Keys.Up:
                    CommitEditAndMoveNext(sender, -1);
                    e.Handled = true;
                    break;
                case Keys.Down:
                    CommitEditAndMoveNext(sender, 1);
                    e.Handled = true;
                    break;
                case Keys.Escape:
                    DiscardEdit(sender);
                    e.Handled = true;
                    break;
            }
        }

        private void DiscardEdit(object editBox)
        {
            // Roll back content of the edit box
            viewData_MouseUp(editBox, null);

            // Hide edit box
            txtInput_Leave(editBox, null);

            // Restore focus
            viewData.Focus();
        }

        private void CommitEditAndMoveNext(object editBox, int delta)
        {
            // Commit
            txtInput_Leave(editBox, null);

            // Move to another line
            viewData.Focus();
            if (viewData.SelectedItems.Count > 0)
            {
                int nextIndex = viewData.SelectedItems[0].Index + delta;
                if (nextIndex < viewData.Items.Count &&
                    nextIndex >= 0)
                {
                    viewData.Items[nextIndex].Selected = true;
                    viewData.Items[nextIndex].Focused = true;
                    viewData.Items[nextIndex].EnsureVisible();
                }
                viewData_MouseUp(editBox, null);
            }
        }

        /************************************************************************/
        /* Debug                                                                */
        /************************************************************************/
        private void menuDebug1_Click(object sender, EventArgs e)
        {
            string strIndex = Microsoft.VisualBasic.Interaction.InputBox(
                "nIndex = 0x?",
                "War3Common.ReadFromGameMemory(nIndex)",
                "0", -1, -1);
            if (String.IsNullOrEmpty(strIndex))
                return;

            Int32 nIndex;
            if (!Int32.TryParse(
                strIndex,
                System.Globalization.NumberStyles.HexNumber,
                System.Globalization.NumberFormatInfo.InvariantInfo,
                out nIndex))
            {
                nIndex = 0;
            }

            try
            {
                UInt32 result = 0;
                using (WindowsApi.ProcessMemory mem = new WindowsApi.ProcessMemory(_currentGameContext.ProcessId))
                {
                    NewChildrenEventArgs args = new NewChildrenEventArgs();
                    War3Common.GetGameMemory(
                        _currentGameContext, ref args);
                    result = War3Common.ReadFromGameMemory(
                        mem, _currentGameContext, args,
                        nIndex);
                }
                MessageBox.Show(
                    "0x" + result.ToString("X"),
                    "War3Common.ReadFromGameMemory(0x" + strIndex + ")");
            }
            catch (WindowsApi.BadProcessIdException ex)
            {
                ReportProcessIdFailure(ex.ProcessId);
            }
        }

        private void 启用ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TopMost = 启用ToolStripMenuItem.Checked;
        }

        private void 解除修改限制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (解除修改限制ToolStripMenuItem == null || txtInput == null)
                return;
            txtInput.MaxLength = 解除修改限制ToolStripMenuItem.Checked ? 35 : 7;
        }

        private void viewFunctions_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F5)
            {
                toolStripButton2_Click(sender, null);
                e.Handled = true;
            }

        }

        private void viewData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F6)
            {
                toolStripButton1_Click(sender, null);
                e.Handled = true;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                ApplyModify();

                // Refresh left
                TreeNode selectedNode = viewFunctions.SelectedNode;
                if (selectedNode == null)
                    return;

                ITrainerNode functionNode = selectedNode.Tag as ITrainerNode;
                if (functionNode != null)
                    RefreshSelectedObject(functionNode);

                // Refresh right
                SelectFunction(selectedNode);
            }
            catch (WindowsApi.BadProcessIdException ex)
            {
                ReportProcessIdFailure(ex.ProcessId);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                GetAllObject();
            }
            catch (WindowsApi.BadProcessIdException ex)
            {
                ReportProcessIdFailure(ex.ProcessId);
            }
        }

        private void GroupedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem currentItem = sender as ToolStripMenuItem;
            if (currentItem == null) return;

            ToolStripMenuItem[] allItems = new ToolStripMenuItem[]
            {
                xToolStripMenuItem,
                xToolStripMenuItem1,
                xToolStripMenuItem2,
                toolStripMenuItem7,
                xToolStripMenuItem3
            };

            foreach (var item in allItems)
            {
                item.Checked = (item == currentItem);
            }
        }
    }
}
