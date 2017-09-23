using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace BetterMiniMap
{
	public class Overlay_NonColonistPawns : Overlay_Pawns
	{
		private Color enemyColor = Color.red;

		private Color traderColor = Color.blue;

		private Color visitorColor = Color.green;

        public Overlay_NonColonistPawns(bool visible) : base(visible) { }

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.overlay_NonColonistPawns;

		public override Color GetColor(Pawn pawn)
		{
			Color result;
			if (pawn.HostileTo(Faction.OfPlayer))
				result = this.enemyColor;
			else if (pawn.trader != null)
                result = this.traderColor;
            else
                result = this.visitorColor;

            return result;
		}

		public override IEnumerable<Pawn> GetPawns()
		{
            return Find.VisibleMap.mapPawns.AllPawns.Where(p => !p.RaceProps.Animal && p.Faction != Faction.OfPlayer);
		}

		public override float GetRadius(Pawn pawn)
		{
			float result;
			if (pawn.HostileTo(Faction.OfPlayer))
				result = (float)BetterMiniMapMod.settings.radiu_NonColonistPawnsEnemy;
            else if (pawn.trader != null)
                result = (float)BetterMiniMapMod.settings.radiu_NonColonistPawnsTrader;
            else
                result = (float)BetterMiniMapMod.settings.radiu_NonColonistPawnsVisitor;

			return result;
		}

	}
}
