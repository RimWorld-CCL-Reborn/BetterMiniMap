using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using Verse;

using static BetterMiniMap.BetterMiniMapSettings;

// TODO: better names?
namespace BetterMiniMap
{
    public class IndicatorProps
    {
        public string name;         // short. used in saving
        public string label;        // longer. informs user in settings
        public Selector selector;
        public Color color;
        public Color edgeColor;
        public float radius;
    }

    public class IndicatorMappings
    {
        public List<IndicatorProps> mappings;

        public delegate IndicatorSettings IndicatorSettingsGetter(object o, Map map);
        private IndicatorSettingsGetter indicatorGetter;

        // uses mappings which were baked into indicatorGetter
        public IndicatorSettingsGetter GetIndicatorSettings
        {
            get
            {
                if (indicatorGetter == null)
                    indicatorGetter = (object o, Map map) => OverlaySettingDatabase.GetIndicatorSettings(this.mappings.First((IndicatorProps props) => props.selector.IsValid(o, map)).name);
                return indicatorGetter;
            }
        }
    }

    public class OverlayDef : Def
    {
        public Type overlayClass;
        public int updatePeriod;
        public List<Selector> selectors;
        public IndicatorMappings indicatorMappings;
        public bool visible = true;
        public ThingRequestGroup requestGroup; // should be not null for ThingOverlay
        public int priority = 0;
 
        public bool IsValid(object o, Map map) => this.selectors.All(s => s.IsValid(o, map));

        // NOTE: all new variables need custom XML writing here.
        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            this.defName = xmlRoot.SelectSingleNode("defName").InnerText;
            this.label = xmlRoot.SelectSingleNode("label").InnerText;
            this.description = xmlRoot.SelectSingleNode("description").InnerText;
            this.overlayClass = Type.GetType(xmlRoot.SelectSingleNode("overlayClass").InnerText);
            this.updatePeriod = Verse.DirectXmlToObject.ObjectFromXml<int>(xmlRoot.SelectSingleNode("updatePeriod"), true);
            this.indicatorMappings = Verse.DirectXmlToObject.ObjectFromXml<IndicatorMappings>(xmlRoot.SelectSingleNode("indicatorMappings"), true);

            XmlNode node = xmlRoot.SelectSingleNode("requestGroup");
            if (node != null)
                this.requestGroup = Verse.DirectXmlToObject.ObjectFromXml<ThingRequestGroup>(node, true);

            node = xmlRoot.SelectSingleNode("priority");
            if (node != null)
                this.priority = Verse.DirectXmlToObject.ObjectFromXml<int>(node, true);

            node = xmlRoot.SelectSingleNode("visible");
            if (node != null)
                this.visible = Verse.DirectXmlToObject.ObjectFromXml<bool>(node, true);

            node = xmlRoot.SelectSingleNode("selectors");
            if (this.ValidateClasses(node))
                this.selectors = Verse.DirectXmlToObject.ObjectFromXml<List<Selector>>(node, true);
            else
                this.selectors = null;
        }

        private bool ValidateClasses(XmlNode selectorsNode)
        {
            foreach (XmlNode xmlNode in selectorsNode.ChildNodes)
            {
                if (Type.GetType(xmlNode.Attributes["Class"].InnerText) == typeof(BetterMiniMap.ClassSelector))
                {
                    Type typeInAnyAssembly = GenTypes.GetTypeInAnyAssembly(xmlNode.InnerText);
                    if (typeInAnyAssembly == null)
                        return false;
                }
            }
            return true;
        }
    }

}
