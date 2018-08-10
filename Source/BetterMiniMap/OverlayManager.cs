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
        public static FogOverlay OverlayFog;
        public static MiningOverlay OverlayMining;
        public static BuildingsOverlay OverlayBuilding;
        public static PowerGridOverlay OverlayPower;
        public static TerrainOverlay OverlayTerrain;
        public static ViewpointOverlay OverlayView;
        public static AreaOverlay OverlayArea;

        public static List<Overlay> Overlays;

        static OverlayManager()
        {
            OverlayFog = new FogOverlay();
            OverlayMining = new MiningOverlay();
            OverlayBuilding = new BuildingsOverlay();
            OverlayPower = new PowerGridOverlay();
            OverlayTerrain = new TerrainOverlay();
            OverlayView = new ViewpointOverlay();
            OverlayArea = new AreaOverlay();
            
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
