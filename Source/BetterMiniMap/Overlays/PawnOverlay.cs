using System.Collections.Generic;
using System.Linq;
using Verse;

using static BetterMiniMap.BetterMiniMapSettings;

namespace BetterMiniMap.Overlays
{
    public class PawnOverlay : MarkerOverlay, IExposable
    {
        public PawnOverlay(OverlayDef def, bool visible = true) : base(visible)
        {
            this.def = def;
        }

        public void ExposeData() => this.ExposeData(this.def.label);
        public IEnumerable<Pawn> GetPawns() => Find.CurrentMap.mapPawns.AllPawns.Where(p => this.def.IsValid(p));
        public override int GetUpdateInterval() => OverlaySettingDatabase.GetOverlaySettings(this.def.defName).updatePeriod;

        public override void Render()
        {
            foreach (Pawn current in this.GetPawns().Where(p => p.CarriedBy == null))
                this.CreateMarker(current, true);
        }

        public virtual void CreateMarker(Pawn pawn, bool transparentEdges = true, float edgeOpacity = 0.5f)
        {
            IndicatorSettings settings = this.def.indicatorMappings.GetIndicatorSettings(pawn);
            base.CreateMarker(pawn.Position, settings.radius, settings.color, settings.edgeColor, edgeOpacity);
        }
    }
}