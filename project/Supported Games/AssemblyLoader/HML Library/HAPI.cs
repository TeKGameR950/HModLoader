using System;
using System.Linq;
using System.Reflection;
using HMLLibrary;
namespace HMLLibrary
{
    public class HAPI
    {
        public static Mod GetMod(string name)
        {
            ModData data = ModManagerPage.modList.Where(m => m.modinfo.modState == ModInfo.ModStateEnum.running && m.modinfo.mainClass != null).ToList().Find(x => x.jsonmodinfo.name == name);
            if (data != null)
                return data.modinfo.mainClass;
            return null;
        }
    }

    public class ModMethod
    {
        public ModMethod(MethodInfo method)
        {
            Name = method.Name;
        }

        public string Name;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ModMethodAttribute : Attribute
    {
        public string Name { get; }

        public ModMethodAttribute(string name)
        {
            this.Name = name;
        }

        public ModMethodAttribute() { }
    }
}