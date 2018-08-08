using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace BetterMiniMap
{
    class TerrainOverlayDef : Def
    {
        public Type overlayClass;
    }

    [StaticConstructorOnStartup]
    static class Testing
    {
        static Testing()
        {
            Log.Message($"{DefDatabase<TerrainOverlayDef>.AllDefs.Count()}");
        }
    }
}
