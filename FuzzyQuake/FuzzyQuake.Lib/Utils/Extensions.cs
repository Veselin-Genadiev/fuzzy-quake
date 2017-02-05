using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyQuake.Lib.Utils
{
    public static class Extensions
    {
        public static bool IsWithin(this GeoCoordinate pt, GeoCoordinate sw, GeoCoordinate ne)
        {
            return pt.Latitude >= sw.Latitude &&
                   pt.Latitude <= ne.Latitude &&
                   pt.Longitude >= sw.Longitude &&
                   pt.Longitude <= ne.Longitude;
        }

        public static double MinDistanceToRectangle(this GeoCoordinate pt, GeoCoordinate sw, GeoCoordinate ne)
        {
            if (IsWithin(pt, sw, ne))
            {
                return 0;
            }

            double targetLatitude = Math.Min(Math.Max(pt.Latitude, sw.Latitude), ne.Latitude);
            double targetLongitude = Math.Min(Math.Max(pt.Longitude, sw.Longitude), ne.Longitude);
            return pt.GetDistanceTo(new GeoCoordinate(targetLatitude, targetLongitude));
        }
    }
}
