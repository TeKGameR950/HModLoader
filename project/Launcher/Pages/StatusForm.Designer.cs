namespace Launcher.Pages
{
    partial class StatusForm
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
            this._layout = new System.Windows.Forms.FlowLayoutPanel();
            this._updatebranch = new System.Windows.Forms.Panel();
            this._websitestatuslabel = new System.Windows.Forms.Label();
            this._websitestatus = new System.Windows.Forms.Panel();
            this._websitebtn = new System.Windows.Forms.Panel();
            this._websitebtnlabel = new System.Windows.Forms.Label();
            this._websitetitle = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this._fastdlstatuslabel = new System.Windows.Forms.Label();
            this._fastdlstatus = new System.Windows.Forms.Panel();
            this._fastdlbtn = new System.Windows.Forms.Panel();
            this._fastdlbtnlabel = new System.Windows.Forms.Label();
            this._fastdltitle = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this._layout.SuspendLayout();
            this._updatebranch.SuspendLayout();
            this._websitebtn.SuspendLayout();
            this.panel2.SuspendLayout();
            this._fastdlbtn.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel7.SuspendLayout();
            this.SuspendLayout();
            // 
            // _title
            // 
            this._title.Font = new System.Drawing.Font("Roboto Condensed", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this._title.Location = new System.Drawing.Point(5, 5);
            this._title.Name = "_title";
            this._title.Size = new System.Drawing.Size(129, 23);
            this._title.TabIndex = 2;
            this._title.Text = "Services Status";
            // 
            // _layout
            // 
            this._layout.Controls.Add(this._updatebranch);
            this._layout.Controls.Add(this.panel2);
            this._layout.Controls.Add(this.panel5);
            this._layout.Location = new System.Drawing.Point(10, 30);
            this._layout.Name = "_layout";
            this._layout.Size = new System.Drawing.Size(380, 478);
            this._layout.TabIndex = 6;
            // 
            // _updatebranch
            // 
            this._updatebranch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(26)))));
            this._updatebranch.Controls.Add(this._websitestatuslabel);
            this._updatebranch.Controls.Add(this._websitestatus);
            this._updatebranch.Controls.Add(this._websitebtn);
            this._updatebranch.Controls.Add(this._websitetitle);
            this._updatebranch.Location = new System.Drawing.Point(0, 0);
            this._updatebranch.Margin = new System.Windows.Forms.Padding(0);
            this._updatebranch.Name = "_updatebranch";
            this._updatebranch.Size = new System.Drawing.Size(380, 55);
            this._updatebranch.TabIndex = 0;
            this._updatebranch.Tag = "r:5";
            // 
            // _websitestatuslabel
            // 
            this._websitestatuslabel.Font = new System.Drawing.Font("Roboto Condensed", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._websitestatuslabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this._websitestatuslabel.Location = new System.Drawing.Point(31, 25);
            this._websitestatuslabel.Name = "_websitestatuslabel";
            this._websitestatuslabel.Size = new System.Drawing.Size(201, 20);
            this._websitestatuslabel.TabIndex = 7;
            this._websitestatuslabel.Text = "Available ! Reached in 1500ms";
            this._websitestatuslabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _websitestatus
            // 
            this._websitestatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(182)))), ((int)(((byte)(125)))));
            this._websitestatus.Location = new System.Drawing.Point(5, 25);
            this._websitestatus.Name = "_websitestatus";
            this._websitestatus.Size = new System.Drawing.Size(20, 20);
            this._websitestatus.TabIndex = 6;
            this._websitestatus.Tag = "r:50";
            // 
            // _websitebtn
            // 
            this._websitebtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(117)))), ((int)(((byte)(126)))));
            this._websitebtn.Controls.Add(this._websitebtnlabel);
            this._websitebtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this._websitebtn.Location = new System.Drawing.Point(238, 25);
            this._websitebtn.Name = "_websitebtn";
            this._websitebtn.Size = new System.Drawing.Size(139, 20);
            this._websitebtn.TabIndex = 5;
            this._websitebtn.Tag = "r:5;btn:bg;linkto:_branchpubliclabel";
            // 
            // _websitebtnlabel
            // 
            this._websitebtnlabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._websitebtnlabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this._websitebtnlabel.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._websitebtnlabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(254)))));
            this._websitebtnlabel.Location = new System.Drawing.Point(5, -5);
            this._websitebtnlabel.Name = "_websitebtnlabel";
            this._websitebtnlabel.Padding = new System.Windows.Forms.Padding(5);
            this._websitebtnlabel.Size = new System.Drawing.Size(129, 30);
            this._websitebtnlabel.TabIndex = 1;
            this._websitebtnlabel.Tag = "btn:bg;linkto:_branchpublicbtn";
            this._websitebtnlabel.Text = "Open in browser";
            this._websitebtnlabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _websitetitle
            // 
            this._websitetitle.Font = new System.Drawing.Font("Roboto Condensed", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._websitetitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this._websitetitle.Location = new System.Drawing.Point(2, 0);
            this._websitetitle.Name = "_websitetitle";
            this._websitetitle.Size = new System.Drawing.Size(161, 23);
            this._websitetitle.TabIndex = 2;
            this._websitetitle.Text = "HModding Website";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(26)))));
            this.panel2.Controls.Add(this._fastdlstatuslabel);
            this.panel2.Controls.Add(this._fastdlstatus);
            this.panel2.Controls.Add(this._fastdlbtn);
            this.panel2.Controls.Add(this._fastdltitle);
            this.panel2.Location = new System.Drawing.Point(0, 65);
            this.panel2.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(380, 55);
            this.panel2.TabIndex = 1;
            this.panel2.Tag = "r:5";
            // 
            // _fastdlstatuslabel
            // 
            this._fastdlstatuslabel.Font = new System.Drawing.Font("Roboto Condensed", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._fastdlstatuslabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this._fastdlstatuslabel.Location = new System.Drawing.Point(31, 25);
            this._fastdlstatuslabel.Name = "_fastdlstatuslabel";
            this._fastdlstatuslabel.Size = new System.Drawing.Size(201, 20);
            this._fastdlstatuslabel.TabIndex = 7;
            this._fastdlstatuslabel.Text = "Available ! Reached in 1500ms";
            this._fastdlstatuslabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _fastdlstatus
            // 
            this._fastdlstatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(182)))), ((int)(((byte)(125)))));
            this._fastdlstatus.Location = new System.Drawing.Point(5, 25);
            this._fastdlstatus.Name = "_fastdlstatus";
            this._fastdlstatus.Size = new System.Drawing.Size(20, 20);
            this._fastdlstatus.TabIndex = 6;
            this._fastdlstatus.Tag = "r:50";
            // 
            // _fastdlbtn
            // 
            this._fastdlbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(117)))), ((int)(((byte)(126)))));
            this._fastdlbtn.Controls.Add(this._fastdlbtnlabel);
            this._fastdlbtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this._fastdlbtn.Location = new System.Drawing.Point(238, 25);
            this._fastdlbtn.Name = "_fastdlbtn";
            this._fastdlbtn.Size = new System.Drawing.Size(139, 20);
            this._fastdlbtn.TabIndex = 5;
            this._fastdlbtn.Tag = "r:5;btn:bg;linkto:_branchpubliclabel";
            // 
            // _fastdlbtnlabel
            // 
            this._fastdlbtnlabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._fastdlbtnlabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this._fastdlbtnlabel.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._fastdlbtnlabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(254)))));
            this._fastdlbtnlabel.Location = new System.Drawing.Point(5, -5);
            this._fastdlbtnlabel.Name = "_fastdlbtnlabel";
            this._fastdlbtnlabel.Padding = new System.Windows.Forms.Padding(5);
            this._fastdlbtnlabel.Size = new System.Drawing.Size(129, 30);
            this._fastdlbtnlabel.TabIndex = 1;
            this._fastdlbtnlabel.Tag = "btn:bg;linkto:_branchpublicbtn";
            this._fastdlbtnlabel.Text = "Open in browser";
            this._fastdlbtnlabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _fastdltitle
            // 
            this._fastdltitle.Font = new System.Drawing.Font("Roboto Condensed", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._fastdltitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this._fastdltitle.Location = new System.Drawing.Point(2, 0);
            this._fastdltitle.Name = "_fastdltitle";
            this._fastdltitle.Size = new System.Drawing.Size(267, 23);
            this._fastdltitle.TabIndex = 2;
            this._fastdltitle.Text = "HModLoader FastDL && API";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(26)))));
            this.panel5.Controls.Add(this.label5);
            this.panel5.Controls.Add(this.panel6);
            this.panel5.Controls.Add(this.panel7);
            this.panel5.Controls.Add(this.label7);
            this.panel5.Location = new System.Drawing.Point(0, 130);
            this.panel5.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(380, 55);
            this.panel5.TabIndex = 2;
            this.panel5.Tag = "r:5";
            this.panel5.Visible = false;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Roboto Condensed", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this.label5.Location = new System.Drawing.Point(31, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(201, 20);
            this.label5.TabIndex = 7;
            this.label5.Text = "Available ! Reached in 1500ms";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(182)))), ((int)(((byte)(125)))));
            this.panel6.Location = new System.Drawing.Point(5, 25);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(20, 20);
            this.panel6.TabIndex = 6;
            this.panel6.Tag = "r:50";
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(117)))), ((int)(((byte)(126)))));
            this.panel7.Controls.Add(this.label6);
            this.panel7.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panel7.Location = new System.Drawing.Point(238, 25);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(139, 20);
            this.panel7.TabIndex = 5;
            this.panel7.Tag = "r:5;btn:bg;linkto:_branchpubliclabel";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label6.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(254)))));
            this.label6.Location = new System.Drawing.Point(5, -5);
            this.label6.Name = "label6";
            this.label6.Padding = new System.Windows.Forms.Padding(5);
            this.label6.Size = new System.Drawing.Size(129, 30);
            this.label6.TabIndex = 1;
            this.label6.Tag = "btn:bg;linkto:_branchpublicbtn";
            this.label6.Text = "Open in browser";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Roboto Condensed", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this.label7.Location = new System.Drawing.Point(2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(267, 23);
            this.label7.TabIndex = 2;
            this.label7.Text = "HModding Master Server";
            // 
            // StatusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(26)))));
            this.ClientSize = new System.Drawing.Size(400, 520);
            this.Controls.Add(this._layout);
            this.Controls.Add(this._title);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "StatusForm";
            this.Text = "StatusForm";
            this._layout.ResumeLayout(false);
            this._updatebranch.ResumeLayout(false);
            this._websitebtn.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this._fastdlbtn.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label _title;
        private System.Windows.Forms.FlowLayoutPanel _layout;
        private System.Windows.Forms.Panel _updatebranch;
        public System.Windows.Forms.Panel _websitebtn;
        public System.Windows.Forms.Label _websitebtnlabel;
        private System.Windows.Forms.Label _websitetitle;
        private System.Windows.Forms.Panel _websitestatus;
        private System.Windows.Forms.Label _websitestatuslabel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label _fastdlstatuslabel;
        private System.Windows.Forms.Panel _fastdlstatus;
        public System.Windows.Forms.Panel _fastdlbtn;
        public System.Windows.Forms.Label _fastdlbtnlabel;
        private System.Windows.Forms.Label _fastdltitle;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel6;
        public System.Windows.Forms.Panel panel7;
        public System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
    }
}