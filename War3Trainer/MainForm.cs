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

        private void cmdAddTalent_Click(object sender, EventArgs e)
        {
            ExecuteTalentUiCommand(true);
        }

        private void cmdRemoveTalent_Click(object sender, EventArgs e)
        {
            ExecuteTalentUiCommand(false);
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
                        ExecuteAddAbility(mem, unitAddress, abilityIdValue);
                        ExecuteSetAbilityLevel(mem, unitAddress, abilityIdValue, abilityLevel);
                    }
                    else
                    {
                        ExecuteRemoveAbility(mem, unitAddress, abilityIdValue);
                    }
                }
            }

            return selectedUnits.Count;
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

        private void EnsureAbilityFunctionAddress(bool addAbility)
        {
            if (addAbility)
            {
                if (_currentGameContext.UnitAddAbilityAddress == 0
                    || _currentGameContext.UnitSetAbilityLevelAddress == 0
                    || _currentGameContext.UnitFindAbilityAddress == 0
                    || _currentGameContext.UnitRefreshAbilityAddress == 0
                    || _currentGameContext.UnitBeginAbilityUpdateAddress == 0
                    || _currentGameContext.UnitEndAbilityUpdateAddress == 0
                    || _currentGameContext.UnitGetAbilityMaxLevelAddress == 0)
                {
                    throw new NotSupportedException(
                        "当前游戏版本没有配置完整的添加/设置技能函数地址。\r\n\r\n"
                        + "请在GameContext.GetAbilityFunctionAddress()里填写当前版本game.dll对应地址后再使用。");
                }
            }
            else
            {
                if (_currentGameContext.UnitRemoveAbilityAddress == 0
                    || _currentGameContext.UnitFindAbilityAddress == 0
                    || _currentGameContext.UnitRefreshAbilityAddress == 0)
                {
                    throw new NotSupportedException(
                        "当前游戏版本没有配置完整的删除技能函数地址。\r\n\r\n"
                        + "请在GameContext.GetAbilityFunctionAddress()里填写当前版本game.dll对应地址后再使用。");
                }
            }
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

            const UInt32 firstJassHandle = 0x100000;
            const UInt32 maxHandlesToScan = 0x200000;

            RemoteCodeBuilder code = new RemoteCodeBuilder();
            code.PushEbx();
            code.PushEsi();
            code.PushEdi();

            code.MovEcxFromAddress(_currentGameContext.JassStateGlobalAddress);
            code.MovEax(_currentGameContext.JassGetManagerAddress);
            code.CallEax();
            code.TestEaxEax();
            code.Jz("not_found");

            code.MovEdiFromEax(0x19C);
            code.TestEdiEdi();
            code.Jz("not_found");

            code.XorEsiEsi();
            code.Mark("scan_loop");
            code.CmpEsi(maxHandlesToScan);
            code.Jge("not_found");

            code.LeaEbxEsiTimes3();
            code.MovEdxFromEdiEbxScale4(4);
            code.TestEdxEdx();
            code.Jz("next_handle");

            code.MovEcxEsi();
            code.AddEcx(firstJassHandle);
            code.MovEax(_currentGameContext.JassHandleToUnitAddress);
            code.CallEax();
            code.CmpEax(unitAddress);
            code.Jz("found");

            code.Mark("next_handle");
            code.IncEsi();
            code.Jmp("scan_loop");

            code.Mark("found");
            code.MovEaxEsi();
            code.AddEax(firstJassHandle);
            code.Jmp("done");

            code.Mark("not_found");
            code.XorEaxEax();

            code.Mark("done");
            code.PopEdi();
            code.PopEsi();
            code.PopEbx();
            code.Ret();

            jassHandle = mem.ExecuteRemoteCode(code.ToArray());
            return jassHandle != 0;
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

        private void ExecuteAddAbility(
            WindowsApi.ProcessMemory mem,
            UInt32 unitAddress,
            UInt32 abilityId)
        {
            RemoteCodeBuilder code = new RemoteCodeBuilder();

            EmitFindAbility(code, unitAddress, abilityId);
            code.TestEaxEax();
            code.Jnz("done");

            code.MovEcx(unitAddress);
            code.MovEax(_currentGameContext.UnitBeginAbilityUpdateAddress);
            code.CallEax();

            code.PushByte(0);
            code.PushByte(0);
            code.PushByte(0);
            code.MovEdx(abilityId);
            code.MovEcx(unitAddress);
            code.MovEax(_currentGameContext.UnitAddAbilityAddress);
            code.CallEax();
            code.MovEsiEax();

            code.MovEcx(unitAddress);
            code.MovEax(_currentGameContext.UnitEndAbilityUpdateAddress);
            code.CallEax();

            code.TestEsiEsi();
            code.Jz("done");
            code.MovEcx(unitAddress);
            code.MovEax(_currentGameContext.UnitRefreshAbilityAddress);
            code.CallEax();

            code.Mark("done");
            code.Ret();
            mem.ExecuteRemoteCode(code.ToArray());
        }

        private void ExecuteRemoveAbility(
            WindowsApi.ProcessMemory mem,
            UInt32 unitAddress,
            UInt32 abilityId)
        {
            RemoteCodeBuilder code = new RemoteCodeBuilder();

            EmitFindAbility(code, unitAddress, abilityId);
            code.TestEaxEax();
            code.Jz("done");

            code.PushEax();
            code.MovEcx(unitAddress);
            code.MovEax(_currentGameContext.UnitRemoveAbilityAddress);
            code.CallEax();

            code.MovEcx(unitAddress);
            code.MovEax(_currentGameContext.UnitRefreshAbilityAddress);
            code.CallEax();

            code.Mark("done");
            code.Ret();
            mem.ExecuteRemoteCode(code.ToArray());
        }

        private void ExecuteSetAbilityLevel(
            WindowsApi.ProcessMemory mem,
            UInt32 unitAddress,
            UInt32 abilityId,
            int abilityLevel)
        {
            if (abilityLevel < 1)
                abilityLevel = 1;

            RemoteCodeBuilder code = new RemoteCodeBuilder();

            EmitFindAbility(code, unitAddress, abilityId);
            code.TestEaxEax();
            code.Jz("done");
            code.MovEdiEax();

            code.MovEcxEdi();
            code.MovEax(_currentGameContext.UnitGetAbilityMaxLevelAddress);
            code.CallEax();

            code.MovEbx(unchecked((UInt32)abilityLevel));
            code.CmpEbxEax();
            code.Jl("level_ok");
            code.MovEbxEax();
            code.Mark("level_ok");

            code.MovEsiFromEdi(0x50);
            code.IncEsi();
            code.CmpEsiEbx();
            code.Jge("maybe_decrease");

            code.Mark("increase_loop");
            code.MovEaxFromEdi();
            code.MovEcxEdi();
            code.CallDwordEax(0x2E4);
            code.IncEsi();
            code.CmpEsiEbx();
            code.Jl("increase_loop");
            code.Jmp("done");

            code.Mark("maybe_decrease");
            code.Jle("done");

            code.Mark("decrease_loop");
            code.MovEdxFromEdi();
            code.MovEcxEdi();
            code.CallDwordEdx(0x2E8);
            code.DecEsi();
            code.CmpEsiEbx();
            code.Jg("decrease_loop");

            code.Mark("done");
            code.Ret();
            mem.ExecuteRemoteCode(code.ToArray());
        }

        private void EmitFindAbility(
            RemoteCodeBuilder code,
            UInt32 unitAddress,
            UInt32 abilityId)
        {
            code.PushByte(1);
            code.PushByte(1);
            code.PushByte(1);
            code.PushByte(0);
            code.Push(abilityId);
            code.MovEcx(unitAddress);
            code.MovEax(_currentGameContext.UnitFindAbilityAddress);
            code.CallEax();
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
            public void PushEbx() { AddByte(0x53); }
            public void PushEsi() { AddByte(0x56); }
            public void PushEdi() { AddByte(0x57); }
            public void PopEbx() { AddByte(0x5B); }
            public void PopEsi() { AddByte(0x5E); }
            public void PopEdi() { AddByte(0x5F); }
            public void MovEax(UInt32 value) { AddByte(0xB8); AddUInt32(value); }
            public void MovEbx(UInt32 value) { AddByte(0xBB); AddUInt32(value); }
            public void MovEcx(UInt32 value) { AddByte(0xB9); AddUInt32(value); }
            public void MovEdx(UInt32 value) { AddByte(0xBA); AddUInt32(value); }
            public void MovEcxFromAddress(UInt32 address) { AddBytes(0x8B, 0x0D); AddUInt32(address); }
            public void MovEdiFromEax(UInt32 offset) { AddBytes(0x8B, 0xB8); AddUInt32(offset); }
            public void MovEsiEax() { AddBytes(0x8B, 0xF0); }
            public void MovEdiEax() { AddBytes(0x8B, 0xF8); }
            public void MovEbxEax() { AddBytes(0x8B, 0xD8); }
            public void MovEcxEdi() { AddBytes(0x8B, 0xCF); }
            public void MovEcxEsi() { AddBytes(0x8B, 0xCE); }
            public void MovEaxEsi() { AddBytes(0x8B, 0xC6); }
            public void MovEaxFromEdi() { AddBytes(0x8B, 0x07); }
            public void MovEdxFromEdi() { AddBytes(0x8B, 0x17); }
            public void MovEsiFromEdi(byte offset) { AddBytes(0x8B, 0x77, offset); }
            public void MovEdxFromEdiEbxScale4(byte offset) { AddBytes(0x8B, 0x54, 0x9F, offset); }
            public void LeaEbxEsiTimes3() { AddBytes(0x8D, 0x1C, 0x76); }
            public void IncEsi() { AddByte(0x46); }
            public void DecEsi() { AddByte(0x4E); }
            public void AddEax(UInt32 value) { AddByte(0x05); AddUInt32(value); }
            public void AddEcx(UInt32 value) { AddBytes(0x81, 0xC1); AddUInt32(value); }
            public void XorEaxEax() { AddBytes(0x33, 0xC0); }
            public void XorEsiEsi() { AddBytes(0x33, 0xF6); }
            public void CmpEax(UInt32 value) { AddByte(0x3D); AddUInt32(value); }
            public void CmpEsi(UInt32 value) { AddBytes(0x81, 0xFE); AddUInt32(value); }
            public void CmpEbxEax() { AddBytes(0x3B, 0xD8); }
            public void CmpEsiEbx() { AddBytes(0x3B, 0xF3); }
            public void TestEaxEax() { AddBytes(0x85, 0xC0); }
            public void TestEsiEsi() { AddBytes(0x85, 0xF6); }
            public void TestEdiEdi() { AddBytes(0x85, 0xFF); }
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
