using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDPG.GeoCoordConversion.Test
{
    internal class GeoTestDataSet
    {
        internal string City { get; set; }
        internal PolarGeoCoordinate OSGB36 { get; set; }
        internal PolarGeoCoordinate WGS84 { get; set; }
        internal GridReference NE { get; set; }

        internal GeoTestDataSet(string city, PolarGeoCoordinate oSGB36, PolarGeoCoordinate wGS84, GridReference nE)
        {
            City = city;
            OSGB36 = oSGB36;
            WGS84 = wGS84;
            NE = nE;
        }
    }
}
