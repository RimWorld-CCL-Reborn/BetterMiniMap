using System.Collections.Generic;
using Verse;

namespace BetterMiniMap
{
    public class MiniMapManager
    {
        // keyed on tile id
        private Dictionary<int, MiniMapWindow> miniMaps;

        public MiniMapManager()
        {
            this.miniMaps = new Dictionary<int, MiniMapWindow>();
        }

        public void AddMiniMap(Map map) => this.miniMaps.Add(map.Tile, new MiniMapWindow(map));
        public void RemoveMiniMap(Map map) => this.miniMaps.Remove(map.Tile);

        public void LoadMaps()
        {
            foreach(Map map in Current.Game.Maps)
                this.AddMiniMap(map);
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
                    items.Add(new FloatMenuOption($"TileMap-{miniMap.Map.Tile}", ()=> {
                        if (Find.WindowStack.Windows.IndexOf(miniMap) == -1)
                            Find.WindowStack.CustomAdd(miniMap);
                    }));
                return items;
            }
        }

        internal MiniMapWindow GetMiniMap(Map map) => miniMaps[map.Tile];
    }
}
