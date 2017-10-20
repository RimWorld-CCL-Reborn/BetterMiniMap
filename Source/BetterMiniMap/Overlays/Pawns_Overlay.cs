using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
	public abstract class Pawns_Overlay : MarkerOverlay
	{
        protected Pawns_Overlay(bool visible = true) : base(visible) { }

		public abstract IEnumerable<Pawn> GetPawns();
        public abstract void GetIndicatorProperities(Pawn pawn, out Color color, out Color edgeColor, out float radius);

		public override void Render()
		{
            foreach (Pawn current in this.GetPawns())
				this.CreateMarker(current, true, 0.5f);
        }

        public virtual void CreateMarker(Pawn pawn, bool transparentEdges = true, float edgeOpacity = 0.5f)
        {
            this.GetIndicatorProperities(pawn, out Color color, out Color edgeColor, out float radius);
            base.CreateMarker(pawn.Position, radius, color, edgeColor, edgeOpacity);
        }
    }
}
