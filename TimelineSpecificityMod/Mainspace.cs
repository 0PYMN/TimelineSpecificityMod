using BepInEx;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TimelineSpecificityMod
{
    [BepInPlugin("000.TimelineSpecificity", "Timeline Icon Enemy Specificity Mod", "0.0.0")]
    public class Mainspace : BaseUnityPlugin
    {
        public void Awake() => Add();

        public static void Add()
        {
        }
    }
}
