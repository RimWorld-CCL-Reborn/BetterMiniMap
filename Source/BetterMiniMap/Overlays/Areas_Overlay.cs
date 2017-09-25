using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
	public class Areas_Overlay : Overlay
	{
		private const float opacity = 0.5f;
        // TODO: property here
		public Area area;

		public Areas_Overlay(Area area, bool visible) : base(visible) => this.area = area;

		public override void Update()
		{
			Color color = this.area.Color;
			color.a = Areas_Overlay.opacity;
			base.ClearTexture(false);
			foreach (IntVec3 current in this.area.ActiveCells)
				base.Texture.SetPixel(current.x, current.z, color);
		}

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.overlay_Area;
	}
}
