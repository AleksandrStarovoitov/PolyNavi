using Itinero.LocalGeo;
using Mapsui.Geometries;
using Mapsui.Projection;

namespace PolyNavi.Extensions
{
    public static class MapsUiExtensions
    {
        public static Point FromLonLat(this Point point)
        {
            return SphericalMercator.FromLonLat(point.X, point.Y);
        }

        public static Point ToWorld(this Coordinate point)
        {
            return SphericalMercator.FromLonLat(point.Longitude, point.Latitude);
        }
    }
}
