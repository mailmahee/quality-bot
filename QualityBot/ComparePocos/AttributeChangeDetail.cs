namespace QualityBot.ComparePocos
{
    using System;

    [Serializable]
    class AttributeChangeDetail
    {
        public string Key { get; set; }

        public string From { get; set; }

        public string To { get; set; }
    }
}
