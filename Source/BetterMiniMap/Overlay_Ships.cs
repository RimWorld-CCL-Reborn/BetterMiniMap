using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace BetterMiniMap
{
	public class Overlay_Ships : Overlay_Things
	{
		private Color shipColor = Color.red;

        public Overlay_Ships(bool visible) : base(visible) { }

		public override Color GetColor(Thing thing) => this.shipColor;

		public override float GetRadius(Thing thing) => BetterMiniMapMod.settings.radiu_Ships;

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.overlay_Ships;

		public override IEnumerable<Thing> GetThings()
		{
            return Find.VisibleMap.listerThings.AllThings.Where(t => t is Building_CrashedShipPart);
            //return Find.VisibleMap.listerThings.AllThings.Where(t => t.def == ThingDefOf.CrashedPoisonShipPart);
        }

	}
}
