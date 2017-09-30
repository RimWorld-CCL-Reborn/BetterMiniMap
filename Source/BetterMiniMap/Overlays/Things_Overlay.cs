using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
	public abstract class Things_Overlay : Overlay
	{
        protected Things_Overlay(bool visible = true) : base(visible) { }

		public abstract IEnumerable<Thing> GetThings();
        public abstract void GetIndicatorProperities(Thing thing, out Color color, out float radius);

		public override void Render()
		{
			foreach (Thing current in this.GetThings())
				this.CreateMarker(current);
        }

        public virtual void CreateMarker(Thing thing, float edgeOpacity = 0.5f)
        {
            this.GetIndicatorProperities(thing, out Color color, out float radius);
            base.CreateMarker(radius, color, thing.Position, edgeOpacity);
        }
    }
}
