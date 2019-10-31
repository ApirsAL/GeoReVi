using System.Threading.Tasks;

namespace GeoReVi
{
    interface IStatisticalAnalysis
    {
        /// <summary>
        /// Checks whether a computation takes place ATM or not
        /// </summary>
        bool IsComputing { get; set; }
        Task Compute();
    }
}
