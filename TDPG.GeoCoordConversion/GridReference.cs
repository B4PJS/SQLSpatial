﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDPG.GeoCoordConversion
{
    public class GridReference
    {
        public long Northing { get; set; }
        public long Easting { get; set; }

        public GridReference(long easting, long northing)
        {
            Northing = northing;
            Easting = easting;
        }

        public static PolarGeoCoordinate ChangeToPolarGeo(GridReference original)
        {
            return Converter.GridReferenceToGeodesic(original);
        }

        public bool IsTheSameAs(GridReference compareTo)
        {
            if (
                compareTo.Northing != this.Northing
                || compareTo.Easting != this.Easting
                )
                return false;
            else
                return true;
        }

        //overload of IsTheSameAs to ignore rounding errors on final digit
        public bool IsTheSameAs(GridReference compareTo, bool ignoreFinalDigit)
        {
            if (ignoreFinalDigit)
            {                
                AlignSigFigs(compareTo);
                GridReference compareToReduced = ReduceSigFigsBy1(compareTo);
                GridReference comparer = ReduceSigFigsBy1(this);

                return comparer.IsTheSameAs(compareToReduced);
            }

            return IsTheSameAs(compareTo);
        }

        internal void AlignSigFigs(GridReference source)
        {
            int SigFigsSrc;
            int SigFigsDest;


            SigFigsSrc = Converter.GetSigFigs(source.Easting);
            SigFigsDest = Converter.GetSigFigs(this.Easting);

            this.Easting = (int)Converter.SetSigFigs(
                 (double)this.Easting,
                 SigFigsDest < SigFigsSrc ? SigFigsDest : SigFigsSrc);

            source.Easting = (int)Converter.SetSigFigs(
                  (double)source.Easting,
                 SigFigsDest < SigFigsSrc ? SigFigsDest : SigFigsSrc);

            SigFigsSrc = Converter.GetSigFigs(source.Northing);
            SigFigsDest = Converter.GetSigFigs(this.Northing);

            this.Northing = (int)Converter.SetSigFigs(
                  (double)this.Northing,
                 SigFigsDest < SigFigsSrc ? SigFigsDest : SigFigsSrc);

            source.Northing = (int)Converter.SetSigFigs(
                  (double)source.Northing,
                 SigFigsDest < SigFigsSrc ? SigFigsDest : SigFigsSrc);
        }

        private static GridReference ReduceSigFigsBy1(GridReference original)
        {
            double Northing = Converter.SetSigFigs(
                 original.Northing,
                 Converter.GetSigFigs(original.Northing) - 1);

            double Easting = Converter.SetSigFigs(
                 original.Easting,
                 Converter.GetSigFigs(original.Easting) - 1);

            return new GridReference((int)Easting, (int)Northing);
        }
    }
}
