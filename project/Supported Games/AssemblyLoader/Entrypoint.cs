using AssemblyLoader;
using HMLLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Doorstop
{
    class Entrypoint
    {
        public static void LoadCompiler()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ModManagerPage.ModAssemblyResolve);
            Assembly.Load(HLoader.GetAssemblyBytes("AssemblyLoader.Dependencies.HCompiler.Mono.dll"));
        }

        public static void Start()
        {
            try
            {
                Directory.CreateDirectory(HLoader.appdataFolder);
                Directory.CreateDirectory(HLoader.logsFolder);
            }
            catch { }
            if (AssemblyLoader.HLoader.StartedWithLauncher())
            {
                try
                {
                    LoadCompiler();
                    AssemblyLoader.HLoader.Load();
                }
                catch (Exception e) { File.WriteAllText("DOORSTOP_ERROR.log", e.ToString()); }
            }
        }

        public static void StartInject()
        {
            try
            {
                Directory.CreateDirectory(HLoader.appdataFolder);
                Directory.CreateDirectory(HLoader.logsFolder);
            }
            catch { }
            try
            {
                LoadCompiler();
                AssemblyLoader.HLoader.Load();
            }
            catch (Exception e) { File.WriteAllText("INJECT_ERROR.log", e.ToString()); }
        }
    }
}