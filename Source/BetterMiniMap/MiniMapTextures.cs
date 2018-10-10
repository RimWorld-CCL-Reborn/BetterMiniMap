using UnityEngine;
using Verse;

namespace BetterMiniMap
{
    [StaticConstructorOnStartup]
    class MiniMapTextures
    {
        public static readonly Texture2D Config;
        public static readonly Texture2D DragA;
        public static readonly Texture2D DragD;
        public static readonly Texture2D HomeA;
        public static readonly Texture2D ResizeA;
        public static readonly Texture2D ResizeD;
        public static readonly Texture2D MapManagerIcon;
        public static readonly Texture2D CloseXSmall;

        static MiniMapTextures()
        {
            MiniMapTextures.Config = ContentFinder<Texture2D>.Get("UI/config", true);
            MiniMapTextures.DragA = ContentFinder<Texture2D>.Get("UI/dragA", true);
            MiniMapTextures.DragD = ContentFinder<Texture2D>.Get("UI/dragD", true);
            MiniMapTextures.HomeA = ContentFinder<Texture2D>.Get("UI/homeA", true);
            MiniMapTextures.ResizeA = ContentFinder<Texture2D>.Get("UI/resizeA", true);
            MiniMapTextures.ResizeD = ContentFinder<Texture2D>.Get("UI/resizeD", true);
            // TODO: real icon?
            MiniMapTextures.MapManagerIcon = ContentFinder<Texture2D>.Get("UI/Commands/ShowMap", true);
            MiniMapTextures.CloseXSmall = ContentFinder<Texture2D>.Get("UI/Widgets/CloseXSmall", true);
        }

    }
}
