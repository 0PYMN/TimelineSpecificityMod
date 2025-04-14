using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TimelineSpecificityMod
{
    public static class Materials
    {
        public static Material _enemyMaterialInstance
        {
            get
            {
                return LoadedAssetsHandler.GetEnemy("Mung_EN").enemyTemplate._enemyMaterialInstance;
            }
        }
    }
}
