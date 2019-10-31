using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// A static class for navigation events
    /// </summary>
    public static class Navigation
    {
        /// <summary>
        /// Getting the next item in a list or colleciton
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T GetNext<T>(IList<T> collection, T value)
        {
            int nextIndex = collection.IndexOf(value) + 1;
            if (nextIndex < collection.Count)
            {
                return collection[nextIndex];
            }
            else
            {
                return value; //Or throw an exception
            }
        }

        /// <summary>
        /// GEtting the previous item in a list or collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T GetPrevious<T>(IList<T> collection, T value)
        {
            int previousIndex = collection.IndexOf(value) + -1;
            if (previousIndex >= 0)
            {
                return collection[previousIndex];
            }
            else
            {
                return value; //Or throw an exception
            }
        }
    }
}

