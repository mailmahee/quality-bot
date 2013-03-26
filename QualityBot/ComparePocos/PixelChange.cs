namespace QualityBot.ComparePocos
{
    using System;
    using System.Drawing;
    [Serializable]
    public class PixelChange
    {
        public Image From { get; set; }
        public string FromStyle { get; set; }

        public Image FromClipped { get; set; }
        public string FromClippedStyle { get; set; }

        public Image FromMask { get; set; }
        public string FromMaskStyle { get; set; }

        public Image To { get; set; }
        public string ToStyle { get; set; }

        public Image ToClipped { get; set; }
        public string ToClippedStyle { get; set; }

        public Image ToMask { get; set; }
        public string ToMaskStyle { get; set; }

        public Image Diff { get; set; }
        public string DiffStyle { get; set; }

    }
}