using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using Verse;

// TODO: better names?
namespace BetterMiniMap
{
    public class IndicatorProps
    {
        public Selector selector;
        public Color color;
        private Color? edgeColor = null;
        public float radius;

        public Color EdgeColor
        {
            get
            {
                if (this.edgeColor == null)
                    this.edgeColor = BetterMiniMapSettings.FadedColor(this.color);
                return (Color)this.edgeColor;
            }
        }
    }

    public class IndicatorMapper
    {
        //public Selector selector; // eh transforms?
        public List<IndicatorProps> mappings;

        public delegate IndicatorProps GetIndicatorProps(object o);
        private GetIndicatorProps indicatorMapper;

        public GetIndicatorProps Mapper
        {
            get
            {
                if (indicatorMapper == null)
                    indicatorMapper = (object o) => this.mappings.First((IndicatorProps props) => props.selector.IsValid(o));
                return indicatorMapper;
            }
        }
    }

    public class OverlayDef : Def
    {
        public Type overlayClass;
        public int updatePeriod;
        public List<Selector> selectors;
        public IndicatorMapper indicatorMappings;
        public bool disabled;

        public bool IsValid(object o) => this.selectors.All(s => s.IsValid(o));

        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            this.defName = xmlRoot.SelectSingleNode("defName").InnerText;
            this.label = xmlRoot.SelectSingleNode("label").InnerText;
            this.description = xmlRoot.SelectSingleNode("description").InnerText;
            this.overlayClass = Type.GetType(xmlRoot.SelectSingleNode("overlayClass").InnerText);
            this.updatePeriod = Verse.DirectXmlToObject.ObjectFromXml<int>(xmlRoot.SelectSingleNode("updatePeriod"), true);
            this.indicatorMappings = Verse.DirectXmlToObject.ObjectFromXml<IndicatorMapper>(xmlRoot.SelectSingleNode("indicatorMappings"), true);

            if (this.ValidateClasses(xmlRoot.SelectSingleNode("selectors")))
                this.selectors = Verse.DirectXmlToObject.ObjectFromXml<List<Selector>>(xmlRoot.SelectSingleNode("selectors"), true);
            else
                this.disabled = true;
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
