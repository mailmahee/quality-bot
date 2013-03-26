namespace QualityBot.ComparePocos
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using QualityBot.Util;
    [Serializable]
    public class CssChange
    {
        public IEnumerable<CssChangeDetail> Changed { get; set; }

        public Dictionary<string, string> Added { get; set; }

        public Dictionary<string, string> Deleted { get; set; }

    }
}