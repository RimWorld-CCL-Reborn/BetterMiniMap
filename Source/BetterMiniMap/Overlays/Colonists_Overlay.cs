using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace BetterMiniMap.Overlays
{
	public class Colonists_Overlay : Pawns_Overlay
	{
		private static readonly Color colonistColor = Color.green;
		private static readonly Color colonistAnimalColor = Color.green;

        public Colonists_Overlay(bool visible) : base(visible) { }

        public override int GetUpdateInterval() => BetterMiniMapMod.settings.overlay_Colonists;

		public override Color GetColor(Pawn pawn)
		{
			return pawn.RaceProps.Animal ? colonistAnimalColor : colonistColor;
		}

		public override IEnumerable<Pawn> GetPawns()
		{
			return Find.VisibleMap.mapPawns.AllPawns.Where(p => p.Faction == Faction.OfPlayer);
        }

		public override float GetRadius(Pawn pawn)
		{
			return pawn.RaceProps.Animal ? BetterMiniMapMod.settings.indicatorSizes.tamedAnimals : BetterMiniMapMod.settings.indicatorSizes.colonists;
		}

	}
}
