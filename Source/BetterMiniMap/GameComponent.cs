using UnityEngine;
using Verse;
using RimWorld;
using Harmony;
using RimWorld.Planet;
using System.Linq;

namespace BetterMiniMap
{
    [StaticConstructorOnStartup]
    class MiniMap_GameComponent : GameComponent
    {
        // TODO: can we remove most of these?
        private const int defaultMargin = 8;

        public static bool overlayColonists = true;
        public static bool overlayFog = true;
        public static bool overlayMining = true;
        public static bool overlayNoncolonist = true;
        public static bool overlayBuilding = true;
        public static bool overlayPower = true;
        public static bool overlayTerrain = true;
        public static bool overlayView = true;
        public static bool overlayWild = true;
        public static bool overlayShips = true;
        public static bool overlayRobots = true;

        private static int resolutionX;
        private static int resolutionY;
        private static Vector2 position;
        private static Vector2 size;

        private static MiniMapWindow miniMap;
        private static bool initialized;

        // used to toggle minimap
        private static bool researchPal; 
        private static bool relationsTab; 

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
        
        // NOTE: this is done here to avoid lazy loading.
        static void Initilize()
        {
#if DEBUG
            Log.Message("MiniMap_GameComponent.Initilize");
#endif

            miniMap = new MiniMapWindow();
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
            miniMap.UpdateLocality(position, size);
        }

        public override void ExposeData()
        {
            base.ExposeData();
#if DEBUG
            Log.Message($"ExposeData: {Scribe.mode}");
#endif
            if (Scribe.mode == LoadSaveMode.Saving)
                miniMap.UpdateSettings();

            Scribe_Values.Look<Vector2>(ref position, "positionY"); // fix this
            Scribe_Values.Look<Vector2>(ref size, "size");
            Scribe_Values.Look<int>(ref resolutionX, "resolutionX", -1, false);
            Scribe_Values.Look<int>(ref resolutionY, "resolutionY", -1, false);

            Scribe_Values.Look<bool>(ref overlayColonists, "overlayColonists", true);
            Scribe_Values.Look<bool>(ref overlayMining, "overlayMining", true);
            Scribe_Values.Look<bool>(ref overlayNoncolonist, "overlayNoncolonist", true);
            Scribe_Values.Look<bool>(ref overlayBuilding, "overlayBuilding", true);
            Scribe_Values.Look<bool>(ref overlayPower, "overlayPower", true);
            Scribe_Values.Look<bool>(ref overlayTerrain, "overlayTerrain", true);
            Scribe_Values.Look<bool>(ref overlayWild, "overlayWild", true);
            Scribe_Values.Look<bool>(ref overlayShips, "overlayShips", true);
            Scribe_Values.Look<bool>(ref overlayRobots, "overlayRobots", true);


            initialized |= Scribe.mode == LoadSaveMode.LoadingVars;
#if DEBUG
            Log.Message($"Initialized: {initialized}");
#endif
        }

        public override void LoadedGame()
        {
            base.LoadedGame();
            // NOTE: this might need to migrate to MapComp
            miniMap.UpdateLocality(position, size);
        }

        public override void GameComponentOnGUI()
        {
            base.GameComponentOnGUI();
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.F8)
                miniMap.Toggle(Find.WindowStack.Windows.IndexOf(miniMap) == -1); // TODO: this is a bit nasty...
        }

    }
}
