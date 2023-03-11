using System;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

namespace Launcher
{
    static class Program
    {

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Application.Run(new MainForm());
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("SharpZipLib"))
            {
                return Assembly.Load(GetEmbeddedFile("Dependencies.SharpZipLib.dll"));
            }
            else if (args.Name.Contains("INIFileParser"))
            {
                return Assembly.Load(GetEmbeddedFile("Dependencies.INIFileParser.dll"));
            }
            else if (args.Name.Contains("Newtonsoft.Json"))
            {
                return Assembly.Load(GetEmbeddedFile("Dependencies.Newtonsoft.Json.dll"));
            }
            else
            {
                string dllName = args.Name.Contains(",") ? args.Name.Substring(0, args.Name.IndexOf(',')) : args.Name.Replace(".dll", "");

                dllName = dllName.Replace(".", "_");

                if (dllName.EndsWith("_resources")) return null;

                System.Resources.ResourceManager rm = new System.Resources.ResourceManager(typeof(Program).Namespace + ".Properties.Resources", System.Reflection.Assembly.GetExecutingAssembly());

                byte[] bytes = (byte[])rm.GetObject(dllName);

                return Assembly.Load(bytes);
            }
        }

        public static byte[] GetEmbeddedFile(string filename)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Launcher.{filename}"))
            {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return assemblyData;
            }
        }
    }
}
