namespace QualityBot.ComparePocos
{
    using System;
    using System.Collections.Generic;
    [Serializable]
    public class PixelResult
    {
        public string[] Images { get; set; }

        public decimal PercentChanged { get; set; }

    }
}