namespace GeoReVi
{
    /// <summary>
    /// An interface for machine learning algorithms
    /// </summary>
    public interface IMachineLearningAnalysis
    {
        /// <summary>
        /// Checks whether a computation takes place ATM or not
        /// </summary>
        bool IsComputing { get; set; }

        /// <summary>
        /// Learning method
        /// </summary>
        void Learn();

        /// <summary>
        /// Prediction method
        /// </summary>
        void Predict();
    }
}
