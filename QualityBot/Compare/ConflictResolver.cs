namespace QualityBot.Compare
{
    using System.Collections.Generic;
    using System.Linq;

    // TODO: high value testing area
    public class ConflictResolver<T>
    {
        public void ResolveAllConflicts(ElementMatch<T>[] elementsA)
        {
            ResolveConflicts(elementsA);
            Finalize(elementsA);
        }

        private void Finalize(IEnumerable<ElementMatch<T>> elements)
        {
            foreach (var element in elements)
            {
                if (element.Match != null)
                {
                    element.Match.Match = element;
                }

                element.Matches.Clear();
            }
        }

        private bool HasConflicts(ElementMatch<T>[] elements, out ElementMatch<T> a, out ElementMatch<T> b)
        {
            a = null;
            b = null;

            foreach (var eA in elements.Where(e => e.Match != null))
            {
                var a1 = eA;
                foreach (var eB in elements.Except(new[] { eA }).Where(e => e.Match != null).Where(eB => ReferenceEquals(a1.Match.This, eB.Match.This)))
                {
                    a = eA;
                    b = eB;

                    return true;
                }
            }

            return false;
        }

        private void ResolveConflict(ElementMatch<T> a, ElementMatch<T> b)
        {
            var loser = a.MatchValue > b.MatchValue ? a : b;
            loser.SetToNext();
        }

        private void ResolveConflicts(ElementMatch<T>[] elements)
        {
            ElementMatch<T> a;
            ElementMatch<T> b;

            while (HasConflicts(elements, out a, out b))
            {
                ResolveConflict(a, b);
            }
        }
    }
}