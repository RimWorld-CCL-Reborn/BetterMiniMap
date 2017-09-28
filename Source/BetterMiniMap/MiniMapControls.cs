using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

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

        public MiniMapControls(MiniMapWindow miniMap)
        {
            this.window = miniMap;
            this.configButtonRect = new Rect(0, 0, buttonWidth, buttonWidth);
            this.dragButtonRect = new Rect(0, 0, buttonWidth, buttonWidth);
            this.homeButtonRect = new Rect(0, 0, buttonWidth, buttonWidth);
            this.resizeButtonRect = new Rect(0, 0, buttonWidth, buttonWidth);
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

        public void DrawOverlayButtons()
        {
            if (window.draggable || window.resize)
                this.SetLocality();

            if (Widgets.ButtonImage(this.configButtonRect, MiniMapTextures.config))
            {
                if (Event.current.button == 1) // right click
                    Find.WindowStack.Add(window.OverlayMenu);
            }

            if (Widgets.ButtonImage(this.dragButtonRect, window.draggable ? MiniMapTextures.dragA : MiniMapTextures.dragD))
            {
                if (Event.current.button == 1) // right click
                {
                    window.draggable = !window.draggable;
                    if (!window.draggable)
                        MiniMap_GameComponent.Position = window.windowRect.position;
                    window.resize = false;
                }
            }

            if (Widgets.ButtonImage(this.homeButtonRect, MiniMapTextures.homeA))
            {
                if (Event.current.button == 1) // right click
                {
                   
                    Find.WindowStack.Add( window.UpdateAreaOverlays() );
                }
            }

            if (Widgets.ButtonImage(this.resizeButtonRect, window.resize ? MiniMapTextures.resizeA : MiniMapTextures.resizeD))
            {
                if (Event.current.button == 1) // right click
                {
                    window.resize = !window.resize;
                    if (!window.resize)
                        MiniMap_GameComponent.Size = window.windowRect.size;
                    window.draggable = false;
                }
            }
        }
    }
}
