using BetterMiniMap.Overlays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace BetterMiniMap
{
    internal class FloatMenuRadioButton : FloatMenuOption
    {
        private const float horizontalMargin = 6f;
        private const float radioButtonSize = 30f; // 24f + 6f for padding

        private static readonly Color colorBackgroundActive = new ColorInt(21, 25, 29).ToColor;
        private static readonly Color colorBackgroundActiveMouseover = new ColorInt(29, 45, 50).ToColor;
        private static readonly Color colorBackgroundDisabled = new ColorInt(40, 40, 40).ToColor;
        private static readonly Color colorTextActive = Color.white;
        private static readonly Color colorTextDisabled = new Color(0.9f, 0.9f, 0.9f);

        protected bool enabled;

        public FloatMenuRadioButton(Area area, MiniMapWindow miniMap) : base($"{area.Label}", null, extraPartWidth: radioButtonSize)
        {
            this.enabled = miniMap.OverlayArea.area?.Label == area.Label;
            this.action = delegate
            {
                if (miniMap.OverlayArea.area?.Label == area.Label)
                    miniMap.OverlayArea.area = null;
                else
                    miniMap.OverlayArea.area = area;
                miniMap.OverlayArea.Update();
            };
        }

        public override bool DoGUI(Rect rect, bool colonistOrdering, FloatMenu floatMenu)
        {
            Text.Font = GameFont.Small;
            Rect rect2 = rect;

            rect2.xMin += horizontalMargin;
            rect2.xMax -= horizontalMargin;

            Color color = GUI.color;

            if (base.Disabled)
                GUI.color = FloatMenuRadioButton.colorBackgroundDisabled * color;
            else if (Mouse.IsOver(rect))
                GUI.color = FloatMenuRadioButton.colorBackgroundActiveMouseover * color;
            else
                GUI.color = FloatMenuRadioButton.colorBackgroundActive * color;

            GUI.DrawTexture(rect, BaseContent.WhiteTex);
            GUI.color = (base.Disabled ? FloatMenuRadioButton.colorTextDisabled : FloatMenuRadioButton.colorTextActive) * color;

            if (Widgets.RadioButtonLabeled(rect2, base.Label, this.enabled))
            {
                this.action?.Invoke();
                return true;
            }

            Widgets.DrawAtlas(rect, TexUI.FloatMenuOptionBG);

            //if (this.tutorTag != null)
            //	UIHighlighter.HighlightOpportunity(rect, this.tutorTag);

            if (!Widgets.ButtonInvisible(rect, false))
                return false;

            //if (this.tutorTag != null && !TutorSystem.AllowAction(this.tutorTag))
            //    return false;

            //base.Chosen(colonistOrdering);
            //if (this.tutorTag != null)
            //    TutorSystem.Notify_Event(this.tutorTag);

            return true;
        }
    }
}
