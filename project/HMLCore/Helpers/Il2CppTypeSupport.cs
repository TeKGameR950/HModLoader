using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.Runtime;
using Il2CppSystem.IO;
using System;
using System.Reflection;

namespace HML
{
    public class Il2CppTypeSupport
    {
        private static Type Il2CppObjectBaseType = null;
        internal static Type Il2CppMethodInfoType = null;

        internal static void Initialize()
        {
            Il2CppObjectBaseType = typeof(Il2CppObjectBase);
            Il2CppMethodInfoType = typeof(Il2CppMethodInfo);
        }

        public static bool IsGeneratedAssemblyType(Type type) => (!Il2CppObjectBaseType.Equals(null) && !type.Equals(null) && type.IsSubclassOf(Il2CppObjectBaseType));

        public static T Il2CppObjectPtrToIl2CppObject<T>(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                throw new NullReferenceException("The ptr cannot be IntPtr.Zero.");
            if (!IsGeneratedAssemblyType(typeof(T)))
                throw new NullReferenceException("The type must be a Generated Assembly Type.");
            return (T)typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(IntPtr) }, new ParameterModifier[0]).Invoke(new object[] { ptr });
        }
    }
}
