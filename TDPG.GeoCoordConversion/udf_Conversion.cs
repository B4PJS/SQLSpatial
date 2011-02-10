using System;
using System.Text;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Types;
using TDPG.GeoCoordConversion;
using System.Collections.Generic;
using System.Numeric;

namespace B4PJS.Conversion
{
    public partial class UserDefinedFunctions
    {

        [SqlFunction()]
        public static SqlString udf_ConvertGridToCoord(long Easting, long Northing)
        {
            GridReference gridRef = new GridReference(Easting, Northing);
            PolarGeoCoordinate polarCoord = GridReference.ChangeToPolarGeo(gridRef);
            // Uncomment the line below if you want to change to use the WGS84 system
            polarCoord = PolarGeoCoordinate.ChangeCoordinateSystem(polarCoord, CoordinateSystems.WGS84);
            return polarCoord.Lon.ToString() + " " + polarCoord.Lat.ToString();
        }
        public static SqlGeography udf_GetGeographyFromGrid(long Easting, long Northing)
        {
            GridReference gridRef = new GridReference(Easting, Northing);
            PolarGeoCoordinate polarCoord = GridReference.ChangeToPolarGeo(gridRef);
            // Uncomment the line below if you want to change to use the WGS84 system
            polarCoord = PolarGeoCoordinate.ChangeCoordinateSystem(polarCoord, CoordinateSystems.WGS84);

            // use SqlGeographyBuilder to help create the SqlGeography type
             SqlGeographyBuilder geographyBuilder = new SqlGeographyBuilder();
             SqlGeography geography;

             // set the Spatial Reference Identifiers that will used to create the point
             geographyBuilder.SetSrid(4326);

             // state what type of geography object that I to create
             geographyBuilder.BeginGeography(OpenGisGeographyType.Point);

             // add the frist figure lat long point
             geographyBuilder.BeginFigure(polarCoord.Lat, polarCoord.Lon);

             // close the figure and geography class
             geographyBuilder.EndFigure();
             geographyBuilder.EndGeography();

             // get the geography builder to return the sqlgeography type
             geography = geographyBuilder.ConstructedGeography;

             return geography;
        }
        public static SqlDouble udf_ConvertGridToLat(long Easting, long Northing)
        {
            GridReference gridRef = new GridReference(Easting, Northing);
            PolarGeoCoordinate polarCoord = GridReference.ChangeToPolarGeo(gridRef);
            // Uncomment the line below if you want to change to use the WGS84 system
            polarCoord = PolarGeoCoordinate.ChangeCoordinateSystem(polarCoord, CoordinateSystems.WGS84);
            return polarCoord.Lat;
        }
        public static SqlDouble udf_ConvertGridToLon(long Easting, long Northing)
        {
            GridReference gridRef = new GridReference(Easting, Northing);
            PolarGeoCoordinate polarCoord = GridReference.ChangeToPolarGeo(gridRef);
            // Uncomment the line below if you want to change to use the WGS84 system
            polarCoord = PolarGeoCoordinate.ChangeCoordinateSystem(polarCoord, CoordinateSystems.WGS84);
            return polarCoord.Lon;
        }
        public static SqlGeography udf_GetGeographyFromCoord(double Latitude, double Longitude)
        {
           
            // use SqlGeographyBuilder to help create the SqlGeography type
            SqlGeographyBuilder geographyBuilder = new SqlGeographyBuilder();
            SqlGeography geography;

            // set the Spatial Reference Identifiers that will used to create the point
            geographyBuilder.SetSrid(4326);

            // state what type of geography object that I to create
            geographyBuilder.BeginGeography(OpenGisGeographyType.Point);

            // add the frist figure lat long point
            geographyBuilder.BeginFigure(Latitude, Longitude);

            // close the figure and geography class
            geographyBuilder.EndFigure();
            geographyBuilder.EndGeography();

            // get the geography builder to return the sqlgeography type
            geography = geographyBuilder.ConstructedGeography;

            return geography;
        }
        public static SqlGeography udf_GetGeogFromGeometry(SqlGeometry Geometry)
        {
            //StringBuilder polystring;// = new StringBuilder();
            string input;
            long east;
            long north;
            bool Multi = false;
            bool bracketOpen = false;
            bool bracketClose = false;
            List<string> polylist = new List<string>();
            

           Geometry.MakeValid();

            if (Geometry.ToString().StartsWith("POLYGON"))
            {
                input = Geometry.ToString().Replace("POLYGON ((", "").Replace("))", "").Replace(", ", ",");
               // polystring.Append("POLYGON ((");
                Multi = false;
            }
            else if (Geometry.ToString().StartsWith("MULTIPOLYGON"))
            {
                input = Geometry.ToString().Replace("MULTIPOLYGON ((", "").Replace(", ", ",");
                // polystring.Append("MULTIPOLYGON ((");
                input = input.Remove(input.Length - 2);
                Multi = true;
            }
            else
            {
                input = "";
            }
            
            string[] pairs = input.Split(',');
            foreach (string s in pairs)
            {
                bracketOpen = false;
                bracketClose = false;

                if (s.StartsWith("("))
                {
                  //  s.Replace("(", "");
                    bracketOpen = true;
                }
                if (s.EndsWith(")"))
                {
                   // s.Replace(")", "");
                    bracketClose = true;
                }

                string[] eastnorth = s.Replace("(", "").Replace(")","").Split(' ');
                try
                {

                    east = (long)double.Parse(eastnorth[0]);
                    north = (long)double.Parse(eastnorth[1]);

                    string coords = udf_ConvertGridToCoord(east, north).Value;

                    if (bracketOpen == true)
                    {
                        coords = "(" + coords;
                    }
                    if (bracketClose == true)
                    {
                        coords = coords + ")";
                    }
                    polylist.Add(coords);
                }
                catch (Exception ex)
                {
                    //return eastnorth[0].ToString() + " " + eastnorth[1].ToString();
                }
            }
            string[] polyarray = polylist.ToArray();
            SqlString output;
            if (Multi == true)
            {
                output = "MULTIPOLYGON ((" + String.Join(",", polyarray) + "))";
            }
            else
            {
                output = "POLYGON ((" + String.Join(",", polyarray) + "))";
            }
            SqlGeometry geom = SqlGeometry.STGeomFromText((SqlChars)output, 4326);
            //geom.MakeValid();
            //try
            //{
            //    geom = SqlGeometry.STGeomCollFromWKB(geom.STUnion(geom.STStartPoint()).STAsBinary(), 4326);
            //}
            //catch
            //{
                geom = SqlGeometry.STGeomFromWKB(geom.MakeValid().STUnion(geom.MakeValid().STStartPoint()).STAsBinary(), 4326);
            //}

                //.STBuffer(0.00001).STBuffer(-0.00001)
                geom = SqlGeometry.STGeomFromWKB(geom.MakeValid().Reduce(0.0001).MakeValid().STBuffer(0.00001).MakeValid().STAsBinary(), 4326);
                try
                {
                    SqlGeography geog = SqlGeography.STGeomFromWKB(geom.MakeValid().STAsBinary(), 4326);
                    return geog;
                }

                catch
                {
                    SqlGeography retval = SqlGeography.Null;
                    return retval;
                }
        }


    }
}