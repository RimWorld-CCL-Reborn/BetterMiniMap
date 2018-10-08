using Verse;

namespace BetterMiniMap.Overlays
{
	public class ViewpointOverlay : Overlay
	{
        public ViewpointOverlay(Map map, bool visible = true) : base(map, visible) { }

        public override int GetUpdateInterval() => BetterMiniMapMod.modSettings.updatePeriods.viewpoint;

		public override void Render()
		{
            if (this.map == Find.CurrentMap)
            {
                IntVec3 shadow;
                foreach (IntVec3 current in Find.CameraDriver.CurrentViewRect.EdgeCells)
                {
                    if (current.InBounds(this.map))
                        base.Texture.SetPixel(current.x, current.z, BetterMiniMapMod.modSettings.overlayColors.viewpoint);
                    shadow = new IntVec3(current.x - 1, 0, current.y + 1);
                    if (shadow.InBounds(this.map))
                        base.Texture.SetPixel(shadow.x, shadow.z, BetterMiniMapMod.modSettings.overlayColors.viewpointEdge);
                }
            }
        }

        public override int OverlayPriority => 10000; // super high
	}
}
