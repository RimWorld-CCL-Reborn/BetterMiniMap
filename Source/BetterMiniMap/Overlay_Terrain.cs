using System;
using UnityEngine;
using Verse;

namespace BetterMiniMap
{
	public class Overlay_Terrain : Overlay
	{
        public Overlay_Terrain(bool visible) : base(visible) { }

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.overlay_Terrain;

		public override void Update()
		{
			for (int i = 0; i < Find.VisibleMap.cellIndices.NumGridCells; i++)
			{
				IntVec3 cell = Find.VisibleMap.cellIndices.IndexToCell(i);
                Color color = Find.VisibleMap.terrainGrid.TerrainAt(i).color;
                if (color == Color.white)
                    color = Color.clear;
                base.Texture.SetPixel(cell.x, cell.z, color);
			}
		}

	}
}
