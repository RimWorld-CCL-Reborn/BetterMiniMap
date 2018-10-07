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
        private static MiniMapWindow miniMap;

        private static bool initialized;

        // used to toggle minimap
        private static bool researchPal; 
        private static bool relationsTab;

        private static MiniMapManager miniMapManager;

        public MiniMap_GameComponent(Game g) => Initialize();

        public MiniMap_GameComponent() => Initialize();

        private static void Initialize()
        {
            if (!initialized)
            {
                // used for toggling minimap 
                researchPal = ModLister.AllInstalledMods.FirstOrDefault(m => m.Name == "ResearchPal")?.Active == true;
                relationsTab = ModLister.AllInstalledMods.FirstOrDefault(m => m.Name == "Relations Tab")?.Active == true;
                initialized = true;
            }
        }

        #region HarmonyPatches

        static MiniMap_GameComponent()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.whyisthat.betterminimap.gamecomponent");
            harmony.Patch(AccessTools.Method(typeof(MainTabsRoot), nameof(MainTabsRoot.ToggleTab)), null, new HarmonyMethod(typeof(MiniMap_GameComponent), nameof(ToggleMiniMap)));
            harmony.Patch(AccessTools.Method(typeof(MainButtonWorker_ToggleWorld), nameof(MainButtonWorker_ToggleWorld.Activate)), null, new HarmonyMethod(typeof(MiniMap_GameComponent), nameof(ToggleMiniMap_WorldTab)));
            harmony.Patch(AccessTools.Method(typeof(Game), nameof(Game.AddMap)), null, new HarmonyMethod(typeof(MiniMap_GameComponent), nameof(AddMiniMap)));
            harmony.Patch(AccessTools.Method(typeof(Game), nameof(Game.DeinitAndRemoveMap)), new HarmonyMethod(typeof(MiniMap_GameComponent), nameof(RemoveMiniMap)));
        }
        
        static void ToggleMiniMap(MainTabsRoot __instance, MainButtonDef newTab)
        {
#if DEBUG
            Log.Message($"MainTabsRoot.ToggleTab: {newTab} {__instance.OpenTab != null}");
#endif
            if (miniMap.Active)
            {
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
        }

        static void ToggleMiniMap_WorldTab()
        {
#if DEBUG
            Log.Message($"MainButtonWorker_ToggleWorld.Activate: {Find.World.renderer.wantedMode}");
#endif
            if (miniMap.Active)
                miniMap.Toggle(!WorldRendererUtility.WorldRenderedNow);
        }

        static void AddMiniMap(Map map) => miniMapManager.RemoveMiniMap(map);
        static void RemoveMiniMap(Map map) => miniMapManager.AddMiniMap(map);

        #endregion HarmonyPatches

        public override void GameComponentOnGUI()
        {
            base.GameComponentOnGUI();
            if (Event.current.type == EventType.KeyDown)
            {
                // TODO: needs to be reworked
                if (Event.current.keyCode == KeyCode.M)
                {
                    bool add = Find.WindowStack.Windows.IndexOf(miniMap) == -1;
                    miniMap.Toggle(add); // TODO: this is a bit nasty...
                    miniMap.Active = add;
                }
                if (Event.current.keyCode == KeyCode.K && miniMap.Active)
                    miniMap.ToggleControls();
            }
        }

        // NOTE: Finalize Init is too early for CurrentMap to be set
        //public override void FinalizeInit()
        public void PostInit()
        {
#if DEBUG
            Log.Message($"PostInit");
#endif
            base.FinalizeInit();
            /*if (miniMap == null) // this should always fire?
                miniMap = new MiniMapWindow();
            
            if (Find.CurrentMap != null)
                Find.WindowStack.Add(miniMap);*/

            // need to reset textures (in case reload)
            //miniMap.ResetOverlays();

            miniMapManager = new MiniMapManager();
            Find.WindowStack.Add(miniMapManager);
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();
            PostInit();
        }


        public override void LoadedGame()
        {
            base.LoadedGame();
            PostInit();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look<MiniMapWindow>(ref miniMap, "miniMap");
        }

    }
}