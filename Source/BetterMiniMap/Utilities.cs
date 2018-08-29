using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace BetterMiniMap
{
	public static class Utilities
	{
        private static Dictionary<int, Color32[]> clearPixelArray = new Dictionary<int, Color32[]>();

		public static Color32[] GetClearPixelArray
		{
			get
			{
                if (!Utilities.clearPixelArray.ContainsKey(Find.CurrentMap.Size.x))
				{
					Utilities.clearPixelArray[Find.CurrentMap.Size.x] = new Color32[Find.CurrentMap.Size.x * Find.CurrentMap.Size.z];
					for (int i = 0; i < Utilities.clearPixelArray[Find.CurrentMap.Size.x].Count<Color32>(); i++)
						Utilities.clearPixelArray[Find.CurrentMap.Size.x][i] = Color.clear;
				}
				return Utilities.clearPixelArray[Find.CurrentMap.Size.x];
			}
		}

	}
}
