using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace BetterMiniMap
{
	public class Overlay_Robots : Overlay_Pawns
	{
		private static readonly Color robotColor = Color.white;

        public Overlay_Robots(bool visible) : base(visible) { }

        public override Color GetColor(Pawn pawn) => robotColor;

		public override float GetRadius(Pawn pawn) => BetterMiniMapMod.settings.radiu_Robots;

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.overlay_Robots;

		public override IEnumerable<Pawn> GetPawns()
		{
			return Find.VisibleMap.mapPawns.AllPawns.Where(p => p.def.thingClass.ToString().ToLower().Contains("robot"));
		}
	}
}
