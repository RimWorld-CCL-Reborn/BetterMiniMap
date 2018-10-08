using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
	public class AreaOverlay : Overlay
	{
		private const float opacity = 0.5f;
		public Area area;

		public AreaOverlay(Map map, Area area = null, bool visible = true) : base(map, visible) => this.area = area;

		public override void Render()
		{
            if (area != null)
            {
                Color color = this.area.Color;
                color.a = AreaOverlay.opacity;
                foreach (IntVec3 current in this.area.ActiveCells)
                    base.Texture.SetPixel(current.x, current.z, color);
            }
        }

		public override int GetUpdateInterval() => BetterMiniMapMod.modSettings.updatePeriods.areas;

        public override bool ShouldUpdateOverlay
        {
            get => area != null && base.ShouldUpdateOverlay;
        }

        public override int OverlayPriority => 700;
    }
}
