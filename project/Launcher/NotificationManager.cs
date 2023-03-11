using Launcher.Forms;
using Launcher.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;

namespace Launcher
{
    public static class NotificationManager
    {
        public static List<Notification> notifications = new List<Notification>();
        public static Bitmap info;
        public static Notification CreateNotification(string text, NotificationType type = NotificationType.Info)
        {
            NotificationForm form = new NotificationForm();
            Control notif = form.Controls[0];
            notif.Location = new Point(880, 550);
            PictureBox image = notif.Controls.Find("_notificon", true).First() as PictureBox;
            image.Image = Resources.info;
            Label label = notif.Controls.Find("_notiftext", true).First() as Label;
            label.Text = text;
            Control loadbg = notif.Controls.Find("_loadbg", true).First();
            Control load = notif.Controls.Find("_load", true).First();
            if (type != NotificationType.Download && type != NotificationType.Spinner)
            {
                loadbg.Enabled = false;
                loadbg.Visible = false;
                label.Margin = new Padding(10, 10, 17, 10);
            }
            Control closeBtn = notif.Controls.Find("_notifclosebtn", true).First();
            closeBtn.Location = new Point(notif.Width - 17, 5);
            closeBtn.MouseDown += async (a, b) =>
            {
                while (notif.Location.X < MainForm.Get().Width + 2)
                {
                    notif.Location = new Point(notif.Location.X + 20, notif.Location.Y);
                    await Task.Delay(1);
                }
                notifications = notifications.Where(x => x.control != notif).ToList();
                RecalculateHeights();
                MainForm.Get().Controls.Remove(notif);
            };
            Notification n = new Notification(notif, label, image, type, closeBtn);
            n.remainingbg.Visible = false;
            n.remainingbg.Enabled = false;
            switch (type)
            {
                case NotificationType.Success:
                    n.SetIcon(Properties.Resources.check);
                    Color success = Color.FromArgb(255, 44, 182, 125);
                    notif.BackColor = success;
                    image.BackColor = success;
                    label.BackColor = success;
                    label.ForeColor = Color.FromArgb(255, 255, 255, 254);
                    closeBtn.BackColor = success;
                    break;
                case NotificationType.Info:
                    n.SetIcon(Properties.Resources.info);
                    Color info = Color.FromArgb(255, 3, 186, 252);
                    notif.BackColor = info;
                    image.BackColor = info;
                    label.BackColor = info;
                    label.ForeColor = Color.FromArgb(255, 255, 255, 254);
                    closeBtn.BackColor = info;
                    break;
                case NotificationType.Warning:
                    n.SetIcon(Properties.Resources.warning);
                    Color warning = Color.FromArgb(255, 255, 140, 33);
                    notif.BackColor = warning;
                    image.BackColor = warning;
                    label.BackColor = warning;
                    label.ForeColor = Color.FromArgb(255, 255, 255, 254);
                    closeBtn.BackColor = warning;
                    break;
                case NotificationType.Error:
                    n.SetIcon(Properties.Resources.error);
                    Color error = Color.FromArgb(255, 217, 43, 43);
                    notif.BackColor = error;
                    image.BackColor = error;
                    label.BackColor = error;
                    label.ForeColor = Color.FromArgb(255, 255, 255, 254);
                    closeBtn.BackColor = error;
                    break;
                case NotificationType.Spinner:
                    n.SetIcon(Properties.Resources.cloudflare);
                    n.ToggleCloseBtn(false);
                    n.SetSpinner();
                    break;
                case NotificationType.Download:
                    n.SetIcon(Properties.Resources.download_solid);
                    n.SetProgress(0);
                    break;
            }

            form.InitFormUtils();
            MainForm.Get().Controls.Add(notif);
            MainForm.Get().Controls.SetChildIndex(notif, 0);
            Point desired = new Point(MainForm.Get().Width - notif.Width - 10, MainForm.Get().Height - notif.Height - 10 - notifications.Sum(x => x.control.Height + 10));
            notif.Location = new Point(desired.X + notif.Width + 12, desired.Y);
            form.Close();
            notifications.Add(n);
            StartMove(n, desired);
            return n;
        }

        public static async void StartMove(Notification notif, Point desired)
        {
            while (notif.control.Location.X > desired.X && !notif.closed)
            {
                notif.control.Location = new Point(Math.Max(desired.X, notif.control.Location.X - 20), notif.control.Location.Y);
                await Task.Delay(1);
            }
        }

        public static void RecalculateHeights()
        {
            List<Notification> n = new List<Notification>();
            notifications.ToArray().ToList().ForEach(async notif =>
            {
                Point desired = new Point(MainForm.Get().Width - notif.control.Width - 10, MainForm.Get().Height - notif.control.Height - 10 - n.Sum(a => a.control.Height + 10));
                n.Add(notif);
                while (notif.control.Location.Y < desired.Y)
                {
                    notif.control.Location = new Point(desired.X, Math.Max(desired.Y, Math.Min(0, notif.control.Location.Y + 10)));
                    await Task.Delay(1);
                }
            });
        }
    }

    public enum NotificationType
    {
        Success,
        Download,
        Normal,
        Error,
        Info,
        Warning,
        Spinner
    }

    public class Notification
    {
        public bool closed;
        public Control control;
        public Label label;
        public PictureBox icon;
        public Control closeBtn;
        public NotificationType type;
        public Timer timer;

        public Control loadbg;
        public Control load;

        public Control remainingbg;
        public Control remaining;

        public Notification(Control control, Label label, PictureBox icon, NotificationType type, Control closeBtn)
        {
            this.control = control;
            this.label = label;
            this.icon = icon;
            this.type = type;
            this.closeBtn = closeBtn;
            this.loadbg = control.Controls.Find("_loadbg", false).First();
            this.load = loadbg.Controls[0];

            this.remainingbg = control.Controls.Find("_remainingbg", false).First();
            this.remaining = remainingbg.Controls[0];
        }

        public Notification AutoClose(int duration = 10)
        {
            Timer t = new Timer();
            t.Interval = duration * 1000;
            t.Tick += (a, b) =>
            {
                Close();
                t.Stop();
                t.Dispose();
            };
            t.Start();
            remaining.Width = remainingbg.Width;
            Point startLoc = new Point(0, 0);
            Point endLoc = new Point(remainingbg.Width, 0);

            int frameCount = remainingbg.Width;
            int frameDuration = (duration*1000) / frameCount;
            int deltaX = 1;
            int frameIndex = 0;
            Timer anim = new Timer();
            anim.Interval = frameDuration;
            anim.Tick += (a, b) =>
            {
                if (frameIndex < frameCount)
                {
                    Point newLocation = new Point(remaining.Location.X + deltaX, 0);
                    remaining.Location = newLocation;
                    frameIndex++;
                }
                else
                {
                    anim.Stop();
                }
            };
            anim.Start();
            remainingbg.Visible = true;
            remainingbg.Enabled = true;

            return this;
        }

        public Notification SetClickAction(Action action, bool closeOnClick = false)
        {
            control.Cursor = Cursors.Hand;
            control.MouseDown += (a, b) =>
            {
                action();
                if (closeOnClick)
                    this?.Close();
            };
            control.GetAllControls().ForEach(x =>
            {
                x.Cursor = Cursors.Hand;
                if (x.Name != "_notifclosebtn")
                {
                    x.MouseDown += (a, b) =>
                    {
                        action();
                        if (closeOnClick)
                            this?.Close();
                    };
                }
            });
            
            return this;
        }

        public Notification SetSpinner(int size = 40, int speed = 10)
        {
            SetProgress(size);
            timer = new Timer();
            timer.Interval = speed;
            timer.Tick += (sender, args) =>
            {
                if (load.Location.X < loadbg.Width)
                {
                    load.Location = new Point(load.Location.X + 5, load.Location.Y);
                }
                else
                {
                    load.Location = new Point(-loadbg.Width, load.Location.Y);
                }
            };
            timer.Start();
            return this;
        }

        public Notification ToggleCloseBtn(bool shown)
        {
            closeBtn.Enabled = shown;
            closeBtn.Visible = shown;
            return this;
        }

        public Notification SetProgress(int progress)
        {
            if (timer != null && timer.Enabled)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }

            load.Location = new Point(0, 0);
            load.Width = (int)Math.Round(progress * ((float)(loadbg.Width / 100f)));
            return this;
        }

        public Notification SetText(string text)
        {
            label.Text = text;
            NotificationManager.RecalculateHeights();
            return this;
        }

        public Notification SetIcon(Bitmap img)
        {
            icon.BackgroundImage = img;
            return this;
        }

        public async void Close()
        {
            if (closed) return;
            closed = true;
            while (control.Location.X < MainForm.Get().Width + 2)
            {
                control.Location = new Point(control.Location.X + 20, control.Location.Y);
                await Task.Delay(1);
            }
            NotificationManager.notifications = NotificationManager.notifications.Where(x => x.control != control).ToList();
            MainForm.Get().Controls.Remove(control);

            NotificationManager.RecalculateHeights();
        }
    }
}
