using System.Globalization;

namespace OrderManagement.Helper
{
    public class MetadataInfo
    {
        public DateTime? AcquisitionDate { get; set; }
        public decimal? Resolution { get; set; }
        public decimal? CloudRate { get; set; }

        public decimal? SunElevation { get; set; }
        public decimal? SunAzimuth { get; set; }
        public decimal? OffNadirAngle { get; set; }

        public string? Satellite { get; set; }
        public string? Sensor { get; set; }
        public string? ProcessingLevel { get; set; }

        public string? ImageType { get; set; }
        public string? PanSharpenAlgorithm { get; set; }

        public string? City { get; set; }
        public string? District { get; set; }
        public string? Provider { get; set; }
    }

    public static class MetadataParser
    {
        public static MetadataInfo Parse(string path)
        {
            if (!File.Exists(path))
                return new MetadataInfo();

            var dict = ReadKeyValues(path);

            var bandId = Get(dict, "bandId");
            var productLevel = Get(dict, "productLevel", "PROCESSING_LEVEL");
            var panSharpenAlgorithm = Get(dict, "panSharpenAlgorithm");

            return new MetadataInfo
            {
                AcquisitionDate = ToDate(dict,
                    "ACQUISITION_DATE",
                    "earliestAcqTime",
                    "latestAcqTime",
                    "firstLineTime"),

                Resolution = ToDecimal(dict,
                    "RESOLUTION",
                    "productGSD",
                    "meanCollectedGSD",
                    "colSpacing",
                    "rowSpacing"),

                CloudRate = ToDecimal(dict,
                    "CLOUD_COVER",
                    "cloudCover"),

                SunElevation = ToDecimal(dict,
                    "SUN_ELEVATION",
                    "meanSunEl",
                    "maxSunEl",
                    "minSunEl"),

                SunAzimuth = ToDecimal(dict,
                    "SUN_AZIMUTH",
                    "meanSunAz",
                    "maxSunAz",
                    "minSunAz"),

                OffNadirAngle = ToDecimal(dict,
                    "OFF_NADIR",
                    "meanOffNadirViewAngle",
                    "maxOffNadirViewAngle",
                    "minOffNadirViewAngle"),

                Satellite = Get(dict,
                    "SATELLITE",
                    "SPACECRAFT_NAME",
                    "satId",
                    "imageDescriptor"),

                Sensor = Get(dict,
                    "SENSOR",
                    "INSTRUMENT",
                    "bandId",
                    "mode"),

                ProcessingLevel = NormalizeProcessingLevel(productLevel),

                ImageType = ResolveImageType(bandId, productLevel, panSharpenAlgorithm),

                PanSharpenAlgorithm = panSharpenAlgorithm,

                City = Get(dict,
                    "CITY",
                    "LOCATION",
                    "PROVINCE"),

                District = Get(dict,
                    "DISTRICT",
                    "TOWN",
                    "COUNTY"),

                Provider = Get(dict,
                    "PROVIDER",
                    "SOURCE",
                    "DATA_PROVIDER")
            };
        }

        private static Dictionary<string, string> ReadKeyValues(string path)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var rawLine in File.ReadAllLines(path))
            {
                var line = rawLine.Trim();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith("BEGIN_GROUP", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (line.StartsWith("END_GROUP", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (line.Equals("END;", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!line.Contains("="))
                    continue;

                var parts = line.Split('=', 2);

                var key = parts[0].Trim();
                var value = CleanValue(parts[1]);

                if (!string.IsNullOrWhiteSpace(key))
                    dict[key] = value;
            }

            return dict;
        }

        private static string CleanValue(string value)
        {
            return value
                .Trim()
                .TrimEnd(';')
                .Trim()
                .Trim('"')
                .Trim();
        }

        private static string? Get(Dictionary<string, string> d, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (d.TryGetValue(key, out var val) && !string.IsNullOrWhiteSpace(val))
                    return val;
            }

            return null;
        }

        private static decimal? ToDecimal(Dictionary<string, string> d, params string[] keys)
        {
            var val = Get(d, keys);

            if (string.IsNullOrWhiteSpace(val))
                return null;

            val = val.Replace(",", ".");

            return decimal.TryParse(
                val,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var result)
                    ? result
                    : null;
        }

        private static DateTime? ToDate(Dictionary<string, string> d, params string[] keys)
        {
            var val = Get(d, keys);

            if (string.IsNullOrWhiteSpace(val))
                return null;

            return DateTime.TryParse(
                val,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var result)
                    ? result
                    : null;
        }

        private static string? NormalizeProcessingLevel(string? level)
        {
            if (string.IsNullOrWhiteSpace(level))
                return null;

            return level.ToUpperInvariant() switch
            {
                "LV2A" => "L2A",
                "LV1B" => "L1B",
                _ => level
            };
        }

        private static string ResolveImageType(string? bandId, string? productLevel, string? panSharpenAlgorithm)
        {
            var band = bandId?.ToUpperInvariant() ?? "";
            var level = NormalizeProcessingLevel(productLevel) ?? "";

            var isPansharpened = !string.IsNullOrWhiteSpace(panSharpenAlgorithm)
                                  && !panSharpenAlgorithm.Equals("NONE", StringComparison.OrdinalIgnoreCase)
                                  && !panSharpenAlgorithm.Equals("OFF", StringComparison.OrdinalIgnoreCase);

            var type = band switch
            {
                var x when x.Contains("PAN") => "Panchromatic",
                var x when x.Contains("BGR") => "Multispectral",
                var x when x.Contains("RGB") => "Multispectral",
                var x when x.Contains("MS") => "Multispectral",
                var x when x == "N" || x.Contains("NIR") => "Near Infrared",
                _ => "Satellite Image"
            };

            if (isPansharpened)
                type = $"Pansharpened {type}";

            if (!string.IsNullOrWhiteSpace(level))
                type = $"{type} {level}";

            return type;
        }
    }
}