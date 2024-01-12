using System.Globalization;
using DataPipelines.Models;

namespace DataPipelines.Infrastructure.Templating.SimplifiedYml;

public partial class SimplifiedYmlProductTemplate
{
    private const bool StrictCleanup = true;

    public ProductInformation Product { get; set; } = null!;
    public SkuInformation Sku { get; set; } = null!;
    public HashSet<string> AllowedAttributes { get; set; } = null!;


    public string ProductName => Product.Name;

    public IEnumerable<string> CategoryStrings => Product.Categories.Select(x =>
        string.Join(" / ", x.CategoryPathElements.Select(y => y.CategoryName)));

    public IEnumerable<string> ProductAlternativeSearchWords => Product.AlternativeSearchWords;
    public string ProductAlternativeSearchWordsString => string.Join(", ", ProductAlternativeSearchWords);
    public string ProductLongDescription => Product.LongDescription;
    public string ShortDescription => Product.ShortDescription;
    public string SkuName => Sku.Name;

    public Dictionary<string, string> Attributes => StringAttributes
        .Concat(NumberAttributes)
        .Concat(IntervalAttributes)
        .ToDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal);

    private Dictionary<string, string> StringAttributes => Sku.StringAttributes
        .Where(x => AllowedAttributes.Contains(x.Key))
        .ToDictionary(x => x.Key,
            x => $"{string.Join(", ", x.Value.Select(y => y))}", StringComparer.Ordinal);

    private Dictionary<string, string> NumberAttributes => Sku.NumberAttributes
        .Where(x => AllowedAttributes.Contains(x.Key))
        .ToDictionary(x => x.Key,
            x => $"{string.Join(", ", x.Value.Select(y => y.ToString(CultureInfo.InvariantCulture)))}", StringComparer.Ordinal);

    private Dictionary<string, string> IntervalAttributes => Sku.IntervalAttributes
        .Where(x => AllowedAttributes.Contains(x.Key))
        .ToDictionary(x => x.Key,
            x => $"{x.Value.Min.ToString(CultureInfo.InvariantCulture)} - {x.Value.Max.ToString(CultureInfo.InvariantCulture)}", StringComparer.Ordinal);
}
