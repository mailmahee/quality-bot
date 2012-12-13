namespace QualityBot.Compare
{
    using System;
    using System.Collections.Generic;

    public class ElementMatch<T>
    {
        public Queue<Tuple<ElementMatch<T>, decimal>> Matches { get; set; }

        public ElementMatch<T> Match { get; set; }

        public decimal MatchValue { get; set; }

        public T This { get; set; }

        public void SetToNext()
        {
            if (Matches.Count > 0)
            {
                var match = Matches.Dequeue();
                Match = match.Item1;
                MatchValue = match.Item2;
            }
            else
            {
                Match = null;
                MatchValue = 0;
            }
        }
    }
}