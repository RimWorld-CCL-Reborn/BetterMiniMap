using UnityEngine;
using Verse;

namespace ColorPicker.Dialog
{
    class ColorSelectDialog : Window
    {
        private static readonly Texture2D colorPresetTexture = new Texture2D(20, 20);
        private static readonly ColorPresets colorPresets = IOUtil.LoadColorPresets();

        private static Texture2D colorPickerTexture;

        private readonly SelectionColorWidget selectionColorWidget;

        private string label;
        private bool allowAlpha;

        public ColorSelectDialog(string label, Color color, SelectionChange selectionChange = null, bool allowAlpha = false) : base()
        {
            this.label = label;
            this.allowAlpha = allowAlpha;
            this.selectionColorWidget = new SelectionColorWidget(color) { selectionChange = selectionChange };
        }

        public override Vector2 InitialSize
        {
            get => new Vector2(400, 550);
        }

        public static Texture2D ColorPickerTexture
        {
            get => colorPickerTexture;
            set
            {
                if (colorPickerTexture == null) 
                    colorPickerTexture = value;
                else
                    Log.Error("colorPickerTexture already assigned to.");
            }
        }

        public override void PreClose()
        {
            base.PreClose();
            if (colorPresets.IsModified)
            {
                IOUtil.SaveColorPresets(colorPresets);
                colorPresets.IsModified = false;
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            GUI.Label(new Rect(5, 5, inRect.width - 10, 25), this.label);
            this.AddColorSelectorWidget(15, 10, inRect.width - 30, this.selectionColorWidget, colorPresets);

            // TODO: do more buttons

            Text.Font = GameFont.Small;
            if (Widgets.ButtonText(new Rect(inRect.width * 0.5f + 30, inRect.height - 30, 60, 30), "AcceptButton".Translate()))
                base.Close();

            if (Widgets.ButtonText(new Rect(inRect.width * 0.5f - 90, inRect.height - 30, 60, 30), "ResetButton".Translate()))
                this.selectionColorWidget.ResetToDefault();
        }

        public void AddColorSelectorWidget(float left, float top, float width, SelectionColorWidget selectionColorWidget, ColorPresets presetsDto)
        {
            Text.Font = GameFont.Medium;

            Rect colorPickerRect = new Rect(0, 25f, width, ColorPickerTexture.height * width / ColorPickerTexture.width);

            GUI.BeginGroup(new Rect(left, top, width, colorPickerRect.height + 20));
            GUI.color = Color.white;
            if (GUI.RepeatButton(colorPickerRect, ColorPickerTexture, GUI.skin.label))
                SetColorToSelected(selectionColorWidget, presetsDto, GetColorFromTexture(Event.current.mousePosition, colorPickerRect, ColorPickerTexture));
            GUI.EndGroup();

            Color rgbColor = Color.white;
            if (presetsDto.HasSelected())
                rgbColor = presetsDto.GetSelectedColor();
            else
                rgbColor = selectionColorWidget.SelectedColor;

            GUI.BeginGroup(new Rect(0, colorPickerRect.height + 90f, width, 30f));
            GUI.Label(new Rect(0f, 0f, 10f, 20f), "R");
            string rText = GUI.TextField(new Rect(12f, 1f, 30f, 20f), ColorConvert(rgbColor.r), 3);

            GUI.Label(new Rect(52f, 0f, 10f, 20f), "G");
            string gText = GUI.TextField(new Rect(64f, 1f, 30f, 20f), ColorConvert(rgbColor.g), 3);

            GUI.Label(new Rect(104f, 0f, 10f, 20f), "B");
            string bText = GUI.TextField(new Rect(116f, 1f, 30f, 20f), ColorConvert(rgbColor.b), 3);

            string aText = "";
            if (allowAlpha)
            {
                GUI.Label(new Rect(156f, 0f, 10f, 20f), "A");
                aText = GUI.TextField(new Rect(168f, 1f, 30f, 20f), ColorConvert(rgbColor.a), 3);
            }
            GUI.EndGroup();

            if (allowAlpha)
            {
                GUI.BeginGroup(new Rect(0, colorPickerRect.height + 50f, width, 30f));
                GUI.Label(new Rect(0, 0, 75, 40), "ReColorStockpile.Alpha".Translate());
                rgbColor.a = Widgets.HorizontalSlider(new Rect(90, 0, 150, 20), rgbColor.a, 0, 1);
                GUI.EndGroup();
            }

            GUI.BeginGroup(new Rect(0, colorPickerRect.height + 130, width, 120));
            GUI.Label(new Rect(0, 0, 100, 30), "Presets:");
            bool skipRGB = false;
            float l = 0;
            for (int i = 0; i < presetsDto.Count; ++i)
            {
                GUI.color = presetsDto[i];
                l += colorPresetTexture.width + 4;
                Rect presetRect = new Rect(l, 32, colorPresetTexture.width, colorPresetTexture.height);
                GUI.Label(presetRect, new GUIContent(colorPresetTexture, "ReColorStockpile.ColorPresetHelp".Translate()));
                if (Widgets.ButtonInvisible(presetRect, false))
                {
                    if (Event.current.shift)
                    {
                        if (presetsDto.IsSelected(i))
                            presetsDto.Deselect();
                        else
                        {
                            if (!presetsDto.HasSelected())
                                presetsDto.SetColor(i, selectionColorWidget.SelectedColor);
                            presetsDto.SetSelected(i);
                        }
                    }
                    else
                        SetColorToSelected(selectionColorWidget, null, presetsDto[i]);

                    skipRGB = true;
                }
                GUI.color = Color.white;
                if (presetsDto.IsSelected(i))
                    Widgets.DrawBox(presetRect, 1);
            }
            GUI.Label(new Rect(0, 30 + colorPresetTexture.height + 2, width, 60), GUI.tooltip);
            GUI.EndGroup();

            if (!skipRGB && presetsDto.HasSelected())
            {
                Color c = new Color(ColorConvert(rText), ColorConvert(gText), ColorConvert(bText), 1);
                if (allowAlpha)
                    c.a = ColorConvert(aText);

                SetColorToSelected(selectionColorWidget, presetsDto, c);
            }
        }

        private static void SetColorToSelected(SelectionColorWidget selectionColorWidget, ColorPresets presetsDto, Color color)
        {
            if (presetsDto != null && presetsDto.HasSelected())
                presetsDto.SetSelectedColor(color);
            else
                selectionColorWidget.SelectedColor = color;
        }

        private Color GetColorFromTexture(Vector2 mousePosition, Rect rect, Texture2D texture)
        {
            float localMouseX = mousePosition.x - rect.x;
            float localMouseY = mousePosition.y - rect.y;
            int imageX = (int)(localMouseX * ((float)ColorPickerTexture.width / (rect.width + 0f)));
            int imageY = (int)((rect.height - localMouseY) * ((float)ColorPickerTexture.height / (rect.height + 0f)));
            Color pixel = texture.GetPixel(imageX, imageY);
            pixel.a = (colorPresets.HasSelected()) ? colorPresets.GetSelectedColor().a : this.selectionColorWidget.SelectedColor.a;
            return pixel;
        }

        private string ColorConvert(float f)
        {
            try
            {
                int i = (int)(f * 255);
                // clamp [0,255]
                if (i > 255) i = 255;
                else if (i < 0) i = 0;
                return i.ToString();
            }
            catch
            {
                return "0";
            }
        }

        private float ColorConvert(string intText)
        {
            try
            {
                float f = int.Parse(intText) / 255f;
                // clamp [0,1]
                if (f > 1) f = 1;
                else if (f < 0) f = 0;
                return f;
            }
            catch
            {
                return 0;
            }
        }
    }
}