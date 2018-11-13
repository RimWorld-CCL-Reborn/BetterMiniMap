using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace BetterMiniMap
{
    public class MiniMapManager : IExposable
    {
        // keyed on uniqueID
        private Dictionary<int, MiniMapWindow> miniMaps;
        private Action gameComponentOnGUI;

        public Action GameComponentOnGUI { get => gameComponentOnGUI; }

        public MiniMapManager()
        {
            this.miniMaps = new Dictionary<int, MiniMapWindow>();
            gameComponentOnGUI = delegate ()
            {
                // wait till in game screen
                if (!GenScene.InPlayScene) return;

                // init
                MiniMap_GameComponent.MiniMap.Toggle(true);

                gameComponentOnGUI = delegate ()
                {
                    if (Event.current.type == EventType.KeyDown)
                    {
                        // TODO: needs to be reworked
                        if (Event.current.keyCode == KeyCode.M)
                        {
                            bool add = Find.WindowStack.Windows.IndexOf(MiniMap_GameComponent.MiniMap) == -1;
                            MiniMap_GameComponent.MiniMap.Toggle(add); // TODO: this is a bit nasty...
                            MiniMap_GameComponent.MiniMap.Active = add;
                        }
                        if (Event.current.keyCode == KeyCode.K && MiniMap_GameComponent.MiniMap.Active)
                            MiniMap_GameComponent.MiniMap.ToggleControls();
                    }
                };
            };
        }

        public void SetMap(Map map)
        {
            if (this.miniMaps.Count == 0) // upgrade case (not sure)
                this.AddMiniMap(map);
            else
            {
                MiniMapWindow oldMMW = this.GetMiniMap(Current.Game.Maps[Current.Game.currentMapIndex]);
                if (oldMMW.Initialized)
                {
                    MiniMapWindow newMMW = new MiniMapWindow(map);
                    newMMW.SetLocality(oldMMW);
                    oldMMW.Toggle();
                    newMMW.Toggle(true);
                    this.miniMaps.Clear();
                    this.miniMaps.Add(map.uniqueID, newMMW);
                }
            }
        }

        public void AddMiniMap(Map map)
        {
            MiniMapWindow miniMap = new MiniMapWindow(map);
            // TODO: look into smart placement scheme
            miniMap.Position = new Vector2(UI.screenWidth - (MiniMapWindow.defaultSize + MiniMapWindow.defaultMargin)*(miniMaps.Count+1), MiniMapWindow.defaultMargin);
            if (this.miniMaps.ContainsKey(map.uniqueID))
                Log.Warning($"BetterMiniMap: miniMaps already contains key {map.uniqueID}.");
            else
                this.miniMaps.Add(map.uniqueID, miniMap);
        }

        public void RemoveMiniMap(Map map) => this.miniMaps.Remove(map.uniqueID);

        public FloatMenu MiniMapMenu
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

        public List<FloatMenuOption> MiniMapMenuItems
        {
            get
            {
                List<FloatMenuOption> items = new List<FloatMenuOption>();
                // TODO: ordering
                foreach(MiniMapWindow miniMap in miniMaps.Values)
                    items.Add(new FloatMenuOption($"TileMap-{miniMap.Map.Tile}", ()=> {
                        if (Find.WindowStack.Windows.IndexOf(miniMap) == -1)
                            Find.WindowStack.CustomAdd(miniMap);
                    }));
                return items;
            }
        }

        internal MiniMapWindow GetMiniMap(Map map)
        {
            if (!miniMaps.ContainsKey(map.uniqueID)) AddMiniMap(map);
            return miniMaps[map.uniqueID];
        }

        public void ExposeData()
        {
            Scribe_Collections.Look<int, MiniMapWindow>(ref miniMaps, "miniMaps", LookMode.Undefined, LookMode.Deep);
        }
    }
}
