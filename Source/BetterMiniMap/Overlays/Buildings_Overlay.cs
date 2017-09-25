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

        public override void Update()
        {
            base.ClearTexture(false);
            List<Building> allBuildingsColonist = Find.VisibleMap.listerBuildings.allBuildingsColonist;
            foreach (Building current in allBuildingsColonist)
            {
                // TODO: AffectsRegions?
                bool flag = current.def.defName.Equals("Wall") || current.def.defName.Equals("Door") || current.def.defName.Equals("Autodoor") || current.def.defName.Equals("Embrasure") || current.def.defName.Equals("ReinforcedFireResistantEmbrasures") || current.def.defName.Equals("ReinforcedEmbrasures") || current.def.defName.Equals("FireResistantEmbrasures");
                if (flag)
                {
                    base.Texture.SetPixel(current.Position.x, current.Position.z, planningColor);
                }
            }
            base.Flush();
        }

    }
}
