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

        public OverlayManager(Map map)
        {
            FogOverlay = new FogOverlay(map);
            MiningOverlay = new MiningOverlay(map);
            BuildingOverlay = new BuildingsOverlay(map);
            PowerOverlay = new PowerGridOverlay(map);
            TerrainOverlay = new TerrainOverlay(map);
            ViewpointOverlay = new ViewpointOverlay(map);
            AreaOverlay = new AreaOverlay(map);
            
            // handle OverlayDefs
            DefOverlays = new List<Overlay>();
            foreach (OverlayDef def in DefDatabase<OverlayDef>.AllDefs.Where(d => d.selectors!=null))
            {
                Overlay overlay = (Overlay)Activator.CreateInstance(def.overlayClass, new object[] {def, map, def.visible});
                DefOverlays.Add(overlay);
            }
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
                return overlays;
            }
        }

        // TODO: revist this concept...
        //public static bool HasTiberium() => LoadedModManager.RunningMods.Any(m => m.Identifier.Equals("TiberiumRim"));
    }
}
