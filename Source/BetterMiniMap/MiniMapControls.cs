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
		private const float buttonMargin = 6f;

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
            this.layer = WindowLayer.GameUI;

            float xPos = buttonMargin;
            this.homeButtonRect = new Rect(buttonMargin, buttonMargin, buttonWidth, buttonWidth);
            this.configButtonRect = new Rect(0 , buttonMargin, buttonWidth, buttonWidth);
            this.resizeButtonRect = new Rect(0, buttonMargin, buttonWidth, buttonWidth);
            this.dragButtonRect = new Rect(0, buttonMargin, buttonWidth, buttonWidth);

            float xDiff = buttonWidth + 2f * buttonMargin;

            this.configButtonRect.x = xPos += xDiff;
            this.resizeButtonRect.x = xPos += xDiff;
            this.dragButtonRect.x = xPos += xDiff;

            this.SetLocality();
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
            float width = this.dragButtonRect.x + buttonWidth + buttonMargin;
            this.windowRect = new Rect(miniMap.windowRect.x + miniMap.windowRect.width - width, miniMap.windowRect.y + miniMap.windowRect.height, width, this.DefaultHeight);
        }

        public float DefaultHeight => buttonWidth + 2f * buttonMargin;

        public override void PostOpen()
        {
            base.PostOpen();
            this.SetLocality();
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (miniMap.draggable || miniMap.resizing)
                this.SetLocality();

            Widgets.DrawHighlightIfMouseover(this.homeButtonRect);
            TooltipHandler.TipRegion(this.homeButtonRect, "BMM_HomeButtonTooltip".Translate());
            if (Widgets.ButtonImage(this.homeButtonRect, MiniMapTextures.homeA))
            {
                if (Event.current.button == 0 || Event.current.button == 1) // left/right click
                    Find.WindowStack.Add(this.UpdateAreaOverlays());
            }

            Widgets.DrawHighlightIfMouseover(this.configButtonRect);
            TooltipHandler.TipRegion(this.configButtonRect, "BMM_ConfigButtonTooltip".Translate());
            if (Widgets.ButtonImage(this.configButtonRect, MiniMapTextures.config))
            {
                if (Event.current.button == 0 || Event.current.button == 1) // left/right click
                    Find.WindowStack.Add(this.overlayMenu);
            }

            Widgets.DrawHighlightIfMouseover(this.resizeButtonRect);
            TooltipHandler.TipRegion(this.resizeButtonRect, "BMM_ResizeButtonTooltip".Translate());
            if (Widgets.ButtonImage(this.resizeButtonRect, miniMap.resizing ? MiniMapTextures.resizeA : MiniMapTextures.resizeD))
            {
                if (Event.current.button == 0 || Event.current.button == 1) // left/right click
                {
                    miniMap.resizing = !miniMap.resizing;
                    if (!miniMap.resizing) // TODO: is this needed?
                        miniMap.Size = miniMap.windowRect.size;
                    miniMap.draggable = false;
                }
            }

            Widgets.DrawHighlightIfMouseover(this.dragButtonRect);
            TooltipHandler.TipRegion(this.dragButtonRect, "BMM_DragButtonTooltip".Translate());
            if (Widgets.ButtonImage(this.dragButtonRect, miniMap.draggable ? MiniMapTextures.dragA : MiniMapTextures.dragD))
            {
                if (Event.current.button == 0 || Event.current.button == 1) // left/right click
                {
                    miniMap.draggable = !miniMap.draggable;
                    if (!miniMap.draggable) // TODO: is this needed?
                        miniMap.Position = miniMap.windowRect.position;
                    miniMap.resizing = false;
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
