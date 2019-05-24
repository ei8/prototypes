using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartRateAnalyzer
{
    public static class ObservableCollectionExtensions
    {
        public static void RemoveAll(this IList list)
        {
            while (list.Count > 0)
            {
                list.RemoveAt(list.Count - 1);
            }
        }
    }
}
