using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
    public abstract class Overlay
    {
        public OverlayDef def;

        private bool visible;
        private Texture2D texture;

        // NOTE: do not use property here or else NPEs at Update()
        protected Overlay(bool visible) => this.visible = visible;

        public bool Visible
        {
            get => visible;
            set
            {
                this.visible = value;
                if (this.visible)
                    this.Update();
            }
        }

        public Texture2D Texture
        {
            get
            {
                // NOTE: is this lazy load necessary?
                if (this.texture == null)
                    this.GenerateTexture();
                return this.texture;
            }
        }

        public void GenerateTexture()
        {
            if (this.texture != null)
                Texture2D.Destroy(this.texture);

            this.texture = new Texture2D(Find.CurrentMap.Size.x, Find.CurrentMap.Size.z, this.SupportedTextureFormat, BetterMiniMapMod.settings.mipMaps)
            {
                //this.texture.SetPixels32(Utilities.GetClearPixelArray);
                //this.texture.filterMode = FilterMode.Trilinear;
                anisoLevel = BetterMiniMapMod.settings.anisoLevel,
                hideFlags = HideFlags.HideAndDontSave,
            };
            this.texture.Apply(BetterMiniMapMod.settings.mipMaps);
        }

        private TextureFormat SupportedTextureFormat
        {
            get
            {
                if (SystemInfo.SupportsTextureFormat(TextureFormat.RGBA32))
                    return TextureFormat.RGBA32;
                else
                    return TextureFormat.ARGB32;
            }
        }

        public void Update(bool clearTexture = true)
        {
            if (clearTexture)
                this.Texture.SetPixels32(Utilities.GetClearPixelArray);
            this.Render();
            this.Texture.Apply(BetterMiniMapMod.settings.mipMaps);
        }

        public abstract void Render();
		public abstract int GetUpdateInterval();

        // NOTE: there maybe a better place to put this...
        protected void ExposeData(string overlayName) => Scribe_Values.Look<bool>(ref this.visible, overlayName, true);

        public virtual bool ShouldUpdateOverlay
        {
            get
            {
                if (!Visible)
                    return false;
                // TODO looks like hashticking... (is this best?)
                return (Time.frameCount + this.GetHashCode()) % this.GetUpdateInterval() == 0;
            }
        }

        public virtual int OverlayPriority => 0;

    }
}
