namespace Launcher.Forms
{
    partial class InstallModForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallModForm));
            this._topbar = new System.Windows.Forms.Panel();
            this._close = new System.Windows.Forms.PictureBox();
            this._title = new System.Windows.Forms.Label();
            this._minimizebtn = new System.Windows.Forms.PictureBox();
            this._closebtn = new System.Windows.Forms.PictureBox();
            this._label = new System.Windows.Forms.Label();
            this._nobtn = new System.Windows.Forms.Panel();
            this._nolabel = new System.Windows.Forms.Label();
            this._yesbtn = new System.Windows.Forms.Panel();
            this._yeslabel = new System.Windows.Forms.Label();
            this._modbanner = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this._topbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._close)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._minimizebtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._closebtn)).BeginInit();
            this._nobtn.SuspendLayout();
            this._yesbtn.SuspendLayout();
            this.SuspendLayout();
            // 
            // _topbar
            // 
            this._topbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(26)))));
            this._topbar.Controls.Add(this._close);
            this._topbar.Controls.Add(this._title);
            this._topbar.Controls.Add(this._minimizebtn);
            this._topbar.Controls.Add(this._closebtn);
            this._topbar.Cursor = System.Windows.Forms.Cursors.Arrow;
            this._topbar.Dock = System.Windows.Forms.DockStyle.Top;
            this._topbar.Location = new System.Drawing.Point(0, 0);
            this._topbar.Name = "_topbar";
            this._topbar.Size = new System.Drawing.Size(350, 30);
            this._topbar.TabIndex = 1;
            this._topbar.Tag = "drag";
            // 
            // _close
            // 
            this._close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._close.BackgroundImage = global::Launcher.Properties.Resources.close;
            this._close.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._close.Cursor = System.Windows.Forms.Cursors.Hand;
            this._close.Location = new System.Drawing.Point(315, 8);
            this._close.Name = "_close";
            this._close.Size = new System.Drawing.Size(15, 15);
            this._close.TabIndex = 5;
            this._close.TabStop = false;
            this._close.Tag = "btn:img";
            // 
            // _title
            // 
            this._title.Cursor = System.Windows.Forms.Cursors.Arrow;
            this._title.Dock = System.Windows.Forms.DockStyle.Fill;
            this._title.Font = new System.Drawing.Font("Roboto Condensed", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this._title.Location = new System.Drawing.Point(0, 0);
            this._title.Margin = new System.Windows.Forms.Padding(0);
            this._title.Name = "_title";
            this._title.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this._title.Size = new System.Drawing.Size(350, 30);
            this._title.TabIndex = 4;
            this._title.Tag = "drag";
            this._title.Text = "Automatic Mod Installation";
            this._title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _minimizebtn
            // 
            this._minimizebtn.BackgroundImage = global::Launcher.Properties.Resources.minimize;
            this._minimizebtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._minimizebtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this._minimizebtn.Location = new System.Drawing.Point(838, 8);
            this._minimizebtn.Name = "_minimizebtn";
            this._minimizebtn.Size = new System.Drawing.Size(15, 15);
            this._minimizebtn.TabIndex = 3;
            this._minimizebtn.TabStop = false;
            this._minimizebtn.Tag = "btn:img";
            // 
            // _closebtn
            // 
            this._closebtn.BackgroundImage = global::Launcher.Properties.Resources.close;
            this._closebtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._closebtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this._closebtn.Location = new System.Drawing.Point(858, 8);
            this._closebtn.Name = "_closebtn";
            this._closebtn.Size = new System.Drawing.Size(15, 15);
            this._closebtn.TabIndex = 2;
            this._closebtn.TabStop = false;
            this._closebtn.Tag = "btn:img";
            // 
            // _label
            // 
            this._label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._label.AutoSize = true;
            this._label.Cursor = System.Windows.Forms.Cursors.Arrow;
            this._label.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this._label.Location = new System.Drawing.Point(10, 200);
            this._label.Margin = new System.Windows.Forms.Padding(0);
            this._label.MaximumSize = new System.Drawing.Size(330, 500);
            this._label.Name = "_label";
            this._label.Size = new System.Drawing.Size(85, 18);
            this._label.TabIndex = 5;
            this._label.Tag = "";
            this._label.Text = "Description";
            this._label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _nobtn
            // 
            this._nobtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._nobtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this._nobtn.Controls.Add(this._nolabel);
            this._nobtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this._nobtn.Location = new System.Drawing.Point(180, 360);
            this._nobtn.Name = "_nobtn";
            this._nobtn.Size = new System.Drawing.Size(150, 30);
            this._nobtn.TabIndex = 6;
            this._nobtn.Tag = "r:5;btn:bg;linkto:_nolabel";
            // 
            // _nolabel
            // 
            this._nolabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this._nolabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._nolabel.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._nolabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(254)))));
            this._nolabel.Location = new System.Drawing.Point(0, 0);
            this._nolabel.Name = "_nolabel";
            this._nolabel.Padding = new System.Windows.Forms.Padding(5);
            this._nolabel.Size = new System.Drawing.Size(150, 30);
            this._nolabel.TabIndex = 1;
            this._nolabel.Tag = "btn:bg;linkto:_nobtn";
            this._nolabel.Text = "No, Cancel.";
            this._nolabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _yesbtn
            // 
            this._yesbtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._yesbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(182)))), ((int)(((byte)(125)))));
            this._yesbtn.Controls.Add(this._yeslabel);
            this._yesbtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this._yesbtn.Location = new System.Drawing.Point(10, 360);
            this._yesbtn.Name = "_yesbtn";
            this._yesbtn.Size = new System.Drawing.Size(150, 30);
            this._yesbtn.TabIndex = 7;
            this._yesbtn.Tag = "r:5;btn:bg;linkto:_yeslabel";
            // 
            // _yeslabel
            // 
            this._yeslabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._yeslabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this._yeslabel.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._yeslabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(254)))));
            this._yeslabel.Location = new System.Drawing.Point(10, 0);
            this._yeslabel.Name = "_yeslabel";
            this._yeslabel.Padding = new System.Windows.Forms.Padding(5);
            this._yeslabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._yeslabel.Size = new System.Drawing.Size(110, 30);
            this._yeslabel.TabIndex = 1;
            this._yeslabel.Tag = "btn:bg;linkto:_yesbtn";
            this._yeslabel.Text = "Yes, Install it!";
            this._yeslabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _modbanner
            // 
            this._modbanner.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._modbanner.BackgroundImage = global::Launcher.Properties.Resources.test_banner;
            this._modbanner.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._modbanner.Location = new System.Drawing.Point(10, 40);
            this._modbanner.Name = "_modbanner";
            this._modbanner.Size = new System.Drawing.Size(320, 100);
            this._modbanner.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label1.Font = new System.Drawing.Font("Roboto Condensed", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this.label1.Location = new System.Drawing.Point(10, 150);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.MaximumSize = new System.Drawing.Size(480, 500);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(321, 20);
            this.label1.TabIndex = 9;
            this.label1.Tag = "";
            this.label1.Text = "Weapons Mod (Alpha)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label2.Font = new System.Drawing.Font("Roboto Condensed", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this.label2.Location = new System.Drawing.Point(10, 175);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.MaximumSize = new System.Drawing.Size(480, 500);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(321, 20);
            this.label2.TabIndex = 10;
            this.label2.Tag = "";
            this.label2.Text = "Made By TeK";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label3.Font = new System.Drawing.Font("Roboto Condensed", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this.label3.Location = new System.Drawing.Point(10, 329);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.MaximumSize = new System.Drawing.Size(480, 500);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(321, 20);
            this.label3.TabIndex = 11;
            this.label3.Tag = "";
            this.label3.Text = "Do you want to install this mod ?";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(26)))));
            this.label4.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label4.Font = new System.Drawing.Font("Roboto", 10F);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this.label4.Location = new System.Drawing.Point(0, 399);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.MaximumSize = new System.Drawing.Size(480, 500);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(350, 51);
            this.label4.TabIndex = 12;
            this.label4.Tag = "";
            this.label4.Text = "HModding takes no liability for user-created mods.\r\nInstallation is at your own r" +
    "isk.";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // InstallModForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(25)))), ((int)(((byte)(27)))));
            this.ClientSize = new System.Drawing.Size(350, 450);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._nobtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._yesbtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._modbanner);
            this.Controls.Add(this._label);
            this.Controls.Add(this._topbar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 500);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(290, 100);
            this.Name = "InstallModForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HMessageBox";
            this._topbar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._close)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._minimizebtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._closebtn)).EndInit();
            this._nobtn.ResumeLayout(false);
            this._yesbtn.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel _topbar;
        private System.Windows.Forms.PictureBox _minimizebtn;
        private System.Windows.Forms.PictureBox _closebtn;
        private System.Windows.Forms.Label _title;
        private System.Windows.Forms.Label _label;
        public System.Windows.Forms.Panel _nobtn;
        private System.Windows.Forms.Label _nolabel;
        public System.Windows.Forms.Panel _yesbtn;
        private System.Windows.Forms.Label _yeslabel;
        private System.Windows.Forms.PictureBox _close;
        private System.Windows.Forms.Panel _modbanner;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}