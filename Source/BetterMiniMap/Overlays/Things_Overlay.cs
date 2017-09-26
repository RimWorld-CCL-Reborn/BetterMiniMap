using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
	public abstract class Things_Overlay : Overlay
	{
        protected Things_Overlay(bool visible) : base(visible) { }

		public virtual void CreateMarker(Thing thing, float edgeOpacity = 0.5f)
		{
			float radius = this.GetRadius(thing);
			Color color = this.GetColor(thing);
            base.CreateMarker(radius, color, thing.Position, edgeOpacity);
		}

		public abstract Color GetColor(Thing thing);

		public virtual float GetRadius(Thing thing) => 3f;

		public abstract IEnumerable<Thing> GetThings();

		public override void Render()
		{
			foreach (Thing current in this.GetThings())
				this.CreateMarker(current);
        }
	}
}
