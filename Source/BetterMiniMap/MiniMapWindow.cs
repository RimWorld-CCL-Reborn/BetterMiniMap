using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

using BetterMiniMap.Overlays;

// TODO: look at tiberium overlays to find better extensiblity

namespace BetterMiniMap
{
	public class MiniMapWindow : Window, IExposable
	{
        public const float defaultSize = 250f;
        public const int defaultMargin = 8;
        private const float minimumSize = 100f;
        private const float scrollWheelZoomRate = 0.015f;

        private readonly MiniMapControls controls;

        private OverlayManager overlayManager;
        private Map map;
        private Vector2 position;
        private Vector2 size;
        private Vector3 prevMousePos;

        public bool resizing = false;
        private bool active = true; // NOTE: do not confuse with toggling (which is a temporary removal of the window)

        private float clampHeight; //cached value

        private Rect coords;
        private string selectedAreaLabel = "";

        // hacky
        private bool initialized;
        public bool Initialized { get => this.initialized; }

        public MiniMapWindow()
        {
            this.closeOnCancel = false;
            this.preventCameraMotion = false;
            this.layer = WindowLayer.GameUI;

            this.controls = new MiniMapControls(this);
            this.clampHeight = UI.screenHeight - MainButtonDef.ButtonHeight - this.controls.DefaultHeight;

            this.coords = new Rect(0f, 0f, 1f, 1f);
        }

        public MiniMapWindow(Map map) : this()
        {
            this.map = map;
            this.overlayManager = new OverlayManager(map);
        }

        public Map Map { get => this.map; }

        public List<Overlay> Overlays { get => overlayManager.Overlays; }
        public AreaOverlay OverlayArea { get => overlayManager.AreaOverlay;  }
        public Vector2 Position { get => this.position; internal set => this.position = value; }
        public Vector2 Size { get => this.size; internal set => this.size = value; }

        protected override float Margin { get => 0f; }

        // TODO: return to float menus.
        // TODO: consider moving this all to overlay manager?
        public List<FloatMenuOption> GenerateOverlayMenuItems()
        {
            List<FloatMenuOption> overlayMenuItems = new List<FloatMenuOption>()
            {
                new FloatMenuCheckBox(overlayManager.BuildingOverlay, "BMM_BuildingsOverlayLabel".Translate()),
                new FloatMenuCheckBox(overlayManager.PowerOverlay, "BMM_PowerGridOverlayLabel".Translate()),
                new FloatMenuCheckBox(overlayManager.MiningOverlay, "BMM_MiningOverlayLabel".Translate()),
                new FloatMenuCheckBox(overlayManager.TerrainOverlay, "BMM_TerrainOverlayLabel".Translate()),
            };

            foreach (Overlay overlay in overlayManager.DefOverlays)
                overlayMenuItems.Add(new FloatMenuCheckBox(overlay, overlay?.def.label));

            return overlayMenuItems;

        }

        public override void Notify_ResolutionChanged()
        {
            // TODO: utilize this method
            //this.SetInitialSizeAndPosition();
            this.clampHeight = UI.screenHeight - MainButtonDef.ButtonHeight - this.controls.DefaultHeight; 
        }

        public override void DoWindowContents(Rect inRect)
		{
            this.Update();

			foreach (Overlay current in this.Overlays)
				if (current.Visible)
					GUI.DrawTextureWithTexCoords(inRect, current.Texture, this.coords);

			if (this.OverlayArea.area != null)
				GUI.DrawTextureWithTexCoords(inRect, this.OverlayArea.Texture, this.coords);

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
                    if (Input.GetMouseButton(0)) // click
                    {
                        // close tabs
                        if (Find.MainTabsRoot.OpenTab != null)
                            Find.MainTabsRoot.ToggleTab(null, false);

                        // set current map
                        if (this.map != Current.Game.CurrentMap)
                            Current.Game.CurrentMap = this.map;

                        Vector2 mousePosition = Event.current.mousePosition;
                        mousePosition = new Vector2(mousePosition.x, inRect.height - mousePosition.y);
                        float scaleFactor = (float)this.map.Size.x / inRect.width; // NOTE: this could be cached
                        Vector3 globalPos = new Vector3(mousePosition.x * scaleFactor, 0f, mousePosition.y * scaleFactor);

                        if (this.coords.width != 0 || this.coords.height != 0) // NOTE: redudant check here but `or` makes it okay bae
                        {
                            Vector3 zoomedPos = new Vector3((float)this.Map.Size.x * this.coords.x + globalPos.x * this.coords.width, 0, (float)this.Map.Size.z * this.coords.y + globalPos.z * this.coords.height);
                            Find.CameraDriver.JumpToCurrentMapLoc(zoomedPos);
                        }
                        else
                            Find.CameraDriver.JumpToCurrentMapLoc(globalPos);
                    } else if (Event.current.type == EventType.ScrollWheel)
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
                                if (this.coords.width != 0.1f) // if not maxZoom
                                {
                                    this.coords.x -= rectDelta * (vector.x / this.windowRect.width);
                                    this.coords.y -= rectDelta * (vector.y / this.windowRect.height);
                                }
                            }
                            else // zooming out
                            {
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
                    else
                        this.preventCameraMotion = false;

                }
            }
            else
                this.preventCameraMotion = false;
        }

        public override void PreOpen()
        {
            base.PreOpen();
            this.Refresh();
        }

        private void Refresh()
        {
            for (int i = 0; i < Overlays.Count; i++)
                Overlays[i].GenerateTexture();

            if (selectedAreaLabel != "")
            {
                List<Area> allAreas = Find.CurrentMap.areaManager.AllAreas;
                for (int i = 0; i < allAreas.Count; i++)
                {
                    if (allAreas[i].Label == selectedAreaLabel)
                    {
                        overlayManager.AreaOverlay.area = allAreas[i];
                        break;
                    }
                }
                selectedAreaLabel = "";
            }

            foreach (Overlay current in Overlays)
                current.Update();

            if (this.OverlayArea.area != null)
                this.OverlayArea.Update();
        }

		public override void PostOpen()
		{
            base.PostOpen();
#if DEBUG
            Log.Message($"PostOpen: {this.position} {this.size}");
#endif
            if (this.size.x == 0)
                this.size = new Vector2(defaultSize, defaultSize);
            this.windowRect = new Rect(this.Position, this.Size);
            Find.WindowStack.CustomAdd(this.controls);
            if (!this.Initialized)
                this.initialized = true;
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

            this.position = this.windowRect.position;
            this.Size = this.windowRect.size;
        }

        internal void SetLocality(MiniMapWindow miniMap)
        {
            this.position = this.windowRect.position = miniMap.position;
            this.Size = this.windowRect.size = miniMap.size;
        }

        public void Toggle(bool add = false)
        {
#if DEBUG
            Log.Message($"MiniMapWindow.Toggle: {add}");
#endif
            if (add)
            {
                if (Find.WindowStack.Windows.IndexOf(this) == -1) // keep from double add...
                    Find.WindowStack.Add(this);
            }
            else
                Find.WindowStack.TryRemove(this, true);
            this.active = add;
        }

        public void ToggleControls()
        {
            if (!this.active) return;
            if (Find.WindowStack.Windows.IndexOf(this.controls) == -1)
                Find.WindowStack.Add(this.controls);
            else
                Find.WindowStack.TryRemove(this.controls, true);
        }

        public void Update()
        {
            foreach (Overlay current in Overlays)
                if (current.ShouldUpdateOverlay)
                    current.Update();

            if (this.OverlayArea.ShouldUpdateOverlay)
                this.OverlayArea.Update();
        }

        // NOTE: this is kind of nasty...
        public void ExposeData()
        {
            Scribe_Values.Look<Vector2>(ref this.position, "position");
            Scribe_Values.Look<Vector2>(ref this.size, "size");

            Scribe_Values.Look<bool>(ref this.active, "active", true);

            Scribe_References.Look<Map>(ref this.map, "map");

            switch(Scribe.mode)
            {
                case LoadSaveMode.LoadingVars: this.LoadingVars(); break;
                case LoadSaveMode.PostLoadInit: this.PostLoadInit(); break;
                case LoadSaveMode.Saving: this.Saving(); break;
            }
        }

        private void LoadingVars()
        {
            Scribe_Values.Look<string>(ref this.selectedAreaLabel, "selectedAreaLabel");
            Vector2 coordsPosition = this.coords.position;
            Vector2 coordsSize = this.coords.size;
            Scribe_Values.Look<Vector2>(ref coordsPosition, "coordsPosition");
            Scribe_Values.Look<Vector2>(ref coordsSize, "coordsSize");
            this.coords.position = coordsPosition;
            this.coords.size = coordsSize;
            if (this.coords.width == 0 && this.coords.height == 0) // null -> default case
                this.coords.size = new Vector2(1f, 1f);
        }

        private void PostLoadInit()
        {
            if (!this.Initialized)
            {
                this.overlayManager = new OverlayManager(this.map);
                this.initialized = true;
            }
            this.ExposeOverlays();
        }

        private void Saving()
        {
            if (overlayManager.AreaOverlay.area != null)
            {
                string selectedAreaLabel = overlayManager.AreaOverlay.area.Label;
                Scribe_Values.Look<string>(ref selectedAreaLabel, "selectedAreaLabel");
            }

            Vector2 coordsPosition = this.coords.position;
            Vector2 coordsSize = this.coords.size; 
            Scribe_Values.Look<Vector2>(ref coordsPosition, "coordsPosition");
            Scribe_Values.Look<Vector2>(ref coordsSize, "coordsSize");

            this.ExposeOverlays();
        }

        private void ExposeOverlays()
        {
            foreach (Overlay overlay in this.Overlays)
                if (overlay is IExposable)
                    ((IExposable)overlay).ExposeData();
        }

    }
}