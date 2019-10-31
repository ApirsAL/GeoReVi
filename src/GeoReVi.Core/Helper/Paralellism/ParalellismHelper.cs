using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeoReVi
{
    public static class ParalellismHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="maxDegreeOfConcurrency"></param>
        /// <param name="collection"></param>
        /// <param name="taskFactory"></param>
        /// <returns></returns>
        public static async Task RunWithMaxDegreeOfConcurrency<T>(
                int maxDegreeOfConcurrency, IEnumerable<T> collection, Func<T, Task> taskFactory)
        {
            var activeTasks = new List<Task>(maxDegreeOfConcurrency);
            foreach (var task in collection.Select(taskFactory))
            {
                activeTasks.Add(task);
                if (activeTasks.Count == maxDegreeOfConcurrency)
                {
                    await Task.WhenAny(activeTasks.ToArray());
                    //observe exceptions here
                    activeTasks.RemoveAll(t => t.IsCompleted);
                }
            }
            await Task.WhenAll(activeTasks.ToArray()).ContinueWith(t =>
            {
                //observe exceptions in a manner consistent with the above   
            });
        }
    }
}
