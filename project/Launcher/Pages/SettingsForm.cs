

namespace Launcher.Pages
{
    public partial class SettingsForm : PageForm
    {
        public SettingsForm()
        {
            InitializeComponent();
            _branchpublicbtn.MouseDown += (a, b) => ToggleBranch("public");
            _branchpubliclabel.MouseDown += (a, b) => ToggleBranch("public");
            _branchindevbtn.MouseDown += (a, b) => ToggleBranch("indev");
            _branchindevlabel.MouseDown += (a, b) => ToggleBranch("indev");
            _instancesCheckbox.OnChanged += (a, b) => OnMultipleInstancesChanged(b);
            _version.Text = $"HModLoader version {Properties.Settings.Default.version}";
        }

        public void OnMultipleInstancesChanged(bool current)
        {
            ConfigManager.config.AllowMultipleGameInstances = current;
            ConfigManager.UpdateConfig(ConfigManager.config);
            UpdateSettings();
        }

        public void ToggleBranch(string branch)
        {
            ConfigManager.config.UpdateBranch = branch;
            ConfigManager.UpdateConfig(ConfigManager.config);
            UpdateSettings();
        }

        public void UpdateSettings()
        {
            if (ConfigManager.config == null) return;
            _instancesCheckbox._checked = ConfigManager.config.AllowMultipleGameInstances;
            switch (ConfigManager.config.UpdateBranch)
            {
                case "public":
                    _branchpublicbtn.InitControlUtilsRecursive(HColors.BTN_BLUE);
                    _branchindevbtn.InitControlUtilsRecursive(HColors.BTN_GREY);
                    break;
                case "indev":
                    _branchpublicbtn.InitControlUtilsRecursive(HColors.BTN_GREY);
                    _branchindevbtn.InitControlUtilsRecursive(HColors.BTN_BLUE);
                    break;
            }
        }
    }
}
