
using System.Xml.Serialization;

namespace XTC.FMP.MOD.PanoramicWander.LIB.Unity
{
    /// <summary>
    /// 配置类
    /// </summary>
    public class MyConfig : MyConfigBase
    {
        public class FootMenu
        {
            [XmlAttribute("visible")]
            public bool visible { get; set; } = false;
            [XmlAttribute("offset")]
            public float offset { get; set; } = 0;
            [XmlAttribute("scale")]
            public float scale { get; set; } = 1;
        }

        public class HeadMenu
        {
            [XmlAttribute("visible")]
            public bool visible { get; set; } = false;
            [XmlAttribute("offset")]
            public float offset { get; set; } = 0;
            [XmlAttribute("scale")]
            public float scale { get; set; } = 1;
        }

        public class FrontMenu
        {
            [XmlAttribute("visible")]
            public bool visible { get; set; } = false;
            [XmlAttribute("offset")]
            public float offset { get; set; } = 0;
            [XmlAttribute("scale")]
            public float scale { get; set; } = 1;
            [XmlAttribute("width")]
            public int width { get; set; } = 300;
            [XmlAttribute("expand")]
            public int expand { get; set; } = 10;
            [XmlAttribute("collapse")]
            public int collapse { get; set; } = 30;
        }

        public class SwitchEffect
        {
            [XmlAttribute("active")]
            public string active { get; set; } = "Clip";
            [XmlAttribute("duration")]
            public float duration { get; set; } = 1.5f;
        }

        public class Hotspot
        {
            [XmlAttribute("linkNormalColor")]
            public string linkNormalColor { get; set; } = "#B3DAFFFF";
            [XmlAttribute("linkActivatedColor")]
            public string linkActivatedColor { get; set; } = "#99FFBEFF";

        }

        public class Debug
        {
            [XmlAttribute("active")]
            public bool active { get; set; } = false;
            [XmlAttribute("rendererRotationY")]
            public float rendererRotationY { get; set; } = 0.5f;
        }

        public class Style
        {
            [XmlAttribute("name")]
            public string name { get; set; } = "";
            [XmlElement("Debug")]
            public Debug debug { get; set; } = new Debug();
            [XmlElement("HeadMenu")]
            public HeadMenu headMenu { get; set; } = new HeadMenu();
            [XmlElement("FootMenu")]
            public FootMenu footMenu { get; set; } = new FootMenu();
            [XmlElement("FrontMenu")]
            public FrontMenu frontMenu { get; set; } = new FrontMenu();
            [XmlElement("SwitchEffect")]
            public SwitchEffect switchEffect { get; set; } = new SwitchEffect();
            [XmlElement("Hotspot")]
            public Hotspot hotspot { get; set; } = new Hotspot();
        }


        [XmlArray("Styles"), XmlArrayItem("Style")]
        public Style[] styles { get; set; } = new Style[0];
    }
}

