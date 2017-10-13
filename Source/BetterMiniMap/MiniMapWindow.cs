using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

using BetterMiniMap.Overlays;
using RimWorld;

namespace BetterMiniMap
{
	internal class MiniMapWindow : Window, IExposable
	{
        private const float minimumSize = 150f;
        private const float defaultSize = 300f;
        private const int defaultMargin = 8;
        private const float scrollWheelZoomRate = 0.015f;

        private Colonists_Overlay overlayColonists;
        private Fog_Overlay overlayFog;
        private Mining_Overlay overlayMining;
        private NonColonists_Overlay overlayNoncolonist;
        private Buildings_Overlay overlayBuilding;
        private PowerGrid_Overlay overlayPower; 
        private Terrain_Overlay overlayTerrain;
        private Viewpoint_Overlay overlayView;
        private Wildlife_Overlay overlayWild;
        private Ships_Overlay overlayShips;
        private Robots_Overlay overlayRobots;

        private List<Overlay> overlays;
        private List<Area_Overlay> areaOverlays = new List<Area_Overlay>();

        private Area_Overlay selectedArea;

        private int mapID = -1;

        private MiniMapControls controls;

        private Vector2 position;
        private Vector2 size;

        private Vector3 prevMousePos; 

        public bool resizing = false; // NOTE: could potential use resizable from base but sticking with this for now...
        private bool active = true; // NOTE: do not confuse with toggling (which is a temporary removal of the window)

        private float clampHeight;

        private Rect coords;

        public MiniMapWindow()
        {
            this.closeOnEscapeKey = false;
            this.preventCameraMotion = false;
            this.layer = WindowLayer.GameUI;

            this.controls = new MiniMapControls(this);
            this.GenerateOverlays();
            this.clampHeight = UI.screenHeight - MainButtonDef.ButtonHeight - this.controls.DefaultHeight;

            this.coords = new Rect(0f, 0f, 1f, 1f);
        }

        public List<Overlay> Overlays { get => this.overlays; }
        public List<Area_Overlay> AreaOverlays { get => this.areaOverlays; }
        public Vector2 Position { get => position; set => position = value; }
        public Vector2 Size { get => size; set => size = value; }
        public bool Active { get => this.active; set => this.active = value; }
        public Area_Overlay SelectedArea { get => selectedArea; set => selectedArea = value; }

        protected override float Margin { get => 0f; }

        public void GenerateOverlays()
        {
            this.overlayColonists = new Colonists_Overlay();
            this.overlayFog = new Fog_Overlay();
            this.overlayMining = new Mining_Overlay();
            this.overlayNoncolonist = new NonColonists_Overlay();
            this.overlayBuilding = new Buildings_Overlay();
            this.overlayPower = new PowerGrid_Overlay();
            this.overlayTerrain = new Terrain_Overlay();
            this.overlayView = new Viewpoint_Overlay();
            this.overlayWild = new Wildlife_Overlay();
            this.overlayShips = new Ships_Overlay();
            this.overlayRobots = new Robots_Overlay();

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

            controls.GenerateOverlayMenu();
        }

        public override void Notify_ResolutionChanged()
        {
            // TODO: utilize this method
            //this.SetInitialSizeAndPosition();
            this.clampHeight = UI.screenHeight - MainButtonDef.ButtonHeight - this.controls.DefaultHeight; 
        }

        public override void DoWindowContents(Rect inRect)
		{
            // NOTE: lazy load but do not see a good way to do redraw otherwise (yet)
            // NOTE: this could be part of a mapcomponent perhaps?
            if (Find.VisibleMap.uniqueID != this.mapID)
            {
                this.mapID = Find.VisibleMap.uniqueID;
                //this.GenerateOverlays();

                for (int i = 0; i < this.overlays.Count; i++)
                    this.overlays[i].GenerateTexture();

                this.Refresh();
            }

            this.Update();

			foreach (Overlay current in this.Overlays)
				if (current.Visible)
					GUI.DrawTextureWithTexCoords(inRect, current.Texture, this.coords);

			if (this.SelectedArea != null)
				GUI.DrawTextureWithTexCoords(inRect, this.SelectedArea.Texture, this.coords);

            if (Mouse.IsOver(inRect))
            {
                this.preventCameraMotion = true;
                if (this.resizing)
                {
                    //NOTE: why no use Input.mouseScrollDelta?
                    // NOTE: look at Event.current.type == EventType.MouseDown ... unsure why this doesn't seem to work right now.
                    if (Input.GetMouseButtonDown(0))
                        this.prevMousePos = Input.mousePosition;
                    //isResizing
                    if (Input.GetMouseButton(0))
                    {
                        float delta = this.prevMousePos.x - Input.mousePosition.x;
                        this.windowRect.x -= delta;
                        this.windowRect.height = this.windowRect.width = this.windowRect.width + delta;
                        this.prevMousePos = Input.mousePosition;
                    }
                }
                else if (!this.draggable)
                {
                    if (Input.GetMouseButton(0))
                    {
                        Vector2 mousePosition = Event.current.mousePosition;
                        Vector2 vector = new Vector2(mousePosition.x, inRect.height - mousePosition.y);
                        Vector2 vector2 = new Vector2((float)Find.VisibleMap.Size.x / inRect.width, (float)Find.VisibleMap.Size.z / inRect.height);
                        Find.CameraDriver.JumpToVisibleMapLoc(new Vector3(vector.x * vector2.x, 0f, vector.y * vector2.y));
                    }
                    if (Event.current.type == EventType.ScrollWheel)
                    {
                        // TODO: clean this section up
                        Vector2 mousePosition = Event.current.mousePosition;
                        Vector2 vector = new Vector2(mousePosition.x, inRect.height - mousePosition.y);

                        float rectDelta = 0f;
                        rectDelta += Event.current.delta.y * scrollWheelZoomRate;
                        this.coords.width += rectDelta;

                        if (this.coords.width < 0.1f)
                            this.coords.width = 0.1f;
                        if (this.coords.width > 1f)
                            this.coords.width = 1f;
                        this.coords.height = this.coords.width;

                        if (this.coords.width < 1f)
                        {
                            if (rectDelta < 0) // zooming in
                            {
                                this.coords.x -= rectDelta * (vector.x / this.windowRect.width);
                                this.coords.y -= rectDelta * (vector.y / this.windowRect.height);
                            }
                            else // zooming out
                            {
                                // TODO: what to call this?
                                float num = this.coords.x + this.coords.width;
                                if (num > 1)
                                    this.coords.x -= rectDelta;
                                else
                                    this.coords.x -= rectDelta * (num * 0.5f);
                                if (this.coords.x < 0)
                                    this.coords.x = 0;

                                num = this.coords.y + this.coords.height;
                                if (num > 1)
                                    this.coords.y -= rectDelta;
                                else
                                    this.coords.y -= rectDelta * (num * 0.5f);
                                if (this.coords.y < 0)
                                    this.coords.y = 0;
                            }
                        }
                        else
                        {
                            this.coords.x = this.coords.y = 0f;
                        }
                    }
                }
            }
            else
                this.preventCameraMotion = false;

        }

        public List<FloatMenuOption> GenerateOverlayMenuItems()
        {
            return new List<FloatMenuOption>()
            {
                new FloatMenuOptionItem(this.overlayColonists, "BMM_ColonistsOverlayLabel".Translate()),
                new FloatMenuOptionItem(this.overlayNoncolonist, "BMM_NoncolonistOverlayLabel".Translate()),
                new FloatMenuOptionItem(this.overlayWild, "BMM_WildlifeOverlayLabel".Translate()),
                new FloatMenuOptionItem(this.overlayBuilding, "BMM_BuildingsOverlayLabel".Translate()),
                new FloatMenuOptionItem(this.overlayPower, "BMM_PowerGridOverlayLabel".Translate()),
                new FloatMenuOptionItem(this.overlayMining, "BMM_MiningOverlayLabel".Translate()),
                new FloatMenuOptionItem(this.overlayTerrain, "BMM_TerrainOverlayLabel".Translate()),
                new FloatMenuOptionItem(this.overlayShips, "BMM_ShipsOverlayLabel".Translate()),
                new FloatMenuOptionItem(this.overlayRobots, "BMM_RobotsOverlayLabel".Translate())
            };

        }

        // TODO: is this necessary or helpful? What would be best to go here?
		public override void PostOpen()
		{
            base.PostOpen();
            if (this.Size.x == 0)
            {
                this.Position = new Vector2(UI.screenWidth - defaultSize - defaultMargin, defaultMargin);
                this.Size = new Vector2(defaultSize, defaultSize);
            }
#if DEBUG
            Log.Message($"PostOpen: {this.position} {this.size}");
#endif
            this.windowRect = new Rect(this.Position, this.Size);
            Find.WindowStack.Add(this.controls);
		}

        public override void PreClose()
        {
            base.PreClose();
            Find.WindowStack.TryRemove(this.controls, false);
        }

        public override void ExtraOnGUI() 
        {
            // ClampWindowToScreen
            if (this.windowRect.width < minimumSize)
                this.windowRect.height = this.windowRect.width = minimumSize;

            if (this.windowRect.xMax > UI.screenWidth)
                this.windowRect.x = UI.screenWidth - this.windowRect.width;

            if (this.windowRect.yMax > this.clampHeight)
                this.windowRect.y = this.clampHeight - this.windowRect.height;

            if (this.windowRect.xMin < 0f)
                this.windowRect.x = 0f; 

            if (this.windowRect.yMin < 0f)
                this.windowRect.y = 0f;

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

        public void ToggleControls()
        {
            if (Find.WindowStack.Windows.IndexOf(this.controls) == -1)
                Find.WindowStack.Add(this.controls);
            else
                Find.WindowStack.TryRemove(this.controls, true);
        }

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
            if (this.SelectedArea != null)
            {
                if (this.SelectedArea.ShouldUpdateOverlay)
                    this.SelectedArea.Update();
            }
        }

        public void Refresh()
        {
            if (this.overlays.Any<Overlay>())
            {
                foreach (Overlay current in this.overlays)
                    current.Update();
            }
            if (this.SelectedArea != null)
                this.SelectedArea.Update();
        }

        // NOTE: this is kind of nasty...
        public void ExposeData()
        {
#if DEBUG
            Log.Message($"ExposeData: {Scribe.mode}");
#endif
            Scribe_Values.Look<Vector2>(ref this.position, "position");
            Scribe_Values.Look<Vector2>(ref this.size, "size");

            Scribe_Values.Look<bool>(ref this.active, "active", true);

            // TODO: finish this.
            //Scribe_Values.Look<float>(ref this.coords, "coords");

            foreach (Overlay overlay in this.Overlays)
                if (overlay is IExposable)
                    ((IExposable)overlay).ExposeData();

            switch(Scribe.mode)
            {
                case LoadSaveMode.LoadingVars: this.LoadingVars(); break;
                case LoadSaveMode.PostLoadInit: this.PostLoadInit(); break;
                case LoadSaveMode.Saving: this.Saving(); break;
            }
        }

        private void LoadingVars()
        {
            Scribe_Values.Look<string>(ref MiniMapControls.selectedAreaLabel, "selectedAreaLabel");
        }


        private void PostLoadInit()
        {
            this.controls.GenerateOverlayMenu();
            // NOTE: there should be a cleaner way to do this...
            // Replace default window
            Find.WindowStack.Add(this);
            if (!this.active)
                Find.WindowStack.TryRemove(this, true);
        }

        private void Saving()
        {
            if (selectedArea != null)
            {
                string selectedAreaLabel = selectedArea.area.Label;
                Scribe_Values.Look<string>(ref selectedAreaLabel, "selectedAreaLabel");
            }
        }

    }
}
