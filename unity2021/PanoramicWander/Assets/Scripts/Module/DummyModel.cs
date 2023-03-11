
using System;
using LibMVCS = XTC.FMP.LIB.MVCS;

namespace XTC.FMP.MOD.PanoramicWander.LIB.Unity
{
    /// <summary>
    /// 虚拟数据
    /// </summary>
    public class DummyModel : DummyModelBase
    {
        public class ArchiveMetaSchema
        {
            public class Hotspot
            {
                public string link { get; set; } = "";
            }

            public class Scene
            {
                public string name { get; set; } = "";
                public string image { get; set; } = "";
                public float rotation { get; set; } = 0;
                public float frontMenuAngle { get; set; } = 0;
                public Hotspot[] hotspotS { get; set; } = new Hotspot[0];
            }

            public Scene[] sceneS { get; set; } = new Scene[0];
        }

        public class DummyStatus : DummyStatusBase
        {
        }

        public DummyModel(string _uid) : base(_uid)
        {
        }
    }
}

