using Microsoft.EntityFrameworkCore;
using OrderManagement.DbContexts;
using OrderManagement.Entity;
using OrderManagement.Helper;
using System.Globalization;
using System.Text;

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

        var counter = 0;

        foreach (var productDir in Directory.GetDirectories(dataRoot))
        {
            var folderName = Path.GetFileName(productDir);

            if (string.IsNullOrWhiteSpace(folderName))
                continue;

            var product = await db.Products
                .Include(x => x.Classes)
                .FirstOrDefaultAsync(x => x.ImageId == folderName && x.CategoryId == 1);

            if (product == null)
            {
                var price = Random.Shared.Next(5000, 20000);

                product = new Product
                {
                    ImageId = folderName,
                    Name = $"Uydu Görüntüsü - {folderName}",
                    Price = price,
                    PriceStr = $"₺{price}",
                    Currency = "TRY",
                    CategoryId = 1,
                    IsDeleted = false,
                    IsInMarket = true,
                    IsCustomArea = false,
                    Description = "Seed edilmiş uydu görüntüsü",
                    City = "Unknown",
                    District = "Unknown",
                    Provider = "Demo Dataset",
                    SourceLabel = "Satellite Image",
                    ImageType = ImageTypeMono,
                    IsOrthorectified = true,
                    IsClassified = true,
                    Classes = GenerateRandomClasses()
                };

                db.Products.Add(product);
            }

            SeedImageFiles(product, dataRoot, productDir, publicBaseUrl);
            SeedGisFiles(product, productDir);

            product.DownloadLink = !string.IsNullOrWhiteSpace(product.GeoTiffPath)
                ? ToPublicFileUrl(dataRoot, product.GeoTiffPath, publicBaseUrl)
                : ToPublicFileUrl(dataRoot, productDir, publicBaseUrl);

            product.ImageType = NormalizeImageType(product.ImageType);
            product.Name = BuildProductName(product, folderName);
            product.IsOrthorectified ??= true;
            product.IsClassified ??= true;
            product.SourceLabel = FirstNotEmpty(product.SourceLabel, product.ProductType, product.SensorMode, "Satellite Image");
            product.Provider = FirstNotEmpty(product.Provider, product.DataOwner, "Demo Dataset");
            product.ImageId = FirstNotEmpty(product.ImageId, folderName);

            counter++;

            if (counter % 100 == 0)
                await db.SaveChangesAsync();
        }

        await db.SaveChangesAsync();
    }

    private static void SeedImageFiles(
        Product product,
        string dataRoot,
        string productDir,
        string publicBaseUrl)
    {
        var imageDir = Path.Combine(productDir, "IMAGE");

        if (Directory.Exists(imageDir))
        {
            product.GeoTiffPath = FindFirstByExtensions(
                imageDir,
                new[] { ".tif", ".tiff", ".geotiff" });

            product.MetadataPath = FindMetadataFile(imageDir);

            if (!string.IsNullOrWhiteSpace(product.MetadataPath))
            {
                ApplyMetadata(product, product.MetadataPath);
                product.MetadataUrl = ToPublicFileUrl(dataRoot, product.MetadataPath, publicBaseUrl);
            }

            product.PreviewPath = FindPreviewFile(imageDir);

            if (!string.IsNullOrWhiteSpace(product.PreviewPath))
                product.PreviewUrl = ToPublicFileUrl(dataRoot, product.PreviewPath, publicBaseUrl);
        }

        var quicklook = FindQuicklookFile(productDir);

        if (!string.IsNullOrWhiteSpace(quicklook))
        {
            product.QuicklookPath = quicklook;
            product.ThumbnailUrl = ToPublicFileUrl(dataRoot, quicklook, publicBaseUrl);
        }
        else if (!string.IsNullOrWhiteSpace(product.PreviewUrl))
        {
            product.ThumbnailUrl = product.PreviewUrl;
        }

        product.City = FirstNotEmpty(product.City, "Unknown");
        product.District = FirstNotEmpty(product.District, "Unknown");
        product.Provider = FirstNotEmpty(product.Provider, "Demo Dataset");
        product.SourceLabel = FirstNotEmpty(product.SourceLabel, "Satellite Image");
        product.ImageType = NormalizeImageType(product.ImageType);
    }

    private static void SeedGisFiles(Product product, string productDir)
    {
        var gisDir = Path.Combine(productDir, "GIS_FILES");

        if (!Directory.Exists(gisDir))
            return;

        var shp = Directory
            .GetFiles(gisDir, "*.shp", SearchOption.AllDirectories)
            .FirstOrDefault();

        product.FootprintPath = shp;

        if (!string.IsNullOrWhiteSpace(shp))
        {
            var attributes = DbfAttributeReader.ReadFirst(shp);
            ApplyDbfAttributes(product, attributes);
        }

        if (string.IsNullOrWhiteSpace(shp))
            return;

        var geom = ShapeReader.Read(shp);

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
            MidpointRounding.AwayFromZero
        );
    }

    private static void ApplyDbfAttributes(Product product, IReadOnlyDictionary<string, string?> dbf)
    {
        if (dbf.Count == 0)
            return;

        product.ImageId = FirstNotEmpty(GetDbf(dbf, "IMAGE_ID"), product.ImageId);
        product.OrderId = FirstNotEmpty(GetDbf(dbf, "ORDER_ID"), product.OrderId);
        product.Satellite = FirstNotEmpty(GetDbf(dbf, "SOURCE"), product.Satellite);
        product.Sensor = FirstNotEmpty(GetDbf(dbf, "SENS_MODE"), product.Sensor);
        product.SensorMode = FirstNotEmpty(GetDbf(dbf, "SENS_MODE"), product.SensorMode);
        product.StripId = FirstNotEmpty(GetDbf(dbf, "STRIP_ID"), product.StripId);
        product.SpatialReference = FirstNotEmpty(GetDbf(dbf, "SPATIALREF"), product.SpatialReference);
        product.DataOwner = FirstNotEmpty(GetDbf(dbf, "DATA_OWNER"), product.DataOwner);
        product.Provider = FirstNotEmpty(GetDbf(dbf, "DATA_OWNER"), product.Provider);

        product.AcquisitionDate = FirstDate(
            ParseDate(GetDbf(dbf, "COLL_DATE")),
            product.AcquisitionDate);

        product.Resolution = FirstDecimal(
            ParseDecimal(GetDbf(dbf, "GSD")),
            product.Resolution);

        product.CloudRate = FirstDecimal(
            ParseDecimal(GetDbf(dbf, "CLOUDS")),
            product.CloudRate);

        product.SunAzimuth = FirstDecimal(
            ParseDecimal(GetDbf(dbf, "SUN_ANGLE")),
            ParseDecimal(GetDbf(dbf, "AZIM_ANGLE")),
            product.SunAzimuth);

        product.SunElevation = FirstDecimal(
            ParseDecimal(GetDbf(dbf, "SUN_ELEV")),
            product.SunElevation);

        product.OffNadirAngle = FirstDecimal(
            ParseDecimal(GetDbf(dbf, "NADIR_ANGL")),
            product.OffNadirAngle);

        product.MetadataUrl = FirstNotEmpty(GetDbf(dbf, "METADATA"), product.MetadataUrl);
        product.PreviewUrl = FirstNotEmpty(GetDbf(dbf, "PREVIEW"), product.PreviewUrl);
        product.SourceLabel = FirstNotEmpty(product.SensorMode, product.SourceLabel, "Satellite Image");

        product.ImageType = NormalizeImageType(product.ImageType);
    }

    private static void ApplyMetadata(Product product, string metadataPath)
    {
        var values = ReadMetadataKeyValues(metadataPath);

        product.OrderId = FirstNotEmpty(GetMeta(values, "productOrderId"), product.OrderId);
        product.CatalogId = FirstNotEmpty(GetMeta(values, "productCatalogId"), GetMeta(values, "CatId"), product.CatalogId);
        product.ImageDescriptor = FirstNotEmpty(GetMeta(values, "imageDescriptor"), product.ImageDescriptor);

        // KRİTİK: ImageType burada SADECE Mono veya Stereo atanır.
        // productType/mode/bandId ASLA ImageType'a yazılmaz.
        product.ImageType = ResolveMonoStereoImageType(metadataPath);

        product.SensorMode = FirstNotEmpty(product.SensorMode, GetMeta(values, "mode"));
        product.BandId = FirstNotEmpty(GetMeta(values, "bandId"), product.BandId);
        product.ProductType = FirstNotEmpty(GetMeta(values, "productType"), product.ProductType);
        product.ProductLevel = FirstNotEmpty(GetMeta(values, "productLevel"), product.ProductLevel);
        product.ProcessingLevel = FirstNotEmpty(product.ProcessingLevel, product.ProductLevel);
        product.RadiometricLevel = FirstNotEmpty(GetMeta(values, "radiometricLevel"), product.RadiometricLevel);
        product.OutputFormat = FirstNotEmpty(GetMeta(values, "outputFormat"), product.OutputFormat);
        product.Satellite = FirstNotEmpty(product.Satellite, GetMeta(values, "satId"), product.ImageDescriptor);
        product.Sensor = FirstNotEmpty(product.Sensor, product.SensorMode, GetMeta(values, "mode"));
        product.ScanDirection = FirstNotEmpty(GetMeta(values, "scanDirection"), product.ScanDirection);

        product.AcquisitionStartDate = FirstDate(
            product.AcquisitionStartDate,
            ParseDate(GetMeta(values, "earliestAcqTime")),
            ParseDate(GetMeta(values, "firstLineTime")));

        product.AcquisitionEndDate = FirstDate(
            product.AcquisitionEndDate,
            ParseDate(GetMeta(values, "latestAcqTime")),
            product.AcquisitionStartDate);

        product.AcquisitionDate = FirstDate(
            product.AcquisitionDate,
            product.AcquisitionStartDate,
            ParseDate(GetMeta(values, "generationTime")));

        product.Resolution = FirstDecimal(
            product.Resolution,
            ParseDecimal(GetMeta(values, "productGSD")),
            ParseDecimal(GetMeta(values, "meanCollectedGSD")),
            ParseDecimal(GetMeta(values, "meanCollectedRowGSD")),
            ParseDecimal(GetMeta(values, "colSpacing")),
            ParseDecimal(GetMeta(values, "rowSpacing")));

        product.CloudRate = FirstDecimal(
            product.CloudRate,
            ParseDecimal(GetMeta(values, "cloudCover")));

        product.SunElevation = FirstDecimal(
            product.SunElevation,
            ParseDecimal(GetMeta(values, "meanSunEl")),
            ParseDecimal(GetMeta(values, "maxSunEl")),
            ParseDecimal(GetMeta(values, "minSunEl")));

        product.SunAzimuth = FirstDecimal(
            product.SunAzimuth,
            ParseDecimal(GetMeta(values, "meanSunAz")),
            ParseDecimal(GetMeta(values, "maxSunAz")),
            ParseDecimal(GetMeta(values, "minSunAz")));

        product.OffNadirAngle = FirstDecimal(
            product.OffNadirAngle,
            ParseDecimal(GetMeta(values, "meanOffNadirViewAngle")),
            ParseDecimal(GetMeta(values, "maxOffNadirViewAngle")),
            ParseDecimal(GetMeta(values, "minOffNadirViewAngle")));

        product.IsPansharpened = FirstBool(
            product.IsPansharpened,
            HasPanSharpen(GetMeta(values, "panSharpenAlgorithm")));

        product.IsOrthorectified = FirstBool(
            product.IsOrthorectified,
            IsOrthorectifiedLevel(product.ProductLevel));

        product.SourceLabel = FirstNotEmpty(
            product.SourceLabel,
            product.ProductType,
            product.SensorMode,
            "Satellite Image");

        product.ImageType = NormalizeImageType(product.ImageType);
    }

    private static string ResolveMonoStereoImageType(string metadataPath)
    {
        if (string.IsNullOrWhiteSpace(metadataPath) || !File.Exists(metadataPath))
            return ImageTypeMono;

        var text = File.ReadAllText(metadataPath);

        if (string.IsNullOrWhiteSpace(text))
            return ImageTypeMono;

        var normalized = text.ToLowerInvariant();

        var hasImage1 =
            normalized.Contains("begin_group = image_1") ||
            normalized.Contains("begin_group=image_1");

        var hasImage2 =
            normalized.Contains("begin_group = image_2") ||
            normalized.Contains("begin_group=image_2");

        if (hasImage1 && hasImage2)
            return ImageTypeStereo;

        if (normalized.Contains("tri-stereo") ||
            normalized.Contains("tristereo") ||
            normalized.Contains("along-track") ||
            normalized.Contains("multi-view") ||
            normalized.Contains("multiview") ||
            normalized.Contains("stereo"))
        {
            return ImageTypeStereo;
        }

        return ImageTypeMono;
    }

    private static string NormalizeImageType(string? imageType)
    {
        if (string.IsNullOrWhiteSpace(imageType))
            return ImageTypeMono;

        if (imageType.Equals(ImageTypeStereo, StringComparison.OrdinalIgnoreCase))
            return ImageTypeStereo;

        if (imageType.Equals(ImageTypeMono, StringComparison.OrdinalIgnoreCase))
            return ImageTypeMono;

        return ImageTypeMono;
    }

    private static Dictionary<string, string?> ReadMetadataKeyValues(string metadataPath)
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

    private static string? FindMetadataFile(string imageDir)
    {
        return Directory
            .GetFiles(imageDir, "*.*", SearchOption.AllDirectories)
            .FirstOrDefault(x =>
                x.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) &&
                x.Contains("_MTL", StringComparison.OrdinalIgnoreCase))
            ?? Directory
                .GetFiles(imageDir, "*.txt", SearchOption.AllDirectories)
                .FirstOrDefault();
    }

    private static string? FindPreviewFile(string imageDir)
    {
        return Directory
            .GetFiles(imageDir, "*.*", SearchOption.AllDirectories)
            .FirstOrDefault(x =>
                x.Contains("PREVIEW", StringComparison.OrdinalIgnoreCase) &&
                IsImageFile(x));
    }

    private static string? FindQuicklookFile(string productDir)
    {
        return Directory
            .GetFiles(productDir, "*.*", SearchOption.AllDirectories)
            .FirstOrDefault(x =>
                x.Contains("QUICKLOOK", StringComparison.OrdinalIgnoreCase) &&
                IsImageFile(x));
    }

    private static string? FindFirstByExtensions(string root, string[] extensions)
    {
        return Directory
            .GetFiles(root, "*.*", SearchOption.AllDirectories)
            .FirstOrDefault(x =>
                extensions.Any(ext =>
                    x.EndsWith(ext, StringComparison.OrdinalIgnoreCase)));
    }

    private static bool IsImageFile(string path)
    {
        return path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".png", StringComparison.OrdinalIgnoreCase);
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

    private static string BuildProductName(Product product, string fallbackFolderName)
    {
        var imageId = FirstNotEmpty(product.ImageId, fallbackFolderName) ?? fallbackFolderName;
        var type = FirstNotEmpty(product.ProductType, product.SensorMode);

        return string.IsNullOrWhiteSpace(type)
            ? $"Uydu Görüntüsü - {imageId}"
            : $"Uydu Görüntüsü - {imageId} ({type})";
    }

    private static string? GetDbf(IReadOnlyDictionary<string, string?> dbf, string key)
    {
        return dbf.TryGetValue(key, out var value) ? value : null;
    }

    private static string? GetMeta(IReadOnlyDictionary<string, string?> meta, string key)
    {
        return meta.TryGetValue(key, out var value) ? value : null;
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

        if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dto))
            return dto.UtcDateTime;

        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dt)
            ? dt
            : null;
    }

    private static bool? HasPanSharpen(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return !value.Equals("NONE", StringComparison.OrdinalIgnoreCase)
            && !value.Equals("OFF", StringComparison.OrdinalIgnoreCase)
            && !value.Equals("N/A", StringComparison.OrdinalIgnoreCase);
    }

    private static bool? IsOrthorectifiedLevel(string? productLevel)
    {
        if (string.IsNullOrWhiteSpace(productLevel))
            return null;

        return productLevel.Contains("2A", StringComparison.OrdinalIgnoreCase)
            || productLevel.Contains("ORTHO", StringComparison.OrdinalIgnoreCase);
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

            var value = Encoding.UTF8.GetString(recordBytes, offset, field.Length).Trim('\0', ' ');

            if (string.IsNullOrWhiteSpace(value))
                value = Encoding.Default.GetString(recordBytes, offset, field.Length).Trim('\0', ' ');

            result[field.Name] = string.IsNullOrWhiteSpace(value) ? null : value;
            offset += field.Length;
        }

        return result;
    }

    private sealed record DbfField(string Name, char Type, int Length, int DecimalCount);
}