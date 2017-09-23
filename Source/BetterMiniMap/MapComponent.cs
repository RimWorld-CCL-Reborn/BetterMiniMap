using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace BetterMiniMap
{
    class MiniMap_MapComponent : MapComponent
    {
        public MiniMap_MapComponent(Map map) : base(map)
        {
#if DEBUG
            Log.Message("MiniMap_MapComponent");
#endif
            MiniMap_GameComponent.InitializeLocality(map);
        }
    }
}
