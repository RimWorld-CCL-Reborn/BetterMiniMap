using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
    public class Buildings_Overlay : Overlay
    {
        private static readonly Color planningColor = Color.white;

        public Buildings_Overlay(bool visible) : base(visible) { }

        public override int GetUpdateInterval() => BetterMiniMapMod.settings.overlay_Buildings;

        public override void Render()
        {
            foreach (Building current in Find.VisibleMap.listerBuildings.allBuildingsColonist)
                if (current.def.AffectsRegions)
                    base.Texture.SetPixel(current.Position.x, current.Position.z, planningColor);
        }

    }
}
