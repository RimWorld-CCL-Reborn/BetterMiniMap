using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace BetterMiniMap.Overlays
{
	public class NonColonists_Overlay : Pawns_Overlay, IExposable
	{
		private static readonly Color enemyColor = Color.red;
		private static readonly Color traderColor = Color.blue;
		private static readonly Color visitorColor = Color.green;

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
                color = enemyColor;
                radius = BetterMiniMapMod.settings.indicatorSizes.enemyPawns;
            }
            else if (pawn.trader != null)
            {
                color = traderColor;
                radius = BetterMiniMapMod.settings.indicatorSizes.traderPawns;
            }
            else
            {
                color = visitorColor;
                radius = BetterMiniMapMod.settings.indicatorSizes.visitorPawns;
            }
        }

        public void ExposeData() => this.ExposeData("overlayNoncolonist");
    }
}
