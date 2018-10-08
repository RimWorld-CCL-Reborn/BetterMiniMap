using Verse;

namespace BetterMiniMap.Overlays
{
    // NOTE: there may be room for impovement on this overlay
    public class BuildingsOverlay : Overlay, IExposable
    {
        public BuildingsOverlay(Map map, bool visible = true) : base(map, visible) { }

        public void ExposeData() => this.ExposeData("overlayBuilding");

        public override int GetUpdateInterval() => BetterMiniMapMod.modSettings.updatePeriods.buildings;

        public override void Render()
        {
            foreach (Building current in this.map.listerBuildings.allBuildingsColonist)
                if (current.def.AffectsRegions)
                    base.Texture.SetPixel(current.Position.x, current.Position.z, BetterMiniMapMod.modSettings.overlayColors.buildings);
        }

        public override int OverlayPriority => 800;
    }
}
