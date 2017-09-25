using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace BetterMiniMap.Overlays
{
	public class Mining_Overlay : Overlay
	{
		private static readonly Color miningColor = new Color(0.75f, 0.4f, 0.125f, 1f);

        public Mining_Overlay(bool visible) : base(visible) { }

		public override void Update()
		{
			base.ClearTexture(false);
			List<Designation> list = Find.VisibleMap.designationManager.allDesignations.Where(d => d.def == DesignationDefOf.Mine).ToList<Designation>();
			foreach (Designation current in list)
			{
				IntVec3 cell = current.target.Cell;
				base.Texture.SetPixel(cell.x, cell.z, miningColor);
			}
		}

		public override int GetUpdateInterval()
		{
			return BetterMiniMapMod.settings.overlay_Mining;
		}
	}
}
