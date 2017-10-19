using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
	public class Robots_Overlay : Pawns_Overlay, IExposable
	{
        public Robots_Overlay(bool visible = true) : base(visible) { }

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.updatePeriods.robots;

		public override IEnumerable<Pawn> GetPawns()
		{
			return Find.VisibleMap.mapPawns.AllPawns.Where(p => p.def.thingClass.ToString().ToLower().Contains("robot"));
		}

        public override void GetIndicatorProperities(Pawn pawn, out Color color, out float radius)
        {
            color = BetterMiniMapMod.settings.overlayColors.robots;
            radius = BetterMiniMapMod.settings.indicatorSizes.robots;
        }

        public void ExposeData() => this.ExposeData("overlayRobots");
    }
}
