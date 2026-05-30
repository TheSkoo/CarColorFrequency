namespace CarColorFrequencyApi.Models
{
    public class ColorData
    {
        public int ColorDictId { get; set; }
        public string Color { get; set; } = string.Empty;
        public int BackgroundColorRGB { get; set; }
        public int ForegroundColorRGB { get; set; }
        public int Count { get; set; }
    }
}
