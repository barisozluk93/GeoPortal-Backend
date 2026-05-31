namespace OrderManagement.Helper
{
    using NetTopologySuite;
    using NetTopologySuite.Geometries;
    using NetTopologySuite.IO;

    public static class ShapeReader
    {
        public static Geometry? Read(string shpPath)
        {
            if (!File.Exists(shpPath))
                return null;

            var factory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            using var reader = new ShapefileDataReader(shpPath, factory);

            var geometries = new List<Geometry>();

            while (reader.Read())
            {
                var geom = reader.Geometry;

                if (geom != null)
                    geometries.Add(geom);
            }

            if (!geometries.Any())
                return null;

            var merged = factory.BuildGeometry(geometries);

            merged.SRID = 4326;

            return merged;
        }
    }
}
