using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
	public class FogOverlay : Overlay
	{
        public FogOverlay(bool visible = true) : base(visible) { }

        public void Update() => base.Update(false);

        public override void Render()
		{
            bool[] fogGrid = Find.CurrentMap.fogGrid.fogGrid;
            // NOTE: consider SetPixels32?
            for (int i = 0; i < Find.CurrentMap.cellIndices.NumGridCells; i++)
			{
				IntVec3 intVec = Find.CurrentMap.cellIndices.IndexToCell(i);
				base.Texture.SetPixel(intVec.x, intVec.z, fogGrid[i] ? BetterMiniMapMod.settings.overlayColors.fog : Color.clear);
			}
        }

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.updatePeriods.fog;

        public override int OverlayPriority => 1000;
	}
}
