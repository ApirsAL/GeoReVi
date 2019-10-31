using System.Collections;

namespace GeoReVi
{
    /// <summary>
    /// Represents an object that contains all necessary properties to conduct a filter operation
    /// </summary>
    public interface INumberFilterOperation<T> where T : struct
    {

        /// <summary>
        /// The property name to compare
        /// </summary>
        string PropertyNameToCompare { get; set; }

        /// <summary>
        /// Value that will be compared
        /// </summary>
        T ValueToCompare { get; set; }

        /// <summary>
        /// Query operator for this operation
        /// </summary>
        NumberQueryOperator QueryOperator { get; set; }

        /// <summary>
        /// Comparing the property with the assigned value
        /// </summary>
        /// <param name="dataSet">Data set that will be filtered</param>
        /// <returns></returns>
        IEnumerable Filter(IEnumerable data);

    }

    /// <summary>
    /// Represents an object that contains all necessary properties to conduct a filter operation
    /// </summary>
    public interface ITextFilterOperation
    {

        /// <summary>
        /// The property name to compare
        /// </summary>
        string PropertyNameToCompare { get; set; }

        /// <summary>
        /// Value that will be compared
        /// </summary>
        string ValueToCompare { get; set; }

        /// <summary>
        /// Query operator for this operation
        /// </summary>
        TextQueryOperator QueryOperator { get; set; }

    }
}
