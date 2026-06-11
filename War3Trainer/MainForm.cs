using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace War3Trainer
{
    public partial class MainForm : Form
    {
        private GameContext _currentGameContext;
        private GameTrainer _mainTrainer;

        public MainForm()
        {
            InitializeComponent();
            InitializeAbilityOptions();
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
                int unitCount = ExecuteRemoteAbilityCommand(abilityId, abilityLevel, addAbility);
                labAbilityCommandState.Text = addAbility
                    ? "已添加技能到" + unitCount.ToString() + "个单位"
                    : "已删除" + unitCount.ToString() + "个单位的技能";
            }
            catch (NotSupportedException ex)
            {
                labAbilityCommandState.Text = "当前版本未配置函数地址";
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

            string skillsFilePath = Path.Combine(Application.StartupPath, "skills.ini");
            EnsureDefaultSkillsFile(skillsFilePath);
            LoadAbilityOptions(skillsFilePath);

            if (cboAbilityId.Items.Count > 0)
                cboAbilityId.SelectedIndex = 0;
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

        private string GetSelectedAbilityId()
        {
            AbilityOption selectedAbility = cboAbilityId.SelectedItem as AbilityOption;
            if (selectedAbility == null)
                return String.Empty;

            return selectedAbility.Id;
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
                return Name;
            }
        }

        private int ExecuteRemoteAbilityCommand(string abilityId, int abilityLevel, bool addAbility)
        {
            EnsureAbilityFunctionAddress(addAbility);

            List<UInt32> selectedUnits = GetSelectedUnitAddresses();
            if (selectedUnits.Count == 0)
                throw new InvalidOperationException("没有选中的单位");

            UInt32 abilityIdValue = AbilityIdToUInt32(abilityId);
            using (WindowsApi.ProcessMemory mem = new WindowsApi.ProcessMemory(_currentGameContext.ProcessId))
            {
                foreach (UInt32 unitAddress in selectedUnits)
                {
                    if (addAbility)
                    {
                        ExecuteThisCall(
                            mem,
                            _currentGameContext.UnitAddAbilityAddress,
                            unitAddress,
                            new UInt32[] { abilityIdValue });
                        ExecuteThisCall(
                            mem,
                            _currentGameContext.UnitSetAbilityLevelAddress,
                            unitAddress,
                            new UInt32[] { abilityIdValue, unchecked((UInt32)abilityLevel) });
                    }
                    else
                    {
                        ExecuteThisCall(
                            mem,
                            _currentGameContext.UnitRemoveAbilityAddress,
                            unitAddress,
                            new UInt32[] { abilityIdValue });
                    }
                }
            }

            return selectedUnits.Count;
        }

        private void EnsureAbilityFunctionAddress(bool addAbility)
        {
            if (addAbility)
            {
                if (_currentGameContext.UnitAddAbilityAddress == 0
                    || _currentGameContext.UnitSetAbilityLevelAddress == 0)
                {
                    throw new NotSupportedException(
                        "当前游戏版本没有配置UnitAddAbility/SetAbilityLevel函数地址。\r\n\r\n"
                        + "请在GameContext.GetAbilityFunctionAddress()里填写当前版本game.dll对应地址后再使用。");
                }
            }
            else
            {
                if (_currentGameContext.UnitRemoveAbilityAddress == 0)
                {
                    throw new NotSupportedException(
                        "当前游戏版本没有配置UnitRemoveAbility函数地址。\r\n\r\n"
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

        private static void ExecuteThisCall(
            WindowsApi.ProcessMemory mem,
            UInt32 functionAddress,
            UInt32 thisAddress,
            UInt32[] args)
        {
            List<byte> code = new List<byte>();

            // mov ecx, thisAddress
            code.Add(0xB9);
            code.AddRange(BitConverter.GetBytes(thisAddress));

            for (int i = args.Length - 1; i >= 0; i--)
            {
                // push arg
                code.Add(0x68);
                code.AddRange(BitConverter.GetBytes(args[i]));
            }

            // mov eax, functionAddress
            code.Add(0xB8);
            code.AddRange(BitConverter.GetBytes(functionAddress));

            // call eax; ret
            code.Add(0xFF);
            code.Add(0xD0);
            code.Add(0xC3);

            mem.ExecuteRemoteCode(code.ToArray());
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
