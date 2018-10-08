using System.Linq;
using Verse;
using RimWorld;

namespace BetterMiniMap.Overlays
{
	public class MiningOverlay : Overlay, IExposable
	{
        public MiningOverlay(Map map, bool visible = true) : base(map, visible) { }

        public void ExposeData() => this.ExposeData("overlayMining");

		public override void Render()
		{
			foreach (Designation current in this.map.designationManager.allDesignations.Where(d => d.def == DesignationDefOf.Mine).ToList<Designation>())
			{
				IntVec3 cell = current.target.Cell;
				base.Texture.SetPixel(cell.x, cell.z, BetterMiniMapMod.modSettings.overlayColors.mining);
			}
        }

		public override int GetUpdateInterval() => BetterMiniMapMod.modSettings.updatePeriods.mining;

        public override int OverlayPriority => 2000;
	}
}
