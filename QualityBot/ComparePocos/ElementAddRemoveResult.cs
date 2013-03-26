namespace QualityBot.ComparePocos
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using QualityBot.Util;
    [Serializable]
    public class ElementAddRemoveResult
    {
        public string Html { get; set; }

        public string Text { get; set; }

        public string Tag { get; set; }

        public Rectangle Location { get; set; }

        public Dictionary<string, string> Attributes { get; set; }

        public Image Image { get; set; }
        
        public string ImageStyle { get; set; }

        public Image ImageClipped { get; set; }

        public string ImageClippedStyle { get; set; }

        public Image ImageMask { get; set; }

        public string ImageMaskStyle { get; set; }

    }
}
