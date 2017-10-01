using Verse;
using UnityEngine;

namespace BetterMiniMap
{
    class BetterMiniMapSettings : ModSettings
    {
        // NOTE: consider singletons?
        public class UpdatePeriods
        {
            public int viewpoint = 5; // should this be variable?
            public int colonists = 5;
            public int noncolonists = 5;
            public int robots = 5;
            public int ships = 5; // TODO: this seems too often?
            public int powerGrid = 50;
            public int wildlife = 80;
            public int buildings = 240;
            public int mining = 240;
            public int areas = 240;
            public int terrain = 2500;
            public int fog = 2500;
        }

        public class IndicatorSizes
        {
            public float colonists = 3f;
            public float tamedAnimals = 2f;
            public float enemyPawns = 2f;
            public float traderPawns = 2f;
            public float visitorPawns = 2f;
            public float robots = 3f;
            public float ships = 3f;
            public float wildlife = 1f;
            public float wildlifeTaming = 1f;
            public float wildlifeHunting = 1f;
            public float wildlifeHostiles = 1f;
        }

        public BetterMiniMapSettings()
        {
            this.updatePeriods = new UpdatePeriods();
            this.indicatorSizes = new IndicatorSizes();
        }

        public UpdatePeriods updatePeriods;
        public IndicatorSizes indicatorSizes;
    }

    class BetterMiniMapMod : Mod
    {
        

        public static BetterMiniMapSettings settings;

        public BetterMiniMapMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<BetterMiniMapSettings>();
        }

        public override string SettingsCategory() => "BMM_SettingsCategoryLabel".Translate();

        public override void DoSettingsWindowContents(Rect rect)
        {
            Listing_Standard listing_Standard = new Listing_Standard() { ColumnWidth = rect.width / 2f };
            listing_Standard.Begin(rect);

            listing_Standard.AddLabelLine("BMM_TimeUpdateLabel".Translate());

            listing_Standard.AddSettingsLine<int>("BMM_AreasOverlayLabel".Translate(), ref settings.updatePeriods.areas);
            listing_Standard.AddSettingsLine<int>("BMM_BuildingsOverlayLabel".Translate(), ref settings.updatePeriods.buildings);
            listing_Standard.AddSettingsLine<int>("BMM_ColonistsOverlayLabel".Translate(), ref settings.updatePeriods.colonists);
            listing_Standard.AddSettingsLine<int>("BMM_MiningOverlayLabel".Translate(), ref settings.updatePeriods.mining);
            listing_Standard.AddSettingsLine<int>("BMM_NoncolonistOverlayLabel".Translate(), ref settings.updatePeriods.noncolonists);
            listing_Standard.AddSettingsLine<int>("BMM_PowerGridOverlayLabel".Translate(), ref settings.updatePeriods.powerGrid);
            listing_Standard.AddSettingsLine<int>("BMM_RobotsOverlayLabel".Translate(), ref settings.updatePeriods.robots);
            listing_Standard.AddSettingsLine<int>("BMM_ShipsOverlayLabel".Translate(), ref settings.updatePeriods.ships);
            listing_Standard.AddSettingsLine<int>("BMM_TerrainOverlayLabel".Translate(), ref settings.updatePeriods.terrain);
            listing_Standard.AddSettingsLine<int>("BMM_WildlifeOverlayLabel".Translate(), ref settings.updatePeriods.wildlife);

            listing_Standard.AddSettingsLine<int>("BMM_FogOverlayLabel".Translate(), ref settings.updatePeriods.fog);
            listing_Standard.AddSettingsLine<int>("BMM_ViewpointOverlayLabel".Translate(), ref settings.updatePeriods.viewpoint);

            listing_Standard.NewColumn();

            listing_Standard.AddLabelLine("BMM_IndicatorSizeLabel".Translate());

            listing_Standard.AddSettingsLine<float>("BMM_ColonistIndicatorSizeLabel".Translate(), ref settings.indicatorSizes.colonists);
            listing_Standard.AddSettingsLine<float>("BMM_AnimalIndicatorSizeLabel".Translate(), ref settings.indicatorSizes.tamedAnimals);
            listing_Standard.AddSettingsLine<float>("BMM_RobotsIndicatorSizeLabel".Translate(), ref settings.indicatorSizes.robots);

            listing_Standard.AddSettingsLine<float>("BMM_EnemyIndicatorSizeLabel".Translate(), ref settings.indicatorSizes.enemyPawns);
            listing_Standard.AddSettingsLine<float>("BMM_TraderIndicatorSizeLabel".Translate(), ref settings.indicatorSizes.traderPawns);
            listing_Standard.AddSettingsLine<float>("BMM_VisitorIndicatorSizeLabel".Translate(), ref settings.indicatorSizes.visitorPawns);

            listing_Standard.AddSettingsLine<float>("BMM_ShipsIndicatorSizeLabel".Translate(), ref settings.indicatorSizes.ships);

            listing_Standard.AddSettingsLine<float>("BMM_WildlifeIndicatorSizeLabel".Translate(), ref settings.indicatorSizes.wildlife);
            listing_Standard.AddSettingsLine<float>("BMM_TamingIndicatorSizeLabel".Translate(), ref settings.indicatorSizes.wildlifeTaming);
            listing_Standard.AddSettingsLine<float>("BMM_HostileAnimalIndicatorSizeLabel".Translate(), ref settings.indicatorSizes.wildlifeHostiles);
            listing_Standard.AddSettingsLine<float>("BMM_HuntingIndicatorSizeLabel".Translate(), ref settings.indicatorSizes.wildlifeHunting);

            listing_Standard.End();
        }

    }

    internal static class ListingStandardHelper
    {
        const float gap = 12f;

        // TODO: reuse this in in AddSettingsLine 
        public static void AddLabelLine(this Listing_Standard listing_Standard, string label)
        {
            listing_Standard.Gap(gap);
            Rect lineRect = listing_Standard.GetRect(Text.LineHeight);
            Rect leftHalf = lineRect.LeftHalf().Rounded();

            // TODO: tooltips
            //Widgets.DrawHighlightIfMouseover(lineRect);
            //TooltipHandler.TipRegion(lineRect, "TODO: TIP GOES HERE");

            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(leftHalf, label);
            Text.Anchor = anchor;
        }

        public static void AddSettingsLine<T>(this Listing_Standard listing_Standard, string label, ref T settingsValue) where T : struct
        {
            listing_Standard.Gap(gap);
            Rect lineRect = listing_Standard.GetRect(Text.LineHeight);
            Rect leftHalf = lineRect.LeftHalf().Rounded();
            Rect rightHalf = lineRect.RightHalf().Rounded();
            rightHalf = rightHalf.LeftPartPixels(rightHalf.width - Text.LineHeight);

            // TODO: tooltips
            //Widgets.DrawHighlightIfMouseover(lineRect);
            //TooltipHandler.TipRegion(lineRect, "TODO: TIP GOES HERE");

            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(leftHalf, label);
            Text.Anchor = anchor;

            string buffer = settingsValue.ToString();
            Widgets.TextFieldNumeric<T>(rightHalf, ref settingsValue, ref buffer, 1f, 100000f);
        }
    }
}
