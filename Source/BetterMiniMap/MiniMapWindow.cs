using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

using BetterMiniMap.Overlays;

namespace BetterMiniMap
{

    // TODO: track weather the minimap is currently shown or not...

	internal class MiniMapWindow : Window, IExposable
	{
        private const float minimumSize = 150f;
        private const float defaultSize = 300f;
        private const int defaultMargin = 8;

        private readonly Colonists_Overlay overlayColonists = new Colonists_Overlay();
        private readonly Fog_Overlay overlayFog = new Fog_Overlay();
        private readonly Mining_Overlay overlayMining = new Mining_Overlay();
        private readonly NonColonists_Overlay overlayNoncolonist = new NonColonists_Overlay();
        private readonly Buildings_Overlay overlayBuilding = new Buildings_Overlay();
        private readonly PowerGrid_Overlay overlayPower = new PowerGrid_Overlay();
        private readonly Terrain_Overlay overlayTerrain = new Terrain_Overlay();
        private readonly Viewpoint_Overlay overlayView = new Viewpoint_Overlay();
        private readonly Wildlife_Overlay overlayWild = new Wildlife_Overlay();
        private readonly Ships_Overlay overlayShips = new Ships_Overlay();
        private readonly Robots_Overlay overlayRobots = new Robots_Overlay();

        private List<Overlay> overlays;
        private List<Areas_Overlay> areaOverlays = new List<Areas_Overlay>();

        private Areas_Overlay selectedArea;

		private List<FloatMenuOption> areasOptions;
		private List<FloatMenuOption> overlayOptions;

		private FloatMenu overlayMenu;
		private FloatMenu areaMenu;

        // TODO: better way to deal with default?
		private Vector3 prevMousePos = new Vector3(-1f, -1f, -1f);

        public bool resize = false; //TODO: resizable from base?

		private int mapID = -1;

        private MiniMapControls controls;

        // TODO: these values are in this.windowRect, do we really need another copy?
        private Vector2 position;
        private Vector2 size;

        // TODO: are these needed?
        private int resolutionX;
        private int resolutionY;

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
                this.overlayWild,
                this.overlayShips,
                this.overlayRobots,
                this.overlayFog,
                this.overlayView
            };

            this.GenerateOverlayOptions();

            this.overlayMenu = new FloatMenu(this.overlayOptions)
            {
                closeOnEscapeKey = true,
                preventCameraMotion = false,
            };

            this.controls = new MiniMapControls(this);

            this.resolutionX = UI.screenWidth;
            this.resolutionY = UI.screenHeight;
        }

        public List<Overlay> Overlays { get => this.overlays; }
        public List<Areas_Overlay> AreaOverlays { get => this.areaOverlays; }
        public FloatMenu OverlayMenu { get => this.overlayMenu; }

        public Vector2 Position { get => this.position; set => this.position = value; }
        public Vector2 Size { get => this.size; set => this.size = value; }

        protected override float Margin { get => 0f; }

        // TODO: we can probably be smarter here...
		public override void Notify_ResolutionChanged()
		{
#if DEBUG
            Log.Message("Notify_ResolutionChanged");
#endif
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
                //TODO: what's going on here..
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

        public void GenerateOverlayOptions()
        {
            this.overlayOptions = new List<FloatMenuOption>()
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
        }

        // TODO: is this necessary or helpful? What would be best to go here?
		public override void PostOpen()
		{
            if (this.Size.x == 0)
            {
                this.Position = new Vector2(UI.screenWidth - defaultSize - defaultMargin, defaultMargin);
                this.Size = new Vector2(defaultSize, defaultSize);
            }
            /*if (resolutionX == 0)
            {
                resolutionX = UI.screenWidth;
                resolutionY = UI.screenHeight;
            }*/
#if DEBUG
            Log.Message($"PostOpen: {this.position} {this.size}");
#endif
            this.windowRect = new Rect(this.position, this.size);
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

            this.Position = this.windowRect.position;
            this.Size = this.windowRect.size;
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

        // NOTE: this is kind of nasty...
        public void ExposeData()
        {
#if DEBUG
            Log.Message($"ExposeData: {Scribe.mode}");
#endif
            Scribe_Values.Look<Vector2>(ref position, "positionY"); // fix this
            Scribe_Values.Look<Vector2>(ref size, "size");

            Scribe_Values.Look<int>(ref resolutionX, "resolutionX", UI.screenWidth, true);
            Scribe_Values.Look<int>(ref resolutionY, "resolutionY", UI.screenHeight, true);

            foreach (Overlay overlay in this.Overlays)
                if (overlay is IExposable)
                    ((IExposable)overlay).ExposeData();

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                Find.WindowStack.Add(this); // replaces default window
                this.GenerateOverlayOptions();
            }

            //if (Find.WindowStack.Windows.IndexOf(this) == -1)
        }

    }
}
