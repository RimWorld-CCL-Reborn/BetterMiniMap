using UnityEngine;
using Verse;

namespace BetterMiniMap
{
	public class Overlay_Fog : Overlay
	{
        public Overlay_Fog(bool visible) : base(visible) { }

		public override void Update()
		{
			bool[] fogGrid = Find.VisibleMap.fogGrid.fogGrid;
			for (int i = 0; i < Find.VisibleMap.cellIndices.NumGridCells; i++)
			{
				IntVec3 intVec = Find.VisibleMap.cellIndices.IndexToCell(i);
				base.Texture.SetPixel(intVec.x, intVec.z, fogGrid[i] ? Color.gray : Color.clear);
			}
		}

		public override int GetUpdateInterval()
		{
			return BetterMiniMapMod.settings.overlay_Fog;
		}
	}
}
