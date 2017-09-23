using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace BetterMiniMap
{
	public class Overlay_Colonists : Overlay_Pawns
	{
		private static readonly Color colonistColor = Color.green;
		private static readonly Color colonistAnimalColor = Color.green;

        public Overlay_Colonists(bool visible) : base(visible) { }

        public override int GetUpdateInterval() => BetterMiniMapMod.settings.overlay_Colonists;

		public override Color GetColor(Pawn pawn)
		{
			return pawn.RaceProps.Animal ? colonistAnimalColor : colonistColor;
		}

		public override IEnumerable<Pawn> GetPawns()
		{
			return Find.VisibleMap.mapPawns.AllPawns.Where(p => p.Faction == Faction.OfPlayer && !p.RaceProps.Animal);
        }

		public override float GetRadius(Pawn pawn)
		{
			return pawn.RaceProps.Animal ? BetterMiniMapMod.settings.radiu_ColonistsAnimal : BetterMiniMapMod.settings.radiu_Colonists;
		}

	}
}
