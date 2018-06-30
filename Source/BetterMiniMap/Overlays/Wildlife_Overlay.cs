using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace BetterMiniMap.Overlays
{
	public class Wildlife_Overlay : Pawns_Overlay, IExposable
	{
        public Wildlife_Overlay(bool visible = true) : base(visible) { }

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.updatePeriods.wildlife;

		public override IEnumerable<Pawn> GetPawns()
		{
            return Find.CurrentMap.mapPawns.AllPawns.Where(p => p.RaceProps.Animal && p.Faction != Faction.OfPlayer);
		}

        public override void GetIndicatorProperities(Pawn pawn, out Color color, out Color edgeColor, out float radius)
        {
            if (pawn.HostileTo(Faction.OfPlayer))
            {
                color = BetterMiniMapMod.settings.overlayColors.wildlifeHostiles;
                edgeColor = BetterMiniMapMod.settings.overlayColors.wildlifeHostilesFaded;
                radius = BetterMiniMapMod.settings.indicatorSizes.wildlifeHostiles;
                return;
            }

            Designation designation = Find.CurrentMap.designationManager.DesignationOn(pawn);
            if (designation != null)
            {
                DesignationDef designationDef = designation.def;
                if (designationDef == DesignationDefOf.Hunt)
                {
                    color = BetterMiniMapMod.settings.overlayColors.wildlifeHunting;
                    edgeColor = BetterMiniMapMod.settings.overlayColors.wildlifeHuntingFaded;
                    radius = BetterMiniMapMod.settings.indicatorSizes.wildlifeHunting;
                    return;
                }
                if (designationDef == DesignationDefOf.Tame)
                {
                    color = BetterMiniMapMod.settings.overlayColors.wildlifeTaming;
                    edgeColor = BetterMiniMapMod.settings.overlayColors.wildlifeTamingFaded;
                    radius = BetterMiniMapMod.settings.indicatorSizes.wildlifeTaming;
                    return; 
                }
            }

            color = BetterMiniMapMod.settings.overlayColors.wildlife;
            edgeColor = BetterMiniMapMod.settings.overlayColors.wildlifeFaded;
            radius = BetterMiniMapMod.settings.indicatorSizes.wildlife;
            return; 
        }

        public void ExposeData() => this.ExposeData("overlayWild");
    }
}
