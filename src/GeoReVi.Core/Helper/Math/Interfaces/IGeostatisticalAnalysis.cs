namespace GeoReVi
{
    interface IGeostatisticalAnalysis
    {
        /// <summary>
        /// Checks whether a computation takes place ATM or not
        /// </summary>
        bool IsComputing { get; set; }
    }
}
