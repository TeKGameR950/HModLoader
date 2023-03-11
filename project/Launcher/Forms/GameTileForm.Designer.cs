namespace Launcher.Forms
{
    partial class GameTileForm
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
            this._playbtn = new System.Windows.Forms.Panel();
            this._playbtnicon = new System.Windows.Forms.Panel();
            this._settingsbtn = new System.Windows.Forms.Panel();
            this._settingsbtnicon = new System.Windows.Forms.Panel();
            this._versionbg = new System.Windows.Forms.Panel();
            this._versionlabel = new System.Windows.Forms.Label();
            this._errormsg = new System.Windows.Forms.Label();
            this._favbtn = new System.Windows.Forms.Panel();
            this._favbtnicon = new System.Windows.Forms.Panel();
            this._banner = new System.Windows.Forms.PictureBox();
            this._playbtn.SuspendLayout();
            this._settingsbtn.SuspendLayout();
            this._versionbg.SuspendLayout();
            this._favbtn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._banner)).BeginInit();
            this.SuspendLayout();
            // 
            // _playbtn
            // 
            this._playbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(182)))), ((int)(((byte)(125)))));
            this._playbtn.Controls.Add(this._playbtnicon);
            this._playbtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this._playbtn.Location = new System.Drawing.Point(91, 155);
            this._playbtn.Name = "_playbtn";
            this._playbtn.Size = new System.Drawing.Size(40, 40);
            this._playbtn.TabIndex = 1;
            this._playbtn.Tag = "r:5;btn:bg;linkto:_playbtnicon";
            // 
            // _playbtnicon
            // 
            this._playbtnicon.BackColor = System.Drawing.Color.Transparent;
            this._playbtnicon.BackgroundImage = global::Launcher.Properties.Resources.play_solid;
            this._playbtnicon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._playbtnicon.Cursor = System.Windows.Forms.Cursors.Hand;
            this._playbtnicon.Location = new System.Drawing.Point(5, 5);
            this._playbtnicon.Name = "_playbtnicon";
            this._playbtnicon.Size = new System.Drawing.Size(30, 30);
            this._playbtnicon.TabIndex = 2;
            this._playbtnicon.Tag = "btn:img;linkto:_playbtn";
            // 
            // _settingsbtn
            // 
            this._settingsbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(117)))), ((int)(((byte)(126)))));
            this._settingsbtn.Controls.Add(this._settingsbtnicon);
            this._settingsbtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this._settingsbtn.Location = new System.Drawing.Point(5, 155);
            this._settingsbtn.Name = "_settingsbtn";
            this._settingsbtn.Size = new System.Drawing.Size(40, 40);
            this._settingsbtn.TabIndex = 3;
            this._settingsbtn.Tag = "r:5;btn:bg;linkto:_settingsbtnicon";
            // 
            // _settingsbtnicon
            // 
            this._settingsbtnicon.BackColor = System.Drawing.Color.Transparent;
            this._settingsbtnicon.BackgroundImage = global::Launcher.Properties.Resources.cogs_solid;
            this._settingsbtnicon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._settingsbtnicon.Cursor = System.Windows.Forms.Cursors.Hand;
            this._settingsbtnicon.Location = new System.Drawing.Point(5, 5);
            this._settingsbtnicon.Name = "_settingsbtnicon";
            this._settingsbtnicon.Size = new System.Drawing.Size(30, 30);
            this._settingsbtnicon.TabIndex = 2;
            this._settingsbtnicon.Tag = "r:5;btn:img;linkto:_settingsbtn";
            // 
            // _versionbg
            // 
            this._versionbg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(26)))));
            this._versionbg.Controls.Add(this._versionlabel);
            this._versionbg.Location = new System.Drawing.Point(5, 120);
            this._versionbg.Name = "_versionbg";
            this._versionbg.Size = new System.Drawing.Size(126, 25);
            this._versionbg.TabIndex = 2;
            this._versionbg.Tag = "r:5";
            // 
            // _versionlabel
            // 
            this._versionlabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._versionlabel.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._versionlabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(254)))));
            this._versionlabel.Location = new System.Drawing.Point(0, 0);
            this._versionlabel.Name = "_versionlabel";
            this._versionlabel.Size = new System.Drawing.Size(126, 25);
            this._versionlabel.TabIndex = 1;
            this._versionlabel.Tag = "r:5";
            this._versionlabel.Text = "7.1.12";
            this._versionlabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _errormsg
            // 
            this._errormsg.Cursor = System.Windows.Forms.Cursors.Arrow;
            this._errormsg.Font = new System.Drawing.Font("Roboto Condensed", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._errormsg.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this._errormsg.Location = new System.Drawing.Point(0, 0);
            this._errormsg.Name = "_errormsg";
            this._errormsg.Size = new System.Drawing.Size(136, 18);
            this._errormsg.TabIndex = 7;
            this._errormsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _favbtn
            // 
            this._favbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(117)))), ((int)(((byte)(126)))));
            this._favbtn.Controls.Add(this._favbtnicon);
            this._favbtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this._favbtn.Location = new System.Drawing.Point(5, 115);
            this._favbtn.Name = "_favbtn";
            this._favbtn.Size = new System.Drawing.Size(30, 30);
            this._favbtn.TabIndex = 8;
            this._favbtn.Tag = "r:10;btn:bg;linkto:_favbtnicon";
            // 
            // _favbtnicon
            // 
            this._favbtnicon.BackColor = System.Drawing.Color.Transparent;
            this._favbtnicon.BackgroundImage = global::Launcher.Properties.Resources.star1;
            this._favbtnicon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._favbtnicon.Cursor = System.Windows.Forms.Cursors.Hand;
            this._favbtnicon.Location = new System.Drawing.Point(2, 2);
            this._favbtnicon.Name = "_favbtnicon";
            this._favbtnicon.Size = new System.Drawing.Size(25, 25);
            this._favbtnicon.TabIndex = 2;
            this._favbtnicon.Tag = "btn:img;linkto:_favbtn";
            // 
            // _banner
            // 
            this._banner.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(26)))));
            this._banner.Location = new System.Drawing.Point(0, 0);
            this._banner.Name = "_banner";
            this._banner.Size = new System.Drawing.Size(136, 150);
            this._banner.TabIndex = 4;
            this._banner.TabStop = false;
            // 
            // GameTileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(26)))));
            this.ClientSize = new System.Drawing.Size(136, 200);
            this.Controls.Add(this._favbtn);
            this.Controls.Add(this._errormsg);
            this.Controls.Add(this._versionbg);
            this.Controls.Add(this._banner);
            this.Controls.Add(this._settingsbtn);
            this.Controls.Add(this._playbtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "GameTileForm";
            this.Tag = "r:5";
            this.Text = "GameTileForm";
            this._playbtn.ResumeLayout(false);
            this._settingsbtn.ResumeLayout(false);
            this._versionbg.ResumeLayout(false);
            this._favbtn.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._banner)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Panel _playbtn;
        public System.Windows.Forms.Panel _playbtnicon;
        public System.Windows.Forms.Panel _settingsbtn;
        public System.Windows.Forms.Panel _settingsbtnicon;
        public System.Windows.Forms.PictureBox _banner;
        public System.Windows.Forms.Panel _versionbg;
        public System.Windows.Forms.Label _errormsg;
        public System.Windows.Forms.Label _versionlabel;
        public System.Windows.Forms.Panel _favbtn;
        public System.Windows.Forms.Panel _favbtnicon;
    }
}