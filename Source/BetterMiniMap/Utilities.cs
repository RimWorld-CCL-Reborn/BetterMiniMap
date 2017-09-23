using System.Linq;
using UnityEngine;
using Verse;

namespace BetterMiniMap
{
	public static class Utilities
	{
		private static Color[] clearPixelArray;

		public static Color[] GetClearPixelArray
		{
			get
			{
				if (Utilities.clearPixelArray == null)
				{
					Utilities.clearPixelArray = new Color[Find.VisibleMap.Size.x * Find.VisibleMap.Size.z];
					for (int i = 0; i < Utilities.clearPixelArray.Count<Color>(); i++)
						Utilities.clearPixelArray[i] = Color.clear;
				}
				return Utilities.clearPixelArray;
			}
		}

		public static void DrawThing(Texture2D texture, Thing thing, Color color)
		{
            foreach (IntVec3 current in thing.OccupiedRect().Cells)
                if (current.InBounds(Find.VisibleMap))
                    texture.SetPixel(current.x, current.z, color);
		}
	}
}
