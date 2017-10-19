using System.IO;
using UnityEngine;
using Verse;
using ColorPicker.Dialog;

namespace BetterMiniMap
{
    [StaticConstructorOnStartup]
    class ColorPickerTexture
    {
        // TODO: there should be a better way to do this
        static ColorPickerTexture()
        {
            byte[] data = File.ReadAllBytes(BetterMiniMapMod.settings.rootDir + "/Textures/UI/colorpicker.png");
            ColorSelectDialog.ColorPickerTexture = new Texture2D(2, 2, TextureFormat.Alpha8, true);
            ColorSelectDialog.ColorPickerTexture.LoadImage(data, false);
        }
    }
}
