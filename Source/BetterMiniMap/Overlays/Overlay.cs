using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
	public abstract class Overlay
	{
        private bool dirty = true;
        private bool visible;
		private Texture2D texture;

        protected Overlay(bool visible) => this.Visible = visible;

        public bool Dirty { get => dirty; set => dirty = value; }

        public bool Visible
        {
            get => visible;
            set
            {
                this.visible = value;
                if (this.visible)
                    this.Dirty = true;
            }
        }

		public Texture2D Texture
		{
			get
			{
				if (this.texture == null)
				{
					this.texture = new Texture2D(Find.VisibleMap.Size.x, Find.VisibleMap.Size.z);
					this.texture.SetPixels(Utilities.GetClearPixelArray);
					this.texture.Apply();
				}
				return this.texture;
			}
		}

        public void ClearTexture(bool apply = false)
		{
			this.Texture.SetPixels(Utilities.GetClearPixelArray);
			if (apply)
				this.Texture.Apply();
		}

		public abstract void Update();
		public abstract int GetUpdateInterval();
	}
}
