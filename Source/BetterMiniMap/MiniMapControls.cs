using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Verse;

using BetterMiniMap.Overlays;

namespace BetterMiniMap
{
    internal class MiniMapControls
    {
		public const float buttonWidth = 20f;
		private const float buttonMargin = 15f;

        private Rect configButtonRect;
        private Rect dragButtonRect;
        private Rect homeButtonRect;
        private Rect resizeButtonRect;

        private MiniMapWindow window;

        private List<FloatMenuOption> areasOptions;

        private FloatMenu overlayMenu;
		private FloatMenu areaMenu;
        
        // TODO: there should be a better way to handle this...
        public static string selectedAreaLabel = ""; // used to initialize selectedArea

        public MiniMapControls(MiniMapWindow miniMap)
        {
            this.window = miniMap;
            this.configButtonRect = new Rect(0, 0, buttonWidth, buttonWidth);
            this.dragButtonRect = new Rect(0, 0, buttonWidth, buttonWidth);
            this.homeButtonRect = new Rect(0, 0, buttonWidth, buttonWidth);
            this.resizeButtonRect = new Rect(0, 0, buttonWidth, buttonWidth);

            this.GenerateOverlayMenu();
        }

        public void GenerateOverlayMenu()
        {
            this.overlayMenu = new FloatMenu(window.GenerateOverlayMenuItems())
            {
                closeOnEscapeKey = true,
                preventCameraMotion = false,
            };
        }

        public void SetLocality()
        {
            float yPos = window.windowRect.y + window.windowRect.height + 1f; // WHY: +1?
            this.configButtonRect.y = this.dragButtonRect.y = this.homeButtonRect.y = this.resizeButtonRect.y = yPos;

            float xDiff = buttonWidth + buttonMargin;
            float xPos = window.windowRect.x + window.windowRect.width - xDiff;
            this.configButtonRect.x = xPos;
            this.dragButtonRect.x = xPos -= xDiff;
            this.homeButtonRect.x = xPos -= xDiff;
            this.resizeButtonRect.x = xPos -= xDiff;
        }

        public void DoOverlayButtons()
        {
            if (window.draggable || window.resizing)
                this.SetLocality();

            if (Widgets.ButtonImage(this.configButtonRect, MiniMapTextures.config))
            {
                if (Event.current.button == 1) // right click
                    Find.WindowStack.Add(this.overlayMenu);
            }

            if (Widgets.ButtonImage(this.dragButtonRect, window.draggable ? MiniMapTextures.dragA : MiniMapTextures.dragD))
            {
                if (Event.current.button == 1) // right click
                {
                    window.draggable = !window.draggable;
                    if (!window.draggable)
                        window.Position = window.windowRect.position;
                    window.resizing = false;
                }
            }

            if (Widgets.ButtonImage(this.homeButtonRect, MiniMapTextures.homeA))
            {
                if (Event.current.button == 1) // right click
                    Find.WindowStack.Add(this.UpdateAreaOverlays());
            }

            if (Widgets.ButtonImage(this.resizeButtonRect, window.resizing ? MiniMapTextures.resizeA : MiniMapTextures.resizeD))
            {
                if (Event.current.button == 1) // right click
                {
                    window.resizing = !window.resizing;
                    if (!window.resizing)
                        window.Size = window.windowRect.size;
                    window.draggable = false;
                }
            }
        }

        public FloatMenu UpdateAreaOverlays()
        {
            bool remakeOptions = false;
            // TODO: is this a good check?
            if (Find.VisibleMap.areaManager.AllAreas.Count != window.AreaOverlays.Count)
            {
                foreach (Area area in Find.VisibleMap.areaManager.AllAreas)
                {
                    // TODO: this seems expensive...
                    if (!window.AreaOverlays.Any((Area_Overlay w) => w.area == area))
                    {
                        window.AreaOverlays.Add(new Area_Overlay(area, false));
                        remakeOptions = true;
                    }
                }

                if (window.AreaOverlays.Any<Area_Overlay>())
                {
                    for (int i = window.AreaOverlays.Count - 1; i >= 0; i--)
                    {
                        Area area = window.AreaOverlays[i].area;
                        if (area == null || !Find.VisibleMap.areaManager.AllAreas.Contains(area))
                        {
                            window.AreaOverlays.RemoveAt(i);
                            remakeOptions = true;
                        }
                    }
                }

                if (remakeOptions)
                    this.MakeAreaOptions();
            }

            // NOTE: this only happens on load
            if (selectedAreaLabel != "")
            {
                foreach (Area_Overlay areaOverlay in window.AreaOverlays)
                {
                    if (areaOverlay.area.Label == selectedAreaLabel)
                    {
                        window.SelectedArea = areaOverlay;
                        areaOverlay.Visible = true;
                        break;
                    }
                }
                selectedAreaLabel = "";
            }

            return this.areaMenu;
        }

        private void MakeAreaOptions()
        {
            if (window.AreaOverlays?.Count > 0)
            {
                this.areasOptions = new List<FloatMenuOption>();

                foreach (Area_Overlay overlayArea in window.AreaOverlays)
                {
                    this.areasOptions.Add(new FloatMenuOptionItem(overlayArea.Visible, overlayArea.area.Label, delegate
                    {
                        overlayArea.Visible = !overlayArea.Visible;
                        if (window.SelectedArea == null)
                            window.SelectedArea = overlayArea;
                        else if (window.SelectedArea == overlayArea)
                            window.SelectedArea = null;
                        else
                        {
                            window.SelectedArea.Visible = false;
                            window.SelectedArea = overlayArea;
                        }
                        // TODO: fix this...
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
    }
}
