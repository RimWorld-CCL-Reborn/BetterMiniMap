using BetterMiniMap.Overlays;
using UnityEngine;
using Verse;

namespace BetterMiniMap
{
    internal class FloatMenuCheckBox : FloatMenuOption
    {
        private const float horizontalMargin = 6f;
        private const float checkSize = 30f; // 24f + 6f for padding

        private static readonly Color colorBackgroundActive = new ColorInt(21, 25, 29).ToColor;
        private static readonly Color colorBackgroundActiveMouseover = new ColorInt(29, 45, 50).ToColor;
        private static readonly Color colorBackgroundDisabled = new ColorInt(40, 40, 40).ToColor;
        private static readonly Color colorTextActive = Color.white;
        private static readonly Color colorTextDisabled = new Color(0.9f, 0.9f, 0.9f);

        protected bool visible;

        public FloatMenuCheckBox(Overlay overlay, string label) : base($"{label}", null, extraPartWidth: checkSize)
        {
            this.visible = overlay.Visible;
            this.action = () => this.visible = overlay.Visible = !overlay.Visible;
        }

        public override bool DoGUI(Rect rect, bool colonistOrdering, FloatMenu floatMenu)
        {
            Text.Font = GameFont.Small;
            Rect rect2 = rect;

            rect2.xMin += horizontalMargin;
            rect2.xMax -= horizontalMargin;

            Color color = GUI.color;

            if (base.Disabled)
                GUI.color = FloatMenuCheckBox.colorBackgroundDisabled * color;
            else if (Mouse.IsOver(rect))
                GUI.color = FloatMenuCheckBox.colorBackgroundActiveMouseover * color;
            else
                GUI.color = FloatMenuCheckBox.colorBackgroundActive * color;

            GUI.DrawTexture(rect, BaseContent.WhiteTex);
            GUI.color = (base.Disabled ? FloatMenuCheckBox.colorTextDisabled : FloatMenuCheckBox.colorTextActive) * color;

            bool prevValue = this.visible;
            Widgets.CheckboxLabeled(rect2, base.Label, ref this.visible);
            if (prevValue != this.visible)
                this.action?.Invoke();

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
