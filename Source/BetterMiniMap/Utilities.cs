using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace BetterMiniMap
{
	public static class Utilities
	{
        private static Dictionary<int, Color32[]> clearPixelArray = new Dictionary<int, Color32[]>();

        // assuming square maps
		public static Color32[] GetClearPixelArray(int size)
		{
            if (!Utilities.clearPixelArray.ContainsKey(size))
            {
                Utilities.clearPixelArray[size] = new Color32[size*size];
                for (int i = 0; i < Utilities.clearPixelArray[size].Count<Color32>(); i++)
                    Utilities.clearPixelArray[size][i] = Color.clear;
            }
            return Utilities.clearPixelArray[size];
		}

	}
}
