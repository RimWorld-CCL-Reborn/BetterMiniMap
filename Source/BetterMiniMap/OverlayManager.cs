using System;
using System.Linq;
using System.Collections.Generic;
using Verse;
using BetterMiniMap.Overlays;

namespace BetterMiniMap
{
    // since I cannot figure out how to clean up garbage textues this should do.
    [StaticConstructorOnStartup]
    public static class OverlayManager
    {
        public static Fog_Overlay OverlayFog;
        public static Mining_Overlay OverlayMining;
        public static Buildings_Overlay OverlayBuilding;
        public static PowerGrid_Overlay OverlayPower;
        public static Terrain_Overlay OverlayTerrain;
        public static Viewpoint_Overlay OverlayView;
        public static Area_Overlay OverlayArea;

        public static List<Overlay> Overlays;

        static OverlayManager()
        {
            OverlayFog = new Fog_Overlay();
            OverlayMining = new Mining_Overlay();
            OverlayBuilding = new Buildings_Overlay();
            OverlayPower = new PowerGrid_Overlay();
            OverlayTerrain = new Terrain_Overlay();
            OverlayView = new Viewpoint_Overlay();
            OverlayArea = new Area_Overlay();
            
            // handle OverlayDefs
            OverlayManager.Overlays = new List<Overlay>();
            foreach (OverlayDef def in DefDatabase<OverlayDef>.AllDefs.Where(d => !d.disabled))
            {
                Overlay overlay = (Overlay)Activator.CreateInstance(def.overlayClass, new object[] {def, true});
                OverlayManager.Overlays.Add(overlay);
            }
        }

        // TODO: revist this concept...
        //public static bool HasTiberium() => LoadedModManager.RunningMods.Any(m => m.Identifier.Equals("TiberiumRim"));
    }
}
