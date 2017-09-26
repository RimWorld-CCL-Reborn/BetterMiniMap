using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace BetterMiniMap.Overlays
{
	public class Ships_Overlay : Things_Overlay
	{
		private static readonly Color shipColor = Color.red;

        public Ships_Overlay(bool visible) : base(visible) { }

		public override Color GetColor(Thing thing) => shipColor;

		public override float GetRadius(Thing thing) => BetterMiniMapMod.settings.indicatorSizes.ships;

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.overlay_Ships;

		public override IEnumerable<Thing> GetThings()
		{
            return Find.VisibleMap.listerThings.AllThings.Where(t => t is Building_CrashedShipPart);
        }
	}
}
