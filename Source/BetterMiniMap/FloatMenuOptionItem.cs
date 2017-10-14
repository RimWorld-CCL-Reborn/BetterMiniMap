using System;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.Sound;

using BetterMiniMap.Overlays;

namespace BetterMiniMap
{
	internal class FloatMenuOptionItem : FloatMenuOption
	{
		private const float colorBoxSize = 15f;
		private const float colorBoxMargin = 4f;
        private const float horizontalMargin = 6f;

		private static readonly Color colorBackgroundActive = new ColorInt(21, 25, 29).ToColor;
		private static readonly Color colorBackgroundActiveMouseover = new ColorInt(29, 45, 50).ToColor;
		private static readonly Color colorBackgroundDisabled = new ColorInt(40, 40, 40).ToColor;
		private static readonly Color colorTextActive = Color.white;
		private static readonly Color colorTextDisabled = new Color(0.9f, 0.9f, 0.9f);

		protected bool visible;

        public FloatMenuOptionItem(Area area, MiniMapWindow miniMap) : base($"XXXX{area.Label}", null)
        {
            this.visible = miniMap.OverlayArea.area?.Label == area.Label;
            Log.Message($"{area}");
            this.action = delegate
            {
                if (miniMap.OverlayArea.area?.Label == area.Label)
                    miniMap.OverlayArea.area = null;
                else
                    miniMap.OverlayArea.area = area;
                miniMap.OverlayArea.Update();
            };
        }

        public FloatMenuOptionItem(Overlay overlay, string label) : base($"XXXX{label}", null)
        {
            this.visible = overlay.Visible;
            this.action = () => this.visible = overlay.Visible = !overlay.Visible;
        }

		public override bool DoGUI(Rect rect, bool colonistOrdering)
		{
            bool mouseOver = !base.Disabled && Mouse.IsOver(rect);
            bool extraPartMouseOver = false;

			Text.Font = GameFont.Small;
            Rect rect2 = rect;

            rect2.xMin += horizontalMargin;
            rect2.xMax -= horizontalMargin;
            rect2.xMax -= colorBoxMargin;

			if (mouseOver)
				rect2.x = rect2.x + colorBoxMargin;

			if (!base.Disabled)
				MouseoverSounds.DoRegion(rect);

			Color color = GUI.color;

			if (base.Disabled)
				GUI.color = FloatMenuOptionItem.colorBackgroundDisabled * color;
			else if (mouseOver && !extraPartMouseOver)
                GUI.color = FloatMenuOptionItem.colorBackgroundActiveMouseover * color;
            else
                GUI.color = FloatMenuOptionItem.colorBackgroundActive * color;
			
			GUI.DrawTexture(rect, BaseContent.WhiteTex);
			GUI.color = (base.Disabled ? FloatMenuOptionItem.colorTextDisabled : FloatMenuOptionItem.colorTextActive) * color;

			Widgets.DrawAtlas(rect, TexUI.FloatMenuOptionBG);
			Text.Anchor = TextAnchor.MiddleLeft;
			Rect rect4 = rect2;
			rect4.x = rect.xMin + colorBoxMargin + colorBoxSize + colorBoxMargin;
			Widgets.Label(rect4, base.Label.Remove(0, 4));
			Text.Anchor = 0;
			GUI.color = color;
			Color backgroundColor = GUI.backgroundColor;
			GUIStyle gUIStyle = new GUIStyle(GUI.skin.box);
            gUIStyle.normal.background = Texture2D.whiteTexture;
			Rect rect5 = new Rect(rect.xMin + colorBoxMargin, rect.y + colorBoxMargin, colorBoxSize, colorBoxSize);

            GUI.backgroundColor = (this.visible) ? Color.green : Color.red;
			GUI.Box(rect5, "", gUIStyle);

			GUI.backgroundColor = backgroundColor;

			/*if (this.extraPartOnGUI != null)
			{
                GUI.color = color;
				if (this.extraPartOnGUI(extraPartRect))
					return true;
			}*/

			if (mouseOver)
                this.mouseoverGuiAction?.Invoke();

			//if (this.tutorTag != null)
			//	UIHighlighter.HighlightOpportunity(rect, this.tutorTag);

			if (!Widgets.ButtonInvisible(rect, false))
				return false;

            //if (this.tutorTag != null && !TutorSystem.AllowAction(this.tutorTag))
            //    return false;

            base.Chosen(colonistOrdering);
            //if (this.tutorTag != null)
            //    TutorSystem.Notify_Event(this.tutorTag);

            return true;
		}
	}
}
