namespace Heiflow.Controls.WinForm.Modflow
{
    partial class LayerGroupForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayerGroupForm));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.olvLayerGroup = new BrightIdeasSoftware.DataListView();
            this.colLayerName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colLayerType = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colLAYAVG = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colLAYVKA = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colCHANI = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colLAYWET = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPageInitial = new System.Windows.Forms.TabPage();
            this.olvLayersUniformProp = new BrightIdeasSoftware.DataListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn7 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn8 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn9 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn10 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn11 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn12 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnSetToUniform = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbLayerHeight = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbSS = new System.Windows.Forms.TextBox();
            this.tbHK = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbSY = new System.Windows.Forms.TextBox();
            this.tbVKA = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.olvLayerGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPageInitial.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvLayersUniformProp)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCancel.Location = new System.Drawing.Point(655, 321);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnOk.Location = new System.Drawing.Point(559, 321);
            this.btnOk.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(90, 30);
            this.btnOk.TabIndex = 17;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // olvLayerGroup
            // 
            this.olvLayerGroup.AllColumns.Add(this.colLayerName);
            this.olvLayerGroup.AllColumns.Add(this.colLayerType);
            this.olvLayerGroup.AllColumns.Add(this.colLAYAVG);
            this.olvLayerGroup.AllColumns.Add(this.colLAYVKA);
            this.olvLayerGroup.AllColumns.Add(this.colCHANI);
            this.olvLayerGroup.AllColumns.Add(this.colLAYWET);
            this.olvLayerGroup.AllowColumnReorder = true;
            this.olvLayerGroup.AllowDrop = true;
            this.olvLayerGroup.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.olvLayerGroup.CellEditUseWholeCell = false;
            this.olvLayerGroup.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colLayerName,
            this.colLayerType,
            this.colLAYAVG,
            this.colLAYVKA,
            this.colCHANI,
            this.colLAYWET});
            this.olvLayerGroup.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvLayerGroup.DataSource = null;
            this.olvLayerGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvLayerGroup.EmptyListMsg = "";
            this.olvLayerGroup.EmptyListMsgFont = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvLayerGroup.FullRowSelect = true;
            this.olvLayerGroup.GridLines = true;
            this.olvLayerGroup.GroupWithItemCountFormat = "";
            this.olvLayerGroup.GroupWithItemCountSingularFormat = "";
            this.olvLayerGroup.HideSelection = false;
            this.olvLayerGroup.Location = new System.Drawing.Point(3, 3);
            this.olvLayerGroup.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.olvLayerGroup.Name = "olvLayerGroup";
            this.olvLayerGroup.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvLayerGroup.SelectedBackColor = System.Drawing.Color.Pink;
            this.olvLayerGroup.SelectedForeColor = System.Drawing.Color.MidnightBlue;
            this.olvLayerGroup.ShowCommandMenuOnRightClick = true;
            this.olvLayerGroup.ShowGroups = false;
            this.olvLayerGroup.ShowImagesOnSubItems = true;
            this.olvLayerGroup.ShowItemToolTips = true;
            this.olvLayerGroup.Size = new System.Drawing.Size(747, 266);
            this.olvLayerGroup.TabIndex = 21;
            this.olvLayerGroup.UseCellFormatEvents = true;
            this.olvLayerGroup.UseCompatibleStateImageBehavior = false;
            this.olvLayerGroup.UseFilterIndicator = true;
            this.olvLayerGroup.UseFiltering = true;
            this.olvLayerGroup.UseHotItem = true;
            this.olvLayerGroup.UseTranslucentHotItem = true;
            this.olvLayerGroup.View = System.Windows.Forms.View.Details;
            this.olvLayerGroup.CellEditFinished += new BrightIdeasSoftware.CellEditEventHandler(this.olvLayerGroup_CellEditFinished);
            this.olvLayerGroup.ItemsChanged += new System.EventHandler<BrightIdeasSoftware.ItemsChangedEventArgs>(this.olvLayerGroup_ItemsChanged);
            // 
            // colLayerName
            // 
            this.colLayerName.AspectName = "Name";
            this.colLayerName.ButtonPadding = new System.Drawing.Size(10, 10);
            this.colLayerName.CellEditUseWholeCell = true;
            this.colLayerName.IsTileViewColumn = true;
            this.colLayerName.Text = "Layer Name";
            this.colLayerName.UseInitialLetterForGroup = true;
            this.colLayerName.Width = 115;
            // 
            // colLayerType
            // 
            this.colLayerType.AspectName = "LAYTYP";
            this.colLayerType.ButtonPadding = new System.Drawing.Size(10, 10);
            this.colLayerType.CellEditUseWholeCell = true;
            this.colLayerType.IsTileViewColumn = true;
            this.colLayerType.Text = "Layer Type";
            this.colLayerType.Width = 117;
            // 
            // colLAYAVG
            // 
            this.colLAYAVG.AspectName = "LAYAVG";
            this.colLAYAVG.CellEditUseWholeCell = true;
            this.colLAYAVG.Text = "LAYAVG";
            this.colLAYAVG.ToolTipText = "Method of calculating interblock transmissivity";
            this.colLAYAVG.Width = 139;
            // 
            // colLAYVKA
            // 
            this.colLAYVKA.AspectName = "LAYVKA";
            this.colLAYVKA.CellEditUseWholeCell = true;
            this.colLAYVKA.Text = "LAYVKA";
            this.colLAYVKA.ToolTipText = "Method of specifying vertical hydraulic conductivity";
            this.colLAYVKA.Width = 124;
            // 
            // colCHANI
            // 
            this.colCHANI.AspectName = "CHANI";
            this.colCHANI.CellEditUseWholeCell = true;
            this.colCHANI.Text = "CHANI";
            this.colCHANI.ToolTipText = "a value for each layer that is a flag or the horizontal anisotropy";
            this.colCHANI.Width = 107;
            // 
            // colLAYWET
            // 
            this.colLAYWET.AspectName = "LAYWET";
            this.colLAYWET.CellEditUseWholeCell = true;
            this.colLAYWET.Text = "LAYWET";
            this.colLAYWET.ToolTipText = "Indicates if wetting is active.";
            this.colLAYWET.Width = 128;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numericUpDown1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numericUpDown1.Location = new System.Drawing.Point(186, 321);
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(68, 27);
            this.numericUpDown1.TabIndex = 25;
            this.numericUpDown1.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label6.Location = new System.Drawing.Point(8, 324);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(175, 20);
            this.label6.TabIndex = 24;
            this.label6.Text = "Number of vertical layers";
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnAdd.Location = new System.Drawing.Point(261, 321);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(90, 30);
            this.btnAdd.TabIndex = 17;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemove.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnRemove.Location = new System.Drawing.Point(357, 321);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(90, 30);
            this.btnRemove.TabIndex = 17;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPageInitial);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(11, 7);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(761, 305);
            this.tabControl1.TabIndex = 26;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.olvLayerGroup);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(753, 272);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General Properties";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPageInitial
            // 
            this.tabPageInitial.Controls.Add(this.olvLayersUniformProp);
            this.tabPageInitial.Location = new System.Drawing.Point(4, 29);
            this.tabPageInitial.Name = "tabPageInitial";
            this.tabPageInitial.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageInitial.Size = new System.Drawing.Size(753, 272);
            this.tabPageInitial.TabIndex = 1;
            this.tabPageInitial.Text = "Uniform Properties";
            this.tabPageInitial.UseVisualStyleBackColor = true;
            // 
            // olvLayersUniformProp
            // 
            this.olvLayersUniformProp.AllColumns.Add(this.olvColumn1);
            this.olvLayersUniformProp.AllColumns.Add(this.olvColumn7);
            this.olvLayersUniformProp.AllColumns.Add(this.olvColumn8);
            this.olvLayersUniformProp.AllColumns.Add(this.olvColumn9);
            this.olvLayersUniformProp.AllColumns.Add(this.olvColumn10);
            this.olvLayersUniformProp.AllColumns.Add(this.olvColumn11);
            this.olvLayersUniformProp.AllColumns.Add(this.olvColumn12);
            this.olvLayersUniformProp.AllowColumnReorder = true;
            this.olvLayersUniformProp.AllowDrop = true;
            this.olvLayersUniformProp.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.olvLayersUniformProp.CellEditUseWholeCell = false;
            this.olvLayersUniformProp.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn7,
            this.olvColumn8,
            this.olvColumn9,
            this.olvColumn10,
            this.olvColumn11,
            this.olvColumn12});
            this.olvLayersUniformProp.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvLayersUniformProp.DataSource = null;
            this.olvLayersUniformProp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvLayersUniformProp.EmptyListMsg = "";
            this.olvLayersUniformProp.EmptyListMsgFont = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvLayersUniformProp.FullRowSelect = true;
            this.olvLayersUniformProp.GridLines = true;
            this.olvLayersUniformProp.GroupWithItemCountFormat = "";
            this.olvLayersUniformProp.GroupWithItemCountSingularFormat = "";
            this.olvLayersUniformProp.HideSelection = false;
            this.olvLayersUniformProp.Location = new System.Drawing.Point(3, 3);
            this.olvLayersUniformProp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.olvLayersUniformProp.Name = "olvLayersUniformProp";
            this.olvLayersUniformProp.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.olvLayersUniformProp.SelectedBackColor = System.Drawing.Color.Pink;
            this.olvLayersUniformProp.SelectedForeColor = System.Drawing.Color.MidnightBlue;
            this.olvLayersUniformProp.ShowCommandMenuOnRightClick = true;
            this.olvLayersUniformProp.ShowGroups = false;
            this.olvLayersUniformProp.ShowImagesOnSubItems = true;
            this.olvLayersUniformProp.ShowItemToolTips = true;
            this.olvLayersUniformProp.Size = new System.Drawing.Size(747, 266);
            this.olvLayersUniformProp.TabIndex = 22;
            this.olvLayersUniformProp.UseCellFormatEvents = true;
            this.olvLayersUniformProp.UseCompatibleStateImageBehavior = false;
            this.olvLayersUniformProp.UseFilterIndicator = true;
            this.olvLayersUniformProp.UseFiltering = true;
            this.olvLayersUniformProp.UseHotItem = true;
            this.olvLayersUniformProp.UseTranslucentHotItem = true;
            this.olvLayersUniformProp.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "Name";
            this.olvColumn1.ButtonPadding = new System.Drawing.Size(10, 10);
            this.olvColumn1.CellEditUseWholeCell = true;
            this.olvColumn1.IsTileViewColumn = true;
            this.olvColumn1.Text = "Layer Name";
            this.olvColumn1.UseInitialLetterForGroup = true;
            this.olvColumn1.Width = 126;
            // 
            // olvColumn7
            // 
            this.olvColumn7.AspectName = "LayerHeight";
            this.olvColumn7.CellEditUseWholeCell = true;
            this.olvColumn7.Text = "Layer Height";
            this.olvColumn7.Width = 109;
            // 
            // olvColumn8
            // 
            this.olvColumn8.AspectName = "HK";
            this.olvColumn8.CellEditUseWholeCell = true;
            this.olvColumn8.Text = "HK";
            this.olvColumn8.Width = 72;
            // 
            // olvColumn9
            // 
            this.olvColumn9.AspectName = "VKA";
            this.olvColumn9.CellEditUseWholeCell = true;
            this.olvColumn9.Text = "VKA";
            this.olvColumn9.Width = 77;
            // 
            // olvColumn10
            // 
            this.olvColumn10.AspectName = "SY";
            this.olvColumn10.CellEditUseWholeCell = true;
            this.olvColumn10.Text = "SY";
            this.olvColumn10.Width = 85;
            // 
            // olvColumn11
            // 
            this.olvColumn11.AspectName = "SS";
            this.olvColumn11.CellEditUseWholeCell = true;
            this.olvColumn11.Text = "SS";
            this.olvColumn11.Width = 97;
            // 
            // olvColumn12
            // 
            this.olvColumn12.AspectName = "WETDRY";
            this.olvColumn12.CellEditUseWholeCell = true;
            this.olvColumn12.Text = "WETDRY";
            this.olvColumn12.Width = 174;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnSetToUniform);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.tbLayerHeight);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.tbSS);
            this.tabPage2.Controls.Add(this.tbHK);
            this.tabPage2.Controls.Add(this.label15);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.tbSY);
            this.tabPage2.Controls.Add(this.tbVKA);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(753, 272);
            this.tabPage2.TabIndex = 2;
            this.tabPage2.Text = "Uniform Values";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnSetToUniform
            // 
            this.btnSetToUniform.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSetToUniform.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSetToUniform.Location = new System.Drawing.Point(152, 203);
            this.btnSetToUniform.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnSetToUniform.Name = "btnSetToUniform";
            this.btnSetToUniform.Size = new System.Drawing.Size(172, 30);
            this.btnSetToUniform.TabIndex = 28;
            this.btnSetToUniform.Text = "Set to Uniform Values";
            this.btnSetToUniform.UseVisualStyleBackColor = true;
            this.btnSetToUniform.Click += new System.EventHandler(this.btnSetToUniform_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 20);
            this.label2.TabIndex = 18;
            this.label2.Text = "Layer Height (m)";
            // 
            // tbLayerHeight
            // 
            this.tbLayerHeight.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tbLayerHeight.Location = new System.Drawing.Point(152, 15);
            this.tbLayerHeight.Name = "tbLayerHeight";
            this.tbLayerHeight.Size = new System.Drawing.Size(363, 27);
            this.tbLayerHeight.TabIndex = 23;
            this.tbLayerHeight.Text = "20";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(73, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 20);
            this.label1.TabIndex = 19;
            this.label1.Text = "HK (m/d)";
            // 
            // tbSS
            // 
            this.tbSS.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tbSS.Location = new System.Drawing.Point(152, 154);
            this.tbSS.Name = "tbSS";
            this.tbSS.Size = new System.Drawing.Size(363, 27);
            this.tbSS.TabIndex = 24;
            this.tbSS.Text = "0.0001";
            // 
            // tbHK
            // 
            this.tbHK.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tbHK.Location = new System.Drawing.Point(152, 50);
            this.tbHK.Name = "tbHK";
            this.tbHK.Size = new System.Drawing.Size(363, 27);
            this.tbHK.TabIndex = 25;
            this.tbHK.Text = "10";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(119, 157);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(25, 20);
            this.label15.TabIndex = 20;
            this.label15.Text = "SS";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(107, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 20);
            this.label3.TabIndex = 21;
            this.label3.Text = "VKA";
            // 
            // tbSY
            // 
            this.tbSY.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tbSY.Location = new System.Drawing.Point(152, 119);
            this.tbSY.Name = "tbSY";
            this.tbSY.Size = new System.Drawing.Size(363, 27);
            this.tbSY.TabIndex = 26;
            this.tbSY.Text = "0.1";
            // 
            // tbVKA
            // 
            this.tbVKA.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tbVKA.Location = new System.Drawing.Point(152, 84);
            this.tbVKA.Name = "tbVKA";
            this.tbVKA.Size = new System.Drawing.Size(363, 27);
            this.tbVKA.TabIndex = 27;
            this.tbVKA.Text = "100";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(119, 122);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 20);
            this.label4.TabIndex = 22;
            this.label4.Text = "SY";
            // 
            // LayerGroupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(771, 358);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnOk);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "LayerGroupForm";
            this.ShowInTaskbar = false;
            this.Text = "Aquifer Layer Groups";
            this.Load += new System.EventHandler(this.LayerGroupForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.olvLayerGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPageInitial.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvLayersUniformProp)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private BrightIdeasSoftware.DataListView olvLayerGroup;
        private BrightIdeasSoftware.OLVColumn colLayerName;
        private BrightIdeasSoftware.OLVColumn colLayerType;
        private BrightIdeasSoftware.OLVColumn colLAYAVG;
        private BrightIdeasSoftware.OLVColumn colLAYVKA;
        private BrightIdeasSoftware.OLVColumn colCHANI;
        private BrightIdeasSoftware.OLVColumn colLAYWET;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPageInitial;
        private BrightIdeasSoftware.DataListView olvLayersUniformProp;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn7;
        private BrightIdeasSoftware.OLVColumn olvColumn8;
        private BrightIdeasSoftware.OLVColumn olvColumn9;
        private BrightIdeasSoftware.OLVColumn olvColumn10;
        private BrightIdeasSoftware.OLVColumn olvColumn11;
        private BrightIdeasSoftware.OLVColumn olvColumn12;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbLayerHeight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbSS;
        private System.Windows.Forms.TextBox tbHK;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbSY;
        private System.Windows.Forms.TextBox tbVKA;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSetToUniform;
    }
}