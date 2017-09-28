using Verse;
using UnityEngine;

namespace BetterMiniMap
{
    class BetterMiniMapSettings : ModSettings
    {
        public int overlay_Area = 240;
        public int overlay_Buildings = 240;
        public int overlay_Colonists = 5;
        public int overlay_Fog = 2500;
        public int overlay_Mining = 240;
        public int overlay_NonColonistPawns = 5;
        public int overlay_PowerGrid = 50;
        public int overlay_Robots = 5;
        public int overlay_Ships = 5;
        public int overlay_Terrain = 2500;
        public int overlay_ViewPort = 5;
        public int overlay_Wildlife = 80;

        // NOTE: consider singletons?

        public class UpdatePeriods
        {
            public int viewpoint = 5; // should this be variable?
            public int colonists = 5;
            public int animals = 5;
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
            float gap = 12f;

            string text = settings.overlay_Area.ToString();
            string text2 = settings.overlay_Buildings.ToString();
            string text3 = settings.overlay_Colonists.ToString();
            string text4 = settings.overlay_Fog.ToString();
            string text5 = settings.overlay_Mining.ToString();
            string text6 = settings.overlay_NonColonistPawns.ToString();
            string text7 = settings.overlay_PowerGrid.ToString();
            string text8 = settings.overlay_Robots.ToString();
            string text9 = settings.overlay_Ships.ToString();
            string text10 = settings.overlay_Terrain.ToString();
            string text11 = settings.overlay_ViewPort.ToString();
            string text12 = settings.overlay_Wildlife.ToString();

            string text13 = settings.indicatorSizes.colonists.ToString();
            string text14 = settings.indicatorSizes.tamedAnimals.ToString();
            string text15 = settings.indicatorSizes.enemyPawns.ToString();
            string text16 = settings.indicatorSizes.traderPawns.ToString();
            string text17 = settings.indicatorSizes.visitorPawns.ToString();
            string text18 = settings.indicatorSizes.robots.ToString();
            string text19 = settings.indicatorSizes.ships.ToString();
            string text20 = settings.indicatorSizes.wildlife.ToString();
            string text21 = settings.indicatorSizes.wildlifeTaming.ToString();
            string text22 = settings.indicatorSizes.wildlifeHostiles.ToString();
            string text23 = settings.indicatorSizes.wildlifeHunting.ToString();

            Listing_Standard listing_Standard = new Listing_Standard();

            listing_Standard.ColumnWidth = rect.width / 2f;
            listing_Standard.Begin(rect);
            listing_Standard.Gap(gap);
            Rect rect2 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect3 = rect2.LeftHalf().Rounded();
            TooltipHandler.TipRegion(rect2, "labeltimeupdated".Translate());
            Widgets.Label(rect3, "BMM_TimeUpdateLabel".Translate());

            listing_Standard.Gap(gap);

            Rect rect4 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect5 = rect4.LeftHalf().Rounded();
            Rect rect6 = rect4.RightHalf().Rounded();
            rect6 = rect6.LeftPartPixels(rect6.width - Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(rect4);
            TooltipHandler.TipRegion(rect4, "overlay_Aread".Translate());
            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect5, "BMM_AreasOverlayLabel".Translate());
            Text.Anchor = anchor;
            Widgets.TextFieldNumeric<int>(rect6, ref settings.overlay_Area, ref text, 0f, 100000f);
            listing_Standard.Gap(gap);

            Rect rect7 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect8 = rect7.LeftHalf().Rounded();
            Rect rect9 = rect7.RightHalf().Rounded();
            rect9 = rect9.LeftPartPixels(rect9.width - Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(rect7);
            TooltipHandler.TipRegion(rect7, "overlay_Buildingsd".Translate());
            TextAnchor anchor2 = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect8, "BMM_BuildingsOverlayLabel".Translate());
            Text.Anchor = anchor2;
            Widgets.TextFieldNumeric<int>(rect9, ref settings.overlay_Buildings, ref text2, 0f, 100000f);
            listing_Standard.Gap(gap);

            Rect rect10 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect11 = rect10.LeftHalf().Rounded();
            Rect rect12 = rect10.RightHalf().Rounded();
            rect12 = rect12.LeftPartPixels(rect12.width - Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(rect10);
            TooltipHandler.TipRegion(rect10, "overlay_Colonistsd".Translate());
            TextAnchor anchor3 = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect11, "BMM_ColonistsOverlayLabel".Translate());
            Text.Anchor = anchor3;
            Widgets.TextFieldNumeric<int>(rect12, ref settings.overlay_Colonists, ref text3, 0f, 100000f);
            listing_Standard.Gap(gap);

            Rect rect13 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect14 = rect13.LeftHalf().Rounded();
            Rect rect15 = rect13.RightHalf().Rounded();
            rect15 = rect15.LeftPartPixels(rect15.width - Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(rect13);
            TooltipHandler.TipRegion(rect13, "overlay_Fogd".Translate());
            TextAnchor anchor4 = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect14, "overlay_Fogt".Translate());
            Text.Anchor = anchor4;
            Widgets.TextFieldNumeric<int>(rect15, ref settings.overlay_Fog, ref text4, 0f, 100000f);
            listing_Standard.Gap(gap);

            Rect rect16 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect17 = rect16.LeftHalf().Rounded();
            Rect rect18 = rect16.RightHalf().Rounded();
            rect18 = rect18.LeftPartPixels(rect18.width - Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(rect16);
            TooltipHandler.TipRegion(rect16, "overlay_Miningd".Translate());
            TextAnchor anchor5 = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect17, "overlay_Miningt".Translate());
            Text.Anchor = anchor5;
            Widgets.TextFieldNumeric<int>(rect18, ref settings.overlay_Mining, ref text5, 0f, 100000f);
            listing_Standard.Gap(gap);

            Rect rect19 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect20 = rect19.LeftHalf().Rounded();
            Rect rect21 = rect19.RightHalf().Rounded();
            rect21 = rect21.LeftPartPixels(rect21.width - Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(rect19);
            TooltipHandler.TipRegion(rect19, "overlay_NonColonistPawnsd".Translate());
            TextAnchor anchor6 = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect20, "overlay_NonColonistPawnst".Translate());
            Text.Anchor = anchor6;
            Widgets.TextFieldNumeric<int>(rect21, ref settings.overlay_NonColonistPawns, ref text6, 0f, 100000f);
            listing_Standard.Gap(gap);

            Rect rect22 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect23 = rect22.LeftHalf().Rounded();
            Rect rect24 = rect22.RightHalf().Rounded();
            rect24 = rect24.LeftPartPixels(rect24.width - Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(rect22);
            TooltipHandler.TipRegion(rect22, "overlay_PowerGridd".Translate());
            TextAnchor anchor7 = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect23, "overlay_PowerGridt".Translate());
            Text.Anchor = anchor7;
            Widgets.TextFieldNumeric<int>(rect24, ref settings.overlay_PowerGrid, ref text7, 0f, 100000f);
            listing_Standard.Gap(gap);

            Rect rect25 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect26 = rect25.LeftHalf().Rounded();
            Rect rect27 = rect25.RightHalf().Rounded();
            rect27 = rect27.LeftPartPixels(rect27.width - Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(rect25);
            TooltipHandler.TipRegion(rect25, "overlay_Robotsd".Translate());
            TextAnchor anchor8 = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect26, "overlay_Robotst".Translate());
            Text.Anchor = anchor8;
            Widgets.TextFieldNumeric<int>(rect27, ref settings.overlay_Robots, ref text8, 0f, 100000f);
            listing_Standard.Gap(gap);

            Rect rect28 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect29 = rect28.LeftHalf().Rounded();
            Rect rect30 = rect28.RightHalf().Rounded();
            rect30 = rect30.LeftPartPixels(rect30.width - Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(rect28);
            TooltipHandler.TipRegion(rect28, "overlay_Shipsd".Translate());
            TextAnchor anchor9 = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect29, "overlay_Shipst".Translate());
            Text.Anchor = anchor9;
            Widgets.TextFieldNumeric<int>(rect30, ref settings.overlay_Ships, ref text9, 0f, 100000f);
            listing_Standard.Gap(gap);

            Rect rect31 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect32 = rect31.LeftHalf().Rounded();
            Rect rect33 = rect31.RightHalf().Rounded();
            rect33 = rect33.LeftPartPixels(rect33.width - Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(rect31);
            TooltipHandler.TipRegion(rect31, "overlay_Terraind".Translate());
            TextAnchor anchor10 = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect32, "overlay_Terraint".Translate());
            Text.Anchor = anchor10;
            Widgets.TextFieldNumeric<int>(rect33, ref settings.overlay_Terrain, ref text10, 0f, 100000f);
            listing_Standard.Gap(gap);

            Rect rect34 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect35 = rect34.LeftHalf().Rounded();
            Rect rect36 = rect34.RightHalf().Rounded();
            rect36 = rect36.LeftPartPixels(rect36.width - Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(rect34);
            TooltipHandler.TipRegion(rect34, "overlay_ViewPortd".Translate());
            TextAnchor anchor11 = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect35, "overlay_ViewPortt".Translate());
            Text.Anchor = anchor11;
            Widgets.TextFieldNumeric<int>(rect36, ref settings.overlay_ViewPort, ref text11, 0f, 100000f);
            listing_Standard.Gap(gap);

            Rect rect37 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect38 = rect37.LeftHalf().Rounded();
            Rect rect39 = rect37.RightHalf().Rounded();
            rect39 = rect39.LeftPartPixels(rect39.width - Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(rect37);
            TooltipHandler.TipRegion(rect37, "overlay_Wildlifed".Translate());
            TextAnchor anchor12 = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect38, "overlay_Wildlifet".Translate());
            Text.Anchor = anchor12;
            Widgets.TextFieldNumeric<int>(rect39, ref settings.overlay_Wildlife, ref text12, 0f, 100000f);
            listing_Standard.NewColumn();
            listing_Standard.Gap(gap);

            Rect rect40 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect41 = rect40.LeftHalf().Rounded();
            TooltipHandler.TipRegion(rect40, "labelradiusd".Translate());
            Widgets.Label(rect41, "labelradiust".Translate());
            listing_Standard.Gap(gap);

            Rect rect42 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect43 = rect42.LeftHalf().Rounded();
            Rect rect44 = rect42.RightHalf().Rounded();
            rect44 = rect44.LeftPartPixels(rect44.width - Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(rect42);
            TooltipHandler.TipRegion(rect42, "radiu_Colonistsd".Translate());
            TextAnchor anchor13 = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect43, "radiu_Colonistst".Translate());
            Text.Anchor = anchor13;
            Widgets.TextFieldNumeric<float>(rect44, ref settings.indicatorSizes.colonists, ref text13, 0f, 100000f);
            listing_Standard.Gap(gap);

            Rect rect45 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect46 = rect45.LeftHalf().Rounded();
            Rect rect47 = rect45.RightHalf().Rounded();
            rect47 = rect47.LeftPartPixels(rect47.width - Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(rect45);
            TooltipHandler.TipRegion(rect45, "radiu_ColonistsAnimald".Translate());
            TextAnchor anchor14 = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect46, "radiu_ColonistsAnimalt".Translate());
            Text.Anchor = anchor14;
            Widgets.TextFieldNumeric<float>(rect47, ref settings.indicatorSizes.tamedAnimals, ref text14, 0f, 100000f);
            listing_Standard.Gap(gap);

            Rect rect48 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect49 = rect48.LeftHalf().Rounded();
            Rect rect50 = rect48.RightHalf().Rounded();
            rect50 = rect50.LeftPartPixels(rect50.width - Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(rect48);
            TooltipHandler.TipRegion(rect48, "radiu_NonColonistPawnsEnemyd".Translate());
            TextAnchor anchor15 = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect49, "radiu_NonColonistPawnsEnemyt".Translate());
            Text.Anchor = anchor15;
            Widgets.TextFieldNumeric<float>(rect50, ref settings.indicatorSizes.enemyPawns, ref text15, 0f, 100000f);
            listing_Standard.Gap(gap);

            Rect rect51 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect52 = rect51.LeftHalf().Rounded();
            Rect rect53 = rect51.RightHalf().Rounded();
            rect53 = rect53.LeftPartPixels(rect53.width - Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(rect51);
            TooltipHandler.TipRegion(rect51, "radiu_NonColonistPawnsTraderd".Translate());
            TextAnchor anchor16 = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect52, "radiu_NonColonistPawnsTradert".Translate());
            Text.Anchor = anchor16;
            Widgets.TextFieldNumeric<float>(rect53, ref settings.indicatorSizes.traderPawns, ref text16, 0f, 100000f);
            listing_Standard.Gap(gap);

            Rect rect54 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect55 = rect54.LeftHalf().Rounded();
            Rect rect56 = rect54.RightHalf().Rounded();
            rect56 = rect56.LeftPartPixels(rect56.width - Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(rect54);
            TooltipHandler.TipRegion(rect54, "radiu_NonColonistPawnsVisitord".Translate());
            TextAnchor anchor17 = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect55, "radiu_NonColonistPawnsVisitort".Translate());
            Text.Anchor = anchor17;
            Widgets.TextFieldNumeric<float>(rect56, ref settings.indicatorSizes.visitorPawns, ref text17, 0f, 100000f);
            listing_Standard.Gap(gap);

            Rect rect57 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect58 = rect57.LeftHalf().Rounded();
            Rect rect59 = rect57.RightHalf().Rounded();
            rect59 = rect59.LeftPartPixels(rect59.width - Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(rect57);
            TooltipHandler.TipRegion(rect57, "radiu_Robotsd".Translate());
            TextAnchor anchor18 = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect58, "radiu_Robotst".Translate());
            Text.Anchor = anchor18;
            Widgets.TextFieldNumeric<float>(rect59, ref settings.indicatorSizes.robots, ref text18, 0f, 100000f);
            listing_Standard.Gap(gap);

            Rect rect60 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect61 = rect60.LeftHalf().Rounded();
            Rect rect62 = rect60.RightHalf().Rounded();
            rect62 = rect62.LeftPartPixels(rect62.width - Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(rect60);
            TooltipHandler.TipRegion(rect60, "radiu_Shipsd".Translate());
            TextAnchor anchor19 = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect61, "radiu_Shipst".Translate());
            Text.Anchor = anchor19;
            Widgets.TextFieldNumeric<float>(rect62, ref settings.indicatorSizes.ships, ref text19, 0f, 100000f);
            listing_Standard.Gap(gap);

            Rect rect63 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect64 = rect63.LeftHalf().Rounded();
            Rect rect65 = rect63.RightHalf().Rounded();
            rect65 = rect65.LeftPartPixels(rect65.width - Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(rect63);
            TooltipHandler.TipRegion(rect63, "radiu_Wildlifewildd".Translate());
            TextAnchor anchor20 = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect64, "radiu_Wildlifewildt".Translate());
            Text.Anchor = anchor20;
            Widgets.TextFieldNumeric<float>(rect65, ref settings.indicatorSizes.wildlife, ref text20, 0f, 100000f);
            listing_Standard.Gap(gap);

            Rect rect66 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect67 = rect66.LeftHalf().Rounded();
            Rect rect68 = rect66.RightHalf().Rounded();
            rect68 = rect68.LeftPartPixels(rect68.width - Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(rect66);
            TooltipHandler.TipRegion(rect66, "radiu_Wildlifetamed".Translate());
            TextAnchor anchor21 = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect67, "radiu_Wildlifetamet".Translate());
            Text.Anchor = anchor21;
            Widgets.TextFieldNumeric<float>(rect68, ref settings.indicatorSizes.wildlifeTaming, ref text21, 0f, 100000f);
            listing_Standard.Gap(gap);

            Rect rect69 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect70 = rect69.LeftHalf().Rounded();
            Rect rect71 = rect69.RightHalf().Rounded();
            rect71 = rect71.LeftPartPixels(rect71.width - Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(rect69);
            TooltipHandler.TipRegion(rect69, "radiu_Wildlifehostiled".Translate());
            TextAnchor anchor22 = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect70, "radiu_Wildlifehostilet".Translate());
            Text.Anchor = anchor22;
            Widgets.TextFieldNumeric<float>(rect71, ref settings.indicatorSizes.wildlifeHostiles, ref text22, 0f, 100000f);
            listing_Standard.Gap(gap);

            Rect rect72 = listing_Standard.GetRect(Text.LineHeight);
            Rect rect73 = rect72.LeftHalf().Rounded();
            Rect rect74 = rect72.RightHalf().Rounded();
            rect74 = rect74.LeftPartPixels(rect74.width - Text.LineHeight);
            Widgets.DrawHighlightIfMouseover(rect72);
            TooltipHandler.TipRegion(rect72, "radiu_Wildlifehuntingd".Translate());
            TextAnchor anchor23 = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect73, "radiu_Wildlifehuntingt".Translate());
            Text.Anchor = anchor23;
            Widgets.TextFieldNumeric<float>(rect74, ref settings.indicatorSizes.wildlifeHunting, ref text23, 0f, 100000f);


            listing_Standard.End();
        }
    }
}
