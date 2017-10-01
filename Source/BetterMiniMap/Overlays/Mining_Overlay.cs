using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace BetterMiniMap.Overlays
{
	public class Mining_Overlay : Overlay, IExposable
	{
		private static readonly Color miningColor = new Color(0.75f, 0.4f, 0.125f, 1f);

        public Mining_Overlay(bool visible = true) : base(visible) { }

		public override void Render()
		{
			foreach (Designation current in Find.VisibleMap.designationManager.allDesignations.Where(d => d.def == DesignationDefOf.Mine).ToList<Designation>())
			{
				IntVec3 cell = current.target.Cell;
				base.Texture.SetPixel(cell.x, cell.z, miningColor);
			}
        }

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.updatePeriods.mining;

        public void ExposeData() => this.ExposeData("overlayMining");
	}
}
