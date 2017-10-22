using System.Linq;
using UnityEngine;
using Verse;

namespace BetterMiniMap
{
	public static class Utilities
	{
		private static Color32[] clearPixelArray;

		public static Color32[] GetClearPixelArray
		{
			get
			{
				if (Utilities.clearPixelArray == null)
				{
					Utilities.clearPixelArray = new Color32[Find.VisibleMap.Size.x * Find.VisibleMap.Size.z];
					for (int i = 0; i < Utilities.clearPixelArray.Count<Color32>(); i++)
						Utilities.clearPixelArray[i] = Color.clear;
				}
				return Utilities.clearPixelArray;
			}
		}

	}
}
