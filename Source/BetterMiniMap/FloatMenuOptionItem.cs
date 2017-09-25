using System;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace BetterMiniMap
{
	internal class FloatMenuOptionItem : FloatMenuOption
	{
		private static readonly Color colorBackgroundActive = new ColorInt(21, 25, 29).ToColor;
		private static readonly Color colorBackgroundActiveMouseover = new ColorInt(29, 45, 50).ToColor;
		private static readonly Color colorBackgroundDisabled = new ColorInt(40, 40, 40).ToColor;
		private static readonly Color colorTextActive = Color.white;
		private static readonly Color colorTextDisabled = new Color(0.9f, 0.9f, 0.9f);

		private float colorBoxSize;
		private float colorBoxMargin;

		private bool visible;

        private GameFont CurrentFont { get => GameFont.Small; }

        private float HorizontalMargin { get => 6f; }

        private float VerticalMargin { get => 4f; }

        public FloatMenuOptionItem(bool visible, string label, Action action) : base($"XXXX{label}", action)
        {
            this.colorBoxSize = 15f;
            this.colorBoxMargin = 4f;
            this.visible = visible;
        }

        private void SetRequiredWidth(float width)
		{
			FieldInfo[] fields = typeof(FloatMenuOption).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetField);
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				if (fieldInfo.Name == "cachedRequiredWidth")
				{
					fieldInfo.SetValue(this, width);
				}
			}
		}

		public override bool DoGUI(Rect rect, bool colonistOrdering)
		{
            bool mouseOver = !base.Disabled && Mouse.IsOver(rect);
            bool extraPartMouseOver = false;

			Text.Font = this.CurrentFont;
			Rect rect2 = rect;

            rect2.xMin += this.HorizontalMargin;
            rect2.xMax -= this.HorizontalMargin;
            rect2.xMax -= this.colorBoxMargin;
            //rect2.xMax -= this.extraPartWidth;

			if (mouseOver)
				rect2.x = rect2.x + this.colorBoxMargin ;

			/*Rect extraPartRect = default(Rect);
			if (this.extraPartWidth != 0f)
			{
				float num = Mathf.Min(Text.CalcSize(base.Label).x, rect2.width - this.colorBoxMargin);
				extraPartRect = new Rect(rect2.xMin + num, rect2.yMin, this.extraPartWidth, this.colorBoxSize * 2f);
				extraPartMouseOver = Mouse.IsOver(extraPartRect);
			}*/

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
			rect4.x = rect.xMin + this.colorBoxMargin + this.colorBoxSize + this.colorBoxMargin;
			Widgets.Label(rect4, base.Label.Remove(0, 4));
			Text.Anchor = 0;
			GUI.color = color;
			Color backgroundColor = GUI.backgroundColor;
			GUIStyle gUIStyle = new GUIStyle(GUI.skin.box);
            gUIStyle.normal.background = Texture2D.whiteTexture;
			Rect rect5 = new Rect(rect.xMin + this.colorBoxMargin, rect.y + this.colorBoxMargin, this.colorBoxSize, this.colorBoxSize);

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
