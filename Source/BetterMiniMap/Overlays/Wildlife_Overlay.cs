using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace BetterMiniMap.Overlays
{
	public class Wildlife_Overlay : Pawns_Overlay
	{
		private static readonly Color wildColor = Color.yellow;
		private static readonly Color tamingColor = Color.green;
		private static readonly Color hostileColor = Color.red;
		private static readonly Color huntingColor = GenUI.MouseoverColor;
		//private static readonly Color tamingColor = GenUI.MouseoverColor;

        public Wildlife_Overlay(bool visible) : base(visible) { }

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.overlay_Wildlife;

		public override IEnumerable<Pawn> GetPawns()
		{
            return Find.VisibleMap.mapPawns.AllPawns.Where(p => p.RaceProps.Animal && p.Faction != Faction.OfPlayer);
		}

        public override void GetIndicatorProperities(Pawn pawn, out Color color, out float radius)
        {
            if (pawn.HostileTo(Faction.OfPlayer))
            {
                color = hostileColor;
                radius = BetterMiniMapMod.settings.indicatorSizes.wildlifeHostiles;
                return;
            }

            Designation designation = Find.VisibleMap.designationManager.DesignationOn(pawn);
            if (designation != null)
            {
                DesignationDef designationDef = designation.def;
                if (designationDef == DesignationDefOf.Hunt)
                {
                    color = huntingColor;
                    radius = BetterMiniMapMod.settings.indicatorSizes.wildlifeHunting;
                    return;
                }
                if (designationDef == DesignationDefOf.Tame)
                {
                    color = tamingColor;
                    radius = BetterMiniMapMod.settings.indicatorSizes.wildlifeTaming;
                    return; 
                }
            }

            color = wildColor;
            radius = BetterMiniMapMod.settings.indicatorSizes.wildlife;
            return; 
        }

    }
}
