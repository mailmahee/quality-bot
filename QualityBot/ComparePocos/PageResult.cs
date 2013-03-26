namespace QualityBot.ComparePocos
{
    using System;
    [Serializable]
    public class PageResult
    {
        public ElementChangeResult[] ChangedItems { get; set; }

        public ElementAddRemoveResult[] AddedItems { get; set; }

        public ElementAddRemoveResult[] RemovedItems { get; set; }

        public int UnchangedItems { get; set; }

        public decimal CssChangePercentage { get; set; }

        public decimal TextChangePercentage { get; set; }

        public decimal OverallElementPositionChangePercentage { get; set; }

        public PixelResult Pixels { get; set; }

        public HtmlResult Html { get; set; }
        
        public string HtmlDiff { get; set; }

    }
}