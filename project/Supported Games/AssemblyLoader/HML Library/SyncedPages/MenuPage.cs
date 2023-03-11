using UnityEngine;

namespace HMLLibrary
{
    public class MenuPage : MonoBehaviour
    {
        public virtual void Initialize() { }

        public virtual void OnPageOpen() { }

        public virtual void OnPageClose() { }
    }
}