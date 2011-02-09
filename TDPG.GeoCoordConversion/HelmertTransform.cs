using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDPG.GeoCoordConversion
{
    internal class HelmertTransform
    {
        //Metres
        internal double tx { get; private set; }
        internal double ty { get; private set; }
        internal double tz { get; private set; }

        //Seconds
        internal double rx { get; private set; }
        internal double ry { get; private set; }
        internal double rz { get; private set; }

        //Parts per million
        internal double s { get; private set; }

        internal CoordinateSystems outputCoordinateSystem { get; private set; }

        private HelmertTransform(double _tx, double _ty, double _tz, double _rx, double _ry, double _rz, double _s, CoordinateSystems _outputCoordinateSystem)
        {
            tx = _tx;
            ty = _ty;
            tz = _tz;
            rx = _rx;
            ry = _ry;
            rz = _rz;
            s = _s;
            outputCoordinateSystem = _outputCoordinateSystem;
        }

        public enum HelmertTransformType { WGS84toOSGB36, OSGB36toWGS84 }

        public static HelmertTransform GetTransform(HelmertTransformType type)
        {
            return m_Transforms[type];
        }

        private static readonly Dictionary<HelmertTransformType, HelmertTransform> m_Transforms = new Dictionary<HelmertTransformType, HelmertTransform>
        {
           {HelmertTransformType.WGS84toOSGB36, new HelmertTransform(-446.448,125.157,-542.060,-0.1502,-0.2470,-0.8421,20.4894,CoordinateSystems.OSGB36)},
            {HelmertTransformType.OSGB36toWGS84, new HelmertTransform(446.448,-125.157,542.060,0.1502,0.2470,0.8421,-20.4894,CoordinateSystems.WGS84)}
        };
    }
}
