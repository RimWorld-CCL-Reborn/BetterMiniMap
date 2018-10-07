using System;
using System.Linq;
using System.Collections.Generic;
using Verse;
using BetterMiniMap.Overlays;

namespace BetterMiniMap
{
    public class OverlayManager
    {
        public FogOverlay FogOverlay;
        public MiningOverlay MiningOverlay;
        public BuildingsOverlay BuildingOverlay;
        public PowerGridOverlay PowerOverlay;
        public TerrainOverlay TerrainOverlay;
        public ViewpointOverlay ViewpointOverlay;
        public AreaOverlay AreaOverlay;

        public List<Overlay> DefOverlays;
        private List<Overlay> overlays;

        public OverlayManager()
        {
            FogOverlay = new FogOverlay();
            MiningOverlay = new MiningOverlay();
            BuildingOverlay = new BuildingsOverlay();
            PowerOverlay = new PowerGridOverlay();
            TerrainOverlay = new TerrainOverlay();
            ViewpointOverlay = new ViewpointOverlay();
            AreaOverlay = new AreaOverlay();
            
            // handle OverlayDefs
            DefOverlays = new List<Overlay>();
            foreach (OverlayDef def in DefDatabase<OverlayDef>.AllDefs.Where(d => d.selectors!=null))
            {
#if DEBUG
                Log.Message($"OverlayManager.cctor: {def.defName} -> {def.disabled}");
#endif
                Overlay overlay = (Overlay)Activator.CreateInstance(def.overlayClass, new object[] {def, def.visible});
                DefOverlays.Add(overlay);
            }
            // add tracking for settings
            OverlaySettingDatabase.InitializeOverlaySettings();
        }

        private IOrderedEnumerable<Overlay> GetOverlays()
        {
            List<Overlay> overlays = new List<Overlay>()
            {
                TerrainOverlay,
                BuildingOverlay,
                PowerOverlay,
                FogOverlay,
                MiningOverlay,
                ViewpointOverlay,
            };

            foreach (Overlay overlay in DefOverlays)
                overlays.Add(overlay);

            return overlays.OrderBy(o=> o.OverlayPriority);
        }

        public List<Overlay> Overlays
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
