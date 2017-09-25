using System.Collections.Generic;
using UnityEngine;
using Verse;

using BetterMiniMap.Overlays;

namespace BetterMiniMap
{
	internal class MiniMapController
	{
		private List<Overlay> overlays;
		private List<Areas_Overlay> areaOverlays = new List<Areas_Overlay>();

		public Areas_Overlay selectedArea;
		public Colonists_Overlay overlayColonists = new Colonists_Overlay(MiniMap_GameComponent.OverlayColonists);
		public Fog_Overlay overlayFog = new Fog_Overlay(MiniMap_GameComponent.OverlayFog);
		public Mining_Overlay overlayMining = new Mining_Overlay(MiniMap_GameComponent.OverlayMining);
		public NonColonists_Overlay overlayNoncolonist = new NonColonists_Overlay(MiniMap_GameComponent.OverlayNoncolonist);
		public Buildings_Overlay overlayBuilding = new Buildings_Overlay(MiniMap_GameComponent.OverlayBuilding);
		public PowerGrid_Overlay overlayPower = new PowerGrid_Overlay(MiniMap_GameComponent.OverlayPower);
		public Terrain_Overlay overlayTerrain = new Terrain_Overlay(MiniMap_GameComponent.OverlayTerrain);
		public Viewpoint_Overlay overlayView = new Viewpoint_Overlay(MiniMap_GameComponent.OverlayView);
		public Wildlife_Overlay overlayWild = new Wildlife_Overlay(MiniMap_GameComponent.OverlayWild);
		public Ships_Overlay overlayShips = new Ships_Overlay(MiniMap_GameComponent.OverlayShips);
		public Robots_Overlay overlayRobots = new Robots_Overlay(MiniMap_GameComponent.OverlayRobots);

		public List<Overlay> Overlays { get => this.overlays; }
		public List<Areas_Overlay> AreaOverlays { get => this.areaOverlays; }

		public MiniMapController()
		{
            this.overlays = new List<Overlay>()
            {
                overlayTerrain,
                overlayColonists,
                overlayMining,
                overlayNoncolonist,
                overlayBuilding,
                overlayPower,
                overlayView,
                overlayWild,
                overlayShips,
                overlayRobots,
                overlayFog
            };
		}

		private bool ShouldUpdateOverlay(Overlay overlay)
		{
			return overlay.Dirty || (overlay.GetUpdateInterval() > 0 && (Time.frameCount + overlay.GetHashCode()) % overlay.GetUpdateInterval() == 0);
		}

        // TODO: are both UpdateOverlays() and UpdateAll() really needed?

		public void Update()
		{
			if (this.overlays.Any<Overlay>())
			{
				foreach (Overlay current in this.overlays)
				{
					if (this.ShouldUpdateOverlay(current))
						current.Update();
				}
			}
			if (this.selectedArea != null)
			{
				if (this.ShouldUpdateOverlay(this.selectedArea))
					this.selectedArea.Update();
			}
		}

		public void Refresh()
		{
			if (this.overlays.Any<Overlay>())
			{
				foreach (Overlay current in this.overlays)
                    current.Update();
			}
			if (this.selectedArea != null)
				this.selectedArea.Update();
		}

	}
}
