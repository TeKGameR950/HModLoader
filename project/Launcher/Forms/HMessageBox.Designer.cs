namespace Launcher.Forms
{
    partial class HMessageBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HMessageBox));
            this._topbar = new System.Windows.Forms.Panel();
            this._close = new System.Windows.Forms.PictureBox();
            this._title = new System.Windows.Forms.Label();
            this._minimizebtn = new System.Windows.Forms.PictureBox();
            this._closebtn = new System.Windows.Forms.PictureBox();
            this._label = new System.Windows.Forms.Label();
            this._nobtn = new System.Windows.Forms.Panel();
            this._nolabel = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._cancelbtn = new System.Windows.Forms.Panel();
            this._cancellabel = new System.Windows.Forms.Label();
            this._yesbtn = new System.Windows.Forms.Panel();
            this._yeslabel = new System.Windows.Forms.Label();
            this._okbtn = new System.Windows.Forms.Panel();
            this._oklabel = new System.Windows.Forms.Label();
            this._path = new System.Windows.Forms.TextBox();
            this._topbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._close)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._minimizebtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._closebtn)).BeginInit();
            this._nobtn.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this._cancelbtn.SuspendLayout();
            this._yesbtn.SuspendLayout();
            this._okbtn.SuspendLayout();
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
            this._topbar.Size = new System.Drawing.Size(500, 30);
            this._topbar.TabIndex = 1;
            this._topbar.Tag = "drag";
            // 
            // _close
            // 
            this._close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._close.BackgroundImage = global::Launcher.Properties.Resources.close;
            this._close.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._close.Cursor = System.Windows.Forms.Cursors.Hand;
            this._close.Location = new System.Drawing.Point(475, 8);
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
            this._title.Size = new System.Drawing.Size(500, 30);
            this._title.TabIndex = 4;
            this._title.Tag = "drag";
            this._title.Text = "Title";
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
            this._label.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._label.AutoSize = true;
            this._label.Cursor = System.Windows.Forms.Cursors.Arrow;
            this._label.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this._label.Location = new System.Drawing.Point(10, 40);
            this._label.Margin = new System.Windows.Forms.Padding(0);
            this._label.MaximumSize = new System.Drawing.Size(480, 500);
            this._label.Name = "_label";
            this._label.Size = new System.Drawing.Size(85, 18);
            this._label.TabIndex = 5;
            this._label.Tag = "";
            this._label.Text = "Description";
            this._label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _nobtn
            // 
            this._nobtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this._nobtn.Controls.Add(this._nolabel);
            this._nobtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this._nobtn.Location = new System.Drawing.Point(322, 5);
            this._nobtn.Name = "_nobtn";
            this._nobtn.Size = new System.Drawing.Size(62, 30);
            this._nobtn.TabIndex = 6;
            this._nobtn.Tag = "r:5;btn:bg;linkto:_nolabel";
            // 
            // _nolabel
            // 
            this._nolabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._nolabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this._nolabel.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._nolabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(254)))));
            this._nolabel.Location = new System.Drawing.Point(10, 0);
            this._nolabel.Name = "_nolabel";
            this._nolabel.Padding = new System.Windows.Forms.Padding(5);
            this._nolabel.Size = new System.Drawing.Size(42, 30);
            this._nolabel.TabIndex = 1;
            this._nolabel.Tag = "btn:bg;linkto:_nobtn";
            this._nolabel.Text = "No";
            this._nolabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this._cancelbtn);
            this.flowLayoutPanel1.Controls.Add(this._nobtn);
            this.flowLayoutPanel1.Controls.Add(this._yesbtn);
            this.flowLayoutPanel1.Controls.Add(this._okbtn);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(10, 153);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(480, 40);
            this.flowLayoutPanel1.TabIndex = 7;
            // 
            // _cancelbtn
            // 
            this._cancelbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(117)))), ((int)(((byte)(126)))));
            this._cancelbtn.Controls.Add(this._cancellabel);
            this._cancelbtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this._cancelbtn.Location = new System.Drawing.Point(390, 5);
            this._cancelbtn.Name = "_cancelbtn";
            this._cancelbtn.Size = new System.Drawing.Size(87, 30);
            this._cancelbtn.TabIndex = 8;
            this._cancelbtn.Tag = "r:5;btn:bg;linkto:_cancellabel";
            // 
            // _cancellabel
            // 
            this._cancellabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._cancellabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this._cancellabel.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._cancellabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(254)))));
            this._cancellabel.Location = new System.Drawing.Point(10, 0);
            this._cancellabel.Name = "_cancellabel";
            this._cancellabel.Padding = new System.Windows.Forms.Padding(5);
            this._cancellabel.Size = new System.Drawing.Size(67, 30);
            this._cancellabel.TabIndex = 1;
            this._cancellabel.Tag = "btn:bg;linkto:_cancelbtn";
            this._cancellabel.Text = "Cancel";
            this._cancellabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _yesbtn
            // 
            this._yesbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(182)))), ((int)(((byte)(125)))));
            this._yesbtn.Controls.Add(this._yeslabel);
            this._yesbtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this._yesbtn.Location = new System.Drawing.Point(248, 5);
            this._yesbtn.Name = "_yesbtn";
            this._yesbtn.Size = new System.Drawing.Size(68, 30);
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
            this._yeslabel.Size = new System.Drawing.Size(48, 30);
            this._yeslabel.TabIndex = 1;
            this._yeslabel.Tag = "btn:bg;linkto:_yesbtn";
            this._yeslabel.Text = "Yes";
            this._yeslabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _okbtn
            // 
            this._okbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(186)))), ((int)(((byte)(252)))));
            this._okbtn.Controls.Add(this._oklabel);
            this._okbtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this._okbtn.Location = new System.Drawing.Point(180, 5);
            this._okbtn.Name = "_okbtn";
            this._okbtn.Size = new System.Drawing.Size(62, 30);
            this._okbtn.TabIndex = 9;
            this._okbtn.Tag = "r:5;btn:bg;linkto:_oklabel";
            // 
            // _oklabel
            // 
            this._oklabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._oklabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this._oklabel.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._oklabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(254)))));
            this._oklabel.Location = new System.Drawing.Point(10, 0);
            this._oklabel.Name = "_oklabel";
            this._oklabel.Padding = new System.Windows.Forms.Padding(5);
            this._oklabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._oklabel.Size = new System.Drawing.Size(42, 30);
            this._oklabel.TabIndex = 1;
            this._oklabel.Tag = "btn:bg;linkto:_okbtn";
            this._oklabel.Text = "Ok";
            this._oklabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _path
            // 
            this._path.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._path.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(117)))), ((int)(((byte)(126)))));
            this._path.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._path.Font = new System.Drawing.Font("Roboto", 11.25F);
            this._path.ForeColor = System.Drawing.Color.White;
            this._path.Location = new System.Drawing.Point(13, 129);
            this._path.Margin = new System.Windows.Forms.Padding(0);
            this._path.Name = "_path";
            this._path.Size = new System.Drawing.Size(474, 19);
            this._path.TabIndex = 9;
            this._path.Tag = "r:5";
            this._path.Text = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Green Hell";
            this._path.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this._path.Visible = false;
            this._path.WordWrap = false;
            // 
            // HMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(25)))), ((int)(((byte)(27)))));
            this.ClientSize = new System.Drawing.Size(500, 200);
            this.Controls.Add(this._path);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this._label);
            this.Controls.Add(this._topbar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 500);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(290, 100);
            this.Name = "HMessageBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HMessageBox";
            this._topbar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._close)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._minimizebtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._closebtn)).EndInit();
            this._nobtn.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this._cancelbtn.ResumeLayout(false);
            this._yesbtn.ResumeLayout(false);
            this._okbtn.ResumeLayout(false);
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
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        public System.Windows.Forms.Panel _yesbtn;
        private System.Windows.Forms.Label _yeslabel;
        public System.Windows.Forms.Panel _cancelbtn;
        private System.Windows.Forms.Label _cancellabel;
        private System.Windows.Forms.PictureBox _close;
        public System.Windows.Forms.Panel _okbtn;
        private System.Windows.Forms.Label _oklabel;
        public System.Windows.Forms.TextBox _path;
    }
}