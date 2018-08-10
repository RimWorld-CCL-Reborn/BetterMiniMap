using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;

namespace BetterMiniMap.Overlays
{
    public class PawnOverlay : MarkerOverlay
    {
        public PawnOverlay(OverlayDef def, bool visible = true) : base(visible)
        {
            this.def = def;
        }

        public IEnumerable<Pawn> GetPawns() => Find.CurrentMap.mapPawns.AllPawns.Where(p => this.def.IsValid(p));

        // TODO: settings
        public override int GetUpdateInterval() => this.def.updatePeriod; 

        public override void Render()
        {
            foreach (Pawn current in this.GetPawns().Where(p => p.CarriedBy == null))
                this.CreateMarker(current, true);
        }

        public virtual void CreateMarker(Pawn pawn, bool transparentEdges = true, float edgeOpacity = 0.5f)
        {
            IndicatorProps props = this.def.indicatorMappings.Mapper(pawn);
            base.CreateMarker(pawn.Position, props.radius, props.color, props.EdgeColor, edgeOpacity);
        }
    }
}
