using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
    /// <summary>
    /// Used to cache the number of inner cells for a given radius
    /// </summary>
    public static class InnerCellsCache
    {
        private static Dictionary<float, int> innerCellsCache = new Dictionary<float, int>();

        public static int GetNumInnerCells(float radius)
        {
            if (!innerCellsCache.ContainsKey(radius))
            {
                int numInnerCells = (radius >= 2f) ? GenRadial.NumCellsInRadius(radius - 1f) : GenRadial.NumCellsInRadius(radius);
                innerCellsCache.Add(radius, numInnerCells);
            }
            return innerCellsCache[radius];
        }

    }

    public abstract class MarkerOverlay : Overlay
    {
        protected MarkerOverlay(Map map, bool visible) : base(map, visible) { }

        protected virtual void CreateMarker(IntVec3 position, float radius, Color color, Color edgeColor, float edgeOpacity = 0.5f)
        {
            // reduce raidus by 1 to make radius=1 be a single cell
            int numInnerCells = InnerCellsCache.GetNumInnerCells(--radius);
            int i = 0;
            foreach (IntVec3 cell in GenRadial.RadialCellsAround(position, radius, true))
            {
                if (cell.InBounds(this.map))
                    this.Texture.SetPixel(cell.x, cell.z, (i > numInnerCells) ? edgeColor : color);
                i++;
            }
        }

        public override int OverlayPriority => this.def.priority;
    }
}
