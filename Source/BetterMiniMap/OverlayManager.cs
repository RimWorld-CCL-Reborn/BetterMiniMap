using Verse;
using BetterMiniMap.Overlays;
using System.Linq;

namespace BetterMiniMap
{
    // since I cannot figure out how to clean up garbage textues this should do.
    [StaticConstructorOnStartup]
    public static class OverlayManager
    {
        public static Colonists_Overlay OverlayColonists;
        public static Fog_Overlay OverlayFog;
        public static Mining_Overlay OverlayMining;
        public static NonColonists_Overlay OverlayNoncolonist;
        public static Buildings_Overlay OverlayBuilding;
        public static PowerGrid_Overlay OverlayPower;
        public static Terrain_Overlay OverlayTerrain;
        public static Viewpoint_Overlay OverlayView;
        public static Wildlife_Overlay OverlayWild;
        public static Ships_Overlay OverlayShips;
        public static Robots_Overlay OverlayRobots;
        public static Area_Overlay OverlayArea;
        public static Tiberium_Overlay OverlayTiberium;

        static OverlayManager()
        {
            OverlayColonists = new Colonists_Overlay();
            OverlayFog = new Fog_Overlay();
            OverlayMining = new Mining_Overlay();
            OverlayNoncolonist = new NonColonists_Overlay();
            OverlayBuilding = new Buildings_Overlay();
            OverlayPower = new PowerGrid_Overlay();
            OverlayTerrain = new Terrain_Overlay();
            OverlayView = new Viewpoint_Overlay();
            OverlayWild = new Wildlife_Overlay();
            OverlayShips = new Ships_Overlay();
            OverlayRobots = new Robots_Overlay();
            OverlayArea = new Area_Overlay();
            
            if (HasTiberium())
                OverlayTiberium = new Tiberium_Overlay();
        }

        public static bool HasTiberium() => LoadedModManager.RunningMods.Any(m => m.Identifier.Equals("TiberiumRim"));
    }
}
