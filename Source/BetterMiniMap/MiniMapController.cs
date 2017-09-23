using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BetterMiniMap
{
	internal class MiniMapController
	{
		private List<Overlay> layers;

		public List<Overlay_Area> overlayAreas = new List<Overlay_Area>();
		public Overlay_Area selectedArea;
		public Overlay_Colonists overlayColonists = new Overlay_Colonists(MiniMap_GameComponent.OverlayColonists);
		public Overlay_Fog overlayFog = new Overlay_Fog(MiniMap_GameComponent.OverlayFog);
		public Overlay_Mining overlayMining = new Overlay_Mining(MiniMap_GameComponent.OverlayMining);
		public Overlay_NonColonistPawns overlayNoncolonist = new Overlay_NonColonistPawns(MiniMap_GameComponent.OverlayNoncolonist);
		public Overlay_Buildings overlayBuilding = new Overlay_Buildings(MiniMap_GameComponent.OverlayBuilding);
		public Overlay_PowerGrid overlayPower = new Overlay_PowerGrid(MiniMap_GameComponent.OverlayPower);
		public Overlay_Terrain overlayTerrain = new Overlay_Terrain(MiniMap_GameComponent.OverlayTerrain);
		public Overlay_ViewPort overlayView = new Overlay_ViewPort(MiniMap_GameComponent.OverlayView);
		public Overlay_Wildlife overlayWild = new Overlay_Wildlife(MiniMap_GameComponent.OverlayWild);
		public Overlay_Ships overlayShips = new Overlay_Ships(MiniMap_GameComponent.OverlayShips);
		public Overlay_Robots overlayRobots = new Overlay_Robots(MiniMap_GameComponent.OverlayRobots);

		public List<Overlay> Layers
		{
            get => this.layers;
			set => this.layers = value;
		}

		public MiniMapController()
		{
            this.layers = new List<Overlay>()
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

		public void UpdateOverlays()
		{
			if (this.layers.Any<Overlay>())
			{
				foreach (Overlay current in this.layers)
				{
					if (this.ShouldUpdateOverlay(current))
					{
						current.Update();
						current.Texture.Apply();
						current.Dirty = false;
					}
				}
			}
			if (this.selectedArea != null)
			{
				if (this.ShouldUpdateOverlay(this.selectedArea))
				{
					this.selectedArea.Update();
					this.selectedArea.Texture.Apply();
					this.selectedArea.Dirty = false;
				}
			}
		}

		public void UpdateAll()
		{
			if (this.layers.Any<Overlay>())
			{
				foreach (Overlay current in this.layers)
				{
                    current.Update();
					current.Texture.Apply();
					current.Dirty = false;
				}
			}
			if (this.selectedArea != null)
			{
				this.selectedArea.Update();
				this.selectedArea.Texture.Apply();
				this.selectedArea.Dirty = false;
			}
		}
	}
}
