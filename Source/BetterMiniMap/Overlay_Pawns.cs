using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace BetterMiniMap
{
	public abstract class Overlay_Pawns : Overlay
	{
        public Overlay_Pawns(bool visible) : base(visible) { }

		public virtual void CreateMarker(Pawn pawn, bool transparentEdges = true, float edgeOpacity = 0.5f)
		{
			float radius = this.GetRadius(pawn);
			/*if (radius < 1f)
			{
				throw new ArgumentOutOfRangeException("radius must be > 1f");
			}*/
			int num = GenRadial.NumCellsInRadius(radius);
			int num2 = (transparentEdges && radius >= 2f) ? GenRadial.NumCellsInRadius(radius - 1f) : num;
			Color color = this.GetColor(pawn);
			Color color2 = new Color(color.r, color.g, color.b, color.a * edgeOpacity);
			IntVec3[] array = GenRadial.RadialCellsAround(pawn.Position, radius, true).ToArray<IntVec3>();
			for (int i = 0; i < num; i++)
			{
				if (array[i].InBounds(Find.VisibleMap))
				{
					base.Texture.SetPixel(array[i].x, array[i].z, (transparentEdges && i > num2) ? color2 : color);
				}
			}
		}

		public abstract Color GetColor(Pawn pawn);

		public virtual float GetRadius(Pawn pawn) => 3f;

		public abstract IEnumerable<Pawn> GetPawns();

		public override void Update()
		{
			IEnumerable<Pawn> pawns = this.GetPawns();
			base.ClearTexture(false);
			foreach (Pawn current in pawns)
			{
				this.CreateMarker(current, true, 0.5f);
			}
		}
	}
}
