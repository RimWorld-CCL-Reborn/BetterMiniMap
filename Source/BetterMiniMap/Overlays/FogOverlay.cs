using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
	public class FogOverlay : Overlay
	{
        public FogOverlay(Map map, bool visible = true) : base(map, visible) { }

        public void Update() => base.Update(false);

        public override void Render()
		{
            bool[] fogGrid = this.map.fogGrid.fogGrid;
            // NOTE: consider SetPixels32?
            for (int i = 0; i < this.map.cellIndices.NumGridCells; i++)
			{
				IntVec3 intVec = this.map.cellIndices.IndexToCell(i);
				base.Texture.SetPixel(intVec.x, intVec.z, fogGrid[i] ? BetterMiniMapMod.modSettings.overlayColors.fog : Color.clear);
			}
        }

		public override int GetUpdateInterval() => BetterMiniMapMod.modSettings.updatePeriods.fog;

        public override int OverlayPriority => 1000;
	}
}
