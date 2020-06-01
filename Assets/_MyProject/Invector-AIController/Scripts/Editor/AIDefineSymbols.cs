#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
#endif
using System.Collections.Generic;
namespace Invector.DefineSymbolsManager
{
    public class AIDefineSymbols : InvectorDefineSymbols
    {
        public override List<string> GetSymbols
        {
            get
            {
                return new List<string>() { "INVECTOR_AI_TEMPLATE" };
            }
        }
    }  
}