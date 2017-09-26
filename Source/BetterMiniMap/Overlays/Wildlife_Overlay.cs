using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace BetterMiniMap.Overlays
{
    // TODO: get color and radius in a single go...

	public class Wildlife_Overlay : Pawns_Overlay
	{
		private static readonly Color wildColor = Color.yellow;
		private static readonly Color tameColor = Color.green;
		private static readonly Color hostileColor = Color.red;
		private static readonly Color huntingColor = GenUI.MouseoverColor;
		private static readonly Color tamingColor = GenUI.MouseoverColor;

        public Wildlife_Overlay(bool visible) : base(visible) { }

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.overlay_Wildlife;

		public override Color GetColor(Pawn pawn)
		{
			if (pawn.def.race.predator || pawn.HostileTo(Faction.OfPlayer))
				return hostileColor;

            Designation designation = Find.VisibleMap.designationManager.DesignationOn(pawn);
            if (designation != null)
            {
                DesignationDef designationDef = designation.def;
                if (designationDef == DesignationDefOf.Hunt)
                    return huntingColor;
                if (designationDef == DesignationDefOf.Tame)
                    return tameColor;
            }

            return wildColor;
		}

		public override IEnumerable<Pawn> GetPawns()
		{
            return Find.VisibleMap.mapPawns.AllPawns.Where(p => p.RaceProps.Animal && p.Faction != Faction.OfPlayer);
		}

        public override float GetRadius(Pawn pawn)
        {
            if (pawn.HostileTo(Faction.OfPlayer))
                return BetterMiniMapMod.settings.indicatorSizes.wildlifeHostiles;

            Designation designation = Find.VisibleMap.designationManager.DesignationOn(pawn);
            if (designation != null)
            {
                DesignationDef designationDef = designation.def;
                if (designationDef == DesignationDefOf.Hunt)
                    return BetterMiniMapMod.settings.indicatorSizes.wildlifeHunting;
                if (designationDef == DesignationDefOf.Tame)
                    return BetterMiniMapMod.settings.indicatorSizes.wildlifeTaming;
            }

            return BetterMiniMapMod.settings.indicatorSizes.wildlife;
        }

    }
}
