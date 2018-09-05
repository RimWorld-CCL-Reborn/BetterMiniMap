using System.Collections.Generic;
using System.Linq;
using Verse;

using static BetterMiniMap.BetterMiniMapSettings;

namespace BetterMiniMap.Overlays
{
    public class ThingOverlay : MarkerOverlay, IExposable
    {
        public ThingOverlay(OverlayDef def, bool visible = true) : base(visible)
        {
            this.def = def;
        }

        public void ExposeData() => this.ExposeData(this.def.label);
        public IEnumerable<Thing> GetThings() => Find.CurrentMap.listerThings.ThingsInGroup(this.def.requestGroup).Where(t => this.def.IsValid(t));
        public override int GetUpdateInterval() => OverlaySettingDatabase.GetOverlaySettings(this.def.defName).updatePeriod;

        public override void Render()
        {
            foreach (Thing current in this.GetThings())
                this.CreateMarker(current);
        }

        public virtual void CreateMarker(Thing thing, float edgeOpacity = 0.5f)
        {
            IndicatorSettings settings = this.def.indicatorMappings.GetIndicatorSettings(thing);
            base.CreateMarker(thing.Position, settings.radius, settings.color, settings.edgeColor, edgeOpacity);
        }
    }
}