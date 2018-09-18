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

        public static List<Overlay> DefOverlays;
        private static List<Overlay> overlays;

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
            OverlayManager.DefOverlays = new List<Overlay>();
            foreach (OverlayDef def in DefDatabase<OverlayDef>.AllDefs.Where(d => d.selectors!=null))
            {
#if DEBUG

                Log.Message($"OverlayManager.cctor: {def.defName} -> {def.disabled}");
#endif
                Overlay overlay = (Overlay)Activator.CreateInstance(def.overlayClass, new object[] {def, !def.disabled});
                OverlayManager.DefOverlays.Add(overlay);
            }
            // add tracking for settings
            OverlaySettingDatabase.InitializeOverlaySettings();
        }

        private static IOrderedEnumerable<Overlay> GetOverlays()
        {
            List<Overlay> overlays = new List<Overlay>()
            {
                OverlayManager.TerrainOverlay,
                OverlayManager.BuildingOverlay,
                OverlayManager.PowerOverlay,
                OverlayManager.FogOverlay,
                OverlayManager.MiningOverlay,
                OverlayManager.ViewpointOverlay,
            };

            foreach (Overlay overlay in OverlayManager.DefOverlays)
                overlays.Add(overlay);

            return overlays.OrderBy(o=> o.OverlayPriority);
        }

        public static List<Overlay> Overlays
        {
            get
            {
                if (overlays == null)
                    overlays = GetOverlays().ToList();
#if DEBUG
                foreach (Overlay o in overlays)
                    Log.Message($"{o}->{o.OverlayPriority}");
#endif
                return overlays;
            }
        }

        // TODO: revist this concept...
        //public static bool HasTiberium() => LoadedModManager.RunningMods.Any(m => m.Identifier.Equals("TiberiumRim"));
    }
}
