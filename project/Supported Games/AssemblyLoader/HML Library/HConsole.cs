using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HMLLibrary
{
    public class HConsole : MonoBehaviour
    {
        public static HConsole instance;
        public virtual void RefreshCommands() { throw new NotImplementedException(); }
    }
}
