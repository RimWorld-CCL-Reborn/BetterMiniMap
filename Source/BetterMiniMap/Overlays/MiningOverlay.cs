using System.Linq;
using Verse;
using RimWorld;

namespace BetterMiniMap.Overlays
{
	public class MiningOverlay : Overlay, IExposable
	{
        public MiningOverlay(bool visible = true) : base(visible) { }

        public void ExposeData() => this.ExposeData("overlayMining");

		public override void Render()
		{
			foreach (Designation current in Find.CurrentMap.designationManager.allDesignations.Where(d => d.def == DesignationDefOf.Mine).ToList<Designation>())
			{
				IntVec3 cell = current.target.Cell;
				base.Texture.SetPixel(cell.x, cell.z, BetterMiniMapMod.settings.overlayColors.mining);
			}
        }

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.updatePeriods.mining;

        public override int OverlayPriority => 2000;
	}
}
