using UnityEngine;
using Verse;
using RimWorld;
using Harmony;
using RimWorld.Planet;
using System.Linq;

namespace BetterMiniMap
{
    // TODO: fix these nasty default values...
    [StaticConstructorOnStartup]
    class MiniMap_GameComponent : GameComponent
    {
        private const int defaultMargin = 8;

        private static bool overlayColonists = true;
        private static bool overlayFog = true;
        private static bool overlayMining = true;
        private static bool overlayNoncolonist = true;
        private static bool overlayBuilding = true;
        private static bool overlayPower = true;
        private static bool overlayTerrain = true;
        private static bool overlayView = true;
        private static bool overlayWild = true;
        private static bool overlayShips = true;
        private static bool overlayRobots = true;

        private static int resolutionX;
        private static int resolutionY;
        private static Vector2 position;
        private static Vector2 size;

        private static MiniMapWindow miniMap;
        private static bool initialized;

        private static bool researchPal; // used to toggle minimap
        private static bool relationsTab; // used to toggle minimap

        public MiniMap_GameComponent(Game g) { }
        public MiniMap_GameComponent() { }

        // NOTE: not sure if I like this...
        static MiniMap_GameComponent()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.whyisthat.betterminimap.gamecomponent");
            harmony.Patch(AccessTools.Method(typeof(UIRoot_Play), nameof(UIRoot_Play.Init)), null, new HarmonyMethod(typeof(MiniMap_GameComponent), nameof(Initilize)));
            harmony.Patch(AccessTools.Method(typeof(MainTabsRoot), nameof(MainTabsRoot.ToggleTab)), null, new HarmonyMethod(typeof(MiniMap_GameComponent), nameof(ToggleMiniMap)));
            harmony.Patch(AccessTools.Method(typeof(MainButtonWorker_ToggleWorld), nameof(MainButtonWorker_ToggleWorld.Activate)), null, new HarmonyMethod(typeof(MiniMap_GameComponent), nameof(ToggleMiniMap_WorldTab)));
        }
        
        static void Initilize()
        {
#if DEBUG
            Log.Message("MiniMap_GameComponent.Initilize");
#endif
            miniMap = new MiniMapWindow();
            miniMap.MakeOptions();
            Find.WindowStack.Add(miniMap);

            // used for toggling minimap 
            researchPal = ModLister.AllInstalledMods.FirstOrDefault(m => m.Name == "ResearchPal")?.Active == true;
            relationsTab = ModLister.AllInstalledMods.FirstOrDefault(m => m.Name == "Relations Tab")?.Active == true;
        }

        static void ToggleMiniMap(MainTabsRoot __instance, MainButtonDef newTab)
        {
#if DEBUG
            Log.Message($"MainTabsRoot.ToggleTab: {newTab} {__instance.OpenTab != null}");
#endif
            if (__instance.OpenTab != null)
            {
                switch (newTab.defName)
                {
                    case "Research":
                        if (researchPal) miniMap.Toggle(false);
                        break;
                    case "Factions":
                        if (relationsTab) miniMap.Toggle(false);
                        break;
                    default:
                        miniMap.Toggle(!WorldRendererUtility.WorldRenderedNow);
                        break;
                }
            }
            else
                miniMap.Toggle(!WorldRendererUtility.WorldRenderedNow);
        }

        static void ToggleMiniMap_WorldTab()
        {
#if DEBUG
            Log.Message($"MainButtonWorker_ToggleWorld.Activate: {Find.World.renderer.wantedMode}");
#endif
            miniMap.Toggle(!WorldRendererUtility.WorldRenderedNow);
        }

        public static bool OverlayColonists { get => overlayColonists; set => overlayColonists = value; }
        public static bool OverlayFog { get => overlayFog; set => overlayFog = value; }
        public static bool OverlayMining { get => overlayMining; set => overlayMining = value; }
        public static bool OverlayNoncolonist { get => overlayNoncolonist; set => overlayNoncolonist = value; }
        public static bool OverlayBuilding { get => overlayBuilding; set => overlayBuilding = value; }
        public static bool OverlayPower { get => overlayPower; set => overlayPower = value; }
        public static bool OverlayTerrain { get => overlayTerrain; set => overlayTerrain = value; }
        public static bool OverlayView { get => overlayView; set => overlayView = value; }
        public static bool OverlayWild { get => overlayWild; set => overlayWild = value; }
        public static bool OverlayShips { get => overlayShips; set => overlayShips = value; }
        public static bool OverlayRobots { get => overlayRobots; set => overlayRobots = value; }

        public static int ResolutionX { get => resolutionX; set => resolutionX = value; }
        public static int ResolutionY { get => resolutionY; set => resolutionY = value; }

        public static Vector2 Position { get => position; set => position = value; }
        public static Vector2 Size { get => size; set => size = value; }

        public static void InitializeLocality(Map map)
        {
            if (!initialized)
            {
                position = new Vector2(UI.screenWidth - map.Size.x - defaultMargin, defaultMargin);
                size = new Vector2(map.Size.x, map.Size.z);
                initialized = true;
            }
            UpdateWindow();
        }

        private static void UpdateWindow() => miniMap.UpdateWindow(position, size);

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Vector2>(ref position, "positionY"); // fix this
            Scribe_Values.Look<Vector2>(ref size, "size");
            Scribe_Values.Look<int>(ref resolutionX, "resolutionX", -1, false);
            Scribe_Values.Look<int>(ref resolutionY, "resolutionY", -1, false);

            Scribe_Values.Look<bool>(ref overlayColonists, "overlayColonists", true);
            Scribe_Values.Look<bool>(ref overlayFog, "overlayFog", true);
            Scribe_Values.Look<bool>(ref overlayMining, "overlayMining", true);
            Scribe_Values.Look<bool>(ref overlayNoncolonist, "overlayNoncolonist", true);
            Scribe_Values.Look<bool>(ref overlayBuilding, "overlayBuilding", true);
            Scribe_Values.Look<bool>(ref overlayPower, "overlayPower", true);
            Scribe_Values.Look<bool>(ref overlayTerrain, "overlayTerrain", true);
            Scribe_Values.Look<bool>(ref overlayView, "overlayView", true);
            Scribe_Values.Look<bool>(ref overlayWild, "overlayWild", true);
            Scribe_Values.Look<bool>(ref overlayShips, "overlayShips", true);
            Scribe_Values.Look<bool>(ref overlayRobots, "overlayRobots", true);
            initialized = Scribe.mode == LoadSaveMode.LoadingVars;
#if DEBUG
            Log.Message($"Initialized: {initialized}");
#endif
        }

        public override void LoadedGame()
        {
            base.LoadedGame();
            // NOTE: this might need to migrate to MapComp
            UpdateWindow();
        }

    }
}
