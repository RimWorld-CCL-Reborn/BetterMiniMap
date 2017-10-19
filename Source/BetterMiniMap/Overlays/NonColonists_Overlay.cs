using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace BetterMiniMap.Overlays
{
	public class NonColonists_Overlay : Pawns_Overlay, IExposable
	{
        public NonColonists_Overlay(bool visible = true) : base(visible) { }

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.updatePeriods.noncolonists;

		public override IEnumerable<Pawn> GetPawns()
		{
            return Find.VisibleMap.mapPawns.AllPawns.Where(p => !p.RaceProps.Animal && p.Faction != Faction.OfPlayer);
		}

        public override void GetIndicatorProperities(Pawn pawn, out Color color, out float radius)
        {
            if (pawn.HostileTo(Faction.OfPlayer))
            {
                color = BetterMiniMapMod.settings.overlayColors.enemyPawns;
                radius = BetterMiniMapMod.settings.indicatorSizes.enemyPawns;
            }
            else if (pawn.trader != null)
            {
                color = BetterMiniMapMod.settings.overlayColors.traderPawns;
                radius = BetterMiniMapMod.settings.indicatorSizes.traderPawns;
            }
            else
            {
                color = BetterMiniMapMod.settings.overlayColors.visitorPawns;
                radius = BetterMiniMapMod.settings.indicatorSizes.visitorPawns;
            }
        }

        public void ExposeData() => this.ExposeData("overlayNoncolonist");
    }
}
