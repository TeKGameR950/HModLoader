namespace Launcher
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this._topbar = new System.Windows.Forms.Panel();
            this._minimizebtn = new System.Windows.Forms.PictureBox();
            this._closebtn = new System.Windows.Forms.PictureBox();
            this._sidebar = new System.Windows.Forms.Panel();
            this._tabfav = new System.Windows.Forms.Panel();
            this._tabfavselect = new System.Windows.Forms.Panel();
            this._tabfavlabel = new System.Windows.Forms.Label();
            this._tabfavicon = new System.Windows.Forms.PictureBox();
            this._tabgames = new System.Windows.Forms.Panel();
            this._tabgamesselect = new System.Windows.Forms.Panel();
            this._tabgameslabel = new System.Windows.Forms.Label();
            this._tabgamesicon = new System.Windows.Forms.PictureBox();
            this._btnsettings = new System.Windows.Forms.Panel();
            this._btnsettingsicon = new System.Windows.Forms.PictureBox();
            this._btnstatus = new System.Windows.Forms.Panel();
            this._btnstatusicon = new System.Windows.Forms.PictureBox();
            this._btndiscord = new System.Windows.Forms.Panel();
            this._btndiscordicon = new System.Windows.Forms.PictureBox();
            this._btnsite = new System.Windows.Forms.Panel();
            this._btnsiteicon = new System.Windows.Forms.PictureBox();
            this._title = new System.Windows.Forms.PictureBox();
            this._gametiles = new System.Windows.Forms.FlowLayoutPanel();
            this._logo = new System.Windows.Forms.PictureBox();
            this._errormsg = new System.Windows.Forms.Label();
            this._galleryBlocker = new TransparentPanel();
            this._logodrag = new TransparentPanel();
            this._topbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._minimizebtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._closebtn)).BeginInit();
            this._sidebar.SuspendLayout();
            this._tabfav.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._tabfavicon)).BeginInit();
            this._tabgames.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._tabgamesicon)).BeginInit();
            this._btnsettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._btnsettingsicon)).BeginInit();
            this._btnstatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._btnstatusicon)).BeginInit();
            this._btndiscord.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._btndiscordicon)).BeginInit();
            this._btnsite.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._btnsiteicon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._title)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._logo)).BeginInit();
            this.SuspendLayout();
            // 
            // _topbar
            // 
            this._topbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(26)))));
            this._topbar.Controls.Add(this._minimizebtn);
            this._topbar.Controls.Add(this._closebtn);
            this._topbar.Cursor = System.Windows.Forms.Cursors.Arrow;
            this._topbar.Dock = System.Windows.Forms.DockStyle.Top;
            this._topbar.Location = new System.Drawing.Point(0, 0);
            this._topbar.Name = "_topbar";
            this._topbar.Size = new System.Drawing.Size(880, 30);
            this._topbar.TabIndex = 0;
            this._topbar.Tag = "drag";
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
            this._minimizebtn.Click += new System.EventHandler(this._minimizebtn_Click);
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
            this._closebtn.Click += new System.EventHandler(this._closebtn_Click);
            // 
            // _sidebar
            // 
            this._sidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(26)))));
            this._sidebar.Controls.Add(this._tabfav);
            this._sidebar.Controls.Add(this._tabgames);
            this._sidebar.Controls.Add(this._btnsettings);
            this._sidebar.Controls.Add(this._btnstatus);
            this._sidebar.Controls.Add(this._btndiscord);
            this._sidebar.Controls.Add(this._btnsite);
            this._sidebar.Controls.Add(this._title);
            this._sidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this._sidebar.Location = new System.Drawing.Point(0, 30);
            this._sidebar.Name = "_sidebar";
            this._sidebar.Size = new System.Drawing.Size(145, 520);
            this._sidebar.TabIndex = 1;
            // 
            // _tabfav
            // 
            this._tabfav.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(32)))));
            this._tabfav.Controls.Add(this._tabfavselect);
            this._tabfav.Controls.Add(this._tabfavlabel);
            this._tabfav.Controls.Add(this._tabfavicon);
            this._tabfav.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tabfav.Location = new System.Drawing.Point(5, 99);
            this._tabfav.Name = "_tabfav";
            this._tabfav.Size = new System.Drawing.Size(135, 40);
            this._tabfav.TabIndex = 8;
            this._tabfav.Tag = "r:10;btn:bg;linkto:_tabfavicon,_tabfavlabel";
            // 
            // _tabfavselect
            // 
            this._tabfavselect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(186)))), ((int)(((byte)(252)))));
            this._tabfavselect.Location = new System.Drawing.Point(0, 35);
            this._tabfavselect.Name = "_tabfavselect";
            this._tabfavselect.Size = new System.Drawing.Size(135, 5);
            this._tabfavselect.TabIndex = 2;
            this._tabfavselect.Visible = false;
            // 
            // _tabfavlabel
            // 
            this._tabfavlabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tabfavlabel.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._tabfavlabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this._tabfavlabel.Location = new System.Drawing.Point(40, 10);
            this._tabfavlabel.Name = "_tabfavlabel";
            this._tabfavlabel.Size = new System.Drawing.Size(90, 23);
            this._tabfavlabel.TabIndex = 0;
            this._tabfavlabel.Tag = "btn:bg;linkto:_tabfav,_tabfavicon";
            this._tabfavlabel.Text = "Favorites";
            this._tabfavlabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _tabfavicon
            // 
            this._tabfavicon.BackgroundImage = global::Launcher.Properties.Resources.star1;
            this._tabfavicon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._tabfavicon.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tabfavicon.Location = new System.Drawing.Point(10, 10);
            this._tabfavicon.Name = "_tabfavicon";
            this._tabfavicon.Size = new System.Drawing.Size(20, 20);
            this._tabfavicon.TabIndex = 1;
            this._tabfavicon.TabStop = false;
            this._tabfavicon.Tag = "btn:img;linkto:_tabfav,_tabfavlabel";
            // 
            // _tabgames
            // 
            this._tabgames.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(32)))));
            this._tabgames.Controls.Add(this._tabgamesselect);
            this._tabgames.Controls.Add(this._tabgameslabel);
            this._tabgames.Controls.Add(this._tabgamesicon);
            this._tabgames.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tabgames.Location = new System.Drawing.Point(5, 54);
            this._tabgames.Name = "_tabgames";
            this._tabgames.Size = new System.Drawing.Size(135, 40);
            this._tabgames.TabIndex = 7;
            this._tabgames.Tag = "r:10;btn:bg;linkto:_tabgamesicon,_tabgameslabel";
            // 
            // _tabgamesselect
            // 
            this._tabgamesselect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(186)))), ((int)(((byte)(252)))));
            this._tabgamesselect.Location = new System.Drawing.Point(0, 35);
            this._tabgamesselect.Name = "_tabgamesselect";
            this._tabgamesselect.Size = new System.Drawing.Size(135, 5);
            this._tabgamesselect.TabIndex = 3;
            this._tabgamesselect.Visible = false;
            // 
            // _tabgameslabel
            // 
            this._tabgameslabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tabgameslabel.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._tabgameslabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this._tabgameslabel.Location = new System.Drawing.Point(40, 10);
            this._tabgameslabel.Name = "_tabgameslabel";
            this._tabgameslabel.Size = new System.Drawing.Size(90, 23);
            this._tabgameslabel.TabIndex = 0;
            this._tabgameslabel.Tag = "btn:bg;linkto:_tabgames,_tabgamesicon";
            this._tabgameslabel.Text = "All Games";
            this._tabgameslabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _tabgamesicon
            // 
            this._tabgamesicon.BackgroundImage = global::Launcher.Properties.Resources.gamepad_solid;
            this._tabgamesicon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._tabgamesicon.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tabgamesicon.Location = new System.Drawing.Point(10, 10);
            this._tabgamesicon.Name = "_tabgamesicon";
            this._tabgamesicon.Size = new System.Drawing.Size(20, 20);
            this._tabgamesicon.TabIndex = 1;
            this._tabgamesicon.TabStop = false;
            this._tabgamesicon.Tag = "btn:img;linkto:_tabgames,_tabgameslabel";
            // 
            // _btnsettings
            // 
            this._btnsettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(32)))));
            this._btnsettings.Controls.Add(this._btnsettingsicon);
            this._btnsettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnsettings.Location = new System.Drawing.Point(110, 485);
            this._btnsettings.Name = "_btnsettings";
            this._btnsettings.Size = new System.Drawing.Size(30, 30);
            this._btnsettings.TabIndex = 6;
            this._btnsettings.Tag = "r:10;btn:bg;linkto:_btnsettingsicon";
            // 
            // _btnsettingsicon
            // 
            this._btnsettingsicon.BackgroundImage = global::Launcher.Properties.Resources.cogs_solid;
            this._btnsettingsicon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._btnsettingsicon.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnsettingsicon.Location = new System.Drawing.Point(5, 5);
            this._btnsettingsicon.Name = "_btnsettingsicon";
            this._btnsettingsicon.Size = new System.Drawing.Size(20, 20);
            this._btnsettingsicon.TabIndex = 1;
            this._btnsettingsicon.TabStop = false;
            this._btnsettingsicon.Tag = "btn:img;linkto:_btnsettings";
            // 
            // _btnstatus
            // 
            this._btnstatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(32)))));
            this._btnstatus.Controls.Add(this._btnstatusicon);
            this._btnstatus.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnstatus.Location = new System.Drawing.Point(75, 485);
            this._btnstatus.Name = "_btnstatus";
            this._btnstatus.Size = new System.Drawing.Size(30, 30);
            this._btnstatus.TabIndex = 5;
            this._btnstatus.Tag = "r:10;btn:bg;linkto:_btnstatusicon";
            // 
            // _btnstatusicon
            // 
            this._btnstatusicon.BackgroundImage = global::Launcher.Properties.Resources.signal_solid;
            this._btnstatusicon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._btnstatusicon.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnstatusicon.Location = new System.Drawing.Point(5, 5);
            this._btnstatusicon.Name = "_btnstatusicon";
            this._btnstatusicon.Size = new System.Drawing.Size(20, 20);
            this._btnstatusicon.TabIndex = 1;
            this._btnstatusicon.TabStop = false;
            this._btnstatusicon.Tag = "btn:img;linkto:_btnstatus";
            // 
            // _btndiscord
            // 
            this._btndiscord.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(32)))));
            this._btndiscord.Controls.Add(this._btndiscordicon);
            this._btndiscord.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btndiscord.Location = new System.Drawing.Point(40, 485);
            this._btndiscord.Name = "_btndiscord";
            this._btndiscord.Size = new System.Drawing.Size(30, 30);
            this._btndiscord.TabIndex = 4;
            this._btndiscord.Tag = "r:10;btn:bg;linkto:_btndiscordicon";
            // 
            // _btndiscordicon
            // 
            this._btndiscordicon.BackgroundImage = global::Launcher.Properties.Resources.discord_brands;
            this._btndiscordicon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._btndiscordicon.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btndiscordicon.Location = new System.Drawing.Point(5, 5);
            this._btndiscordicon.Name = "_btndiscordicon";
            this._btndiscordicon.Size = new System.Drawing.Size(20, 20);
            this._btndiscordicon.TabIndex = 1;
            this._btndiscordicon.TabStop = false;
            this._btndiscordicon.Tag = "btn:img;linkto:_btndiscord";
            // 
            // _btnsite
            // 
            this._btnsite.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(32)))));
            this._btnsite.Controls.Add(this._btnsiteicon);
            this._btnsite.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnsite.Location = new System.Drawing.Point(5, 485);
            this._btnsite.Name = "_btnsite";
            this._btnsite.Size = new System.Drawing.Size(30, 30);
            this._btnsite.TabIndex = 3;
            this._btnsite.Tag = "r:10;btn:bg;linkto:_btnsiteicon";
            // 
            // _btnsiteicon
            // 
            this._btnsiteicon.BackgroundImage = global::Launcher.Properties.Resources.globe_solid;
            this._btnsiteicon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._btnsiteicon.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnsiteicon.Location = new System.Drawing.Point(5, 5);
            this._btnsiteicon.Name = "_btnsiteicon";
            this._btnsiteicon.Size = new System.Drawing.Size(20, 20);
            this._btnsiteicon.TabIndex = 0;
            this._btnsiteicon.TabStop = false;
            this._btnsiteicon.Tag = "btn:img;linkto:_btnsite";
            // 
            // _title
            // 
            this._title.BackgroundImage = global::Launcher.Properties.Resources.hmodloader_text;
            this._title.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._title.Location = new System.Drawing.Point(25, 32);
            this._title.Name = "_title";
            this._title.Size = new System.Drawing.Size(100, 15);
            this._title.TabIndex = 2;
            this._title.TabStop = false;
            // 
            // _gametiles
            // 
            this._gametiles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(30)))), ((int)(((byte)(33)))));
            this._gametiles.Location = new System.Drawing.Point(158, 40);
            this._gametiles.Margin = new System.Windows.Forms.Padding(0);
            this._gametiles.Name = "_gametiles";
            this._gametiles.Size = new System.Drawing.Size(710, 500);
            this._gametiles.TabIndex = 0;
            this._gametiles.Tag = "r:10";
            // 
            // _logo
            // 
            this._logo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(26)))));
            this._logo.BackgroundImage = global::Launcher.Properties.Resources.hytek_logo_white;
            this._logo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._logo.Location = new System.Drawing.Point(52, 15);
            this._logo.Name = "_logo";
            this._logo.Size = new System.Drawing.Size(40, 40);
            this._logo.TabIndex = 1;
            this._logo.TabStop = false;
            this._logo.Tag = "";
            // 
            // _errormsg
            // 
            this._errormsg.Cursor = System.Windows.Forms.Cursors.Arrow;
            this._errormsg.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._errormsg.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this._errormsg.Location = new System.Drawing.Point(787, 488);
            this._errormsg.Name = "_errormsg";
            this._errormsg.Size = new System.Drawing.Size(88, 57);
            this._errormsg.TabIndex = 6;
            this._errormsg.Text = "Error Message";
            this._errormsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _galleryBlocker
            // 
            this._galleryBlocker.BackColor = System.Drawing.Color.Red;
            this._galleryBlocker.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this._galleryBlocker.Location = new System.Drawing.Point(145, 30);
            this._galleryBlocker.Name = "_galleryBlocker";
            this._galleryBlocker.Size = new System.Drawing.Size(732, 10);
            this._galleryBlocker.TabIndex = 5;
            this._galleryBlocker.Tag = "";
            // 
            // _logodrag
            // 
            this._logodrag.BackColor = System.Drawing.Color.Black;
            this._logodrag.Cursor = System.Windows.Forms.Cursors.Arrow;
            this._logodrag.Location = new System.Drawing.Point(52, 0);
            this._logodrag.Name = "_logodrag";
            this._logodrag.Size = new System.Drawing.Size(40, 30);
            this._logodrag.TabIndex = 4;
            this._logodrag.Tag = "drag";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(30)))), ((int)(((byte)(33)))));
            this.ClientSize = new System.Drawing.Size(880, 550);
            this.Controls.Add(this._errormsg);
            this.Controls.Add(this._galleryBlocker);
            this.Controls.Add(this._logodrag);
            this.Controls.Add(this._logo);
            this.Controls.Add(this._sidebar);
            this.Controls.Add(this._topbar);
            this.Controls.Add(this._gametiles);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "";
            this.Text = "HModLoader";
            this._topbar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._minimizebtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._closebtn)).EndInit();
            this._sidebar.ResumeLayout(false);
            this._tabfav.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._tabfavicon)).EndInit();
            this._tabgames.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._tabgamesicon)).EndInit();
            this._btnsettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._btnsettingsicon)).EndInit();
            this._btnstatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._btnstatusicon)).EndInit();
            this._btndiscord.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._btndiscordicon)).EndInit();
            this._btnsite.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._btnsiteicon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._title)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._logo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _topbar;
        private System.Windows.Forms.PictureBox _title;
        private System.Windows.Forms.PictureBox _logo;
        private System.Windows.Forms.Panel _sidebar;
        private System.Windows.Forms.Panel _btnsettings;
        private System.Windows.Forms.Panel _btnstatus;
        private System.Windows.Forms.Panel _btndiscord;
        private System.Windows.Forms.Panel _btnsite;
        private System.Windows.Forms.PictureBox _btnsettingsicon;
        private System.Windows.Forms.PictureBox _btnstatusicon;
        private System.Windows.Forms.PictureBox _btndiscordicon;
        private System.Windows.Forms.PictureBox _btnsiteicon;
        private System.Windows.Forms.PictureBox _closebtn;
        private System.Windows.Forms.PictureBox _minimizebtn;
        private TransparentPanel _logodrag;
        private TransparentPanel _galleryBlocker;
        public System.Windows.Forms.Label _errormsg;
        public System.Windows.Forms.FlowLayoutPanel _gametiles;
        private System.Windows.Forms.Panel _tabfav;
        private System.Windows.Forms.Label _tabfavlabel;
        private System.Windows.Forms.PictureBox _tabfavicon;
        private System.Windows.Forms.Panel _tabgames;
        private System.Windows.Forms.Label _tabgameslabel;
        private System.Windows.Forms.PictureBox _tabgamesicon;
        private System.Windows.Forms.Panel _tabfavselect;
        private System.Windows.Forms.Panel _tabgamesselect;
    }
}