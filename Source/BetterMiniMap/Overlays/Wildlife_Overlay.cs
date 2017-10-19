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
            return Find.VisibleMap.mapPawns.AllPawns.Where(p => p.RaceProps.Animal && p.Faction != Faction.OfPlayer);
		}

        public override void GetIndicatorProperities(Pawn pawn, out Color color, out float radius)
        {
            if (pawn.HostileTo(Faction.OfPlayer))
            {
                color = BetterMiniMapMod.settings.overlayColors.wildlifeHostiles;
                radius = BetterMiniMapMod.settings.indicatorSizes.wildlifeHostiles;
                return;
            }

            Designation designation = Find.VisibleMap.designationManager.DesignationOn(pawn);
            if (designation != null)
            {
                DesignationDef designationDef = designation.def;
                if (designationDef == DesignationDefOf.Hunt)
                {
                    color = BetterMiniMapMod.settings.overlayColors.wildlifeHunting;
                    radius = BetterMiniMapMod.settings.indicatorSizes.wildlifeHunting;
                    return;
                }
                if (designationDef == DesignationDefOf.Tame)
                {
                    color = BetterMiniMapMod.settings.overlayColors.wildlifeTaming;
                    radius = BetterMiniMapMod.settings.indicatorSizes.wildlifeTaming;
                    return; 
                }
            }

            color = BetterMiniMapMod.settings.overlayColors.wildlife;
            radius = BetterMiniMapMod.settings.indicatorSizes.wildlife;
            return; 
        }

        public void ExposeData() => this.ExposeData("overlayWild");
    }
}
