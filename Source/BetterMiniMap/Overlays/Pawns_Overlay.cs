using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
	public abstract class Pawns_Overlay : Overlay
	{
        public Pawns_Overlay(bool visible) : base(visible) { }

		public virtual void CreateMarker(Pawn pawn, bool transparentEdges = true, float edgeOpacity = 0.5f)
		{
			float radius = this.GetRadius(pawn);
			Color color = this.GetColor(pawn);
            base.CreateMarker(radius, color, pawn.Position, edgeOpacity);
		}

		public abstract Color GetColor(Pawn pawn);

		public virtual float GetRadius(Pawn pawn) => 3f;

		public abstract IEnumerable<Pawn> GetPawns();

		public override void Render()
		{
			foreach (Pawn current in this.GetPawns())
				this.CreateMarker(current, true, 0.5f);
        }
	}
}
