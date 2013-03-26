namespace QualityBot.ComparePocos
{
    using System;

    [Serializable]
    public class CssChangeDetail
    {
        public string Key { get; set; }

        public string From { get; set; }

        public string To { get; set; }

    }
}