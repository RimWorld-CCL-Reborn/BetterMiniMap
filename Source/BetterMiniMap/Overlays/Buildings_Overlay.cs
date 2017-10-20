using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
    // NOTE: there may be room for impovement on this overlay
    public class Buildings_Overlay : Overlay, IExposable
    {
        public Buildings_Overlay(bool visible = true) : base(visible) { }

        public void ExposeData() => this.ExposeData("overlayBuilding");

        public override int GetUpdateInterval() => BetterMiniMapMod.settings.updatePeriods.buildings;

        public override void Render()
        {
            foreach (Building current in Find.VisibleMap.listerBuildings.allBuildingsColonist)
                if (current.def.AffectsRegions)
                    base.Texture.SetPixel(current.Position.x, current.Position.z, BetterMiniMapMod.settings.overlayColors.buildings);
        }

    }
}
