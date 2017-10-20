using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace BetterMiniMap.Overlays
{
	public class Ships_Overlay : Things_Overlay, IExposable
	{
        public Ships_Overlay(bool visible = true) : base(visible) { }

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.updatePeriods.ships;

		public override IEnumerable<Thing> GetThings()
		{
            return Find.VisibleMap.listerThings.AllThings.Where(t => t is Building_CrashedShipPart);
        }

        public override void GetIndicatorProperities(Thing thing, out Color color, out Color edgeColor, out float radius)
        {
            color = BetterMiniMapMod.settings.overlayColors.ships;
            edgeColor = BetterMiniMapMod.settings.overlayColors.shipsFaded;
            radius = BetterMiniMapMod.settings.indicatorSizes.ships;
        }

        public void ExposeData() => this.ExposeData("overlayShips");
    }
}
