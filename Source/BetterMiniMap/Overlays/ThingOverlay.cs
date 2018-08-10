using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
    public class ThingOverlay : MarkerOverlay
    {
        public OverlayDef def;

        public ThingOverlay(OverlayDef def, bool visible = true) : base(visible)
        {
            this.def = def;
        }

        public IEnumerable<Thing> GetThings() => Find.CurrentMap.listerThings.AllThings.Where(t => this.def.IsValid(t));

        // TODO: settings
        public override int GetUpdateInterval() => this.def.updatePeriod;

        public override void Render()
        {
            foreach (Thing current in this.GetThings())
                this.CreateMarker(current);
        }

        public virtual void CreateMarker(Thing thing, float edgeOpacity = 0.5f)
        {
            IndicatorProps props = this.def.indicatorMappings.Mapper(thing);
            base.CreateMarker(thing.Position, props.radius, props.color, props.EdgeColor, edgeOpacity);
        }

    }
}
