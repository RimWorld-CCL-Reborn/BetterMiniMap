using System;
using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
	public class Terrain_Overlay : Overlay, IExposable
	{
        public Terrain_Overlay(bool visible = true) : base(visible) { }

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.updatePeriods.terrain;

        public void Update() => base.Update(false);

		public override void Render()
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

        public void ExposeData() => this.ExposeData("overlayTerrain");
    }
}
