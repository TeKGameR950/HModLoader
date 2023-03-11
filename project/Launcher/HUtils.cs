using System.Runtime.InteropServices;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Drawing.Text;
using System.Reflection;
using System.Net;
using System.Text;
using System.IO;
using Microsoft.Win32;
using Launcher.Forms;

namespace Launcher
{
    public static class HColors
    {
        public static readonly Color BTN_RED = Color.FromArgb(255, 217, 43, 43);
        public static readonly Color BTN_GREY = Color.FromArgb(255, 114, 117, 126);
        public static readonly Color BTN_BLUE = Color.FromArgb(255, 3, 186, 252);
        public static readonly Color BTN_GREEN = Color.FromArgb(255, 44, 182, 125);
        public static readonly Color BTN_PURPLE = Color.FromArgb(255, 127, 90, 240);
        public static readonly Color BTN_YELLOW = Color.FromArgb(255, 251, 221, 116);
    }

    static class HUtils
    {
        public static Dictionary<Control, ButtonFadeData> buttonsFadeData = new Dictionary<Control, ButtonFadeData>();

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );

        [DllImport("gdi32.dll")]
        static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);
        public static PrivateFontCollection fonts = new PrivateFontCollection();

        public static DialogResult ShowPopup(string title, string message, MessageBoxButtons buttons = MessageBoxButtons.OK, string path = "")
        {
            using (HMessageBox box = new HMessageBox(title, message, buttons, path))
            {
                DialogResult result = box.ShowDialog();
                return result;
            }
        }

        public static void ShowPopupNonBlocking(string title, string message, MessageBoxButtons buttons = MessageBoxButtons.OK, string path = "")
        {
            using (HMessageBox box = new HMessageBox(title, message, buttons, path))
            {
                box.Show();
            }
        }

        public static string FindGameFolder(Game game, bool askUser = true)
        {
            if (game.appid != "0")
            {
                // Steam Games
                string steamFolder = HUtils.GetSteamFolder();
                if (Directory.Exists(steamFolder))
                {
                    string potential = Path.Combine(steamFolder, "steamapps", "common", game.name);
                    if (Directory.Exists(potential) && HUtils.IsGameFolderValid(potential, game))
                    {
                        DialogResult result = ShowPopup($"Confirm Installation Path", $"Please confirm the installation path for {game.prefix}ML by reviewing the path displayed below.\n\nPress Yes to proceed with installation, No to choose a different location, or Cancel to abort the installation.", MessageBoxButtons.YesNoCancel, potential);
                        if (result == DialogResult.Yes)
                            return potential;
                        else if (result == DialogResult.Cancel)
                            return null;
                        // No will continue the method and leads to folder selection.
                    }
                }
            }
            else
            {
                // Non Steam Games
            }

            var dialog = new FolderSelectDialog
            {
                InitialDirectory = "",
                Title = $"Please select your {game.name} folder where {game.executable} is located."
            };
            if (dialog.Show(MainForm.Get().Handle))
            {
                string selectedFolder = dialog.FileName;
                if (HUtils.IsGameFolderValid(selectedFolder, game))
                    return selectedFolder;
            }

            return null;
        }

        public static Dictionary<string, Timer> timers = new Dictionary<string, Timer>();
        public static async void SlideRightPage(this PageForm form, bool shown = true, int speed = 1)
        {
            int closedLoc = MainForm.Get().Width;
            int openLoc = MainForm.Get().Width - form.Width;
            form.PageOpened = shown;

            if (!timers.ContainsKey(form.Name))
                timers.Add(form.Name, new Timer());

            var timer = timers[form.Name];

            timer.Stop();
            timer.Dispose();
            timer = new Timer();
            timers[form.Name] = timer;

            timer.Interval = 1;
            timer.Tick += (sender, args) =>
            {
                if (shown && form.Location.X > openLoc)
                {
                    form.Location = new Point(Math.Max(openLoc, Math.Min(closedLoc, form.Location.X - speed)), form.Location.Y);
                }
                else if (!shown && form.Location.X < closedLoc)
                {
                    form.Location = new Point(Math.Max(openLoc, Math.Min(closedLoc, form.Location.X + speed)), form.Location.Y);
                }
                else
                {
                    timer.Stop();
                    timer.Dispose();
                    timer = new Timer();
                    timers[form.Name] = timer;
                }
            };

            timer.Start();
        }

        public static byte[] ToByteArray(this Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

        public static async Task<WebResult> DoWebRequest(string url)
        {
            WebResult finalResult = new WebResult();
            Task waitTask = Task.Delay(15000);
            try
            {
                Task<string> webTask = WebRequest(new Uri(url));
                if (await Task.WhenAny(webTask, waitTask).ConfigureAwait(true) == webTask)
                {
                    if (webTask.Result.Length > 10 && !webTask.IsFaulted && webTask.Exception == null && !webTask.IsCanceled && webTask.IsCompleted)
                    {
                        finalResult.success = true;
                        finalResult.result = webTask.Result;
                    }
                    else
                    {
                        if (webTask.Exception != null)
                        {
                            finalResult.error = FlattenException(webTask.Exception);
                        }
                        else
                        {
                            finalResult.error = "An unknown error occured !";
                        }
                    }
                }
            }
            catch (Exception ex) { return new WebResult() { error = FlattenException(ex) }; }
            return finalResult;
        }

        private static async Task<string> WebRequest(Uri uri)
        {
            WebDownload wd = new WebDownload();
            wd.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            string result = await wd.DownloadStringTaskAsync(uri);
            wd.Dispose();
            return result;
        }

        public static string FlattenException(Exception exception)
        {
            var stringBuilder = new StringBuilder();

            while (exception != null)
            {
                stringBuilder.AppendLine(exception.Message);
                stringBuilder.AppendLine(exception.StackTrace);

                exception = exception.InnerException;
            }
            return stringBuilder.ToString();
        }

        public static void LoadFont()
        {
            try
            {
                byte[] fontData = Launcher.Properties.Resources.Roboto_Regular;
                IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
                Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
                uint dummy = 0;
                fonts.AddMemoryFont(fontPtr, fontData.Length);
                AddFontMemResourceEx(fontPtr, (uint)fontData.Length, IntPtr.Zero, ref dummy);
                Marshal.FreeCoTaskMem(fontPtr);
            }
            catch { }
        }

        public static async Task FadeIn(this Form o, int interval = 80)
        {
            await Task.Delay(50);
            while (o.Opacity < 1.0)
            {
                await Task.Delay(interval);
                o.Opacity += 0.07;
            }
            o.Opacity = 1;
        }

        public static async Task CloseForm(this Form o, int interval = 80)
        {
            while (o.Opacity > 0.0)
            {
                await Task.Delay(interval);
                o.Opacity -= 0.07;
            }
            o.Opacity = 0;
            Application.Exit();
        }

        public static void InitControlUtilsRecursive(this Control ctrl, Color backColorOverride = new Color())
        {
            InitControlUtils(ctrl, backColorOverride);
            foreach (Control c in ctrl.Controls)
            {
                InitControlUtils(c, backColorOverride);
            }
        }

        public static void InitControlUtils(this Control ctrl, Color backColorOverride = new Color())
        {
            // Make rounded borders for controls with "r:<value>" tag.
            ctrl.RoundCorners(ctrl.GetRoundTag());

            // Add fading effect for mouse events for controls with "btn:<value>"
            if (ctrl.HasTag("btn:"))
            {
                int value = 30;
                if (ctrl.HasTag("fade:"))
                    value = int.Parse(ctrl.GetTag("fade:"));
                string type = ctrl.GetTag("btn:");
                bool update = backColorOverride != new Color() && buttonsFadeData.ContainsKey(ctrl);
                if (backColorOverride != new Color())
                {
                    if (type == "bg")
                        ctrl.BackColor = backColorOverride;
                }
                if (update)
                    ApplyButtonColor(ctrl, false);
                ButtonFadeData origData = update ? buttonsFadeData[ctrl] : null;
                if (type == "img")
                {
                    ButtonFadeData data = new ButtonFadeData((update ? origData.originalImage : ctrl.BackgroundImage), (update ? origData.originalImage : ctrl.BackgroundImage).DarkenImage(value / 2), "img");
                    if (!buttonsFadeData.ContainsKey(ctrl))
                        buttonsFadeData.Add(ctrl, data);
                    else
                        buttonsFadeData[ctrl] = data;
                }
                else if (type == "bg")
                {
                    ButtonFadeData data = new ButtonFadeData((update ? backColorOverride : ctrl.BackColor), (update ? backColorOverride : ctrl.BackColor).DarkenColor(value), "bg");
                    if (!buttonsFadeData.ContainsKey(ctrl))
                        buttonsFadeData.Add(ctrl, data);
                    else
                        buttonsFadeData[ctrl] = data;
                }
                if (update)
                {
                    ApplyButtonColor(ctrl, false);
                }
                if (!update)
                {
                    ctrl.MouseEnter += (a, b) => ApplyButtonColor(ctrl, true);
                    ctrl.MouseLeave += (a, b) => ApplyButtonColor(ctrl, false);
                    ctrl.MouseUp += (a, b) => ApplyButtonColor(ctrl, true);
                    ctrl.MouseDown += (a, b) => ApplyButtonColor(ctrl, false);
                }
            }

            // Adding drag ability to controls with "drag" tag
            if (ctrl.HasTag("drag"))
            {
                ctrl.MouseDown += (a, b) => DragMouseDown(a, b);
                ctrl.MouseMove += (a, b) => DragMouseMove(a, b);
                ctrl.MouseUp += (a, b) => mouseDown = false;

            }
        }

        public static void InitFormUtils(this Form form)
        {
            foreach (Control ctrl in form.GetAllControls())
            {
                InitControlUtils(ctrl);
            }
        }

        public static Dictionary<string, Timer> tabsTimers = new Dictionary<string, Timer>();
        public static void SetTabSelected(Control ctrl, string name, bool isSelected)
        {
            if (!tabsTimers.ContainsKey(name))
                tabsTimers.Add(name, new Timer());

            var timer = tabsTimers[name];

            timer.Stop();
            timer.Dispose();
            timer = new Timer();
            tabsTimers[name] = timer;

            timer.Interval = 1;
            ctrl.Location = new Point(ctrl.Parent.Width / 2, ctrl.Location.Y);
            ctrl.Visible = true;
            int targetWidth = isSelected ? ctrl.Parent.Width : 0;
            timer.Tick += (sender, args) =>
            {
                if (isSelected ? (ctrl.Width < targetWidth) : (ctrl.Width > 0))
                {
                    ctrl.Width += isSelected ? 10 : -10;
                    ctrl.Location = new Point((ctrl.Parent.Width / 2) - (ctrl.Width / 2), ctrl.Location.Y);
                }
                else
                {
                    timer.Stop();
                }
            };
            timer.Start();
        }

        public static void ApplyButtonColor(Control ctrl, bool hover)
        {
            if (buttonsFadeData.TryGetValue(ctrl, out ButtonFadeData d))
            {
                d.state = hover;
                if (d.type == "bg")
                    ctrl.BackColor = hover ? d.hoverColor : d.originalColor;
                else if (d.type == "img")
                    ctrl.BackgroundImage = hover ? d.hoverImage : d.originalImage;
            }

            if (ctrl.HasTag("linkto:"))
                ctrl.GetTag("linkto:").Split(',').ToList().ForEach(link =>
                {
                    KeyValuePair<Control, ButtonFadeData> kvp = buttonsFadeData.Where(x => x.Key.FindForm() == ctrl.FindForm()).ToList().Find(x => x.Key.Name == link);
                    if (kvp.Key != null && kvp.Value != null && kvp.Value.state != hover)
                        ApplyButtonColor(kvp.Key, hover);
                });
        }

        public static async Task LoadGameBanner(Game game, PictureBox pic, string name, Control err)
        {
            string cachedBanner = Path.Combine(FileManager.folderCache_games, game.uniquename, name);

            try
            {
                if (File.Exists(cachedBanner)) pic.Load(cachedBanner);

                await Task.Run(() => { try { pic.Load($"{game.images}/{name}"); } catch { } });

                if (pic.Image != null)
                    File.WriteAllBytes(cachedBanner, pic.Image.ToByteArray());
                else if (!File.Exists(cachedBanner))
                    throw new Exception();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                pic.Image = null;
                err.Visible = err.Enabled = true;
                err.Height = 150;
                err.Text = game.name;
            }
        }

        public static void GetAllControls(Control container, List<Control> controlList)
        {
            foreach (Control c in container.Controls)
            {
                controlList.Add(c);
                GetAllControls(c, controlList);
            }
        }

        private static bool mouseDown;
        private static int mouseX = 0, mouseY = 0;
        private static int mouseinX = 0, mouseinY = 0;

        private static void DragMouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            mouseinX = Form.MousePosition.X - Form.ActiveForm.Bounds.X;
            mouseinY = Form.MousePosition.Y - Form.ActiveForm.Bounds.Y;
        }

        private static void DragMouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                mouseX = Form.MousePosition.X - mouseinX;
                mouseY = Form.MousePosition.Y - mouseinY;

                Form.ActiveForm.SetDesktopLocation(mouseX, mouseY);
            }
        }

        // Call this method to get a List of all controls on the form
        public static List<Control> GetAllControls(this Form form)
        {
            List<Control> controlList = new List<Control>();
            GetAllControls(form, controlList);
            return controlList;
        }

        public static List<Control> GetAllControls(this Control ctrl)
        {
            List<Control> controlList = new List<Control>();
            GetAllControls(ctrl, controlList);
            return controlList;
        }

        public static Bitmap DarkenImage(this Image image, float percent)
        {
            float brightness = -255 * percent / 100f;
            float scale = 1.0f + brightness / 255.0f;
            ColorMatrix matrix = new ColorMatrix(new float[][] {
        new float[] {scale, 0, 0, 0, 0},
        new float[] {0, scale, 0, 0, 0},
        new float[] {0, 0, scale, 0, 0},
        new float[] {0, 0, 0, 1, 0},
        new float[] {brightness / 255.0f, brightness / 255.0f, brightness / 255.0f, 0, 1}
    });
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(matrix);
            Bitmap result = new Bitmap(image.Width, image.Height);
            using (Graphics graphics = Graphics.FromImage(result))
            {
                graphics.DrawImage(image, new Rectangle(0, 0, result.Width, result.Height),
                    0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
            }
            return result;
        }

        public static string GetSteamFolder()
        {
            try
            {
                string steamKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Valve\\Steam";
                string steamPath = (string)Registry.GetValue(steamKey, "InstallPath", null);
                if (!string.IsNullOrEmpty(steamPath) && Directory.Exists(steamPath))
                    return steamPath;
                steamKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Valve\\Steam";
                steamPath = (string)Registry.GetValue(steamKey, "InstallPath", null);
                if (!string.IsNullOrEmpty(steamPath) && Directory.Exists(steamPath))
                    return steamPath;
            }
            catch { }
            return null;
        }

        public static bool IsGameFolderValid(string folder, Game game) => File.Exists(Path.Combine(folder, game.executable));

        public static Color DarkenColor(this Color color, float percent)
        {
            float amount = 1 - percent / 100f;
            int r = (int)(color.R * amount);
            int g = (int)(color.G * amount);
            int b = (int)(color.B * amount);
            return Color.FromArgb(color.A, r, g, b);
        }

        public static void RoundCorners(this Control ctrl, int radius = 10)
        {
            if (radius <= 0) return;
            ctrl.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, ctrl.Width, ctrl.Height, radius, radius));
        }

        public static int GetRoundTag(this Control ctrl)
        {
            int result = -1;
            if (ctrl.HasTag("r:"))
                result = int.Parse(ctrl.GetTag("r:"));
            return result;
        }

        public static bool HasTag(this Control ctrl, string tag)
        {
            if (ctrl.Tag == null) return false;
            return ctrl.Tag.ToString().Split(';').Any(x => x.StartsWith(tag));
        }

        public static string GetTag(this Control ctrl, string tag)
        {
            if (ctrl.Tag == null) return "";
            string[] tags = ctrl.Tag.ToString().Split(';');
            foreach (string t in tags)
            {
                if (t.ToLower().StartsWith(tag))
                    return t.Substring(tag.Length);
            }
            return "";
        }
    }

    public class WebResult
    {
        public bool success;
        public string result;
        public string error;
        public string localized_error;
    }

    public class WebDownload : WebClient
    {
        public int Timeout { get; set; }

        public WebDownload() : this(1) { }

        public WebDownload(int timeout)
        {
            this.Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = this.Timeout;
            }
            return request;
        }
    }

    public class ButtonFadeData
    {
        public string type;
        public Color originalColor;
        public Color hoverColor;
        public Image originalImage;
        public Image hoverImage;
        public int percent;
        public bool state;

        public ButtonFadeData(Color original, Color hover, string type)
        {
            this.originalColor = original;
            this.hoverColor = hover;
            this.type = type;
        }

        public ButtonFadeData(Image original, Bitmap hover, string type)
        {
            this.originalImage = original;
            this.hoverImage = hover;
            this.type = type;
        }
    }

}
