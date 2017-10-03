using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace BetterMiniMap.Overlays
{
	public class Terrain_Overlay : Overlay, IExposable
	{
        static Dictionary<ushort, Color> colorMapping;

        static Terrain_Overlay()
        {
            colorMapping = new Dictionary<ushort, Color>()
            {
                { TerrainDefOf.WaterMovingDeep.shortHash, new ColorInt(0,160,224).ToColor },
                { TerrainDefOf.WaterOceanDeep.shortHash, new ColorInt(17,37,110).ToColor },
                { TerrainDefOf.WaterDeep.shortHash, new ColorInt(4,119,186).ToColor },
                { TerrainDefOf.WaterMovingShallow.shortHash, new ColorInt(163,206,231).ToColor },
                { TerrainDefOf.WaterOceanShallow.shortHash, new ColorInt(85,195,220).ToColor },
                { TerrainDefOf.WaterShallow.shortHash, new ColorInt(115,189,248).ToColor },
                { DefDatabase<TerrainDef>.GetNamed("BrokenAsphalt").shortHash, new ColorInt(80,80,80).ToColor },
                { DefDatabase<TerrainDef>.GetNamed("PackedDirt").shortHash, new ColorInt(120,72,0).ToColor },
                { TerrainDefOf.Sand.shortHash, new ColorInt(199,169,110).ToColor },
                { TerrainDefOf.Soil.shortHash, new ColorInt(141,95,43).ToColor },
                { DefDatabase<TerrainDef>.GetNamed("SoilRich").shortHash, new ColorInt(118,76,49).ToColor },
                { DefDatabase<TerrainDef>.GetNamed("Marsh").shortHash, new ColorInt(82,87,56).ToColor },
                { DefDatabase<TerrainDef>.GetNamed("Mud").shortHash, new ColorInt(87,68,56).ToColor },
                { DefDatabase<TerrainDef>.GetNamed("MarshyTerrain").shortHash, new ColorInt(106,107,76).ToColor },
                { TerrainDefOf.Ice.shortHash, new ColorInt(177,246,249).ToColor },
                { TerrainDefOf.Gravel.shortHash, new ColorInt(195,195,195).ToColor },
                { DefDatabase<TerrainDef>.GetNamed("MossyTerrain").shortHash, new ColorInt(108,124,71).ToColor },
            };
        }

        public Terrain_Overlay(bool visible = true) : base(visible) { }

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.updatePeriods.terrain;

        public void Update() => base.Update(false);

		public override void Render()
		{
			for (int i = 0; i < Find.VisibleMap.cellIndices.NumGridCells; i++)
			{
				IntVec3 cell = Find.VisibleMap.cellIndices.IndexToCell(i);
                TerrainDef terrainDef = Find.VisibleMap.terrainGrid.TerrainAt(i);
                Color color = terrainDef.color;

                if (color == Color.white)
                {
                    colorMapping.TryGetValue(terrainDef.shortHash, out color);
                    if (color == null)
                        color = Color.clear;
                }
                base.Texture.SetPixel(cell.x, cell.z, color);
			}
        }

        public void ExposeData() => this.ExposeData("overlayTerrain");
    }
}
