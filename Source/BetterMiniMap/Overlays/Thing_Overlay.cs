using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
    public class Thing_Overlay : MarkerOverlay
    {
        protected Thing_Overlay(bool visible = true) : base(visible) { }

        public abstract IEnumerable<Thing> GetThings();
        public abstract void GetIndicatorProperities(Thing thing, out Color color, out Color edgeColor, out float radius);

        // TODO: store in xml (as default)
        public override int GetUpdateInterval() => 60; //TODO

        public override void Render()
        {
            foreach (Thing current in this.GetThings())
                this.CreateMarker(current);
        }

        public virtual void CreateMarker(Thing thing, float edgeOpacity = 0.5f)
        {
            this.GetIndicatorProperities(thing, out Color color, out Color edgeColor, out float radius);
            base.CreateMarker(thing.Position, radius, color, edgeColor, edgeOpacity);
        }


    }
}
