using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
	public abstract class Pawns_Overlay : Overlay
	{
        protected Pawns_Overlay(bool visible) : base(visible) { }

		public abstract IEnumerable<Pawn> GetPawns();
        public abstract void GetIndicatorProperities(Pawn pawn, out Color color, out float radius);

		public override void Render()
		{
			foreach (Pawn current in this.GetPawns())
				this.CreateMarker(current, true, 0.5f);
        }

        public virtual void CreateMarker(Pawn pawn, bool transparentEdges = true, float edgeOpacity = 0.5f)
        {
            this.GetIndicatorProperities(pawn, out Color color, out float radius);
            base.CreateMarker(radius, color, pawn.Position, edgeOpacity);
        }
    }
}
