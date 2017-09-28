using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

using BetterMiniMap.Overlays;

namespace BetterMiniMap
{

	internal class MiniMapWindow : Window
	{
        private const float minimumSize = 40f;

        private readonly Colonists_Overlay overlayColonists = new Colonists_Overlay(MiniMap_GameComponent.overlayColonists);
        private readonly Fog_Overlay overlayFog = new Fog_Overlay(MiniMap_GameComponent.overlayFog);
        private readonly Mining_Overlay overlayMining = new Mining_Overlay(MiniMap_GameComponent.overlayMining);
        private readonly NonColonists_Overlay overlayNoncolonist = new NonColonists_Overlay(MiniMap_GameComponent.overlayNoncolonist);
        private readonly Buildings_Overlay overlayBuilding = new Buildings_Overlay(MiniMap_GameComponent.overlayBuilding);
        private readonly PowerGrid_Overlay overlayPower = new PowerGrid_Overlay(MiniMap_GameComponent.overlayPower);
        private readonly Terrain_Overlay overlayTerrain = new Terrain_Overlay(MiniMap_GameComponent.overlayTerrain);
        private readonly Viewpoint_Overlay overlayView = new Viewpoint_Overlay(MiniMap_GameComponent.overlayView);
        private readonly Wildlife_Overlay overlayWild = new Wildlife_Overlay(MiniMap_GameComponent.overlayWild);
        private readonly Ships_Overlay overlayShips = new Ships_Overlay(MiniMap_GameComponent.overlayShips);
        private readonly Robots_Overlay overlayRobots = new Robots_Overlay(MiniMap_GameComponent.overlayRobots);

        private List<Overlay> overlays;
        private List<Areas_Overlay> areaOverlays = new List<Areas_Overlay>();

        private Areas_Overlay selectedArea;

		private List<FloatMenuOption> areasOptions;
		private List<FloatMenuOption> overlayOpions;

		private FloatMenu overlayMenu;
		private FloatMenu areaMenu;

        // TODO: better way to deal with default?
		private Vector3 prevMousePos = new Vector3(-1f, -1f, -1f);

        public bool resize = false; //resizable?

		private int mapID = -1;

        private MiniMapControls controls;

        public MiniMapWindow()
        {
            this.closeOnEscapeKey = false;
            this.preventCameraMotion = false;

            this.overlays = new List<Overlay>()
            {
                this.overlayTerrain,
                this.overlayColonists,
                this.overlayMining,
                this.overlayNoncolonist,
                this.overlayBuilding,
                this.overlayPower,
                this.overlayView,
                this.overlayWild,
                this.overlayShips,
                this.overlayRobots,
                this.overlayFog
            };

            // NOTE: some of these could migrate to controls...
            this.overlayOpions = new List<FloatMenuOption>()
            {
                new FloatMenuOptionItem(this.overlayColonists, "BMM_ColonistsOverlayLabel".Translate()),
                new FloatMenuOptionItem(this.overlayNoncolonist, "overlay_NonColonistPawnst".Translate()),
                new FloatMenuOptionItem(this.overlayWild, "overlay_Wildlifet".Translate()),
                new FloatMenuOptionItem(this.overlayBuilding, "BMM_BuildingsOverlayLabel".Translate()),
                new FloatMenuOptionItem(this.overlayPower, "overlay_PowerGridt".Translate()),
                new FloatMenuOptionItem(this.overlayMining, "overlay_Miningt".Translate()),
                new FloatMenuOptionItem(this.overlayFog, "overlay_Fogt".Translate()),
                new FloatMenuOptionItem(this.overlayTerrain, "overlay_Terraint".Translate()),
                new FloatMenuOptionItem(this.overlayShips, "overlay_Shipst".Translate()),
                new FloatMenuOptionItem(this.overlayRobots, "overlay_Robotst".Translate())
            };

            this.overlayMenu = new FloatMenu(this.overlayOpions)
            {
                closeOnEscapeKey = true,
                preventCameraMotion = false,
            };

            this.controls = new MiniMapControls(this);
        }

        public List<Overlay> Overlays { get => this.overlays; }
        public List<Areas_Overlay> AreaOverlays { get => this.areaOverlays; }
        public FloatMenu OverlayMenu { get => this.overlayMenu; }

		protected override float Margin { get => 0f; }

        // TODO: we can probably be smarter here...
		public override void Notify_ResolutionChanged()
		{
			base.Notify_ResolutionChanged();
			this.PostOpen();
		}

		public override void DoWindowContents(Rect inRect)
		{
			if (Find.VisibleMap.uniqueID != this.mapID)
			{
				this.mapID = Find.VisibleMap.uniqueID;
				this.Refresh();
			}

            // NOTE: why?
			//new WaitForEndOfFrame();
			this.Update();

			foreach (Overlay current in this.Overlays)
				if (current.Visible)
					GUI.DrawTexture(inRect, current.Texture);

			if (this.selectedArea != null)
				GUI.DrawTexture(inRect, this.selectedArea.Texture);

			if (!this.draggable && !this.resize && Mouse.IsOver(inRect) && Input.GetMouseButton(0))
			{
				Vector2 mousePosition = Event.current.mousePosition;
				Vector2 vector = new Vector2(mousePosition.x, inRect.height - mousePosition.y);
				Vector2 vector2 = new Vector2((float)Find.VisibleMap.Size.x / inRect.width, (float)Find.VisibleMap.Size.z / inRect.height);
				Find.CameraDriver.JumpToVisibleMapLoc(new Vector3(vector.x * vector2.x, 0f, vector.y * vector2.y));
			}
			else
			{
				if (this.resize && Mouse.IsOver(inRect) && Input.GetMouseButton(0))
				{
					if (this.prevMousePos.x == -1f)
					{
						this.prevMousePos = Input.mousePosition;
					}
					this.windowRect.width = this.windowRect.width + (this.prevMousePos.x - Input.mousePosition.x);
					this.windowRect.height = this.windowRect.width;
					this.prevMousePos = Input.mousePosition;
				}
                else if (this.resize && Mouse.IsOver(inRect) && !Input.GetMouseButton(0))
                    this.prevMousePos = new Vector3(-1f, -1f, -1f);
			}
		}

		public override void PostOpen()
		{
            this.windowRect = new Rect(MiniMap_GameComponent.Position, MiniMap_GameComponent.Size);
            // TODO: what? defaults?
            MiniMap_GameComponent.ResolutionX = UI.screenWidth;
            MiniMap_GameComponent.ResolutionY = UI.screenHeight;
            this.controls.SetLocality();
		}

        public void UpdateLocality(Vector2 position, Vector2 size)
        {
            this.windowRect.position = position;
            this.windowRect.size = size;
            this.controls.SetLocality();
        }

		public override void ExtraOnGUI()
		{
            if (this.draggable || this.resize)
                this.controls.SetLocality();
            this.ClampWindowToScreen();
			this.controls.DrawOverlayButtons();
		}

        private void ClampWindowToScreen()
        {
            if (this.windowRect.width < minimumSize)
                this.windowRect.height = this.windowRect.width = minimumSize;

            if (this.windowRect.xMax > UI.screenWidth)
                this.windowRect.x = UI.screenWidth - this.windowRect.width;

            if (this.windowRect.yMax > UI.screenHeight - MiniMapControls.buttonWidth - 1)
                this.windowRect.y = UI.screenHeight - this.windowRect.height - MiniMapControls.buttonWidth - 1;

            if (this.windowRect.xMin < 0f)
                this.windowRect.x = this.windowRect.x - this.windowRect.xMin;

            if (this.windowRect.yMin < 0f)
                this.windowRect.y = this.windowRect.y - this.windowRect.yMin;

            MiniMap_GameComponent.Position = this.windowRect.position;
            MiniMap_GameComponent.Size = this.windowRect.size;
        }


        public void Toggle(bool add = false)
        {
            if (add)
            {
                if (Find.WindowStack.Windows.IndexOf(this) == -1) // keep from double add...
                    Find.WindowStack.Add(this);
            }
            else
                Find.WindowStack.TryRemove(this, true);
        }

		public FloatMenu UpdateAreaOverlays()
		{
			bool remakeOptions = false;
			if (Find.VisibleMap.areaManager.AllAreas.Count != this.AreaOverlays.Count)
			{
                foreach (Area area in Find.VisibleMap.areaManager.AllAreas)
                {
                    // TODO: this seems expensive...
                    if (!this.AreaOverlays.Any((Areas_Overlay w) => w.area == area))
                    {
                        this.AreaOverlays.Add(new Areas_Overlay(area, false));
                        remakeOptions = true;
                    }
                }

				if (this.AreaOverlays.Any<Areas_Overlay>())
				{
					for (int i = this.AreaOverlays.Count - 1; i >= 0; i--)
					{
						Area area = this.AreaOverlays[i].area;
						if (area == null || !Find.VisibleMap.areaManager.AllAreas.Contains(area))
						{
							this.AreaOverlays.RemoveAt(i);
                            remakeOptions = true;
						}
					}
				}

				if (remakeOptions)
					this.MakeAreaOptions();
			}

            return this.areaMenu;
		}

        private void MakeAreaOptions()
        {
            if (this.AreaOverlays?.Count > 0)
            {
                this.areasOptions = new List<FloatMenuOption>();

                foreach (Areas_Overlay overlayArea in this.AreaOverlays)
                {
                    this.areasOptions.Add(new FloatMenuOptionItem(overlayArea.Visible, overlayArea.area.Label, delegate
                    {
                        overlayArea.Visible = !overlayArea.Visible;
                        if (this.selectedArea == null)
                            this.selectedArea = overlayArea;
                        else if (this.selectedArea == overlayArea)
                            this.selectedArea = null;
                        else
                        {
                            this.selectedArea.Visible = false;
                            this.selectedArea = overlayArea;
                        }
                        this.MakeAreaOptions();
                    }));
                }

                this.areaMenu = new FloatMenu(this.areasOptions)
                {
                    closeOnEscapeKey = true,
                    preventCameraMotion = false,
                };
            }
        }

        public void UpdateSettings()
        {
            MiniMap_GameComponent.overlayColonists = this.overlayColonists.Visible;
            MiniMap_GameComponent.overlayBuilding = this.overlayBuilding.Visible;
            MiniMap_GameComponent.overlayMining = this.overlayMining.Visible;
            MiniMap_GameComponent.overlayNoncolonist = this.overlayNoncolonist.Visible;
            MiniMap_GameComponent.overlayPower = this.overlayPower.Visible;
            MiniMap_GameComponent.overlayShips = this.overlayShips.Visible;
            MiniMap_GameComponent.overlayRobots = this.overlayRobots.Visible;
            MiniMap_GameComponent.overlayWild = this.overlayWild.Visible;
            MiniMap_GameComponent.overlayTerrain = this.overlayTerrain.Visible;
        }

        // TODO: are both UpdateOverlays() and UpdateAll() really needed?
        public void Update()
        {
            if (this.overlays.Any<Overlay>())
            {
                foreach (Overlay current in this.overlays)
                {
                    if (current.ShouldUpdateOverlay)
                        current.Update();
                }
            }
            if (this.selectedArea != null)
            {
                if (this.selectedArea.ShouldUpdateOverlay)
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
