using System.Collections.Generic;
using System.Linq;

namespace GeoReVi
{
    /// <summary>
    /// A class implementing the IGrouping-interface for individual groups
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    public class MyGrouping<TKey, TElement> : List<TElement>, IGrouping<TKey, TElement>
    {
        public TKey Key
        {
            get;
            set;
        }
    }
}