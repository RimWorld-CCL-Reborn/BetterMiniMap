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
		private Color wildColor = Color.yellow;
		private Color tameColor = Color.green;
		private Color hostileColor = Color.red;
		private Color huntingColor = GenUI.MouseoverColor;
		private Color tamingColor = GenUI.MouseoverColor;

        public Wildlife_Overlay(bool visible) : base(visible) { }

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.overlay_Wildlife;

		public override Color GetColor(Pawn pawn)
		{
			if (pawn.def.race.predator || pawn.HostileTo(Faction.OfPlayer))
				return this.hostileColor;

            Designation designation = Find.VisibleMap.designationManager.DesignationOn(pawn);
            if (designation != null)
            {
                DesignationDef designationDef = designation.def;
                if (designationDef == DesignationDefOf.Hunt)
                    return this.huntingColor;
                if (designationDef == DesignationDefOf.Tame)
                    return this.tameColor;
            }

            return this.wildColor;
		}

		public override IEnumerable<Pawn> GetPawns()
		{
            return Find.VisibleMap.mapPawns.AllPawns.Where(p => p.RaceProps.Animal && p.Faction != Faction.OfPlayer);
		}

        public override float GetRadius(Pawn pawn)
        {
            if (pawn.HostileTo(Faction.OfPlayer))
                return (float)BetterMiniMapMod.settings.radiu_Wildlifehostile;

            Designation designation = Find.VisibleMap.designationManager.DesignationOn(pawn);
            if (designation != null)
            {
                DesignationDef designationDef = designation.def;
                if (designationDef == DesignationDefOf.Hunt)
                    return (float)BetterMiniMapMod.settings.radiu_Wildlifehunting;
                if (designationDef == DesignationDefOf.Tame)
                    return (float)BetterMiniMapMod.settings.radiu_Wildlifetame;
            }

            return (float)BetterMiniMapMod.settings.radiu_Wildlifewild;
        }

    }
}
