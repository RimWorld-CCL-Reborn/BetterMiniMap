using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
    public class Tiberium_Overlay : Things_Overlay, IExposable
    {
        public Tiberium_Overlay(bool visible = true) : base(visible) { }

        //Also not sure about this
        public override int GetUpdateInterval() => BetterMiniMapMod.settings.updatePeriods.ships;

        public override IEnumerable<Thing> GetThings()
        {
            List<Thing> crystals = new List<Thing>();
            ListerThings lister = Find.CurrentMap.listerThings;
            crystals.AddRange(lister.ThingsOfDef(DefDatabase<ThingDef>.GetNamed("TiberiumGreen", false)));
            crystals.AddRange(lister.ThingsOfDef(DefDatabase<ThingDef>.GetNamed("TiberiumPod", false)));
            crystals.AddRange(lister.ThingsOfDef(DefDatabase<ThingDef>.GetNamed("TiberiumShardsGreen", false)));
            crystals.AddRange(lister.ThingsOfDef(DefDatabase<ThingDef>.GetNamed("TiberiumMossGreen", false)));
            crystals.AddRange(lister.ThingsOfDef(DefDatabase<ThingDef>.GetNamed("TiberiumBlue", false)));
            crystals.AddRange(lister.ThingsOfDef(DefDatabase<ThingDef>.GetNamed("TiberiumShardsBlue", false)));
            crystals.AddRange(lister.ThingsOfDef(DefDatabase<ThingDef>.GetNamed("TiberiumMossBlue", false)));
            crystals.AddRange(lister.ThingsOfDef(DefDatabase<ThingDef>.GetNamed("TiberiumRed", false)));
            crystals.AddRange(lister.ThingsOfDef(DefDatabase<ThingDef>.GetNamed("TiberiumShardsRed", false)));
            crystals.AddRange(lister.ThingsOfDef(DefDatabase<ThingDef>.GetNamed("TiberiumGlacier", false)));
            crystals.AddRange(lister.ThingsOfDef(DefDatabase<ThingDef>.GetNamed("TiberiumVein", false)));
            return crystals.AsEnumerable();
        }

        public override void GetIndicatorProperities(Thing thing, out Color color, out Color edgeColor, out float radius)
        {
            color = Color.clear;
            switch (thing.def.defName)
            {
                case "TiberiumGreen":
                case "TiberiumPod":
                case "TiberiumShardsGreen":
                case "TiberiumMossGreen":
                    color = new ColorInt(0, 200, 0).ToColor;
                    break;
                case "TiberiumBlue":
                case "TiberiumShardsBlue":
                case "TiberiumMossBlue":
                    color = new ColorInt(0, 160, 230).ToColor;
                    break;
                case "TiberiumRed":
                case "TiberiumShardsRed":
                    color = new ColorInt(230, 0, 80).ToColor;
                    break;
                case "TiberiumGlacier":
                    color = new ColorInt(0, 85, 80).ToColor;
                    break;
                case "TiberiumVein":
                    color = new ColorInt(240, 100, 70).ToColor;
                    break;
            }
            edgeColor = Color.white;
            //Not sure about the sizes, should simply cover the terrain tile
            radius = 1f;
        }

        public void ExposeData() => this.ExposeData("overlayTiberium");
    }
}