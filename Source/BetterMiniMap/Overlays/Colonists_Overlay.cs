using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace BetterMiniMap.Overlays
{
	public class Colonists_Overlay : Pawns_Overlay, IExposable
	{
        public Colonists_Overlay(bool visible = true) : base(visible) { }

        public void ExposeData() => this.ExposeData("overlayColonists");

        public override int GetUpdateInterval() => BetterMiniMapMod.settings.updatePeriods.colonists;

		public override IEnumerable<Pawn> GetPawns() => Find.CurrentMap.mapPawns.AllPawns.Where(p => p.Faction == Faction.OfPlayer);

        public override void GetIndicatorProperities(Pawn pawn, out Color color, out Color edgeColor, out float radius)
        {
            if (pawn.RaceProps.Animal)
            {
                color = BetterMiniMapMod.settings.overlayColors.tamedAnimals;
                edgeColor = BetterMiniMapMod.settings.overlayColors.tamedAnimalsFaded;
                radius = BetterMiniMapMod.settings.indicatorSizes.tamedAnimals;
            }
            else
            {
                color = BetterMiniMapMod.settings.overlayColors.colonists;
                edgeColor = BetterMiniMapMod.settings.overlayColors.colonistsFaded;
                radius = BetterMiniMapMod.settings.indicatorSizes.colonists;
            }
        }

	}
}
