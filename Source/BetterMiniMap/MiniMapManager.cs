using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace BetterMiniMap
{
    public class MiniMapManager : Window
    {
        // keyed on tile id
        private Dictionary<int, MiniMapWindow> miniMaps;

        public MiniMapManager()
        {
            this.closeOnCancel = false;
            this.preventCameraMotion = false;
            this.layer = WindowLayer.GameUI;
            this.windowRect = new Rect(5, 5, 50, 50);

            this.miniMaps = new Dictionary<int, MiniMapWindow>();

        }

        public void AddMiniMap(Map map) => this.miniMaps.Add(map.Tile, new MiniMapWindow(map));
        public void RemoveMiniMap(Map map) => this.miniMaps.Remove(map.Tile);
        protected override float Margin { get => 0f; }

        public override void DoWindowContents(Rect inRect)
        {
            if (Widgets.ButtonImage(inRect, MiniMapTextures.mapManagerIcon)) // handle float menu
            {
                if (Event.current.button == 0 || Event.current.button == 1) // left/right click
                    Find.WindowStack.Add(this.MiniMapMenu);
            }
        }

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
                    items.Add(new FloatMenuOption($"TileMap-{miniMap.Map.Tile}", ()=> { Log.Message("bang"); }));
                return items;
            }
        }

    }
}
