namespace QualityBot.ComparePocos
{
    public class PixelChange
    {
        public string From { get; set; }

        public string FromClipped { get; set; }

        public string FromMask { get; set; }

        public string To { get; set; }

        public string ToClipped { get; set; }

        public string ToMask { get; set; }

        public string Diff { get; set; }
    }
}