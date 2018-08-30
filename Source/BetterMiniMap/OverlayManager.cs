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
        public static FogOverlay FogOverlay;
        public static MiningOverlay MiningOverlay;
        public static BuildingsOverlay BuildingOverlay;
        public static PowerGridOverlay PowerOverlay;
        public static TerrainOverlay TerrainOverlay;
        public static ViewpointOverlay ViewpointOverlay;
        public static AreaOverlay AreaOverlay;

        public static List<Overlay> Overlays;

        static OverlayManager()
        {
            FogOverlay = new FogOverlay();
            MiningOverlay = new MiningOverlay();
            BuildingOverlay = new BuildingsOverlay();
            PowerOverlay = new PowerGridOverlay();
            TerrainOverlay = new TerrainOverlay();
            ViewpointOverlay = new ViewpointOverlay();
            AreaOverlay = new AreaOverlay();
            
            // handle OverlayDefs
            OverlayManager.Overlays = new List<Overlay>();
            foreach (OverlayDef def in DefDatabase<OverlayDef>.AllDefs.Where(d => !d.disabled))
            {
                Overlay overlay = (Overlay)Activator.CreateInstance(def.overlayClass, new object[] {def, true});
                OverlayManager.Overlays.Add(overlay);
            }
            // add tracking for settings
            OverlaySettingDatabase.InitializeOverlaySettings();
        }

        // TODO: revist this concept...
        //public static bool HasTiberium() => LoadedModManager.RunningMods.Any(m => m.Identifier.Equals("TiberiumRim"));
    }
}
