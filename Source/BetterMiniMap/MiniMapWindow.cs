using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

using BetterMiniMap.Overlays;

// TODO: look at tiberium overlays to find better extensiblity

namespace BetterMiniMap
{
	internal class MiniMapWindow : Window, IExposable
	{
        private const float minimumSize = 150f;
        private const float defaultSize = 300f;
        private const int defaultMargin = 8;
        private const float scrollWheelZoomRate = 0.015f;

        private readonly List<Overlay> overlays;

        private readonly OverlayManager overlayManager;

        // TODO: readonly
        private Map map;
        public Map Map { get => this.map; }

        private int tileHash = 0;

        private readonly MiniMapControls controls;

        private Vector2 position;
        private Vector2 size;

        private Vector3 prevMousePos;

        public bool resizing = false;
        private bool active = true; // NOTE: do not confuse with toggling (which is a temporary removal of the window)

        private float clampHeight;

        private Rect coords;
        private string selectedAreaLabel = "";

        // TODO: set map

        public MiniMapWindow(Map map)
        {
            this.closeOnCancel = false;
            this.preventCameraMotion = false;
            //this.doCloseX = true;
            this.layer = WindowLayer.GameUI;

            this.overlayManager = new OverlayManager();

            this.controls = new MiniMapControls(this);
            this.overlays = overlayManager.Overlays;
            this.clampHeight = UI.screenHeight - MainButtonDef.ButtonHeight - this.controls.DefaultHeight;

            this.coords = new Rect(0f, 0f, 1f, 1f);
        }

        public List<Overlay> Overlays { get => this.overlays; }
        public Vector2 Position { get => position; set => position = value; }
        public Vector2 Size { get => size; set => size = value; }
        public bool Active { get => this.active; set => this.active = value; }
        public AreaOverlay OverlayArea { get => overlayManager.AreaOverlay;  }

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
            // NOTE: lazy load but do not see a good way to do redraw otherwise (yet)
            // NOTE: this could be part of a mapcomponent perhaps?
            if (Find.CurrentMap.TileInfo.GetHashCode() != this.tileHash)
            {
#if DEBUG
                Log.Message($"MiniMapWindow.DoWindowContents -> regenerating {Find.CurrentMap.Size.x} {Find.CurrentMap.Size.z}");
#endif
                this.tileHash = Find.CurrentMap.TileInfo.GetHashCode();

                for (int i = 0; i < this.overlays.Count; i++)
                    this.overlays[i].GenerateTexture();

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

                this.Refresh();
            }

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
                    if (Input.GetMouseButton(0))
                    {
                        Vector2 mousePosition = Event.current.mousePosition;
                        mousePosition = new Vector2(mousePosition.x, inRect.height - mousePosition.y);
                        float scaleFactor = (float)Find.CurrentMap.Size.x / inRect.width; // NOTE: this could be cached
                        Vector3 globalPos = new Vector3(mousePosition.x * scaleFactor, 0f, mousePosition.y * scaleFactor);

                        if (this.coords.width != 0 || this.coords.height != 0) // NOTE: redudant check here but `or` makes it okay bae
                        {
                            Vector3 zoomedPos = new Vector3((float)Find.CurrentMap.Size.x * this.coords.x + globalPos.x * this.coords.width, 0, (float)Find.CurrentMap.Size.z * this.coords.y + globalPos.z * this.coords.height);
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
            foreach (Overlay current in this.overlays)
                if (current.ShouldUpdateOverlay)
                    current.Update();

            if (this.OverlayArea.ShouldUpdateOverlay)
                this.OverlayArea.Update();
        }

        public void Refresh() // initialize
        {
            foreach (Overlay current in this.overlays)
                current.Update();

            if (this.OverlayArea.area != null)
                this.OverlayArea.Update();
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

        // TODO: fix this... it's nasty.
        private void PostLoadInit()
        {
            // NOTE: there should be a cleaner way to do this...
            // Replace default window
            Find.WindowStack.Add(this);
            if (!this.active)
                Find.WindowStack.TryRemove(this, true);
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
        }

    }
}