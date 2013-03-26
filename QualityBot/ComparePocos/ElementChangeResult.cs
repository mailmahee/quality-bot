namespace QualityBot.ComparePocos
{
    using System;
    using System.Drawing;
    [Serializable]
    public class ElementChangeResult
    {
        public CssChange CssChanges { get; set; }

        public string HtmlChanges { get; set; }

        public string TextChanges { get; set; }

        public string LocationChanges { get; set; }

        public PixelChange PixelChanges { get; set; }

        public decimal CssPercentageChange { get; set; }

        public decimal HtmlPercentageChange { get; set; }

        public decimal TextPercentageChange { get; set; }

        public decimal LocationPercentageChange { get; set; }

        public decimal PixelPercentageChange { get; set; }

        public Rectangle LocationOnScreenshot { get; set; }

    }
}