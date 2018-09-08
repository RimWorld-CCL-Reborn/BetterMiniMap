using Verse;

namespace BetterMiniMap.Overlays
{
    // NOTE: there may be room for impovement on this overlay
    public class BuildingsOverlay : Overlay, IExposable
    {
        public BuildingsOverlay(bool visible = true) : base(visible) { }

        public void ExposeData() => this.ExposeData("overlayBuilding");

        public override int GetUpdateInterval() => BetterMiniMapMod.settings.updatePeriods.buildings;

        public override void Render()
        {
            foreach (Building current in Find.CurrentMap.listerBuildings.allBuildingsColonist)
                if (current.def.AffectsRegions)
                    base.Texture.SetPixel(current.Position.x, current.Position.z, BetterMiniMapMod.settings.overlayColors.buildings);
        }

        public override int OverlayPriority => 800;
    }
}
