using System.Collections.Generic;
using System.Linq;

namespace DPA.Sapewin.Domain.Models
{
    public class Grouping<TKey, TElement> : List<TElement>, IGrouping<TKey, TElement>
    {
        public Grouping(TKey key, IEnumerable<TElement> collection)
            : base(collection) => Key = key;
        public TKey Key { get; }
    }
}