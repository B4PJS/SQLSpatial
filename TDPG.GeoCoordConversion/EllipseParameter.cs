using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDPG.GeoCoordConversion
{
    /// <summary>
    /// http://en.wikipedia.org/wiki/Ellipse#Elements_of_an_ellipse
    /// </summary>
    internal class EllipseParameter
    {
        /// <summary>
        /// a in original source code
        /// </summary>
        internal double SemimajorAxis { get; set; }

        /// <summary>
        /// b in original source code
        /// </summary>
        internal double SemiMinorAxis { get; set; }

        /// <summary>
        /// f in original source code
        /// </summary>
        internal double Eccentricity { get; set; }

        internal EllipseParameter(double semimajorAxis, double semiMinorAxis, double eccentricity)
        {
            SemiMinorAxis = semiMinorAxis;
            SemimajorAxis = semimajorAxis;
            Eccentricity = eccentricity;
        }

        internal enum Ellipses { WGS84, Airy1830, Airy1849 }

        internal static EllipseParameter GetEllipseParameters(Ellipses type)
        {
            return m_Transforms[type];
        }

        private static readonly Dictionary<Ellipses, EllipseParameter> m_Transforms = new Dictionary<Ellipses, EllipseParameter>
        {
           {Ellipses.WGS84, new EllipseParameter(6378137,6356752.3142,1/298.257223563)},
            {Ellipses.Airy1830, new EllipseParameter(6377563.396,6356256.910,1d/299.3249646)},
            {Ellipses.Airy1849, new EllipseParameter(6377340.189,6356034.447,1d/299.3249646)}
        };
    }
}
