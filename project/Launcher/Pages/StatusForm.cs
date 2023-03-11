using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher.Pages
{
    public partial class StatusForm : PageForm
    {
        public static Color FAILED_COLOR = Color.FromArgb(255, 217, 43, 43);
        public static Color SUCCESS_COLOR = Color.FromArgb(255, 44, 182, 125);
        public static Color FETCHING_COLOR = Color.FromArgb(255, 3, 186, 252);

        public static Dictionary<string, Timer> timers = new Dictionary<string, Timer>();


        public StatusForm()
        {
            InitializeComponent();
        }

        public List<string> dots = new List<string>()
        {
            "",
            ".",
            "..",
            "..."
        };

        public string error = "Could not be resolved !";

        public override void OnPageOpened()
        {
            timers.Clear();
            timers.ToList().ForEach(x => x.Value.Stop());
            CheckStatus("https://www.hmodding.com", "Fetching website{0}", "Could not reach the website !", "Available ! Reached in {0}ms", _websitestatus, _websitestatuslabel);
            CheckStatus("https://fastdl.hmodding.com", "Fetching fastdl server{0}", "Could not reach the FastDL server !", "Available ! Reached in {0}ms", _fastdlstatus, _fastdlstatuslabel);

        }

        public void CheckStatus(string url, string text, string errorText, string successText, Control statusIcon, Control statusLabel)
        {
            Timer timer = new Timer();
            timers.Add(url, timer);
            timer.Interval = 500;
            statusIcon.BackColor = FETCHING_COLOR;
            statusLabel.Text = string.Format(text, "");
            int current = 0;
            timer.Tick += (a, b) =>
            {
                statusLabel.Text = string.Format(text, dots[current]);
                if (current == dots.Count - 1)
                    current = 0;
                else
                    current++;
            };
            timer.Start();

            Action errorAction = () =>
            {
                timer.Stop();
                statusLabel.Text = error;
                statusIcon.BackColor = FAILED_COLOR;
            };

            Action<int> successAction = (time) =>
            {
                timer.Stop();
                statusLabel.Text = string.Format(successText, time);
                statusIcon.BackColor = SUCCESS_COLOR;
            };

            try
            {
                DateTime start = DateTime.Now;
                HUtils.DoWebRequest("https://www.hmodding.com").ContinueWith((r) =>
                {
                    DateTime end = DateTime.Now;
                    WebResult result = r.Result;
                    if (result.success)
                        this.Invoke(successAction, (end - start).Milliseconds);
                    else
                        this.Invoke(errorAction);
                });
            }
            catch
            {
                this.Invoke(errorAction);
            }
        }
    }
}
