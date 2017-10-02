using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Verse;

using BetterMiniMap.Overlays;

namespace BetterMiniMap
{
    internal class MiniMapControls : Window
    {
		public const float buttonWidth = 20f;
		private const float buttonMargin = 15f;

        private Rect configButtonRect;
        private Rect dragButtonRect;
        private Rect homeButtonRect;
        private Rect resizeButtonRect;

        private MiniMapWindow miniMap;

        private List<FloatMenuOption> areasOptions;

        private FloatMenu overlayMenu;
		private FloatMenu areaMenu;
        
        // TODO: there should be a better way to handle this...
        public static string selectedAreaLabel = ""; // used to initialize selectedArea

        public MiniMapControls(MiniMapWindow miniMap)
        {
            this.miniMap = miniMap;

            this.closeOnEscapeKey = false;
            this.preventCameraMotion = false;

            this.configButtonRect = new Rect(0, 0, buttonWidth, buttonWidth);
            this.dragButtonRect = new Rect(0, 0, buttonWidth, buttonWidth);
            this.homeButtonRect = new Rect(0, 0, buttonWidth, buttonWidth);
            this.resizeButtonRect = new Rect(0, 0, buttonWidth, buttonWidth);
        }

        public void GenerateOverlayMenu()
        {
            this.overlayMenu = new FloatMenu(miniMap.GenerateOverlayMenuItems())
            {
                closeOnEscapeKey = true,
                preventCameraMotion = false,
            };
        }

        protected override float Margin { get => 0f; }

        public void SetLocality()
        {
            float yPos = miniMap.windowRect.y + miniMap.windowRect.height + 1f; // WHY: +1?
            this.configButtonRect.y = this.dragButtonRect.y = this.homeButtonRect.y = this.resizeButtonRect.y = yPos;

            float xDiff = buttonWidth + buttonMargin;
            float xPos = miniMap.windowRect.x + miniMap.windowRect.width - xDiff;
            this.configButtonRect.x = xPos;
            this.dragButtonRect.x = xPos -= xDiff;
            this.homeButtonRect.x = xPos -= xDiff;
            this.resizeButtonRect.x = xPos -= xDiff;

            this.windowRect = new Rect(miniMap.windowRect.x, miniMap.windowRect.y + miniMap.windowRect.height, miniMap.windowRect.width, buttonWidth + 10f);
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (miniMap.draggable || miniMap.resizing)
                this.SetLocality();

            Listing_Standard listing_Standard = new Listing_Standard() { ColumnWidth = inRect.width / 4f };
            listing_Standard.Begin(inRect);

            if (listing_Standard.ButtonImage(MiniMapTextures.config, buttonWidth, buttonWidth))
            {
                if (Event.current.button == 1) // right click
                    Find.WindowStack.Add(this.overlayMenu);
            }

            listing_Standard.NewColumn();

            if (listing_Standard.ButtonImage(miniMap.draggable ? MiniMapTextures.dragA : MiniMapTextures.dragD, buttonWidth, buttonWidth))
            {
                if (Event.current.button == 1) // right click
                {
                    miniMap.draggable = !miniMap.draggable;
                    if (!miniMap.draggable)
                        miniMap.Position = miniMap.windowRect.position;
                    miniMap.resizing = false;
                }
            }

            listing_Standard.End();
        }

        public override void PostOpen()
        {
            base.PostOpen();
            this.windowRect = new Rect(miniMap.windowRect.x, miniMap.windowRect.y + miniMap.windowRect.height, miniMap.windowRect.width, buttonWidth + 10f);
        }

        public void DoOverlayButtons()
        {
            if (miniMap.draggable || miniMap.resizing)
                this.SetLocality();

            if (Widgets.ButtonImage(this.configButtonRect, MiniMapTextures.config))
            {
                if (Event.current.button == 1) // right click
                    Find.WindowStack.Add(this.overlayMenu);
            }

            if (Widgets.ButtonImage(this.dragButtonRect, miniMap.draggable ? MiniMapTextures.dragA : MiniMapTextures.dragD))
            {
                if (Event.current.button == 1) // right click
                {
                    miniMap.draggable = !miniMap.draggable;
                    if (!miniMap.draggable)
                        miniMap.Position = miniMap.windowRect.position;
                    miniMap.resizing = false;
                }
            }

            if (Widgets.ButtonImage(this.homeButtonRect, MiniMapTextures.homeA))
            {
                if (Event.current.button == 1) // right click
                    Find.WindowStack.Add(this.UpdateAreaOverlays());
            }

            if (Widgets.ButtonImage(this.resizeButtonRect, miniMap.resizing ? MiniMapTextures.resizeA : MiniMapTextures.resizeD))
            {
                if (Event.current.button == 1) // right click
                {
                    miniMap.resizing = !miniMap.resizing;
                    if (!miniMap.resizing)
                        miniMap.Size = miniMap.windowRect.size;
                    miniMap.draggable = false;
                }
            }
        }

        public FloatMenu UpdateAreaOverlays()
        {
            bool remakeOptions = false;
            // TODO: is this a good check?
            if (Find.VisibleMap.areaManager.AllAreas.Count != miniMap.AreaOverlays.Count)
            {
                foreach (Area area in Find.VisibleMap.areaManager.AllAreas)
                {
                    // TODO: this seems expensive...
                    if (!miniMap.AreaOverlays.Any((Area_Overlay w) => w.area == area))
                    {
                        miniMap.AreaOverlays.Add(new Area_Overlay(area, false));
                        remakeOptions = true;
                    }
                }

                if (miniMap.AreaOverlays.Any<Area_Overlay>())
                {
                    for (int i = miniMap.AreaOverlays.Count - 1; i >= 0; i--)
                    {
                        Area area = miniMap.AreaOverlays[i].area;
                        if (area == null || !Find.VisibleMap.areaManager.AllAreas.Contains(area))
                        {
                            miniMap.AreaOverlays.RemoveAt(i);
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
                foreach (Area_Overlay areaOverlay in miniMap.AreaOverlays)
                {
                    if (areaOverlay.area.Label == selectedAreaLabel)
                    {
                        miniMap.SelectedArea = areaOverlay;
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
            if (miniMap.AreaOverlays?.Count > 0)
            {
                this.areasOptions = new List<FloatMenuOption>();

                foreach (Area_Overlay overlayArea in miniMap.AreaOverlays)
                {
                    this.areasOptions.Add(new FloatMenuOptionItem(overlayArea.Visible, overlayArea.area.Label, delegate
                    {
                        overlayArea.Visible = !overlayArea.Visible;
                        if (miniMap.SelectedArea == null)
                            miniMap.SelectedArea = overlayArea;
                        else if (miniMap.SelectedArea == overlayArea)
                            miniMap.SelectedArea = null;
                        else
                        {
                            miniMap.SelectedArea.Visible = false;
                            miniMap.SelectedArea = overlayArea;
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
