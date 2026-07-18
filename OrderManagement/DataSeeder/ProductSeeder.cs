using Microsoft.EntityFrameworkCore;
using OrderManagement.DbContexts;
using OrderManagement.Entity;
using OrderManagement.Helper;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace OrderManagement.Seed;

public static class ProductSeeder
{
    private const string ImageTypeMono = "Mono";
    private const string ImageTypeStereo = "Stereo";

    public static async Task SeedAsync(
        OrderManagementContext db,
        IWebHostEnvironment env,
        IConfiguration configuration)
    {
        await SeedApiKeyAsync(db);

        var dataRoot = Path.Combine(env.ContentRootPath, "Data");

        if (!Directory.Exists(dataRoot))
            return;

        var publicBaseUrl =
            configuration["AppSettings:PublicBaseUrl"]?.TrimEnd('/')
            ?? "http://localhost:5065";

        var addedCount = 0;
        var updatedCount = 0;
        var skippedCount = 0;

        foreach (var productDir in Directory.GetDirectories(dataRoot))
        {
            var imageDir = Path.Combine(productDir, "IMAGE");
            var metadataPath = FindMetadataFile(imageDir);
            var shpPath = FindShpFile(productDir);

            var meta = !string.IsNullOrWhiteSpace(metadataPath)
                ? ReadMetadataKeyValues(metadataPath)
                : new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

            var dbf = !string.IsNullOrWhiteSpace(shpPath)
                ? DbfAttributeReader.ReadFirst(shpPath)
                : new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

            var imageId = GetDbfAny(dbf, "IMAGEID", "IMAGE_ID");

            if (string.IsNullOrWhiteSpace(imageId))
            {
                skippedCount++;
                continue;
            }

            var product = await db.Products
                .Include(x => x.Classes)
                .FirstOrDefaultAsync(x =>
                    x.ImageId == imageId &&
                    x.CategoryId == 1);

            if (product == null)
            {
                var price = Random.Shared.Next(5000, 20000);

                product = new Product
                {
                    ImageId = imageId,
                    Name = $"Uydu Görüntüsü - {imageId}",
                    Price = price,
                    PriceStr = $"₺{price}",
                    Currency = "TRY",
                    CategoryId = 1,
                    IsDeleted = false,
                    IsInMarket = true,
                    IsCustomArea = false,
                    Description = "Seed edilmiş uydu görüntüsü",
                    IsClassified = true,
                    Classes = GenerateRandomClasses()
                };

                await db.Products.AddAsync(product);
                addedCount++;
            }
            else
            {
                updatedCount++;

                if (product.Classes == null || !product.Classes.Any())
                    product.Classes = GenerateRandomClasses();
            }

            ApplyFiles(product, dataRoot, productDir, publicBaseUrl, metadataPath, shpPath);
            ApplyDbfThenMetadata(product, dbf, meta);
            product.IsNVDIAnalysis = DetectNdviAnalysis(productDir, dbf, meta);
            ApplyGeometry(product, shpPath);

            product.ImageId = imageId;
            product.Name = BuildProductName(product, imageId);

            product.DownloadLink = !string.IsNullOrWhiteSpace(product.GeoTiffPath)
                ? ToPublicFileUrl(dataRoot, product.GeoTiffPath, publicBaseUrl)
                : ToPublicFileUrl(dataRoot, productDir, publicBaseUrl);
        }

        await db.SaveChangesAsync();

        Console.WriteLine($"ProductSeeder tamamlandı. Added: {addedCount}, Updated: {updatedCount}, Skipped: {skippedCount}");
    }

    private static void ApplyFiles(
        Product product,
        string dataRoot,
        string productDir,
        string publicBaseUrl,
        string? metadataPath,
        string? shpPath)
    {
        var imageDir = Path.Combine(productDir, "IMAGE");

        product.FootprintPath = shpPath;
        product.MetadataPath = metadataPath;

        var dbfPath = !string.IsNullOrWhiteSpace(shpPath)
            ? Path.ChangeExtension(shpPath, ".dbf")
            : null;

        product.PropertyUrl = !string.IsNullOrWhiteSpace(dbfPath) && File.Exists(dbfPath)
            ? ToPublicFileUrl(dataRoot, dbfPath, publicBaseUrl)
            : null;

        product.MetadataUrl = !string.IsNullOrWhiteSpace(metadataPath)
            ? ToPublicFileUrl(dataRoot, metadataPath, publicBaseUrl)
            : null;

        if (Directory.Exists(imageDir))
        {
            product.GeoTiffPath = FindFirstByExtensions(
                imageDir,
                ".tif", ".tiff", ".geotiff");

            var preview = FindPreviewFile(imageDir);

            if (!string.IsNullOrWhiteSpace(preview))
            {
                product.PreviewPath = preview;
                product.PreviewUrl = ToPublicFileUrl(dataRoot, preview, publicBaseUrl);
            }

            var thumbnail = FindThumbnailFile(imageDir);

            if (!string.IsNullOrWhiteSpace(thumbnail))
                product.ThumbnailUrl = ToPublicFileUrl(dataRoot, thumbnail, publicBaseUrl);
        }

        var quicklook = FindQuicklookFile(productDir);

        if (!string.IsNullOrWhiteSpace(quicklook))
        {
            product.QuicklookPath = quicklook;

            // Preview yoksa Quicklook'u kullan
            if (string.IsNullOrWhiteSpace(product.PreviewPath))
                product.PreviewPath = quicklook;

            if (string.IsNullOrWhiteSpace(product.PreviewUrl))
                product.PreviewUrl = ToPublicFileUrl(dataRoot, quicklook, publicBaseUrl);
        }
    }

    private static void ApplyDbfThenMetadata(
        Product product,
        IReadOnlyDictionary<string, string?> dbf,
        IReadOnlyDictionary<string, string?> meta)
    {
        product.OrderId = FirstNotEmpty(
            GetDbfAny(dbf, "ORDER_ID", "ORDERID"),
            GetMetaAny(meta, "productOrderId", "orderId", "sceneID", "sceneId"),
            product.OrderId);

        product.CatalogId = FirstNotEmpty(
            GetDbfAny(dbf, "CATALOGID", "CATALOG_ID"),
            GetMetaAny(meta, "productCatalogId", "CatId", "catalogId", "productID", "productId"),
            product.CatalogId);

        product.ImageDescriptor = FirstNotEmpty(
            GetDbfAny(dbf, "IMAGE_DESC", "IMAGEDESC"),
            GetMetaAny(meta, "imageDescriptor", "imageName"),
            product.ImageDescriptor);

        product.Satellite = FirstNotEmpty(
            GetDbfAny(dbf, "PLATFORM", "SATELLITE", "SOURCE"),
            GetMetaAny(meta, "satelliteID", "satelliteId", "satId", "satellite"),
            product.Satellite);

        product.Sensor = FirstNotEmpty(
            GetDbfAny(dbf, "SENSOR", "SENS_MODE", "SENSORID"),
            GetMetaAny(meta, "sensorID", "sensorId", "sensor"),
            product.Sensor);

        product.ImageType = NormalizeImageType(FirstNotEmpty(
            GetDbfAny(dbf, "IMAGETYPE", "IMAGE_TYPE"),
            product.ImageType));

        product.SensorMode = FirstNotEmpty(
            GetDbfAny(dbf, "SENSOR_MODE", "SENS_MODE"),
            GetMetaAny(meta, "instrumentMode", "mode", "sensorMode"),
            product.SensorMode);

        product.BandId = FirstNotEmpty(
            GetDbfAny(dbf, "BANDS", "BANDID", "BAND_ID"),
            GetMetaAny(meta, "bands", "bandId"),
            product.BandId);

        product.ProductType = FirstNotEmpty(
            GetDbfAny(dbf, "PRODUCTTYPE", "PRODUCT_TYPE"),
            GetMetaAny(meta, "productType"),
            product.ProductType);

        product.ProductLevel = FirstNotEmpty(
            GetDbfAny(dbf, "PRODUCTLEVEL", "PRODUCT_LEVEL"),
            GetMetaAny(meta, "productLevel"),
            product.ProductLevel);

        product.ProcessingLevel = FirstNotEmpty(
            product.ProductLevel,
            product.ProcessingLevel);

        product.RadiometricLevel = FirstNotEmpty(
            GetDbfAny(dbf, "RADIOMETRIC", "RADIOMETRIC_LEVEL"),
            GetMetaAny(meta, "radiometricLevel"),
            product.RadiometricLevel);

        product.OutputFormat = FirstNotEmpty(
            GetDbfAny(dbf, "OUTPUTFORMAT", "OUTPUT_FORMAT", "FORMAT"),
            GetMetaAny(meta, "PgProductFormat", "outputFormat"),
            product.OutputFormat);

        product.SpatialReference = FirstNotEmpty(
            GetDbfAny(dbf, "SPATIALREF", "SPATIAL_REF", "SRS"),
            BuildSpatialReferenceFromMeta(meta),
            product.SpatialReference);

        product.ScanDirection = FirstNotEmpty(
            GetDbfAny(dbf, "DIRECTION", "SCAN_DIR"),
            GetMetaAny(meta, "direction", "scanDirection"),
            product.ScanDirection);

        product.StripId = FirstNotEmpty(
            GetDbfAny(dbf, "STRIP_ID", "STRIPID"),
            GetMetaAny(meta, "orbitID", "RawdataID"),
            product.StripId);

        product.DataOwner = FirstNotEmpty(
            GetDbfAny(dbf, "DATA_OWNER", "OWNER"),
            GetMetaAny(meta, "dataOwner", "owner"),
            product.DataOwner);

        product.Provider = FirstNotEmpty(
            GetDbfAny(dbf, "PROVIDER", "VENDOR", "DATA_OWNER", "OWNER", "COMPANY", "AGENCY", "SUPPLIER"),
            GetMetaAny(meta, "provider", "vendor", "dataOwner", "owner", "company", "agency", "supplier"),
            product.Provider);

        product.AcquisitionDate = FirstDate(
            ParseDate(GetDbfAny(dbf, "COLL_DATE", "ACQ_DATE", "ACQUISITION_DATE")),
            ParseDate(GetMetaAny(meta, "Scene_imagingStartTime", "productDate", "generationTime", "acquisitionDate")),
            product.AcquisitionDate);

        product.AcquisitionStartDate = FirstDate(
            ParseDate(GetDbfAny(dbf, "COLL_DATE", "ACQ_DATE", "ACQUISITION_DATE")),
            ParseDate(GetMetaAny(meta, "Scene_imagingStartTime", "earliestAcqTime", "firstLineTime", "acquisitionStartDate")),
            product.AcquisitionStartDate);

        product.AcquisitionEndDate = FirstDate(
            ParseDate(GetMetaAny(meta, "Scene_imagingStopTime", "latestAcqTime", "acquisitionEndDate")),
            product.AcquisitionEndDate);

        product.Resolution = FirstDecimal(
            ParseDecimal(GetDbfAny(dbf, "GSD", "RESOLUTION")),
            ParseDecimal(GetMetaAny(meta, "sensorGSD", "productGSD", "meanCollectedGSD", "PixelSpacing", "resolution")),
            product.Resolution);

        product.CloudRate = FirstDecimal(
            ParseDecimal(GetDbfAny(dbf, "CLOUDRATE", "CLOUDS", "CLOUD_COVER")),
            ParseDecimal(GetMetaAny(meta, "cloudCover", "cloudCoverQuote", "cloudRate")),
            product.CloudRate);

        product.OffNadirAngle = FirstDecimal(
            ParseDecimal(GetDbfAny(dbf, "AVRNADIR", "AVR_NADIR", "NADIR_ANGL", "OFFNADIR")),
            ParseDecimal(GetMetaAny(meta, "satOffNadir", "meanOffNadirViewAngle", "offNadirAngle")),
            product.OffNadirAngle);

        product.SunAzimuth = FirstDecimal(
            ParseDecimal(GetDbfAny(dbf, "AVRSUNAZM", "AVRSUNAZIMUTH", "SUN_AZIMUTH", "SUN_ANGLE", "AZIM_ANGLE")),
            ParseDecimal(GetMetaAny(meta, "sunAzimuth", "meanSunAz", "maxSunAz", "minSunAz")),
            product.SunAzimuth);

        product.SunElevation = FirstDecimal(
            ParseDecimal(GetDbfAny(dbf, "AVRSUNELEV", "AVRSUNELE", "AVRSUNELEVATION", "SUN_ELEVATION", "SUN_ELEV")),
            ParseDecimal(GetMetaAny(meta, "sunElevation", "meanSunEl", "maxSunEl", "minSunEl")),
            product.SunElevation);

        product.IsPansharpened = FirstBool(
            HasPanSharpen(GetMetaAny(meta, "FusionMethod", "panSharpenAlgorithm")),
            product.IsPansharpened);

        product.IsOrthorectified = FirstBool(
            IsOrthorectifiedLevel(product.ProductLevel),
            product.IsOrthorectified);

        product.SourceLabel = FirstNotEmpty(
            product.ProductType,
            product.Sensor,
            product.Satellite,
            product.SourceLabel);

        product.Provider = NullIfEmpty(product.Provider);
        product.DataOwner = NullIfEmpty(product.DataOwner);
        product.City = NullIfEmpty(product.City);
        product.District = NullIfEmpty(product.District);
    }

    private static void ApplyGeometry(Product product, string? shpPath)
    {
        if (string.IsNullOrWhiteSpace(shpPath) || !File.Exists(shpPath))
            return;

        var geom = ShapeReader.Read(shpPath);

        if (geom == null || geom.IsEmpty)
            return;

        product.Geometry = geom;

        var bbox = geom.EnvelopeInternal;

        product.BboxMinX = Math.Round((decimal)bbox.MinX, 6);
        product.BboxMinY = Math.Round((decimal)bbox.MinY, 6);
        product.BboxMaxX = Math.Round((decimal)bbox.MaxX, 6);
        product.BboxMaxY = Math.Round((decimal)bbox.MaxY, 6);

        var widthKm = Math.Abs(bbox.MaxX - bbox.MinX) * 111;
        var heightKm = Math.Abs(bbox.MaxY - bbox.MinY) * 111;

        product.AreaKm2 = Math.Round(
            (decimal)(widthKm * heightKm),
            2,
            MidpointRounding.AwayFromZero);
    }

    private static Dictionary<string, string?> ReadMetadataKeyValues(string metadataPath)
    {
        if (metadataPath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            return ReadXmlMetadataKeyValues(metadataPath);

        return ReadTextMetadataKeyValues(metadataPath);
    }

    private static Dictionary<string, string?> ReadXmlMetadataKeyValues(string metadataPath)
    {
        var result = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        try
        {
            var doc = XDocument.Load(metadataPath);

            foreach (var element in doc.Descendants())
            {
                if (!element.HasElements)
                {
                    var key = element.Name.LocalName.Trim();
                    var value = element.Value?.Trim();

                    if (!string.IsNullOrWhiteSpace(key) &&
                        !string.IsNullOrWhiteSpace(value) &&
                        !result.ContainsKey(key))
                    {
                        result[key] = value;
                    }
                }

                foreach (var attr in element.Attributes())
                {
                    var key = attr.Name.LocalName.Trim();
                    var value = attr.Value?.Trim();

                    if (!string.IsNullOrWhiteSpace(key) &&
                        !string.IsNullOrWhiteSpace(value) &&
                        !result.ContainsKey(key))
                    {
                        result[key] = value;
                    }
                }
            }
        }
        catch
        {
            return ReadTextMetadataKeyValues(metadataPath);
        }

        return result;
    }

    private static Dictionary<string, string?> ReadTextMetadataKeyValues(string metadataPath)
    {
        var result = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        foreach (var rawLine in File.ReadLines(metadataPath))
        {
            var line = rawLine.Trim();

            if (string.IsNullOrWhiteSpace(line) ||
                line.StartsWith("BEGIN_GROUP", StringComparison.OrdinalIgnoreCase) ||
                line.StartsWith("END_GROUP", StringComparison.OrdinalIgnoreCase))
                continue;

            var index = line.IndexOf('=');

            if (index <= 0)
                continue;

            var key = line[..index].Trim();
            var value = line[(index + 1)..].Trim().TrimEnd(';').Trim();

            if (value.StartsWith('"') && value.EndsWith('"') && value.Length >= 2)
                value = value[1..^1];

            if (!string.IsNullOrWhiteSpace(key) && !result.ContainsKey(key))
                result[key] = string.IsNullOrWhiteSpace(value) ? null : value;
        }

        return result;
    }

    private static string? BuildSpatialReferenceFromMeta(IReadOnlyDictionary<string, string?> meta)
    {
        var earthModel = GetMetaAny(meta, "earthModel");
        var projection = GetMetaAny(meta, "mapProjection");
        var zone = GetMetaAny(meta, "Zone_Number");

        var parts = new[] { earthModel, projection, zone }
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        return parts.Length == 0 ? null : string.Join(" / ", parts);
    }

    private static string NormalizeImageType(string? imageType)
    {
        if (string.IsNullOrWhiteSpace(imageType))
            return ImageTypeMono;

        if (imageType.Equals(ImageTypeStereo, StringComparison.OrdinalIgnoreCase))
            return ImageTypeStereo;

        if (imageType.Equals(ImageTypeMono, StringComparison.OrdinalIgnoreCase))
            return ImageTypeMono;

        if (imageType.Contains("stereo", StringComparison.OrdinalIgnoreCase))
            return ImageTypeStereo;

        return imageType.Trim();
    }

    private static string? FindMetadataFile(string imageDir)
    {
        if (string.IsNullOrWhiteSpace(imageDir) || !Directory.Exists(imageDir))
            return null;

        var files = Directory.GetFiles(imageDir, "*.*", SearchOption.AllDirectories);

        return files.FirstOrDefault(x =>
                   x.Contains("metadata", StringComparison.OrdinalIgnoreCase) &&
                   IsMetadataFile(x))
               ?? files.FirstOrDefault(x =>
                   x.Contains("_MTL", StringComparison.OrdinalIgnoreCase) &&
                   IsMetadataFile(x))
               ?? files.FirstOrDefault(x => x.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
               ?? files.FirstOrDefault(x => x.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
               ?? files.FirstOrDefault(x => x.EndsWith(".json", StringComparison.OrdinalIgnoreCase));
    }

    private static string? FindShpFile(string productDir)
    {
        var gisDir = Path.Combine(productDir, "GIS_FILES");

        if (!Directory.Exists(gisDir))
            return null;

        return Directory
            .GetFiles(gisDir, "*.shp", SearchOption.AllDirectories)
            .FirstOrDefault();
    }

    private static string? FindPreviewFile(string imageDir)
    {
        if (!Directory.Exists(imageDir))
            return null;

        var files = Directory.GetFiles(imageDir, "*.*", SearchOption.AllDirectories);

        return files.FirstOrDefault(x =>
                   x.Contains("PREVIEW", StringComparison.OrdinalIgnoreCase) &&
                   IsImageFile(x))
               ?? files.FirstOrDefault(x =>
                   x.Contains("BROWSE", StringComparison.OrdinalIgnoreCase) &&
                   IsImageFile(x));
    }

    private static string? FindThumbnailFile(string imageDir)
    {
        if (!Directory.Exists(imageDir))
            return null;

        var files = Directory.GetFiles(imageDir, "*.*", SearchOption.AllDirectories);

        return files.FirstOrDefault(x =>
                   x.Contains("THUMBNAIL", StringComparison.OrdinalIgnoreCase) &&
                   IsImageFile(x))
               ?? files.FirstOrDefault(x =>
                   x.Contains("THUMB", StringComparison.OrdinalIgnoreCase) &&
                   IsImageFile(x));
    }

    private static string? FindQuicklookFile(string productDir)
    {
        if (!Directory.Exists(productDir))
            return null;

        var files = Directory.GetFiles(productDir, "*.*", SearchOption.AllDirectories);

        return files.FirstOrDefault(x =>
                   x.Contains("QUICKLOOK", StringComparison.OrdinalIgnoreCase) &&
                   IsImageFile(x))
               ?? files.FirstOrDefault(x =>
                   x.Contains("QL", StringComparison.OrdinalIgnoreCase) &&
                   IsImageFile(x));
    }

    private static string? FindFirstByExtensions(string root, params string[] extensions)
    {
        if (!Directory.Exists(root))
            return null;

        return Directory
            .GetFiles(root, "*.*", SearchOption.AllDirectories)
            .FirstOrDefault(x =>
                extensions.Any(ext =>
                    x.EndsWith(ext, StringComparison.OrdinalIgnoreCase)));
    }

    private static bool IsMetadataFile(string path)
    {
        return path.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".json", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsImageFile(string path)
    {
        return path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".tif", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase);
    }

    private static string ToPublicFileUrl(
        string dataRoot,
        string fullPath,
        string publicBaseUrl)
    {
        var relativePath = Path.GetRelativePath(dataRoot, fullPath)
            .Replace("\\", "/");

        var encodedPath = string.Join("/",
            relativePath
                .Split("/", StringSplitOptions.RemoveEmptyEntries)
                .Select(Uri.EscapeDataString));

        return $"{publicBaseUrl}/files/{encodedPath}";
    }

    private static string BuildProductName(Product product, string imageId)
    {
        var type = FirstNotEmpty(product.ProductType, product.SensorMode, product.Sensor);

        return string.IsNullOrWhiteSpace(type)
            ? $"Uydu Görüntüsü - {imageId}"
            : $"Uydu Görüntüsü - {imageId} ({type})";
    }

    private static string? GetMetaAny(
        IReadOnlyDictionary<string, string?> meta,
        params string[] keys)
    {
        foreach (var key in keys)
        {
            if (meta.TryGetValue(key, out var value) &&
                !string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }
        }

        return null;
    }

    private static string? GetDbfAny(
        IReadOnlyDictionary<string, string?> dbf,
        params string[] keys)
    {
        foreach (var key in keys)
        {
            if (dbf.TryGetValue(key, out var value) &&
                !string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }
        }

        return null;
    }

    private static string? FirstNotEmpty(params string?[] values)
    {
        return values.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))?.Trim();
    }

    private static decimal? FirstDecimal(params decimal?[] values)
    {
        return values.FirstOrDefault(x => x.HasValue);
    }

    private static DateTime? FirstDate(params DateTime?[] values)
    {
        return values.FirstOrDefault(x => x.HasValue);
    }

    private static bool? FirstBool(params bool?[] values)
    {
        return values.FirstOrDefault(x => x.HasValue);
    }

    private static string? NullIfEmpty(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static decimal? ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        value = value.Trim().TrimEnd(';').Replace(",", ".");

        return decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)
            ? result
            : null;
    }

    private static DateTime? ParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        value = value.Trim().TrimEnd(';');

        var formats = new[]
        {
            "yyyy MM dd HH:mm:ss",
            "yyyy MM dd HH:mm:ss.FFFFFF",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm:ssZ"
        };

        if (DateTime.TryParseExact(
                value,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out var exact))
            return exact;

        if (DateTimeOffset.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out var dto))
            return dto.UtcDateTime;

        return DateTime.TryParse(
            value,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal,
            out var dt)
            ? dt
            : null;
    }

    private static bool DetectNdviAnalysis(
        string productDir,
        IReadOnlyDictionary<string, string?> dbf,
        IReadOnlyDictionary<string, string?> meta)
    {
        var explicitValue = FirstNotEmpty(
            GetDbfAny(dbf,
                "IS_NDVI", "ISNDVI", "NDVI", "NDVI_DONE", "NDVI_ANALYSIS",
                "IS_NVDI", "ISNVDI", "NVDI", "NVDI_DONE", "NVDI_ANALYSIS"),
            GetMetaAny(meta,
                "isNdvi", "isNDVI", "ndvi", "ndviDone", "ndviAnalysis", "hasNdvi",
                "isNvdi", "isNVDI", "nvdi", "nvdiDone", "nvdiAnalysis", "hasNvdi"));

        var parsedExplicitValue = ParseAnalysisBool(explicitValue);

        if (parsedExplicitValue.HasValue)
            return parsedExplicitValue.Value;

        if (ContainsNdviMarker(productDir))
            return true;

        if (Directory.Exists(productDir))
        {
            try
            {
                if (Directory.EnumerateFileSystemEntries(productDir, "*", SearchOption.AllDirectories)
                    .Any(ContainsNdviMarker))
                {
                    return true;
                }
            }
            catch (IOException)
            {
                // Erişilemeyen tekil dosya/klasörler seed işlemini durdurmasın.
            }
            catch (UnauthorizedAccessException)
            {
                // Erişilemeyen tekil dosya/klasörler seed işlemini durdurmasın.
            }
        }

        return dbf.Any(x => ContainsNdviMarker(x.Key) || ContainsNdviMarker(x.Value))
            || meta.Any(x => ContainsNdviMarker(x.Key) || ContainsNdviMarker(x.Value));
    }

    private static bool ContainsNdviMarker(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return value.Contains("NDVI", StringComparison.OrdinalIgnoreCase)
            || value.Contains("NVDI", StringComparison.OrdinalIgnoreCase);
    }

    private static bool? ParseAnalysisBool(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized = value.Trim().ToLowerInvariant();

        if (normalized is "1" or "true" or "yes" or "y" or "evet" or "var" or "done" or "completed" or "complete")
            return true;

        if (normalized is "0" or "false" or "no" or "n" or "hayır" or "hayir" or "yok" or "not done" or "not_done" or "none")
            return false;

        return ContainsNdviMarker(normalized) ? true : null;
    }

    private static bool? HasPanSharpen(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value.Contains("pan", StringComparison.OrdinalIgnoreCase)
            || value.Contains("fusion", StringComparison.OrdinalIgnoreCase)
            || value.Contains("pansharpen", StringComparison.OrdinalIgnoreCase);
    }

    private static bool? IsOrthorectifiedLevel(string? productLevel)
    {
        if (string.IsNullOrWhiteSpace(productLevel))
            return null;

        return productLevel.Contains("2A", StringComparison.OrdinalIgnoreCase)
            || productLevel.Contains("ORTHO", StringComparison.OrdinalIgnoreCase)
            || productLevel.Contains("LEVEL2", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task SeedApiKeyAsync(OrderManagementContext db)
    {
        var apiProduct = await db.Products
            .FirstOrDefaultAsync(x => x.Name == "API Key" && x.CategoryId == 2);

        if (apiProduct == null)
        {
            db.Products.Add(new Product
            {
                Name = "API Key",
                Price = 1000,
                PriceStr = "₺1000",
                Currency = "TRY",
                CategoryId = 2,
                IsDeleted = false,
                IsInMarket = true,
                IsCustomArea = false,
                Description = "API erişim ürünü"
            });

            await db.SaveChangesAsync();
            return;
        }

        apiProduct.Price = 1000;
        apiProduct.PriceStr = "₺1000";
        apiProduct.Currency = "TRY";
        apiProduct.IsDeleted = false;
        apiProduct.IsInMarket = true;
        apiProduct.IsCustomArea = false;
        apiProduct.Description = "API erişim ürünü";

        await db.SaveChangesAsync();
    }

    private static List<ProductClass> GenerateRandomClasses()
    {
        var rnd = Random.Shared;

        var allClasses = new List<(string Name, string Color)>
        {
            ("Yerleşim", "#ff6b6b"),
            ("Yol", "#ffd166"),
            ("Yeşil Alan", "#06d6a0"),
            ("Su", "#118ab2"),
            ("Tarım", "#8ac926"),
            ("Orman", "#2a9d8f"),
            ("Sanayi", "#6a4c93")
        };

        return allClasses
            .OrderBy(_ => rnd.Next())
            .Take(rnd.Next(3, 6))
            .Select(c => new ProductClass
            {
                ClassName = c.Name,
                PixelCount = rnd.Next(10000, 150000),
                ColorHex = c.Color
            })
            .ToList();
    }
}

internal static class DbfAttributeReader
{
    public static IReadOnlyDictionary<string, string?> ReadFirst(string shpPath)
    {
        var dbfPath = Path.ChangeExtension(shpPath, ".dbf");

        if (string.IsNullOrWhiteSpace(dbfPath) || !File.Exists(dbfPath))
            return new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        using var stream = File.OpenRead(dbfPath);
        using var reader = new BinaryReader(stream, Encoding.ASCII, leaveOpen: false);

        if (stream.Length < 32)
            return new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        reader.ReadByte();
        reader.ReadBytes(3);

        var recordCount = reader.ReadInt32();
        var headerLength = reader.ReadInt16();
        var recordLength = reader.ReadInt16();

        reader.ReadBytes(20);

        if (recordCount <= 0 || headerLength <= 32 || recordLength <= 1)
            return new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        var fields = new List<DbfField>();

        while (stream.Position < headerLength - 1)
        {
            var first = reader.ReadByte();

            if (first == 0x0D)
                break;

            var nameBytes = new byte[11];
            nameBytes[0] = first;
            reader.Read(nameBytes, 1, 10);

            var name = Encoding.ASCII.GetString(nameBytes).TrimEnd('\0', ' ');
            var type = (char)reader.ReadByte();

            reader.ReadBytes(4);

            var length = reader.ReadByte();
            var decimalCount = reader.ReadByte();

            reader.ReadBytes(14);

            if (!string.IsNullOrWhiteSpace(name))
                fields.Add(new DbfField(name, type, length, decimalCount));
        }

        stream.Position = headerLength;

        var recordBytes = reader.ReadBytes(recordLength);

        if (recordBytes.Length != recordLength || recordBytes[0] == '*')
            return new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        var result = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        var offset = 1;

        foreach (var field in fields)
        {
            if (offset + field.Length > recordBytes.Length)
                break;

            var raw = recordBytes.Skip(offset).Take(field.Length).ToArray();

            var value = Encoding.UTF8.GetString(raw).Trim('\0', ' ');

            if (string.IsNullOrWhiteSpace(value))
                value = Encoding.Default.GetString(raw).Trim('\0', ' ');

            result[field.Name] = string.IsNullOrWhiteSpace(value) ? null : value;

            offset += field.Length;
        }

        return result;
    }

    private sealed record DbfField(string Name, char Type, int Length, int DecimalCount);
}