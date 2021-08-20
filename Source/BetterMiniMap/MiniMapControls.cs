using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Verse;

using BetterMiniMap.Overlays;

namespace BetterMiniMap
{
    internal class MiniMapControls : Window
    {
		public const float buttonWidth = 18f;
		private const float buttonMargin = 4f;

        private Rect configButtonRect;
        private Rect dragButtonRect;
        private Rect homeButtonRect;
        private Rect resizeButtonRect;
        private Rect miniMapManagerRect;
        private Rect closeButtonRect;

        private MiniMapWindow miniMap;

        private List<FloatMenuOption> areasOptions;

        private FloatMenu overlayMenu;
        
        public MiniMapControls(MiniMapWindow miniMap)
        {
            this.miniMap = miniMap;

            this.closeOnCancel = false;
            this.preventCameraMotion = false;
            this.layer = WindowLayer.GameUI;

            float xPos = buttonMargin;
            this.homeButtonRect = new Rect(buttonMargin, buttonMargin, buttonWidth, buttonWidth);
            this.configButtonRect = new Rect(0 , buttonMargin, buttonWidth, buttonWidth);
            this.resizeButtonRect = new Rect(0, buttonMargin, buttonWidth, buttonWidth);
            this.dragButtonRect = new Rect(0, buttonMargin, buttonWidth, buttonWidth);
            this.miniMapManagerRect = new Rect(0, buttonMargin, buttonWidth, buttonWidth);
            this.closeButtonRect = new Rect(0, buttonMargin, buttonWidth, buttonWidth);

            float xDiff = buttonWidth + 2f * buttonMargin;

            this.configButtonRect.x = xPos += xDiff;
            this.resizeButtonRect.x = xPos += xDiff;
            this.dragButtonRect.x = xPos += xDiff;
            this.miniMapManagerRect.x = xPos += xDiff;
            this.closeButtonRect.x = xPos += xDiff;
        }

        protected override float Margin { get => 0f; }

        public void SetLocality()
        {
            float width = 0; 
            if (BetterMiniMapMod.modSettings.singleMode)
                width = this.dragButtonRect.x + buttonWidth + buttonMargin;
            else
                width = this.closeButtonRect.x + buttonWidth + buttonMargin;
            this.windowRect = new Rect(miniMap.windowRect.x + miniMap.windowRect.width - width, miniMap.windowRect.y + miniMap.windowRect.height, width, this.DefaultHeight);
        }

        public float DefaultHeight => buttonWidth + 2f * buttonMargin;

        public FloatMenu OverlayMenu
        {
            get
            {
                if (this.overlayMenu == null)
                {
                    this.overlayMenu = new FloatMenu(miniMap.GenerateOverlayMenuItems())
                    {
                        closeOnCancel = false,
                        preventCameraMotion = false,
                    };
                }
                return this.overlayMenu;
            }
        }

        public override void PostOpen()
        {
            base.PostOpen();
            this.SetLocality();
        }

        public override void Notify_ResolutionChanged() => this.SetLocality();

        public override void DoWindowContents(Rect inRect)
        {
            if (miniMap.draggable || miniMap.resizing)
                this.SetLocality();

            Widgets.DrawHighlightIfMouseover(this.homeButtonRect);
            TooltipHandler.TipRegion(this.homeButtonRect, "BMM_HomeButtonTooltip".Translate());
            if (Widgets.ButtonImage(this.homeButtonRect, MiniMapTextures.HomeA))
            {
                if (Event.current.button == 0 || Event.current.button == 1) // left/right click
                    Find.WindowStack.Add(this.GetAreaOptions());
            }

            Widgets.DrawHighlightIfMouseover(this.configButtonRect);
            TooltipHandler.TipRegion(this.configButtonRect, "BMM_ConfigButtonTooltip".Translate());
            if (Widgets.ButtonImage(this.configButtonRect, MiniMapTextures.Config))
            {
                if (Event.current.button == 0 || Event.current.button == 1) // left/right click
                    Find.WindowStack.Add(this.OverlayMenu);
            }

            Widgets.DrawHighlightIfMouseover(this.resizeButtonRect);
            TooltipHandler.TipRegion(this.resizeButtonRect, "BMM_ResizeButtonTooltip".Translate());
            if (Widgets.ButtonImage(this.resizeButtonRect, miniMap.resizing ? MiniMapTextures.ResizeA : MiniMapTextures.ResizeD))
            {
                if (Event.current.button == 0 || Event.current.button == 1) // left/right click
                {
                    miniMap.resizing = !miniMap.resizing;
                    miniMap.draggable = false;
                }
            }

            Widgets.DrawHighlightIfMouseover(this.dragButtonRect);
            TooltipHandler.TipRegion(this.dragButtonRect, "BMM_DragButtonTooltip".Translate());
            if (Widgets.ButtonImage(this.dragButtonRect, miniMap.draggable ? MiniMapTextures.DragA : MiniMapTextures.DragD))
            {
                if (Event.current.button == 0 || Event.current.button == 1) // left/right click
                {
                    miniMap.draggable = !miniMap.draggable;
                    miniMap.resizing = false;
                }
            }

            if (Widgets.ButtonImage(this.miniMapManagerRect, MiniMapTextures.MapManagerIcon))
            {
                if (Event.current.button == 0 || Event.current.button == 1) // left/right click
                    Find.WindowStack.Add(MiniMap_GameComponentHelper.MiniMapMenu);
            }

            if (Widgets.ButtonImage(this.closeButtonRect, MiniMapTextures.CloseXSmall))
            {
                if (Event.current.button == 0 || Event.current.button == 1) // left/right click
                    this.miniMap.Close();
            }
        }

        private FloatMenu GetAreaOptions()
        {
            this.areasOptions = new List<FloatMenuOption>();

            List<Area> allAreas = this.miniMap.Map.areaManager.AllAreas;

            for (int i=0; i < allAreas.Count; i++)
                this.areasOptions.Add(new FloatMenuRadioButton(allAreas[i], this.miniMap));

            return new FloatMenu(this.areasOptions)
            {
                closeOnCancel = false,
                preventCameraMotion = false,
            };
        }

    }
}
