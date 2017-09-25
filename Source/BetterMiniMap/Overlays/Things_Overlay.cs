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

        // TODO: consolidate?
		public virtual void CreateMarker(Thing thing, bool transparentEdges = true, float edgeOpacity = 0.5f)
		{
			float radius = this.GetRadius(thing);
			int num = GenRadial.NumCellsInRadius(radius);
			int num2 = (transparentEdges && radius >= 2f) ? GenRadial.NumCellsInRadius(radius - 1f) : num;
			Color color = this.GetColor(thing);
			Color color2 = new Color(color.r, color.g, color.b, color.a * edgeOpacity);
			IntVec3[] array = GenRadial.RadialCellsAround(thing.Position, radius, true).ToArray<IntVec3>();

            for (int i = 0; i < num; i++)
				if (array[i].InBounds(Find.VisibleMap))
					base.Texture.SetPixel(array[i].x, array[i].z, (transparentEdges && i > num2) ? color2 : color);
		}

		public abstract Color GetColor(Thing thing);

		public virtual float GetRadius(Thing thing) => 3f;

		public abstract IEnumerable<Thing> GetThings();

		public override void Update()
		{
			base.ClearTexture(false);
			foreach (Thing current in this.GetThings())
				this.CreateMarker(current, true, 0.5f);
		}
	}
}
