using System.Collections.Generic;
using System.Linq;
using Verse;

using static BetterMiniMap.BetterMiniMapSettings;

namespace BetterMiniMap.Overlays
{
    public class ThingOverlay : MarkerOverlay, IExposable
    {
        public ThingOverlay(OverlayDef def, Map map, bool visible = true) : base(map, visible)
        {
            this.def = def;
        }

        public void ExposeData() => this.ExposeData(this.def.label);
        public IEnumerable<Thing> GetThings() => this.map.listerThings.ThingsInGroup(this.def.requestGroup).Where(t => this.def.IsValid(t, this.map));
        public override int GetUpdateInterval() => OverlaySettingDatabase.GetOverlaySettings(this.def.defName).updatePeriod;

        public override void Render()
        {
            foreach (Thing current in this.GetThings())
                this.CreateMarker(current);
        }

        public virtual void CreateMarker(Thing thing, float edgeOpacity = 0.5f)
        {
            IndicatorSettings settings = this.def.indicatorMappings.GetIndicatorSettings(thing, this.map);
            base.CreateMarker(thing.Position, settings.radius, settings.color, settings.edgeColor, edgeOpacity);
        }
    }
}