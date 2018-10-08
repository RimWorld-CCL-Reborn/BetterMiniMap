using UnityEngine;
using Verse;
using RimWorld;
using Harmony;
using RimWorld.Planet;
using System.Linq;

namespace BetterMiniMap
{
    [StaticConstructorOnStartup]
    public class MiniMap_GameComponent : GameComponent
    {
        // TODO: remove this variable
        //private static MiniMapWindow miniMap;

        private static bool initialized;

        // used to toggle minimap
        private static bool researchPal; 
        private static bool relationsTab;

        private static MiniMapManager miniMapManager; 

        public static MiniMapManager MiniMapManager
        {
            get => miniMapManager;
        }

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
            miniMapManager = new MiniMapManager();
        }

        // TODO: should this stay?
        private static MiniMapWindow MiniMap
        {
            get => miniMapManager.GetMiniMap(Find.CurrentMap);
        }

        #region HarmonyPatches

        static MiniMap_GameComponent()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.whyisthat.betterminimap.gamecomponent");
            //harmony.Patch(AccessTools.Method(typeof(MainTabsRoot), nameof(MainTabsRoot.ToggleTab)), null, new HarmonyMethod(typeof(MiniMap_GameComponent), nameof(ToggleMiniMap)));
            //harmony.Patch(AccessTools.Method(typeof(MainButtonWorker_ToggleWorld), nameof(MainButtonWorker_ToggleWorld.Activate)), null, new HarmonyMethod(typeof(MiniMap_GameComponent), nameof(ToggleMiniMap_WorldTab)));
            harmony.Patch(AccessTools.Method(typeof(Game), nameof(Game.AddMap)), null, new HarmonyMethod(typeof(MiniMap_GameComponent), nameof(AddMiniMap)));
            harmony.Patch(AccessTools.Method(typeof(Game), nameof(Game.DeinitAndRemoveMap)), new HarmonyMethod(typeof(MiniMap_GameComponent), nameof(RemoveMiniMap)));
        }
        
        /*static void ToggleMiniMap(MainTabsRoot __instance, MainButtonDef newTab)
        {
#if DEBUG
            Log.Message($"MainTabsRoot.ToggleTab: {newTab} {__instance.OpenTab != null}");
#endif
            if (MiniMap.Active)
            {
                if (__instance.OpenTab != null)
                {
                    switch (newTab.defName)
                    {
                        case "Research":
                            if (researchPal) MiniMap.Toggle(false);
                            break;
                        case "Factions":
                            if (relationsTab) MiniMap.Toggle(false);
                            break;
                        default:
                            MiniMap.Toggle(!WorldRendererUtility.WorldRenderedNow);
                            break;
                    }
                }
                else
                    MiniMap.Toggle(!WorldRendererUtility.WorldRenderedNow);
            }
        }*/

        /*static void ToggleMiniMap_WorldTab()
        {
#if DEBUG
            Log.Message($"MainButtonWorker_ToggleWorld.Activate: {Find.World.renderer.wantedMode}");
#endif
            if (MiniMap.Active)
                MiniMap.Toggle(!WorldRendererUtility.WorldRenderedNow);
        }*/

        static void AddMiniMap(Map map) => miniMapManager.AddMiniMap(map);
        static void RemoveMiniMap(Map map) => miniMapManager.RemoveMiniMap(map);

        #endregion HarmonyPatches

        public override void GameComponentOnGUI()
        {
            base.GameComponentOnGUI();
            if (Event.current.type == EventType.KeyDown)
            {
                // TODO: needs to be reworked
                if (Event.current.keyCode == KeyCode.M)
                {
                    bool add = Find.WindowStack.Windows.IndexOf(MiniMap) == -1;
                    MiniMap.Toggle(add); // TODO: this is a bit nasty...
                    MiniMap.Active = add;
                }
                if (Event.current.keyCode == KeyCode.K && MiniMap.Active)
                    MiniMap.ToggleControls();
            }
        }

        /*public void PostInit()
        {
#if DEBUG
            Log.Message($"PostInit");
#endif
            base.FinalizeInit();

            // need to reset textures (in case reload)
            //miniMap.ResetOverlays();

            Find.WindowStack.Add(miniMapManager);
        }*/

        public override void StartedNewGame()
        {
            base.StartedNewGame();
            //PostInit();
        }


        public override void LoadedGame()
        {
            base.LoadedGame();
            miniMapManager.LoadMaps();
            //PostInit();
        }

        // TODO
        /*public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look<MiniMapWindow>(ref miniMap, "miniMap");
        }*/

    }
}