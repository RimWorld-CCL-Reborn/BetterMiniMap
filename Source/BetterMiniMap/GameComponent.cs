﻿using Verse;
using RimWorld;
using HarmonyLib;
using System.Linq;
using System.Collections.Generic;

namespace BetterMiniMap
{
    // NOTE: patches must be delayed till defs are loaded and settings initilized
    public static class MiniMap_GameComponentHelper
    {
        // TODO: remove static context
        public static MiniMapManager miniMapManager; 

        static MiniMap_GameComponentHelper()
        {
            miniMapManager = new MiniMapManager();
        }

        internal static MiniMapWindow MiniMap
        {
            get => miniMapManager.GetMiniMap(Find.CurrentMap);
        }

        public static void ApplyPatches()
        {
            Harmony harmony = new Harmony("rimworld.whyisthat.betterminimap.gamecomponent");
            // TODO: why did I disable this?
            //harmony.Patch(AccessTools.Method(typeof(MainTabsRoot), nameof(MainTabsRoot.ToggleTab)), null, new HarmonyMethod(typeof(MiniMap_GameComponent), nameof(ToggleMiniMap)));
            //harmony.Patch(AccessTools.Method(typeof(MainButtonWorker_ToggleWorld), nameof(MainButtonWorker_ToggleWorld.Activate)), null, new HarmonyMethod(typeof(MiniMap_GameComponent), nameof(ToggleMiniMap_WorldTab)));

            if (BetterMiniMapMod.modSettings.singleMode)
            {
                harmony.Patch(AccessTools.Property(typeof(Game), nameof(Game.CurrentMap)).GetSetMethod(), new HarmonyMethod(typeof(MiniMap_GameComponentHelper), nameof(CurrentMap_Prefix)));
            }
            else
            {
                harmony.Patch(AccessTools.Method(typeof(Game), nameof(Game.AddMap)), null, new HarmonyMethod(typeof(MiniMap_GameComponentHelper), nameof(AddMiniMap)));
                harmony.Patch(AccessTools.Method(typeof(Game), nameof(Game.DeinitAndRemoveMap)), new HarmonyMethod(typeof(MiniMap_GameComponentHelper), nameof(RemoveMiniMap)));
                harmony.Patch(AccessTools.Method(typeof(PlaySettings), nameof(PlaySettings.DoPlaySettingsGlobalControls)), new HarmonyMethod(typeof(MiniMap_GameComponentHelper), nameof(PlaySettings_DoPlaySettingsGlobalControls_Prefix)));
            }
        }

        #region HarmonyPatches

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

        static void PlaySettings_DoPlaySettingsGlobalControls_Prefix(WidgetRow row, bool worldView)
        {
            if (row.ButtonIcon(MiniMapTextures.MapManagerIcon))
                Find.WindowStack.Add(MiniMap_GameComponentHelper.MiniMapMenu);
        }

        static void CurrentMap_Prefix(Game __instance, Map value)
        {
            if (__instance.currentMapIndex >= 0)
                miniMapManager.SetMap(value);
        }

        #endregion HarmonyPatches

        public static FloatMenu MiniMapMenu
        {
            get
            {
                return new FloatMenu(MiniMapMenuItems)
                {
                    closeOnCancel = false,
                    preventCameraMotion = false,
                };
            }
        }

        public static List<FloatMenuOption> MiniMapMenuItems
        {
            get
            {
                List<FloatMenuOption> items = new List<FloatMenuOption>();
                // TODO: ordering
                foreach (MiniMapWindow miniMap in miniMapManager.MiniMaps.Values)
                    items.Add(new FloatMenuOption($"TileMap-{miniMap.Map.Tile}", () => {
                        if (Find.WindowStack.Windows.IndexOf(miniMap) == -1)
                            Find.WindowStack.CustomAdd(miniMap);
                    }));
                return items;
            }
        }
    }

    [StaticConstructorOnStartup]
    public class MiniMap_GameComponent : GameComponent
    {
        // used to toggle minimap
        private static bool researchPal; 
        private static bool relationsTab;

        public MiniMap_GameComponent(Game g) : this() { }
        public MiniMap_GameComponent() { }

        static MiniMap_GameComponent()
        {
            // used for toggling minimap 
            researchPal = ModLister.AllInstalledMods.FirstOrDefault(m => m.Name == "ResearchPal")?.Active == true;
            relationsTab = ModLister.AllInstalledMods.FirstOrDefault(m => m.Name == "Relations Tab")?.Active == true;
        } 

        public override void GameComponentOnGUI()
        {
            base.GameComponentOnGUI();
            MiniMap_GameComponentHelper.miniMapManager.GameComponentOnGUI();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look<MiniMapManager>(ref MiniMap_GameComponentHelper.miniMapManager, "miniMapManager");

            // Handles upgrading settings
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                if (MiniMap_GameComponentHelper.miniMapManager == null)
                    MiniMap_GameComponentHelper.miniMapManager = new MiniMapManager();
            } 
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();
            if (BetterMiniMapMod.modSettings.singleMode)
                MiniMap_GameComponentHelper.miniMapManager.AddMiniMap(Find.CurrentMap);
        }
    }
}