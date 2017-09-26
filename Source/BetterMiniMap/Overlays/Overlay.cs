using System.Linq;
using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
    public abstract class Overlay
    {
        private bool visible;
        private Texture2D texture;

        // caching
        private static Map map;

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
                if (this.texture == null)
                {
                    this.texture = new Texture2D(Find.VisibleMap.Size.x, Find.VisibleMap.Size.z);
                    this.texture.SetPixels(Utilities.GetClearPixelArray);
                    this.texture.Apply();
                }
                return this.texture;
            }
        }

        // NOTE: consider splitting this into two methods...
        // NOTE: consider more caching? (e.g. edgeColor)?
        protected virtual void CreateMarker(float radius, Color color, IntVec3 position, float edgeOpacity = 0.5f)
        {
            map = Find.VisibleMap;

            int numRadiusCells = GenRadial.NumCellsInRadius(radius);
            int numInnerCells = (radius >= 2f) ? GenRadial.NumCellsInRadius(radius - 1f) : numRadiusCells;

            Color edgeColor = new Color(color.r, color.g, color.b, color.a * edgeOpacity);
            // NOTE: it would be nice to do this once per all things/pawns within each render...
            IntVec3[] array = GenRadial.RadialCellsAround(position, radius, true).ToArray<IntVec3>();

            for (int i = 0; i < numRadiusCells; i++)
                if (array[i].InBounds(map))
                    this.Texture.SetPixel(array[i].x, array[i].z, (i > numInnerCells) ? edgeColor : color);
        }

        public void Update(bool clearTexture = true)
        {
            if (clearTexture)
                this.Texture.SetPixels(Utilities.GetClearPixelArray);
            this.Render();
            this.Texture.Apply();
        }

        public abstract void Render();

		public abstract int GetUpdateInterval();

        public virtual  bool ShouldUpdateOverlay
        {
            get
            {
                if (!Visible)
                    return false;
                // TODO looks like hashticking... (is this best?)
                return (Time.frameCount + this.GetHashCode()) % this.GetUpdateInterval() == 0;
            }
        }
    }
}
