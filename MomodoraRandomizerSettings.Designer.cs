
namespace LiveSplit.UI.Components
{
    partial class MomodoraRandomizerSettings
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSeed = new System.Windows.Forms.Button();
            this.chkRandom = new System.Windows.Forms.CheckBox();
            this.chkIvoryBugs = new System.Windows.Forms.CheckBox();
            this.chkVitality = new System.Windows.Forms.CheckBox();
            this.textSeed = new System.Windows.Forms.TextBox();
            this.chkHard = new System.Windows.Forms.CheckBox();
            this.groupBoxSeed = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxOptions = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxFont = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.lblFont = new System.Windows.Forms.Label();
            this.btnFont = new System.Windows.Forms.Button();
            this.chkFont = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxColor = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.chkColor = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnShadowColor = new System.Windows.Forms.Button();
            this.btnTextColor = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.btnColor1 = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.btnColor2 = new System.Windows.Forms.Button();
            this.cmbGradientType = new System.Windows.Forms.ComboBox();
            this.btnClipboard = new System.Windows.Forms.Button();
            this.groupBoxSeed.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.groupBoxOptions.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBoxFont.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.groupBoxColor.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSeed
            // 
            this.btnSeed.Location = new System.Drawing.Point(165, 28);
            this.btnSeed.Name = "btnSeed";
            this.btnSeed.Size = new System.Drawing.Size(88, 24);
            this.btnSeed.TabIndex = 0;
            this.btnSeed.Text = "Generate seed";
            this.btnSeed.UseVisualStyleBackColor = true;
            this.btnSeed.Click += new System.EventHandler(this.btnSeed_Click);
            // 
            // chkRandom
            // 
            this.chkRandom.AutoSize = true;
            this.chkRandom.Location = new System.Drawing.Point(3, 3);
            this.chkRandom.Name = "chkRandom";
            this.chkRandom.Size = new System.Drawing.Size(111, 17);
            this.chkRandom.TabIndex = 1;
            this.chkRandom.Text = "Use random Seed";
            this.chkRandom.UseVisualStyleBackColor = true;
            this.chkRandom.CheckedChanged += new System.EventHandler(this.UseRandomSeed_CheckedChanged);
            // 
            // chkIvoryBugs
            // 
            this.chkIvoryBugs.AutoSize = true;
            this.chkIvoryBugs.Location = new System.Drawing.Point(3, 27);
            this.chkIvoryBugs.Name = "chkIvoryBugs";
            this.chkIvoryBugs.Size = new System.Drawing.Size(114, 17);
            this.chkIvoryBugs.TabIndex = 4;
            this.chkIvoryBugs.Text = "Include Ivory Bugs";
            this.chkIvoryBugs.UseVisualStyleBackColor = true;
            // 
            // chkVitality
            // 
            this.chkVitality.AutoSize = true;
            this.chkVitality.Location = new System.Drawing.Point(3, 3);
            this.chkVitality.Name = "chkVitality";
            this.chkVitality.Size = new System.Drawing.Size(146, 17);
            this.chkVitality.TabIndex = 5;
            this.chkVitality.Text = "Include Vitality Fragments";
            this.chkVitality.UseVisualStyleBackColor = true;
            // 
            // textSeed
            // 
            this.textSeed.AllowDrop = true;
            this.textSeed.Enabled = false;
            this.textSeed.Location = new System.Drawing.Point(3, 28);
            this.textSeed.Name = "textSeed";
            this.textSeed.Size = new System.Drawing.Size(152, 20);
            this.textSeed.TabIndex = 44;
            // 
            // chkHard
            // 
            this.chkHard.AutoSize = true;
            this.chkHard.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkHard.ForeColor = System.Drawing.Color.DarkRed;
            this.chkHard.Location = new System.Drawing.Point(220, 3);
            this.chkHard.Name = "chkHard";
            this.chkHard.Size = new System.Drawing.Size(109, 18);
            this.chkHard.TabIndex = 45;
            this.chkHard.Text = "HARD MODE";
            this.chkHard.UseVisualStyleBackColor = true;
            this.chkHard.Click += new System.EventHandler(this.chkHard_CheckedChanged);
            // 
            // groupBoxSeed
            // 
            this.groupBoxSeed.Controls.Add(this.tableLayoutPanel5);
            this.groupBoxSeed.Location = new System.Drawing.Point(3, 3);
            this.groupBoxSeed.Name = "groupBoxSeed";
            this.groupBoxSeed.Size = new System.Drawing.Size(460, 82);
            this.groupBoxSeed.TabIndex = 4;
            this.groupBoxSeed.TabStop = false;
            this.groupBoxSeed.Text = "Seed";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 3;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 191F));
            this.tableLayoutPanel5.Controls.Add(this.chkRandom, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.textSeed, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.btnSeed, 1, 1);
            this.tableLayoutPanel5.Controls.Add(this.btnClipboard, 2, 1);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(6, 19);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(448, 57);
            this.tableLayoutPanel5.TabIndex = 2;
            // 
            // groupBoxOptions
            // 
            this.groupBoxOptions.Controls.Add(this.tableLayoutPanel3);
            this.groupBoxOptions.Location = new System.Drawing.Point(3, 91);
            this.groupBoxOptions.Name = "groupBoxOptions";
            this.groupBoxOptions.Size = new System.Drawing.Size(460, 75);
            this.groupBoxOptions.TabIndex = 3;
            this.groupBoxOptions.TabStop = false;
            this.groupBoxOptions.Text = "Options";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 153F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 231F));
            this.tableLayoutPanel3.Controls.Add(this.chkVitality, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.chkIvoryBugs, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.chkHard, 2, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(6, 19);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(448, 48);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // groupBoxFont
            // 
            this.groupBoxFont.Controls.Add(this.tableLayoutPanel6);
            this.groupBoxFont.Location = new System.Drawing.Point(3, 208);
            this.groupBoxFont.Name = "groupBoxFont";
            this.groupBoxFont.Size = new System.Drawing.Size(460, 76);
            this.groupBoxFont.TabIndex = 48;
            this.groupBoxFont.TabStop = false;
            this.groupBoxFont.Text = "Text Font";
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 3;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 157F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 86F));
            this.tableLayoutPanel6.Controls.Add(this.lblFont, 1, 1);
            this.tableLayoutPanel6.Controls.Add(this.btnFont, 2, 1);
            this.tableLayoutPanel6.Controls.Add(this.chkFont, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 2;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(451, 57);
            this.tableLayoutPanel6.TabIndex = 0;
            // 
            // lblFont
            // 
            this.lblFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFont.AutoSize = true;
            this.lblFont.Location = new System.Drawing.Point(160, 37);
            this.lblFont.Name = "lblFont";
            this.lblFont.Size = new System.Drawing.Size(202, 13);
            this.lblFont.TabIndex = 4;
            this.lblFont.Text = "Font";
            // 
            // btnFont
            // 
            this.btnFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFont.Location = new System.Drawing.Point(368, 32);
            this.btnFont.Name = "btnFont";
            this.btnFont.Size = new System.Drawing.Size(80, 23);
            this.btnFont.TabIndex = 1;
            this.btnFont.Text = "Choose...";
            this.btnFont.UseVisualStyleBackColor = true;
            this.btnFont.Click += new System.EventHandler(this.btnFont_Click);
            // 
            // chkFont
            // 
            this.chkFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkFont.AutoSize = true;
            this.tableLayoutPanel6.SetColumnSpan(this.chkFont, 2);
            this.chkFont.Location = new System.Drawing.Point(7, 6);
            this.chkFont.Margin = new System.Windows.Forms.Padding(7, 3, 3, 3);
            this.chkFont.Name = "chkFont";
            this.chkFont.Size = new System.Drawing.Size(355, 17);
            this.chkFont.TabIndex = 0;
            this.chkFont.Text = "Override Layout Settings";
            this.chkFont.UseVisualStyleBackColor = true;
            this.chkFont.CheckedChanged += new System.EventHandler(this.chkFont_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Font:";
            // 
            // groupBoxColor
            // 
            this.groupBoxColor.Controls.Add(this.tableLayoutPanel1);
            this.groupBoxColor.Location = new System.Drawing.Point(3, 290);
            this.groupBoxColor.Name = "groupBoxColor";
            this.groupBoxColor.Size = new System.Drawing.Size(460, 76);
            this.groupBoxColor.TabIndex = 49;
            this.groupBoxColor.TabStop = false;
            this.groupBoxColor.Text = "Text Color";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 157F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 91F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 172F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel1.Controls.Add(this.chkColor, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnShadowColor, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnTextColor, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(451, 57);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // chkColor
            // 
            this.chkColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkColor.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.chkColor, 2);
            this.chkColor.Location = new System.Drawing.Point(7, 5);
            this.chkColor.Margin = new System.Windows.Forms.Padding(7, 3, 3, 3);
            this.chkColor.Name = "chkColor";
            this.chkColor.Size = new System.Drawing.Size(238, 17);
            this.chkColor.TabIndex = 0;
            this.chkColor.Text = "Override Layout Settings";
            this.chkColor.UseVisualStyleBackColor = true;
            this.chkColor.CheckedChanged += new System.EventHandler(this.chkColor_CheckedChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(151, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Inline:";
            // 
            // btnShadowColor
            // 
            this.btnShadowColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnShadowColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnShadowColor.Location = new System.Drawing.Point(423, 31);
            this.btnShadowColor.Name = "btnShadowColor";
            this.btnShadowColor.Size = new System.Drawing.Size(23, 24);
            this.btnShadowColor.TabIndex = 12;
            this.btnShadowColor.UseVisualStyleBackColor = false;
            this.btnShadowColor.Click += new System.EventHandler(this.colorButton_Click);
            // 
            // btnTextColor
            // 
            this.btnTextColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTextColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnTextColor.Location = new System.Drawing.Point(160, 31);
            this.btnTextColor.Name = "btnTextColor";
            this.btnTextColor.Size = new System.Drawing.Size(23, 24);
            this.btnTextColor.TabIndex = 1;
            this.btnTextColor.UseVisualStyleBackColor = false;
            this.btnTextColor.Click += new System.EventHandler(this.colorButton_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(251, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(166, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Shadow:";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 77.80374F));
            this.tableLayoutPanel2.Controls.Add(this.groupBoxColor, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.groupBoxSeed, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.groupBoxFont, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.groupBoxOptions, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 0, 2);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 88F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(471, 379);
            this.tableLayoutPanel2.TabIndex = 50;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 4;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 161F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 238F));
            this.tableLayoutPanel4.Controls.Add(this.btnColor1, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.label11, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnColor2, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.cmbGradientType, 3, 0);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 172);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(460, 30);
            this.tableLayoutPanel4.TabIndex = 51;
            // 
            // btnColor1
            // 
            this.btnColor1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnColor1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnColor1.Location = new System.Drawing.Point(164, 3);
            this.btnColor1.Name = "btnColor1";
            this.btnColor1.Size = new System.Drawing.Size(23, 24);
            this.btnColor1.TabIndex = 42;
            this.btnColor1.UseVisualStyleBackColor = false;
            this.btnColor1.Click += new System.EventHandler(this.colorButton_Click);
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 8);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(155, 13);
            this.label11.TabIndex = 40;
            this.label11.Text = "Background Color:";
            // 
            // btnColor2
            // 
            this.btnColor2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnColor2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnColor2.Location = new System.Drawing.Point(194, 3);
            this.btnColor2.Name = "btnColor2";
            this.btnColor2.Size = new System.Drawing.Size(23, 24);
            this.btnColor2.TabIndex = 41;
            this.btnColor2.UseVisualStyleBackColor = false;
            this.btnColor2.Click += new System.EventHandler(this.colorButton_Click);
            // 
            // cmbGradientType
            // 
            this.cmbGradientType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbGradientType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGradientType.FormattingEnabled = true;
            this.cmbGradientType.Items.AddRange(new object[] {
            "Plain",
            "Vertical",
            "Horizontal"});
            this.cmbGradientType.Location = new System.Drawing.Point(225, 4);
            this.cmbGradientType.Name = "cmbGradientType";
            this.cmbGradientType.Size = new System.Drawing.Size(232, 21);
            this.cmbGradientType.TabIndex = 43;
            this.cmbGradientType.SelectedIndexChanged += new System.EventHandler(this.mbGradientType_SelectedIndexChanged);
            // 
            // btnClipboard
            // 
            this.btnClipboard.Location = new System.Drawing.Point(260, 28);
            this.btnClipboard.Name = "btnClipboard";
            this.btnClipboard.Size = new System.Drawing.Size(103, 24);
            this.btnClipboard.TabIndex = 45;
            this.btnClipboard.Text = "Copy to clipboard";
            this.btnClipboard.UseVisualStyleBackColor = true;
            this.btnClipboard.Click += new System.EventHandler(this.btnClipboard_Click);
            // 
            // MomodoraRandomizerSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "MomodoraRandomizerSettings";
            this.Size = new System.Drawing.Size(476, 379);
            this.Load += new System.EventHandler(this.MomodoraRandomizerSettings_Load);
            this.groupBoxSeed.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.groupBoxOptions.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.groupBoxFont.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.groupBoxColor.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSeed;
        private System.Windows.Forms.GroupBox groupBoxSeed;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.GroupBox groupBoxOptions;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.GroupBox groupBoxFont;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Label lblFont;
        private System.Windows.Forms.Button btnFont;
        private System.Windows.Forms.CheckBox chkFont;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxColor;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox chkColor;
        private System.Windows.Forms.Button btnTextColor;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnColor1;
        private System.Windows.Forms.Button btnColor2;
        private System.Windows.Forms.ComboBox cmbGradientType;
        public System.Windows.Forms.CheckBox chkIvoryBugs;
        public System.Windows.Forms.CheckBox chkVitality;
        public System.Windows.Forms.CheckBox chkHard;
        public System.Windows.Forms.CheckBox chkRandom;
        private System.Windows.Forms.TextBox textSeed;
        private System.Windows.Forms.Button btnShadowColor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnClipboard;
    }
}
