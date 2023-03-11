using Il2CppInterop.Runtime.Injection;
using UnityEngine;

namespace HML
{
    public class MainBehaviour : MonoBehaviour
    {
        public static KeyCode MenuKey = KeyCode.F9;
        public static KeyCode ConsoleKey = KeyCode.F10;

        void Start()
        {
            /*
            // Everything we instantiate is under the MainBehaviour for easy unloading of the whole mod loader.
            HConsole.Initialize(Instantiate(HMLCore.hmlBundle.Load<GameObject>("HMLConsole"), Vector3.zero, Quaternion.identity, transform));
            */ClassInjector.RegisterTypeInIl2Cpp<TestBehaviour>();
            gameObject.AddComponent<TestBehaviour>();

            ClassInjector.RegisterTypeInIl2Cpp<TempRawSharp>();
            gameObject.AddComponent<TempRawSharp>();
        }
    }
}
