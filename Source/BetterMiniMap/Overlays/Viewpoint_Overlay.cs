using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
	public class Viewpoint_Overlay : Overlay
	{
		private static readonly Color color = Color.white;

        public Viewpoint_Overlay(bool visible) : base(visible) { }

        public override int GetUpdateInterval() => BetterMiniMapMod.settings.overlay_ViewPort;

		public override void Render()
		{
			foreach (IntVec3 current in Find.CameraDriver.CurrentViewRect.EdgeCells)
				if (current.InBounds(Find.VisibleMap))
					base.Texture.SetPixel(current.x, current.z, color);
        }

	}
}
