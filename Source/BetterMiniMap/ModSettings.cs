using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using SettingsHelper;
using ColorPicker.Dialog;

using static BetterMiniMap.BetterMiniMapSettings;
using Harmony;

namespace BetterMiniMap
{
    public static class OverlaySettingDatabase
    {
        private static Dictionary<string, OverlaySettings> overlaySettings;
        private static Dictionary<string, IndicatorSettings> indicatorSettings;

        static OverlaySettingDatabase()
        {
            OverlaySettingDatabase.overlaySettings = new Dictionary<string, OverlaySettings>();
            OverlaySettingDatabase.indicatorSettings = new Dictionary<string, IndicatorSettings>();
        }

        public static void InitializeOverlaySettings()
        {
            foreach (OverlayDef def in DefDatabase<OverlayDef>.AllDefs.Where(d => !d.disabled))
                OverlaySettingDatabase.AddOverlaySettings(def);
            LoadedModManager.GetMod<BetterMiniMapMod>().FetchSettings();
        }

        private static void AddOverlaySettings(OverlayDef def)
        {
            OverlaySettingDatabase.overlaySettings.Add(def.defName, new OverlaySettings(def));
            foreach (IndicatorProps props in def.indicatorMappings.mappings)
                OverlaySettingDatabase.indicatorSettings.Add(props.name, new IndicatorSettings(props));
        }

        public static List<OverlaySettings> OverlaySettings
        {
            get => OverlaySettingDatabase.overlaySettings.Values.ToList();
        }

        public static List<IndicatorSettings> IndicatorSettings
        {
            get => OverlaySettingDatabase.indicatorSettings.Values.ToList();
        }

        public static OverlaySettings GetOverlaySettings(string key) => OverlaySettingDatabase.overlaySettings[key];
        public static IndicatorSettings GetIndicatorSettings(string key) => OverlaySettingDatabase.indicatorSettings[key];

    }

    public class BetterMiniMapSettings : ModSettings
    {
        
        #region NestedClasses
        public class OverlaySettings
        {
            public string name; //defName
            public string label;
            public int updatePeriod;
            public int defaultUpdatePeriod;

            public OverlaySettings(OverlayDef def)
            {
                this.name = def.defName;
                this.label = def.label;
                this.defaultUpdatePeriod = this.updatePeriod = def.updatePeriod;
            }
        }

        public class IndicatorSettings
        {
            public string name;
            public string label;
            public float radius;
            public float defaultRadius;
            public Color color;
            public Color defaultColor;
            public Color edgeColor;
            public Color defaultEdgeColor;

            public IndicatorSettings(IndicatorProps props)
            {
                this.name = props.name;
                this.label = props.label;
                this.defaultColor = this.color = props.color;
                this.defaultEdgeColor = this.edgeColor = props.edgeColor;
                this.defaultRadius = this.radius = props.radius;
            }
        }

        public class UpdatePeriods : IExposable
        {
            public int viewpoint = 5;
            public int powerGrid = 60;
            public int buildings = 250;
            public int mining = 250;
            public int areas = 250;
            public int terrain = 2500;
            public int fog = 2500;

            public void ExposeData()
            {
                Scribe_Values.Look(ref this.viewpoint, "viewpoint", 5);
                Scribe_Values.Look(ref this.powerGrid, "powerGrid", 60);
                Scribe_Values.Look(ref this.buildings, "buildings", 250);
                Scribe_Values.Look(ref this.mining, "mining", 250);
                Scribe_Values.Look(ref this.areas, "areas", 250);
                Scribe_Values.Look(ref this.terrain, "terrain", 2500);
                Scribe_Values.Look(ref this.fog, "fog", 2500);
                foreach (OverlaySettings settings in OverlaySettingDatabase.OverlaySettings)
                    Scribe_Values.Look(ref settings.updatePeriod, settings.name, settings.defaultUpdatePeriod);
            }
        }

        // NOTE: shadow relic class -- only used for scribing values tracked in OverlaySettingDatabase (imported from OverlayDefs) 
        // TODO: consider new setting format
        public class IndicatorSizes : IExposable
        {
            public void ExposeData()
            {
                foreach(IndicatorSettings settings in OverlaySettingDatabase.IndicatorSettings)
                    Scribe_Values.Look(ref settings.radius, settings.name, settings.defaultRadius); ;
            }
        }

        // TODO: store edgeColors in config
        public class OverlayColors : IExposable
        {
            private static readonly Color miningColorDefault = new Color(0.75f, 0.4f, 0.125f, 1f);
            private static readonly Color darkGray = new Color(0.50f, 0.42f, 0.40f, 1f);

            public Color viewpoint = Color.black;
            public Color fog = darkGray;
            public Color buildings = Color.white;

            public Color poweredOn = Color.yellow;
            public Color poweredByBatteries = Color.green;
            public Color notPowered = Color.red;
            public Color powererOff = Color.gray;

            public Color mining = miningColorDefault;

            public Color viewpointFaded;

            public OverlayColors() => this.SetFadedColors();

            public void ExposeData()
            {
                Scribe_Values.Look(ref viewpoint, "viewpoint", Color.black);
                Scribe_Values.Look(ref fog, "fog", darkGray);
                Scribe_Values.Look(ref buildings, "buildings", Color.white);

                foreach (IndicatorSettings settings in OverlaySettingDatabase.IndicatorSettings)
                    Scribe_Values.Look(ref settings.color, settings.name, settings.defaultColor);

                Scribe_Values.Look(ref poweredOn, "poweredOn", Color.yellow);
                Scribe_Values.Look(ref poweredByBatteries, "poweredByBatteries", Color.green);
                Scribe_Values.Look(ref notPowered, "notPowered", Color.red);
                Scribe_Values.Look(ref powererOff, "powererOff", Color.gray);

                Scribe_Values.Look(ref mining, "mining", miningColorDefault);

                this.SetFadedColors();
            }

            private void SetFadedColors()
            {
                this.viewpointFaded = BetterMiniMapSettings.FadedColor(this.viewpoint, 0.25f);
                foreach (IndicatorSettings settings in OverlaySettingDatabase.IndicatorSettings)
                    settings.edgeColor = BetterMiniMapSettings.FadedColor(settings.color);
            }
        }
        #endregion NestedClasses

        // Initializes settings for users without config file
        public BetterMiniMapSettings()
        {
            this.updatePeriods = new UpdatePeriods();
            this.overlayColors = new OverlayColors();
        }

        public string rootDir; // NOTE: no need to expose
        public UpdatePeriods updatePeriods;
        public OverlayColors overlayColors;

        // HIDDEN SETTINGS
        public bool mipMaps = false;
        public int anisoLevel = 1;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref this.updatePeriods, "updatePeriods");
            Scribe_Deep.Look(ref this.overlayColors, "overlayColors");

            // HIDDEN SETTINGS
            Scribe_Values.Look<bool>(ref this.mipMaps, "mipMaps", false);
            Scribe_Values.Look<int>(ref this.anisoLevel, "anisoLevel", 1);

            // Handles upgrading settings
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                if (this.updatePeriods == null)
                    this.updatePeriods = new UpdatePeriods();
                if (this.overlayColors == null)
                    this.overlayColors = new OverlayColors();
            }
        }

        public static Color FadedColor(Color inColor, float alpha = 0.5f)
        {
            Color faded = inColor;
            faded.a = alpha;
            return faded;
        }
    }

    public class BetterMiniMapMod : Mod
    {
        public static BetterMiniMapSettings settings;

        public BetterMiniMapMod(ModContentPack content) : base(content)
        {
            FetchSettings();
            settings.rootDir = content.RootDir;
            ListingStandardHelper.Gap = 10f;
        }

        public void FetchSettings()
        {
            Traverse.Create(this).Field("modSettings").SetValue(null);
            settings = GetSettings<BetterMiniMapSettings>();
        }

        public override string SettingsCategory() => "BMM_SettingsCategoryLabel".Translate();

        public override void DoSettingsWindowContents(Rect rect)
        {
            rect.height += 50f;
            Listing_Standard listing_Standard = rect.BeginListingStandard();

            // UpdatePeriods
            listing_Standard.AddLabelLine("BMM_TimeUpdateLabel".Translate());

            listing_Standard.AddLabeledNumericalTextField<int>("BMM_AreasOverlayLabel".Translate(), ref settings.updatePeriods.areas, 0.75f);
            listing_Standard.AddLabeledNumericalTextField<int>("BMM_BuildingsOverlayLabel".Translate(), ref settings.updatePeriods.buildings, 0.75f);
            listing_Standard.AddLabeledNumericalTextField<int>("BMM_MiningOverlayLabel".Translate(), ref settings.updatePeriods.mining, 0.75f);
            listing_Standard.AddLabeledNumericalTextField<int>("BMM_PowerGridOverlayLabel".Translate(), ref settings.updatePeriods.powerGrid, 0.75f);
            listing_Standard.AddLabeledNumericalTextField<int>("BMM_TerrainOverlayLabel".Translate(), ref settings.updatePeriods.terrain, 0.75f);
            listing_Standard.AddLabeledNumericalTextField<int>("BMM_FogOverlayLabel".Translate(), ref settings.updatePeriods.fog, 0.75f);
            listing_Standard.AddLabeledNumericalTextField<int>("BMM_ViewpointOverlayLabel".Translate(), ref settings.updatePeriods.viewpoint, 0.75f);

            foreach (OverlaySettings overlaySettings in OverlaySettingDatabase.OverlaySettings)
                listing_Standard.AddLabeledNumericalTextField<int>(overlaySettings.label, ref overlaySettings.updatePeriod, 0.75f);

            // IndicatorSizes
            listing_Standard.AddSubSettingsButton("BMM_IndicatorSizeLabel".Translate(), (Rect inRect) =>
            {
                Listing_Standard subsettings = inRect.BeginListingStandard();
                foreach(IndicatorSettings settings in OverlaySettingDatabase.IndicatorSettings)
                {
                    // TODO: remove the need for this translate...(Def Injection)
                    subsettings.AddLabeledNumericalTextField<float>(settings.label.Translate(), ref settings.radius, 0.9f, 0f, 25f);
                }
                subsettings.End();
            });

            // Customize colors
            listing_Standard.AddSubSettingsButton("BMM_ColorPickersLabel".Translate(), (Rect inRect) =>
            {
                Listing_Standard subsettings = inRect.BeginListingStandard();
                //listing_Standard.AddColorPickerButton("BMM_FogOverlayLabel".Translate(), settings.overlayColors.fog, (SelectionColorWidget scw) => { settings.overlayColors.fog = scw.SelectedColor; });
                subsettings.AddColorPickerButton("BMM_BuildingsOverlayLabel".Translate(), settings.overlayColors.buildings, (SelectionColorWidget scw) => { settings.overlayColors.buildings = scw.SelectedColor; });

                subsettings.AddColorPickerButton("BMM_ViewpointOverlayLabel".Translate(), settings.overlayColors.viewpoint, (SelectionColorWidget scw) => {
                    settings.overlayColors.viewpoint = scw.SelectedColor;
                    settings.overlayColors.viewpointFaded = BetterMiniMapSettings.FadedColor(scw.SelectedColor, 0.25f);
                });

                subsettings.AddColorPickerButton("BMM_PoweredOnLabel".Translate(), settings.overlayColors.poweredOn, (SelectionColorWidget scw) => { settings.overlayColors.poweredOn = scw.SelectedColor; });
                subsettings.AddColorPickerButton("BMM_PoweredByBatteriesLabel".Translate(), settings.overlayColors.poweredByBatteries, (SelectionColorWidget scw) => { settings.overlayColors.poweredByBatteries = scw.SelectedColor; });
                subsettings.AddColorPickerButton("BMM_NotPoweredLabel".Translate(), settings.overlayColors.notPowered, (SelectionColorWidget scw) => { settings.overlayColors.notPowered = scw.SelectedColor; });
                subsettings.AddColorPickerButton("BMM_PoweredOffLabel".Translate(), settings.overlayColors.powererOff, (SelectionColorWidget scw) => { settings.overlayColors.powererOff = scw.SelectedColor; });

                subsettings.AddColorPickerButton("BMM_MiningOverlayLabel".Translate(), settings.overlayColors.mining, (SelectionColorWidget scw) => { settings.overlayColors.mining = scw.SelectedColor; });

                foreach (IndicatorSettings settings in OverlaySettingDatabase.IndicatorSettings)
                {
                    // TODO: remove the need for this translate...(Def Injection)
                    subsettings.AddColorPickerButton(settings.label.Translate(), settings.color, (SelectionColorWidget scw) => {
                        settings.color = scw.SelectedColor;
                        settings.edgeColor = BetterMiniMapSettings.FadedColor(settings.color);
                        // TODO: handle faded/secondary color
                    });
                }

                subsettings.End();
            });

            listing_Standard.End();

            settings.Write();
        }

    }

}