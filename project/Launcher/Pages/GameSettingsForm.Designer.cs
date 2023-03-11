namespace Launcher.Pages
{
    partial class GameSettingsForm
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
            this._steambtn = new System.Windows.Forms.Panel();
            this._steamlabel = new System.Windows.Forms.Label();
            this._executablebtn = new System.Windows.Forms.Panel();
            this._executablelabel = new System.Windows.Forms.Label();
            this._updatebranchtitle = new System.Windows.Forms.Label();
            this._modsfolderbtn = new System.Windows.Forms.Panel();
            this._modsfolderlabel = new System.Windows.Forms.Label();
            this._modcreatorbtn = new System.Windows.Forms.Panel();
            this._modcreatorlabel = new System.Windows.Forms.Label();
            this._uninstallbtn.SuspendLayout();
            this._layout.SuspendLayout();
            this._updatebranch.SuspendLayout();
            this._steambtn.SuspendLayout();
            this._executablebtn.SuspendLayout();
            this._modsfolderbtn.SuspendLayout();
            this._modcreatorbtn.SuspendLayout();
            this.SuspendLayout();
            // 
            // _title
            // 
            this._title.Font = new System.Drawing.Font("Roboto Condensed", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this._title.Location = new System.Drawing.Point(5, 5);
            this._title.Name = "_title";
            this._title.Size = new System.Drawing.Size(326, 23);
            this._title.TabIndex = 1;
            this._title.Text = "Game Settings - Raft";
            // 
            // _version
            // 
            this._version.Font = new System.Drawing.Font("Roboto Condensed", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._version.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this._version.Location = new System.Drawing.Point(10, 490);
            this._version.Name = "_version";
            this._version.Size = new System.Drawing.Size(380, 20);
            this._version.TabIndex = 3;
            this._version.Text = "Installed version : 1.0.0";
            this._version.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _uninstallbtn
            // 
            this._uninstallbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this._uninstallbtn.Controls.Add(this._uninstallbtnlabel);
            this._uninstallbtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this._uninstallbtn.Location = new System.Drawing.Point(231, 457);
            this._uninstallbtn.Name = "_uninstallbtn";
            this._uninstallbtn.Size = new System.Drawing.Size(159, 30);
            this._uninstallbtn.TabIndex = 4;
            this._uninstallbtn.Tag = "r:5;btn:bg;linkto:_uninstallbtnlabel";
            // 
            // _uninstallbtnlabel
            // 
            this._uninstallbtnlabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._uninstallbtnlabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this._uninstallbtnlabel.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._uninstallbtnlabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(254)))));
            this._uninstallbtnlabel.Location = new System.Drawing.Point(5, 0);
            this._uninstallbtnlabel.Name = "_uninstallbtnlabel";
            this._uninstallbtnlabel.Padding = new System.Windows.Forms.Padding(5);
            this._uninstallbtnlabel.Size = new System.Drawing.Size(149, 30);
            this._uninstallbtnlabel.TabIndex = 1;
            this._uninstallbtnlabel.Tag = "btn:bg;linkto:_uninstallbtn";
            this._uninstallbtnlabel.Text = "Uninstall Game";
            this._uninstallbtnlabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _layout
            // 
            this._layout.Controls.Add(this._updatebranch);
            this._layout.Location = new System.Drawing.Point(10, 30);
            this._layout.Name = "_layout";
            this._layout.Size = new System.Drawing.Size(380, 298);
            this._layout.TabIndex = 6;
            // 
            // _updatebranch
            // 
            this._updatebranch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(26)))));
            this._updatebranch.Controls.Add(this._steambtn);
            this._updatebranch.Controls.Add(this._executablebtn);
            this._updatebranch.Controls.Add(this._updatebranchtitle);
            this._updatebranch.Location = new System.Drawing.Point(0, 0);
            this._updatebranch.Margin = new System.Windows.Forms.Padding(0);
            this._updatebranch.Name = "_updatebranch";
            this._updatebranch.Size = new System.Drawing.Size(380, 62);
            this._updatebranch.TabIndex = 1;
            this._updatebranch.Tag = "r:5";
            // 
            // _steambtn
            // 
            this._steambtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(117)))), ((int)(((byte)(126)))));
            this._steambtn.Controls.Add(this._steamlabel);
            this._steambtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this._steambtn.Location = new System.Drawing.Point(122, 26);
            this._steambtn.Name = "_steambtn";
            this._steambtn.Size = new System.Drawing.Size(80, 30);
            this._steambtn.TabIndex = 6;
            this._steambtn.Tag = "r:5;btn:bg;linkto:_steamlabel";
            // 
            // _steamlabel
            // 
            this._steamlabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._steamlabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this._steamlabel.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._steamlabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(254)))));
            this._steamlabel.Location = new System.Drawing.Point(5, 0);
            this._steamlabel.Name = "_steamlabel";
            this._steamlabel.Padding = new System.Windows.Forms.Padding(5);
            this._steamlabel.Size = new System.Drawing.Size(70, 30);
            this._steamlabel.TabIndex = 1;
            this._steamlabel.Tag = "btn:bg;linkto:_steambtn";
            this._steamlabel.Text = "Steam";
            this._steamlabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _executablebtn
            // 
            this._executablebtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(117)))), ((int)(((byte)(126)))));
            this._executablebtn.Controls.Add(this._executablelabel);
            this._executablebtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this._executablebtn.Location = new System.Drawing.Point(5, 26);
            this._executablebtn.Name = "_executablebtn";
            this._executablebtn.Size = new System.Drawing.Size(112, 30);
            this._executablebtn.TabIndex = 5;
            this._executablebtn.Tag = "r:5;btn:bg;linkto:_executablelabel";
            // 
            // _executablelabel
            // 
            this._executablelabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._executablelabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this._executablelabel.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._executablelabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(254)))));
            this._executablelabel.Location = new System.Drawing.Point(5, 0);
            this._executablelabel.Name = "_executablelabel";
            this._executablelabel.Padding = new System.Windows.Forms.Padding(5);
            this._executablelabel.Size = new System.Drawing.Size(102, 30);
            this._executablelabel.TabIndex = 1;
            this._executablelabel.Tag = "btn:bg;linkto:_executablebtn";
            this._executablelabel.Text = "Executable";
            this._executablelabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _updatebranchtitle
            // 
            this._updatebranchtitle.Font = new System.Drawing.Font("Roboto Condensed", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._updatebranchtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this._updatebranchtitle.Location = new System.Drawing.Point(2, 0);
            this._updatebranchtitle.Name = "_updatebranchtitle";
            this._updatebranchtitle.Size = new System.Drawing.Size(180, 23);
            this._updatebranchtitle.TabIndex = 2;
            this._updatebranchtitle.Text = "Game Starting Method";
            // 
            // _modsfolderbtn
            // 
            this._modsfolderbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(117)))), ((int)(((byte)(126)))));
            this._modsfolderbtn.Controls.Add(this._modsfolderlabel);
            this._modsfolderbtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this._modsfolderbtn.Location = new System.Drawing.Point(10, 457);
            this._modsfolderbtn.Name = "_modsfolderbtn";
            this._modsfolderbtn.Size = new System.Drawing.Size(210, 30);
            this._modsfolderbtn.TabIndex = 7;
            this._modsfolderbtn.Tag = "r:5;btn:bg;linkto:_modsfolderlabel";
            // 
            // _modsfolderlabel
            // 
            this._modsfolderlabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._modsfolderlabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this._modsfolderlabel.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._modsfolderlabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(254)))));
            this._modsfolderlabel.Location = new System.Drawing.Point(5, 0);
            this._modsfolderlabel.Name = "_modsfolderlabel";
            this._modsfolderlabel.Padding = new System.Windows.Forms.Padding(5);
            this._modsfolderlabel.Size = new System.Drawing.Size(200, 30);
            this._modsfolderlabel.TabIndex = 1;
            this._modsfolderlabel.Tag = "btn:bg;linkto:_modsfolderbtn";
            this._modsfolderlabel.Text = "Open mods folder";
            this._modsfolderlabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _modcreatorbtn
            // 
            this._modcreatorbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(186)))), ((int)(((byte)(252)))));
            this._modcreatorbtn.Controls.Add(this._modcreatorlabel);
            this._modcreatorbtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this._modcreatorbtn.Location = new System.Drawing.Point(10, 417);
            this._modcreatorbtn.Name = "_modcreatorbtn";
            this._modcreatorbtn.Size = new System.Drawing.Size(380, 30);
            this._modcreatorbtn.TabIndex = 8;
            this._modcreatorbtn.Tag = "r:5;btn:bg;linkto:_modcreatorlabel";
            // 
            // _modcreatorlabel
            // 
            this._modcreatorlabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._modcreatorlabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this._modcreatorlabel.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._modcreatorlabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(254)))));
            this._modcreatorlabel.Location = new System.Drawing.Point(5, 0);
            this._modcreatorlabel.Name = "_modcreatorlabel";
            this._modcreatorlabel.Padding = new System.Windows.Forms.Padding(5);
            this._modcreatorlabel.Size = new System.Drawing.Size(370, 30);
            this._modcreatorlabel.TabIndex = 1;
            this._modcreatorlabel.Tag = "btn:bg;linkto:_modcreatorbtn";
            this._modcreatorlabel.Text = "Create a new mod project";
            this._modcreatorlabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GameSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(26)))));
            this.ClientSize = new System.Drawing.Size(400, 520);
            this.Controls.Add(this._modcreatorbtn);
            this.Controls.Add(this._modsfolderbtn);
            this.Controls.Add(this._layout);
            this.Controls.Add(this._uninstallbtn);
            this.Controls.Add(this._version);
            this.Controls.Add(this._title);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "GameSettingsForm";
            this.Text = "SettingsForm";
            this._uninstallbtn.ResumeLayout(false);
            this._layout.ResumeLayout(false);
            this._updatebranch.ResumeLayout(false);
            this._steambtn.ResumeLayout(false);
            this._executablebtn.ResumeLayout(false);
            this._modsfolderbtn.ResumeLayout(false);
            this._modcreatorbtn.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label _title;
        private System.Windows.Forms.Label _version;
        public System.Windows.Forms.Panel _uninstallbtn;
        private System.Windows.Forms.Label _uninstallbtnlabel;
        private System.Windows.Forms.FlowLayoutPanel _layout;
        private System.Windows.Forms.Panel _updatebranch;
        public System.Windows.Forms.Panel _steambtn;
        public System.Windows.Forms.Label _steamlabel;
        public System.Windows.Forms.Panel _executablebtn;
        public System.Windows.Forms.Label _executablelabel;
        private System.Windows.Forms.Label _updatebranchtitle;
        public System.Windows.Forms.Panel _modsfolderbtn;
        public System.Windows.Forms.Label _modsfolderlabel;
        public System.Windows.Forms.Panel _modcreatorbtn;
        public System.Windows.Forms.Label _modcreatorlabel;
    }
}