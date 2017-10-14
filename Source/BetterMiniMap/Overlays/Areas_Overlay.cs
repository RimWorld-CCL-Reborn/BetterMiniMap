using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
	public class Area_Overlay : Overlay
	{
		private const float opacity = 0.5f;
		public Area area;

		public Area_Overlay(Area area = null, bool visible = true) : base(visible) => this.area = area;

		public override void Render()
		{
            if (area != null)
            {
                Color color = this.area.Color;
                color.a = Area_Overlay.opacity;
                foreach (IntVec3 current in this.area.ActiveCells)
                    base.Texture.SetPixel(current.x, current.z, color);
            }
        }

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.updatePeriods.areas;

        public override bool ShouldUpdateOverlay
        {
            get => area != null && base.ShouldUpdateOverlay;
        }
    }
}
