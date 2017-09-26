using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
	public class Robots_Overlay : Pawns_Overlay
	{
		private static readonly Color robotColor = Color.white;

        public Robots_Overlay(bool visible) : base(visible) { }

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.overlay_Robots;

		public override IEnumerable<Pawn> GetPawns()
		{
			return Find.VisibleMap.mapPawns.AllPawns.Where(p => p.def.thingClass.ToString().ToLower().Contains("robot"));
		}

        public override void GetIndicatorProperities(Pawn pawn, out Color color, out float radius)
        {
            color = robotColor;
            radius = BetterMiniMapMod.settings.indicatorSizes.robots;
        }
    }
}
