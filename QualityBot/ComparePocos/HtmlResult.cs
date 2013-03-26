namespace QualityBot.ComparePocos
{
    using System;

    [Serializable]
    public class HtmlResult
    {
        public string[] Images { get; set; }

        public decimal PercentChanged { get; set; }

    }
}