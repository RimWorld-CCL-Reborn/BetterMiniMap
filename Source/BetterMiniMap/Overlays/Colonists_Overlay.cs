using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace BetterMiniMap.Overlays
{
	public class Colonists_Overlay : Pawns_Overlay, IExposable
	{
		private static readonly Color colonistColor = Color.green;
		private static readonly Color colonistAnimalColor = Color.green;

        public Colonists_Overlay(bool visible = true) : base(visible) { }

        public override int GetUpdateInterval() => BetterMiniMapMod.settings.updatePeriods.colonists;

		public override IEnumerable<Pawn> GetPawns() => Find.VisibleMap.mapPawns.AllPawns.Where(p => p.Faction == Faction.OfPlayer);

        public override void GetIndicatorProperities(Pawn pawn, out Color color, out float radius)
        {
            if (pawn.RaceProps.Animal)
            {
                color = colonistAnimalColor;
                radius = BetterMiniMapMod.settings.indicatorSizes.tamedAnimals;
            }
            else
            {
                color = colonistColor;
                radius = BetterMiniMapMod.settings.indicatorSizes.colonists;
            }
        }

        public void ExposeData() => this.ExposeData("overlayColonists");
	}
}
