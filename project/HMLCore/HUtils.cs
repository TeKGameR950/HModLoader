using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HML
{
    public class HUtils
    {
        public static byte[] GetAssemblyBytes(string embeddedResource)
        {
            try
            {
                byte[] ba = null;
                Assembly curAsm = Assembly.GetExecutingAssembly();

                using (Stream stm = curAsm.GetManifestResourceStream(embeddedResource))
                {
                    if (stm == null)
                    {
                        //HMLCore.LogError(embeddedResource + " could not be found in embedded resources !");
                        return null;
                    }

                    ba = new byte[(int)stm.Length];
                    stm.Read(ba, 0, (int)stm.Length);
                    if (ba.Length > 1000)
                    {
                        return ba;
                    }
                }
            }
            catch { }
            return null;
        }

        public static Assembly ModAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly modAssembly = args.RequestingAssembly;
            string modName = GetModnameFromAssembly(modAssembly);
            string dllName = args.Name.Split(',')[0];
            try
            {
                byte[] assembly = HUtils.GetAssemblyBytes("HML.Dependencies.HCompiler.Core." + dllName + ".dll");
                if (assembly != null && assembly.Length > 0)
                {
                    return Assembly.Load(assembly);
                }
            }
            catch { }
            if (dllName == "Microsoft.CodeAnalysis.CSharp.resources")
                return AppDomain.CurrentDomain.GetAssemblies().ToList().Find(x => x.FullName.Split(',')[0] == "Microsoft.CodeAnalysis.CSharp");
            if (modName != "" && modName != null)
            {
                //HMLCore.Log("[ModManager] " + modName + " > Retrieving hotloaded reference \"" + dllName + "\"...");
                /*if (activeModReferences.ContainsKey(modName))
                    foreach (Assembly reference in activeModReferences[modName])
                        if (reference.GetName().Name.EndsWith(args.Name))
                        {
                            //HMLCore.Log("[ModManager] " + modName + " > Successfully bound \"" + dllName + "\" to \"" + GetCleanAssemblyName(reference) + "\" !");
                            return reference;
                        }*/
            }
            foreach (Assembly reference in AppDomain.CurrentDomain.GetAssemblies())
            {
                string refName = reference.FullName.Split(',')[0];
                if (refName == dllName)
                {
                    return reference;
                }
                if (reference.GetName().Name.EndsWith(args.Name))
                {
                    //HMLCore.Log("[ModManager] Successfully bound hotloaded reference \"" + dllName + "\" to \"" + GetCleanAssemblyName(reference) + "\" !");
                    return reference;
                }
            }
            return null;
        }

        public static string GetCleanAssemblyName(Assembly asm)
        {
            return asm.FullName.Substring(0, asm.FullName.IndexOf(','));
        }

        public static string GetModnameFromAssembly(Assembly asm)
        {
            return "";
            //return modList.Find(x => x.modinfo.assembly == asm)?.jsonmodinfo.name;
        }
    }
}
