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

        public void ExposeData() => this.ExposeData("overlayNoncolonist");

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.updatePeriods.noncolonists;

		public override IEnumerable<Pawn> GetPawns()
		{
            return Find.VisibleMap.mapPawns.AllPawns.Where(p => !p.RaceProps.Animal && p.Faction != Faction.OfPlayer);
		}

        public override void GetIndicatorProperities(Pawn pawn, out Color color, out Color edgeColor, out float radius)
        {
            if (pawn.HostileTo(Faction.OfPlayer))
            {
                color = BetterMiniMapMod.settings.overlayColors.enemyPawns;
                edgeColor = BetterMiniMapMod.settings.overlayColors.enemyPawnsFaded;
                radius = BetterMiniMapMod.settings.indicatorSizes.enemyPawns;
            }
            else if (pawn.trader != null)
            {
                color = BetterMiniMapMod.settings.overlayColors.traderPawns;
                edgeColor = BetterMiniMapMod.settings.overlayColors.traderPawnsFaded;
                radius = BetterMiniMapMod.settings.indicatorSizes.traderPawns;
            }
            else
            {
                color = BetterMiniMapMod.settings.overlayColors.visitorPawns;
                edgeColor = BetterMiniMapMod.settings.overlayColors.visitorPawnsFaded;
                radius = BetterMiniMapMod.settings.indicatorSizes.visitorPawns;
            }
        }

    }
}
