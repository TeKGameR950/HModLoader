namespace Launcher.Pages
{
    partial class SettingsForm
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
            this._title = new System.Windows.Forms.Label();
            this._version = new System.Windows.Forms.Label();
            this._uninstallbtn = new System.Windows.Forms.Panel();
            this._uninstallbtnlabel = new System.Windows.Forms.Label();
            this._layout = new System.Windows.Forms.FlowLayoutPanel();
            this._updatebranch = new System.Windows.Forms.Panel();
            this._branchindevbtn = new System.Windows.Forms.Panel();
            this._branchindevlabel = new System.Windows.Forms.Label();
            this._branchpublicbtn = new System.Windows.Forms.Panel();
            this._branchpubliclabel = new System.Windows.Forms.Label();
            this._updatebranchtitle = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this._instancestitle = new System.Windows.Forms.Label();
            this._instancesCheckbox = new HCheckbox();
            this._uninstallbtn.SuspendLayout();
            this._layout.SuspendLayout();
            this._updatebranch.SuspendLayout();
            this._branchindevbtn.SuspendLayout();
            this._branchpublicbtn.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _title
            // 
            this._title.Font = new System.Drawing.Font("Roboto Condensed", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this._title.Location = new System.Drawing.Point(5, 5);
            this._title.Name = "_title";
            this._title.Size = new System.Drawing.Size(129, 23);
            this._title.TabIndex = 1;
            this._title.Text = "Launcher Settings";
            // 
            // _version
            // 
            this._version.Font = new System.Drawing.Font("Roboto Condensed", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._version.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this._version.Location = new System.Drawing.Point(175, 480);
            this._version.Name = "_version";
            this._version.Size = new System.Drawing.Size(215, 30);
            this._version.TabIndex = 3;
            this._version.Text = "HModLoader version : 1.0.0";
            this._version.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // _uninstallbtn
            // 
            this._uninstallbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this._uninstallbtn.Controls.Add(this._uninstallbtnlabel);
            this._uninstallbtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this._uninstallbtn.Location = new System.Drawing.Point(10, 480);
            this._uninstallbtn.Name = "_uninstallbtn";
            this._uninstallbtn.Size = new System.Drawing.Size(159, 30);
            this._uninstallbtn.TabIndex = 4;
            this._uninstallbtn.Tag = "r:5;btn:bg;linkto:_uninstallbtnlabel";
            // 
            // _uninstallbtnlabel
            // 
            this._uninstallbtnlabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this._uninstallbtnlabel.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._uninstallbtnlabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(254)))));
            this._uninstallbtnlabel.Location = new System.Drawing.Point(5, 0);
            this._uninstallbtnlabel.Name = "_uninstallbtnlabel";
            this._uninstallbtnlabel.Padding = new System.Windows.Forms.Padding(5);
            this._uninstallbtnlabel.Size = new System.Drawing.Size(149, 30);
            this._uninstallbtnlabel.TabIndex = 1;
            this._uninstallbtnlabel.Tag = "btn:bg;linkto:_uninstallbtn";
            this._uninstallbtnlabel.Text = "Uninstall";
            this._uninstallbtnlabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _layout
            // 
            this._layout.Controls.Add(this._updatebranch);
            this._layout.Controls.Add(this.panel1);
            this._layout.Location = new System.Drawing.Point(10, 30);
            this._layout.Name = "_layout";
            this._layout.Size = new System.Drawing.Size(380, 440);
            this._layout.TabIndex = 5;
            // 
            // _updatebranch
            // 
            this._updatebranch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(26)))));
            this._updatebranch.Controls.Add(this._branchindevbtn);
            this._updatebranch.Controls.Add(this._branchpublicbtn);
            this._updatebranch.Controls.Add(this._updatebranchtitle);
            this._updatebranch.Location = new System.Drawing.Point(0, 0);
            this._updatebranch.Margin = new System.Windows.Forms.Padding(0);
            this._updatebranch.Name = "_updatebranch";
            this._updatebranch.Size = new System.Drawing.Size(380, 62);
            this._updatebranch.TabIndex = 0;
            this._updatebranch.Tag = "r:5";
            // 
            // _branchindevbtn
            // 
            this._branchindevbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(117)))), ((int)(((byte)(126)))));
            this._branchindevbtn.Controls.Add(this._branchindevlabel);
            this._branchindevbtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this._branchindevbtn.Location = new System.Drawing.Point(90, 26);
            this._branchindevbtn.Name = "_branchindevbtn";
            this._branchindevbtn.Size = new System.Drawing.Size(80, 30);
            this._branchindevbtn.TabIndex = 6;
            this._branchindevbtn.Tag = "r:5;btn:bg;linkto:_branchindevlabel";
            // 
            // _branchindevlabel
            // 
            this._branchindevlabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._branchindevlabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this._branchindevlabel.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._branchindevlabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(254)))));
            this._branchindevlabel.Location = new System.Drawing.Point(5, 0);
            this._branchindevlabel.Name = "_branchindevlabel";
            this._branchindevlabel.Padding = new System.Windows.Forms.Padding(5);
            this._branchindevlabel.Size = new System.Drawing.Size(70, 30);
            this._branchindevlabel.TabIndex = 1;
            this._branchindevlabel.Tag = "btn:bg;linkto:_branchindevbtn";
            this._branchindevlabel.Text = "Indev";
            this._branchindevlabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _branchpublicbtn
            // 
            this._branchpublicbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(117)))), ((int)(((byte)(126)))));
            this._branchpublicbtn.Controls.Add(this._branchpubliclabel);
            this._branchpublicbtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this._branchpublicbtn.Location = new System.Drawing.Point(5, 26);
            this._branchpublicbtn.Name = "_branchpublicbtn";
            this._branchpublicbtn.Size = new System.Drawing.Size(80, 30);
            this._branchpublicbtn.TabIndex = 5;
            this._branchpublicbtn.Tag = "r:5;btn:bg;linkto:_branchpubliclabel";
            // 
            // _branchpubliclabel
            // 
            this._branchpubliclabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._branchpubliclabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this._branchpubliclabel.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._branchpubliclabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(254)))));
            this._branchpubliclabel.Location = new System.Drawing.Point(5, 0);
            this._branchpubliclabel.Name = "_branchpubliclabel";
            this._branchpubliclabel.Padding = new System.Windows.Forms.Padding(5);
            this._branchpubliclabel.Size = new System.Drawing.Size(70, 30);
            this._branchpubliclabel.TabIndex = 1;
            this._branchpubliclabel.Tag = "btn:bg;linkto:_branchpublicbtn";
            this._branchpubliclabel.Text = "Public";
            this._branchpubliclabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _updatebranchtitle
            // 
            this._updatebranchtitle.Font = new System.Drawing.Font("Roboto Condensed", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._updatebranchtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this._updatebranchtitle.Location = new System.Drawing.Point(2, 0);
            this._updatebranchtitle.Name = "_updatebranchtitle";
            this._updatebranchtitle.Size = new System.Drawing.Size(99, 23);
            this._updatebranchtitle.TabIndex = 2;
            this._updatebranchtitle.Text = "Update Branch";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(26)))));
            this.panel1.Controls.Add(this._instancestitle);
            this.panel1.Controls.Add(this._instancesCheckbox);
            this.panel1.Location = new System.Drawing.Point(0, 72);
            this.panel1.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(380, 116);
            this.panel1.TabIndex = 1;
            this.panel1.Tag = "r:5";
            // 
            // _instancestitle
            // 
            this._instancestitle.Font = new System.Drawing.Font("Roboto Condensed", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._instancestitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this._instancestitle.Location = new System.Drawing.Point(2, 0);
            this._instancestitle.Name = "_instancestitle";
            this._instancestitle.Size = new System.Drawing.Size(378, 23);
            this._instancestitle.TabIndex = 4;
            this._instancestitle.Text = "Allow multiple game instances (Experimental)";
            // 
            // _instancesCheckbox
            // 
            this._instancesCheckbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this._instancesCheckbox.Checked = false;
            this._instancesCheckbox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._instancesCheckbox.Location = new System.Drawing.Point(5, 25);
            this._instancesCheckbox.Name = "_instancesCheckbox";
            this._instancesCheckbox.Size = new System.Drawing.Size(30, 30);
            this._instancesCheckbox.TabIndex = 3;
            this._instancesCheckbox.Tag = "r:5";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(26)))));
            this.ClientSize = new System.Drawing.Size(400, 520);
            this.Controls.Add(this._layout);
            this.Controls.Add(this._uninstallbtn);
            this.Controls.Add(this._version);
            this.Controls.Add(this._title);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            this._uninstallbtn.ResumeLayout(false);
            this._layout.ResumeLayout(false);
            this._updatebranch.ResumeLayout(false);
            this._branchindevbtn.ResumeLayout(false);
            this._branchpublicbtn.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label _title;
        private System.Windows.Forms.Label _version;
        public System.Windows.Forms.Panel _uninstallbtn;
        private System.Windows.Forms.Label _uninstallbtnlabel;
        private System.Windows.Forms.FlowLayoutPanel _layout;
        private System.Windows.Forms.Panel _updatebranch;
        private System.Windows.Forms.Label _updatebranchtitle;
        public System.Windows.Forms.Panel _branchpublicbtn;
        public System.Windows.Forms.Panel _branchindevbtn;
        public System.Windows.Forms.Label _branchpubliclabel;
        public System.Windows.Forms.Label _branchindevlabel;
        private System.Windows.Forms.Panel panel1;
        private HCheckbox _instancesCheckbox;
        private System.Windows.Forms.Label _instancestitle;
    }
}