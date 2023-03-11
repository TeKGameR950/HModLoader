namespace Launcher.Forms
{
    partial class NotificationForm
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
            this._notification = new System.Windows.Forms.Panel();
            this._remainingbg = new System.Windows.Forms.Panel();
            this._remaining = new System.Windows.Forms.Panel();
            this._loadbg = new System.Windows.Forms.Panel();
            this._load = new System.Windows.Forms.Panel();
            this._notifclosebtn = new System.Windows.Forms.PictureBox();
            this._notiftext = new System.Windows.Forms.Label();
            this._notificon = new System.Windows.Forms.PictureBox();
            this._notification.SuspendLayout();
            this._remainingbg.SuspendLayout();
            this._loadbg.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._notifclosebtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._notificon)).BeginInit();
            this.SuspendLayout();
            // 
            // _notification
            // 
            this._notification.AutoSize = true;
            this._notification.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._notification.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(25)))));
            this._notification.Controls.Add(this._remainingbg);
            this._notification.Controls.Add(this._loadbg);
            this._notification.Controls.Add(this._notifclosebtn);
            this._notification.Controls.Add(this._notiftext);
            this._notification.Controls.Add(this._notificon);
            this._notification.Location = new System.Drawing.Point(10, 10);
            this._notification.Margin = new System.Windows.Forms.Padding(0);
            this._notification.Name = "_notification";
            this._notification.Size = new System.Drawing.Size(176, 45);
            this._notification.TabIndex = 6;
            this._notification.Tag = "r:5";
            // 
            // _remainingbg
            // 
            this._remainingbg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(22)))));
            this._remainingbg.Controls.Add(this._remaining);
            this._remainingbg.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._remainingbg.Location = new System.Drawing.Point(0, 41);
            this._remainingbg.Margin = new System.Windows.Forms.Padding(0);
            this._remainingbg.Name = "_remainingbg";
            this._remainingbg.Size = new System.Drawing.Size(176, 4);
            this._remainingbg.TabIndex = 8;
            this._remainingbg.Tag = "";
            // 
            // _remaining
            // 
            this._remaining.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this._remaining.Location = new System.Drawing.Point(0, 0);
            this._remaining.Margin = new System.Windows.Forms.Padding(5);
            this._remaining.Name = "_remaining";
            this._remaining.Size = new System.Drawing.Size(83, 10);
            this._remaining.TabIndex = 8;
            // 
            // _loadbg
            // 
            this._loadbg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._loadbg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(22)))));
            this._loadbg.Controls.Add(this._load);
            this._loadbg.Location = new System.Drawing.Point(5, 34);
            this._loadbg.Margin = new System.Windows.Forms.Padding(5);
            this._loadbg.Name = "_loadbg";
            this._loadbg.Size = new System.Drawing.Size(166, 6);
            this._loadbg.TabIndex = 7;
            this._loadbg.Tag = "r:2";
            // 
            // _load
            // 
            this._load.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(182)))), ((int)(((byte)(125)))));
            this._load.Location = new System.Drawing.Point(0, 0);
            this._load.Margin = new System.Windows.Forms.Padding(5);
            this._load.Name = "_load";
            this._load.Size = new System.Drawing.Size(83, 10);
            this._load.TabIndex = 8;
            // 
            // _notifclosebtn
            // 
            this._notifclosebtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._notifclosebtn.BackgroundImage = global::Launcher.Properties.Resources.close;
            this._notifclosebtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._notifclosebtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this._notifclosebtn.Location = new System.Drawing.Point(20, -7);
            this._notifclosebtn.Margin = new System.Windows.Forms.Padding(0);
            this._notifclosebtn.Name = "_notifclosebtn";
            this._notifclosebtn.Size = new System.Drawing.Size(10, 10);
            this._notifclosebtn.TabIndex = 3;
            this._notifclosebtn.TabStop = false;
            this._notifclosebtn.Tag = "btn:img";
            // 
            // _notiftext
            // 
            this._notiftext.AutoSize = true;
            this._notiftext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(25)))));
            this._notiftext.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._notiftext.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(161)))), ((int)(((byte)(178)))));
            this._notiftext.Location = new System.Drawing.Point(40, 10);
            this._notiftext.Margin = new System.Windows.Forms.Padding(10, 10, 17, 15);
            this._notiftext.MaximumSize = new System.Drawing.Size(185, 0);
            this._notiftext.MinimumSize = new System.Drawing.Size(0, 20);
            this._notiftext.Name = "_notiftext";
            this._notiftext.Size = new System.Drawing.Size(119, 20);
            this._notiftext.TabIndex = 0;
            this._notiftext.Text = "Notification Text";
            this._notiftext.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _notificon
            // 
            this._notificon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(25)))));
            this._notificon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._notificon.InitialImage = null;
            this._notificon.Location = new System.Drawing.Point(10, 10);
            this._notificon.Margin = new System.Windows.Forms.Padding(0);
            this._notificon.Name = "_notificon";
            this._notificon.Size = new System.Drawing.Size(20, 20);
            this._notificon.TabIndex = 1;
            this._notificon.TabStop = false;
            this._notificon.Tag = "";
            // 
            // NotificationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(25)))));
            this.ClientSize = new System.Drawing.Size(300, 300);
            this.Controls.Add(this._notification);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "NotificationForm";
            this.Text = "NotificationForm";
            this.TransparencyKey = System.Drawing.Color.Magenta;
            this._notification.ResumeLayout(false);
            this._notification.PerformLayout();
            this._remainingbg.ResumeLayout(false);
            this._loadbg.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._notifclosebtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._notificon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Panel _notification;
        public System.Windows.Forms.PictureBox _notifclosebtn;
        public System.Windows.Forms.Label _notiftext;
        public System.Windows.Forms.PictureBox _notificon;
        public System.Windows.Forms.Panel _loadbg;
        public System.Windows.Forms.Panel _load;
        public System.Windows.Forms.Panel _remainingbg;
        public System.Windows.Forms.Panel _remaining;
    }
}