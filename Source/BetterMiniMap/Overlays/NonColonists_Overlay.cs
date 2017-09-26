using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace BetterMiniMap.Overlays
{
	public class NonColonists_Overlay : Pawns_Overlay
	{
		private static readonly Color enemyColor = Color.red;
		private static readonly Color traderColor = Color.blue;
		private static readonly Color visitorColor = Color.green;

        public NonColonists_Overlay(bool visible) : base(visible) { }

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.overlay_NonColonistPawns;

		public override Color GetColor(Pawn pawn)
		{
			Color result;
			if (pawn.HostileTo(Faction.OfPlayer))
				result = enemyColor;
			else if (pawn.trader != null)
                result = traderColor;
            else
                result = visitorColor;

            return result;
		}

		public override IEnumerable<Pawn> GetPawns()
		{
            return Find.VisibleMap.mapPawns.AllPawns.Where(p => !p.RaceProps.Animal && p.Faction != Faction.OfPlayer);
		}

		public override float GetRadius(Pawn pawn)
		{
			if (pawn.HostileTo(Faction.OfPlayer))
				return BetterMiniMapMod.settings.indicatorSizes.enemyPawns;
            else if (pawn.trader != null)
                return BetterMiniMapMod.settings.indicatorSizes.traderPawns;
            return BetterMiniMapMod.settings.indicatorSizes.visitorPawns;
		}

	}
}
