namespace War3Trainer
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolContainer = new System.Windows.Forms.ToolStripContainer();
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.cmdScanGame = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.labGameScanState = new System.Windows.Forms.ToolStripLabel();
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDebug1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSplit1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAbility = new System.Windows.Forms.ToolStripMenuItem();
            this.置顶ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.启用ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.解除修改限制ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.修改倍数ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.xToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.xToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.xToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.viewFunctions = new System.Windows.Forms.TreeView();
            this.grpAbilityCommand = new System.Windows.Forms.GroupBox();
            this.labAbilityCommandState = new System.Windows.Forms.Label();
            this.cmdRemoveGroupTalent = new System.Windows.Forms.Button();
            this.cmdAddGroupTalent = new System.Windows.Forms.Button();
            this.cmdRemoveGroupAbility = new System.Windows.Forms.Button();
            this.cmdAddGroupAbility = new System.Windows.Forms.Button();
            this.lstAbilityGroupItems = new System.Windows.Forms.ListBox();
            this.cboAbilityGroup = new System.Windows.Forms.ComboBox();
            this.labAbilityGroup = new System.Windows.Forms.Label();
            this.cmdRemoveTalent = new System.Windows.Forms.Button();
            this.cmdAddTalent = new System.Windows.Forms.Button();
            this.cmdRemoveAbility = new System.Windows.Forms.Button();
            this.cmdAddAbility = new System.Windows.Forms.Button();
            this.numAbilityLevel = new System.Windows.Forms.NumericUpDown();
            this.labAbilityLevel = new System.Windows.Forms.Label();
            this.cboAbilityId = new System.Windows.Forms.ComboBox();
            this.labAbilityId = new System.Windows.Forms.Label();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.viewData = new War3Trainer.ListViewEx();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colOriginalValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colUnsavedValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolContainer.ContentPanel.SuspendLayout();
            this.toolContainer.TopToolStripPanel.SuspendLayout();
            this.toolContainer.SuspendLayout();
            this.toolStripMain.SuspendLayout();
            this.menuMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.grpAbilityCommand.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAbilityLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // toolContainer
            // 
            // 
            // toolContainer.ContentPanel
            // 
            this.toolContainer.ContentPanel.Controls.Add(this.toolStripMain);
            this.toolContainer.ContentPanel.Margin = new System.Windows.Forms.Padding(4);
            this.toolContainer.ContentPanel.Size = new System.Drawing.Size(686, 27);
            this.toolContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolContainer.Location = new System.Drawing.Point(0, 0);
            this.toolContainer.Margin = new System.Windows.Forms.Padding(4);
            this.toolContainer.Name = "toolContainer";
            this.toolContainer.Size = new System.Drawing.Size(686, 55);
            this.toolContainer.TabIndex = 0;
            this.toolContainer.Text = "toolStripContainer1";
            // 
            // toolContainer.TopToolStripPanel
            // 
            this.toolContainer.TopToolStripPanel.Controls.Add(this.menuMain);
            // 
            // toolStripMain
            // 
            this.toolStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmdScanGame,
            this.toolStripSeparator1,
            this.toolStripButton2,
            this.toolStripButton1,
            this.toolStripSeparator2,
            this.labGameScanState});
            this.toolStripMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.toolStripMain.Size = new System.Drawing.Size(686, 31);
            this.toolStripMain.TabIndex = 1;
            this.toolStripMain.Text = "toolStrip1";
            // 
            // cmdScanGame
            // 
            this.cmdScanGame.Image = ((System.Drawing.Image)(resources.GetObject("cmdScanGame.Image")));
            this.cmdScanGame.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdScanGame.Name = "cmdScanGame";
            this.cmdScanGame.Size = new System.Drawing.Size(93, 24);
            this.cmdScanGame.Text = "查找游戏";
            this.cmdScanGame.Click += new System.EventHandler(this.cmdScanGame_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(90, 24);
            this.toolStripButton2.Text = "刷新(&F5)";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(90, 24);
            this.toolStripButton1.Text = "修改(&F6)";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // labGameScanState
            // 
            this.labGameScanState.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labGameScanState.Name = "labGameScanState";
            this.labGameScanState.Size = new System.Drawing.Size(84, 24);
            this.labGameScanState.Text = "游戏未运行";
            // 
            // menuMain
            // 
            this.menuMain.Dock = System.Windows.Forms.DockStyle.None;
            this.menuMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.menuMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuAbility,
            this.置顶ToolStripMenuItem,
            this.menuHelp});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuMain.Size = new System.Drawing.Size(686, 28);
            this.menuMain.TabIndex = 2;
            this.menuMain.Text = "menuStrip1";
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuDebug1,
            this.menuSplit1,
            this.menuFileExit});
            this.menuFile.Image = ((System.Drawing.Image)(resources.GetObject("menuFile.Image")));
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(91, 24);
            this.menuFile.Text = "文件(&F)";
            // 
            // menuDebug1
            // 
            this.menuDebug1.Name = "menuDebug1";
            this.menuDebug1.Size = new System.Drawing.Size(337, 26);
            this.menuDebug1.Text = "调试专用-ReadFromGameMemory";
            this.menuDebug1.Click += new System.EventHandler(this.menuDebug1_Click);
            // 
            // menuSplit1
            // 
            this.menuSplit1.Name = "menuSplit1";
            this.menuSplit1.Size = new System.Drawing.Size(334, 6);
            // 
            // menuFileExit
            // 
            this.menuFileExit.Name = "menuFileExit";
            this.menuFileExit.Size = new System.Drawing.Size(337, 26);
            this.menuFileExit.Text = "退出(&X)";
            this.menuFileExit.Click += new System.EventHandler(this.MenuFileExit_Click);
            //
            // menuAbility
            //
            this.menuAbility.Name = "menuAbility";
            this.menuAbility.Size = new System.Drawing.Size(51, 24);
            this.menuAbility.Text = "技能";
            this.menuAbility.Click += new System.EventHandler(this.menuAbility_Click);
            //
            // 置顶ToolStripMenuItem
            // 
            this.置顶ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.启用ToolStripMenuItem,
            this.toolStripMenuItem1,
            this.解除修改限制ToolStripMenuItem,
            this.toolStripMenuItem2,
            this.修改倍数ToolStripMenuItem});
            this.置顶ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("置顶ToolStripMenuItem.Image")));
            this.置顶ToolStripMenuItem.Name = "置顶ToolStripMenuItem";
            this.置顶ToolStripMenuItem.Size = new System.Drawing.Size(98, 24);
            this.置顶ToolStripMenuItem.Text = "其他(&W)";
            // 
            // 启用ToolStripMenuItem
            // 
            this.启用ToolStripMenuItem.CheckOnClick = true;
            this.启用ToolStripMenuItem.Name = "启用ToolStripMenuItem";
            this.启用ToolStripMenuItem.Size = new System.Drawing.Size(182, 26);
            this.启用ToolStripMenuItem.Text = "窗口总在最前";
            this.启用ToolStripMenuItem.Click += new System.EventHandler(this.启用ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(179, 6);
            // 
            // 解除修改限制ToolStripMenuItem
            // 
            this.解除修改限制ToolStripMenuItem.CheckOnClick = true;
            this.解除修改限制ToolStripMenuItem.Name = "解除修改限制ToolStripMenuItem";
            this.解除修改限制ToolStripMenuItem.Size = new System.Drawing.Size(182, 26);
            this.解除修改限制ToolStripMenuItem.Text = "加大修改长度";
            this.解除修改限制ToolStripMenuItem.Click += new System.EventHandler(this.解除修改限制ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(179, 6);
            // 
            // 修改倍数ToolStripMenuItem
            // 
            this.修改倍数ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.xToolStripMenuItem,
            this.toolStripMenuItem3,
            this.xToolStripMenuItem1,
            this.toolStripMenuItem4,
            this.xToolStripMenuItem2,
            this.toolStripMenuItem5,
            this.toolStripMenuItem7,
            this.toolStripMenuItem6,
            this.xToolStripMenuItem3});
            this.修改倍数ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("修改倍数ToolStripMenuItem.Image")));
            this.修改倍数ToolStripMenuItem.Name = "修改倍数ToolStripMenuItem";
            this.修改倍数ToolStripMenuItem.Size = new System.Drawing.Size(182, 26);
            this.修改倍数ToolStripMenuItem.Text = "数值倍数修改";
            // 
            // xToolStripMenuItem
            // 
            this.xToolStripMenuItem.Checked = true;
            this.xToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.xToolStripMenuItem.Name = "xToolStripMenuItem";
            this.xToolStripMenuItem.Size = new System.Drawing.Size(109, 26);
            this.xToolStripMenuItem.Text = "1x";
            this.xToolStripMenuItem.Click += new System.EventHandler(this.GroupedToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(106, 6);
            // 
            // xToolStripMenuItem1
            // 
            this.xToolStripMenuItem1.Name = "xToolStripMenuItem1";
            this.xToolStripMenuItem1.Size = new System.Drawing.Size(109, 26);
            this.xToolStripMenuItem1.Text = "2x";
            this.xToolStripMenuItem1.Click += new System.EventHandler(this.GroupedToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(106, 6);
            // 
            // xToolStripMenuItem2
            // 
            this.xToolStripMenuItem2.Name = "xToolStripMenuItem2";
            this.xToolStripMenuItem2.Size = new System.Drawing.Size(109, 26);
            this.xToolStripMenuItem2.Text = "3x";
            this.xToolStripMenuItem2.Click += new System.EventHandler(this.GroupedToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(106, 6);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(109, 26);
            this.toolStripMenuItem7.Text = "4x";
            this.toolStripMenuItem7.Click += new System.EventHandler(this.GroupedToolStripMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(106, 6);
            // 
            // xToolStripMenuItem3
            // 
            this.xToolStripMenuItem3.Name = "xToolStripMenuItem3";
            this.xToolStripMenuItem3.Size = new System.Drawing.Size(109, 26);
            this.xToolStripMenuItem3.Text = "5x";
            this.xToolStripMenuItem3.Click += new System.EventHandler(this.GroupedToolStripMenuItem_Click);
            // 
            // menuHelp
            // 
            this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuHelpAbout});
            this.menuHelp.Image = ((System.Drawing.Image)(resources.GetObject("menuHelp.Image")));
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(95, 24);
            this.menuHelp.Text = "帮助(&H)";
            // 
            // menuHelpAbout
            // 
            this.menuHelpAbout.Name = "menuHelpAbout";
            this.menuHelpAbout.Size = new System.Drawing.Size(188, 26);
            this.menuHelpAbout.Text = "关于修改器(&A)";
            this.menuHelpAbout.Click += new System.EventHandler(this.MenuHelpAbout_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "0.png");
            this.imageList1.Images.SetKeyName(1, "1.png");
            this.imageList1.Images.SetKeyName(2, "2.png");
            // 
            // splitMain
            // 
            this.splitMain.Location = new System.Drawing.Point(1, 59);
            this.splitMain.Margin = new System.Windows.Forms.Padding(4);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.viewFunctions);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.txtInput);
            this.splitMain.Panel2.Controls.Add(this.viewData);
            this.splitMain.Size = new System.Drawing.Size(684, 658);
            this.splitMain.SplitterDistance = 279;
            this.splitMain.SplitterWidth = 1;
            this.splitMain.TabIndex = 7;
            // 
            // viewFunctions
            // 
            this.viewFunctions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewFunctions.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.viewFunctions.HideSelection = false;
            this.viewFunctions.ImageIndex = 0;
            this.viewFunctions.ImageList = this.imageList1;
            this.viewFunctions.Location = new System.Drawing.Point(0, 0);
            this.viewFunctions.Margin = new System.Windows.Forms.Padding(4);
            this.viewFunctions.Name = "viewFunctions";
            this.viewFunctions.SelectedImageIndex = 0;
            this.viewFunctions.Size = new System.Drawing.Size(279, 658);
            this.viewFunctions.TabIndex = 3;
            this.viewFunctions.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.viewFunctions_BeforeSelect);
            this.viewFunctions.KeyDown += new System.Windows.Forms.KeyEventHandler(this.viewFunctions_KeyDown);
            // 
            // grpAbilityCommand
            // 
            this.grpAbilityCommand.Controls.Add(this.labAbilityCommandState);
            this.grpAbilityCommand.Controls.Add(this.cmdRemoveGroupTalent);
            this.grpAbilityCommand.Controls.Add(this.cmdAddGroupTalent);
            this.grpAbilityCommand.Controls.Add(this.cmdRemoveGroupAbility);
            this.grpAbilityCommand.Controls.Add(this.cmdAddGroupAbility);
            this.grpAbilityCommand.Controls.Add(this.lstAbilityGroupItems);
            this.grpAbilityCommand.Controls.Add(this.cboAbilityGroup);
            this.grpAbilityCommand.Controls.Add(this.labAbilityGroup);
            this.grpAbilityCommand.Controls.Add(this.cmdRemoveTalent);
            this.grpAbilityCommand.Controls.Add(this.cmdAddTalent);
            this.grpAbilityCommand.Controls.Add(this.cmdRemoveAbility);
            this.grpAbilityCommand.Controls.Add(this.cmdAddAbility);
            this.grpAbilityCommand.Controls.Add(this.numAbilityLevel);
            this.grpAbilityCommand.Controls.Add(this.labAbilityLevel);
            this.grpAbilityCommand.Controls.Add(this.cboAbilityId);
            this.grpAbilityCommand.Controls.Add(this.labAbilityId);
            this.grpAbilityCommand.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.grpAbilityCommand.Location = new System.Drawing.Point(0, 0);
            this.grpAbilityCommand.Margin = new System.Windows.Forms.Padding(4);
            this.grpAbilityCommand.Name = "grpAbilityCommand";
            this.grpAbilityCommand.Padding = new System.Windows.Forms.Padding(4);
            this.grpAbilityCommand.Size = new System.Drawing.Size(404, 270);
            this.grpAbilityCommand.TabIndex = 11;
            this.grpAbilityCommand.TabStop = false;
            this.grpAbilityCommand.Text = "JASS技能";
            // 
            // labAbilityCommandState
            // 
            this.labAbilityCommandState.AutoSize = true;
            this.labAbilityCommandState.Location = new System.Drawing.Point(9, 244);
            this.labAbilityCommandState.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labAbilityCommandState.Name = "labAbilityCommandState";
            this.labAbilityCommandState.Size = new System.Drawing.Size(219, 20);
            this.labAbilityCommandState.TabIndex = 6;
            this.labAbilityCommandState.Text = "选择技能，代码中可继续添加";
            //
            // cmdRemoveGroupTalent
            //
            this.cmdRemoveGroupTalent.Location = new System.Drawing.Point(318, 124);
            this.cmdRemoveGroupTalent.Margin = new System.Windows.Forms.Padding(4);
            this.cmdRemoveGroupTalent.Name = "cmdRemoveGroupTalent";
            this.cmdRemoveGroupTalent.Size = new System.Drawing.Size(78, 29);
            this.cmdRemoveGroupTalent.TabIndex = 15;
            this.cmdRemoveGroupTalent.Text = "删组天赋";
            this.cmdRemoveGroupTalent.UseVisualStyleBackColor = true;
            this.cmdRemoveGroupTalent.Click += new System.EventHandler(this.cmdRemoveGroupTalent_Click);
            //
            // cmdAddGroupTalent
            //
            this.cmdAddGroupTalent.Location = new System.Drawing.Point(232, 124);
            this.cmdAddGroupTalent.Margin = new System.Windows.Forms.Padding(4);
            this.cmdAddGroupTalent.Name = "cmdAddGroupTalent";
            this.cmdAddGroupTalent.Size = new System.Drawing.Size(78, 29);
            this.cmdAddGroupTalent.TabIndex = 14;
            this.cmdAddGroupTalent.Text = "添组天赋";
            this.cmdAddGroupTalent.UseVisualStyleBackColor = true;
            this.cmdAddGroupTalent.Click += new System.EventHandler(this.cmdAddGroupTalent_Click);
            //
            // cmdRemoveGroupAbility
            //
            this.cmdRemoveGroupAbility.Location = new System.Drawing.Point(318, 91);
            this.cmdRemoveGroupAbility.Margin = new System.Windows.Forms.Padding(4);
            this.cmdRemoveGroupAbility.Name = "cmdRemoveGroupAbility";
            this.cmdRemoveGroupAbility.Size = new System.Drawing.Size(78, 29);
            this.cmdRemoveGroupAbility.TabIndex = 13;
            this.cmdRemoveGroupAbility.Text = "删组技能";
            this.cmdRemoveGroupAbility.UseVisualStyleBackColor = true;
            this.cmdRemoveGroupAbility.Click += new System.EventHandler(this.cmdRemoveGroupAbility_Click);
            //
            // cmdAddGroupAbility
            //
            this.cmdAddGroupAbility.Location = new System.Drawing.Point(232, 91);
            this.cmdAddGroupAbility.Margin = new System.Windows.Forms.Padding(4);
            this.cmdAddGroupAbility.Name = "cmdAddGroupAbility";
            this.cmdAddGroupAbility.Size = new System.Drawing.Size(78, 29);
            this.cmdAddGroupAbility.TabIndex = 12;
            this.cmdAddGroupAbility.Text = "添组技能";
            this.cmdAddGroupAbility.UseVisualStyleBackColor = true;
            this.cmdAddGroupAbility.Click += new System.EventHandler(this.cmdAddGroupAbility_Click);
            //
            // lstAbilityGroupItems
            //
            this.lstAbilityGroupItems.FormattingEnabled = true;
            this.lstAbilityGroupItems.ItemHeight = 20;
            this.lstAbilityGroupItems.Location = new System.Drawing.Point(13, 126);
            this.lstAbilityGroupItems.Margin = new System.Windows.Forms.Padding(4);
            this.lstAbilityGroupItems.Name = "lstAbilityGroupItems";
            this.lstAbilityGroupItems.Size = new System.Drawing.Size(211, 104);
            this.lstAbilityGroupItems.TabIndex = 11;
            //
            // cboAbilityGroup
            //
            this.cboAbilityGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAbilityGroup.FormattingEnabled = true;
            this.cboAbilityGroup.Location = new System.Drawing.Point(59, 92);
            this.cboAbilityGroup.Margin = new System.Windows.Forms.Padding(4);
            this.cboAbilityGroup.Name = "cboAbilityGroup";
            this.cboAbilityGroup.Size = new System.Drawing.Size(165, 28);
            this.cboAbilityGroup.TabIndex = 10;
            this.cboAbilityGroup.SelectedIndexChanged += new System.EventHandler(this.cboAbilityGroup_SelectedIndexChanged);
            //
            // labAbilityGroup
            //
            this.labAbilityGroup.AutoSize = true;
            this.labAbilityGroup.Location = new System.Drawing.Point(9, 95);
            this.labAbilityGroup.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labAbilityGroup.Name = "labAbilityGroup";
            this.labAbilityGroup.Size = new System.Drawing.Size(39, 20);
            this.labAbilityGroup.TabIndex = 9;
            this.labAbilityGroup.Text = "分组";
            //
            // cmdRemoveTalent
            //
            this.cmdRemoveTalent.Location = new System.Drawing.Point(318, 57);
            this.cmdRemoveTalent.Margin = new System.Windows.Forms.Padding(4);
            this.cmdRemoveTalent.Name = "cmdRemoveTalent";
            this.cmdRemoveTalent.Size = new System.Drawing.Size(78, 29);
            this.cmdRemoveTalent.TabIndex = 8;
            this.cmdRemoveTalent.Text = "删除天赋";
            this.cmdRemoveTalent.UseVisualStyleBackColor = true;
            this.cmdRemoveTalent.Click += new System.EventHandler(this.cmdRemoveTalent_Click);
            //
            // cmdAddTalent
            //
            this.cmdAddTalent.Location = new System.Drawing.Point(232, 57);
            this.cmdAddTalent.Margin = new System.Windows.Forms.Padding(4);
            this.cmdAddTalent.Name = "cmdAddTalent";
            this.cmdAddTalent.Size = new System.Drawing.Size(78, 29);
            this.cmdAddTalent.TabIndex = 7;
            this.cmdAddTalent.Text = "添加天赋";
            this.cmdAddTalent.UseVisualStyleBackColor = true;
            this.cmdAddTalent.Click += new System.EventHandler(this.cmdAddTalent_Click);
            //
            // cmdRemoveAbility
            // 
            this.cmdRemoveAbility.Location = new System.Drawing.Point(318, 24);
            this.cmdRemoveAbility.Margin = new System.Windows.Forms.Padding(4);
            this.cmdRemoveAbility.Name = "cmdRemoveAbility";
            this.cmdRemoveAbility.Size = new System.Drawing.Size(78, 29);
            this.cmdRemoveAbility.TabIndex = 5;
            this.cmdRemoveAbility.Text = "删除";
            this.cmdRemoveAbility.UseVisualStyleBackColor = true;
            this.cmdRemoveAbility.Click += new System.EventHandler(this.cmdRemoveAbility_Click);
            // 
            // cmdAddAbility
            // 
            this.cmdAddAbility.Location = new System.Drawing.Point(232, 24);
            this.cmdAddAbility.Margin = new System.Windows.Forms.Padding(4);
            this.cmdAddAbility.Name = "cmdAddAbility";
            this.cmdAddAbility.Size = new System.Drawing.Size(78, 29);
            this.cmdAddAbility.TabIndex = 4;
            this.cmdAddAbility.Text = "添加";
            this.cmdAddAbility.UseVisualStyleBackColor = true;
            this.cmdAddAbility.Click += new System.EventHandler(this.cmdAddAbility_Click);
            // 
            // numAbilityLevel
            // 
            this.numAbilityLevel.Location = new System.Drawing.Point(175, 25);
            this.numAbilityLevel.Margin = new System.Windows.Forms.Padding(4);
            this.numAbilityLevel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numAbilityLevel.Name = "numAbilityLevel";
            this.numAbilityLevel.Size = new System.Drawing.Size(49, 27);
            this.numAbilityLevel.TabIndex = 3;
            this.numAbilityLevel.Value = new decimal(new int[] {
            99,
            0,
            0,
            0});
            // 
            // labAbilityLevel
            // 
            this.labAbilityLevel.AutoSize = true;
            this.labAbilityLevel.Location = new System.Drawing.Point(132, 28);
            this.labAbilityLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labAbilityLevel.Name = "labAbilityLevel";
            this.labAbilityLevel.Size = new System.Drawing.Size(39, 20);
            this.labAbilityLevel.TabIndex = 2;
            this.labAbilityLevel.Text = "等级";
            // 
            // cboAbilityId
            // 
            this.cboAbilityId.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAbilityId.FormattingEnabled = true;
            this.cboAbilityId.Location = new System.Drawing.Point(59, 25);
            this.cboAbilityId.Margin = new System.Windows.Forms.Padding(4);
            this.cboAbilityId.Name = "cboAbilityId";
            this.cboAbilityId.Size = new System.Drawing.Size(65, 28);
            this.cboAbilityId.TabIndex = 1;
            // 
            // labAbilityId
            // 
            this.labAbilityId.AutoSize = true;
            this.labAbilityId.Location = new System.Drawing.Point(9, 28);
            this.labAbilityId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labAbilityId.Name = "labAbilityId";
            this.labAbilityId.Size = new System.Drawing.Size(44, 20);
            this.labAbilityId.TabIndex = 0;
            this.labAbilityId.Text = "技能";
            // 
            // txtInput
            // 
            this.txtInput.Location = new System.Drawing.Point(76, 65);
            this.txtInput.Margin = new System.Windows.Forms.Padding(0);
            this.txtInput.MaximumSize = new System.Drawing.Size(49708, 16);
            this.txtInput.MaxLength = 7;
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(272, 25);
            this.txtInput.TabIndex = 10;
            this.txtInput.Text = "数值在这里";
            this.txtInput.Visible = false;
            this.txtInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInput_KeyDown);
            this.txtInput.Leave += new System.EventHandler(this.txtInput_Leave);
            // 
            // viewData
            // 
            this.viewData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colOriginalValue,
            this.colUnsavedValue});
            this.viewData.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.viewData.FullRowSelect = true;
            this.viewData.GridLines = true;
            this.viewData.HideSelection = false;
            this.viewData.Location = new System.Drawing.Point(0, 0);
            this.viewData.Margin = new System.Windows.Forms.Padding(4);
            this.viewData.MultiSelect = false;
            this.viewData.Name = "viewData";
            this.viewData.Size = new System.Drawing.Size(404, 658);
            this.viewData.SmallImageList = this.imageList1;
            this.viewData.TabIndex = 9;
            this.viewData.UseCompatibleStateImageBehavior = false;
            this.viewData.View = System.Windows.Forms.View.Details;
            this.viewData.Scrolling += new System.EventHandler(this.viewData_Scrolling);
            this.viewData.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.viewData_ColumnWidthChanging);
            this.viewData.KeyDown += new System.Windows.Forms.KeyEventHandler(this.viewData_KeyDown);
            this.viewData.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.viewData_KeyPress);
            this.viewData.MouseUp += new System.Windows.Forms.MouseEventHandler(this.viewData_MouseUp);
            // 
            // colName
            // 
            this.colName.Text = "名称";
            this.colName.Width = 160;
            // 
            // colOriginalValue
            // 
            this.colOriginalValue.Text = "数值";
            this.colOriginalValue.Width = 80;
            // 
            // colUnsavedValue
            // 
            this.colUnsavedValue.Text = "修改";
            this.colUnsavedValue.Width = 100;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(686, 718);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.toolContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Warcraft III Frozen Throne";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.toolContainer.ContentPanel.ResumeLayout(false);
            this.toolContainer.ContentPanel.PerformLayout();
            this.toolContainer.TopToolStripPanel.ResumeLayout(false);
            this.toolContainer.TopToolStripPanel.PerformLayout();
            this.toolContainer.ResumeLayout(false);
            this.toolContainer.PerformLayout();
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            this.splitMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.grpAbilityCommand.ResumeLayout(false);
            this.grpAbilityCommand.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAbilityLevel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolContainer;
        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuAbility;
        private System.Windows.Forms.ToolStripMenuItem menuFileExit;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStripMenuItem menuHelpAbout;
        private ListViewEx viewData;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colOriginalValue;
        private System.Windows.Forms.ColumnHeader colUnsavedValue;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.ToolStripMenuItem menuDebug1;
        private System.Windows.Forms.ToolStripSeparator menuSplit1;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripMenuItem 置顶ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 启用ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 解除修改限制ToolStripMenuItem;
        private System.Windows.Forms.TreeView viewFunctions;
        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton cmdScanGame;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel labGameScanState;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem 修改倍数ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem3;
        private System.Windows.Forms.GroupBox grpAbilityCommand;
        private System.Windows.Forms.Label labAbilityCommandState;
        private System.Windows.Forms.Button cmdRemoveGroupTalent;
        private System.Windows.Forms.Button cmdAddGroupTalent;
        private System.Windows.Forms.Button cmdRemoveGroupAbility;
        private System.Windows.Forms.Button cmdAddGroupAbility;
        private System.Windows.Forms.ListBox lstAbilityGroupItems;
        private System.Windows.Forms.ComboBox cboAbilityGroup;
        private System.Windows.Forms.Label labAbilityGroup;
        private System.Windows.Forms.Button cmdRemoveTalent;
        private System.Windows.Forms.Button cmdAddTalent;
        private System.Windows.Forms.Button cmdRemoveAbility;
        private System.Windows.Forms.Button cmdAddAbility;
        private System.Windows.Forms.NumericUpDown numAbilityLevel;
        private System.Windows.Forms.Label labAbilityLevel;
        private System.Windows.Forms.ComboBox cboAbilityId;
        private System.Windows.Forms.Label labAbilityId;
    }
}
