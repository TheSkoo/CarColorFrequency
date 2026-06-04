using Azure;
using Azure.Data.Tables;

public class ColorDataEntity : ITableEntity
{
    public string PartitionKey { get; set; } = default!;
    public string RowKey { get; set; } = default!;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    // Custom business properties
    public int ColorDictId { get; set; }
    public string Color { get; set; } = string.Empty;
    public int BackgroundColorRGB { get; set; }
    public int ForegroundColorRGB { get; set; }
    public int Count { get; set; }
}