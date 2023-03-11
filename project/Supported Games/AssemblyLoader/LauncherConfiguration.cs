using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyLoader
{
    [Serializable]
    public class LauncherConfiguration
    {
        public string gamePath = "";
        public string coreVersion = "?";
        public int agreeWithTOS = 0;
        public bool skipSplashScreen = false;
        public bool startGameFromSteam = false;
        public string branch = "public";
    }
}
