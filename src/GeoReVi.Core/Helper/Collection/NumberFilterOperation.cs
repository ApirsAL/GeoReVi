using System.Collections;
using System.Linq;
using System.Linq.Dynamic;

namespace GeoReVi
{
    public class NumberFilterOperation<T> : INumberFilterOperation<T> where T : struct
    {
        #region Public Properties

        /// <summary>
        /// The property name to compare
        /// </summary>
        public string PropertyNameToCompare { get; set; }
        
        /// <summary>
        /// Value that will be compared
        /// </summary>
        public T ValueToCompare { get; set; }
        
        /// <summary>
        /// Query operator for this operation
        /// </summary>
        public NumberQueryOperator QueryOperator { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dataSet"></param>
        public NumberFilterOperation()
        {

        }

        #endregion

        /// <summary>
        /// Comparing the property with the assigned value
        /// </summary>
        /// <param name="dataSet">Data set that will be filtered</param>
        /// <returns></returns>
        public IEnumerable Filter(IEnumerable dataSet)
        {
            string symbol = "";

            switch(QueryOperator)
            {
                case NumberQueryOperator.BiggerOrSimilar:
                            symbol = " >= ";
                    break;
                case NumberQueryOperator.BiggerThan:
                        symbol = " > ";
                    break;
                case NumberQueryOperator.LowerOrSimilar:
                        symbol = " <= ";
                    break;
                case NumberQueryOperator.LowerThan:
                        symbol = " < ";
                    break;
            }

            try
            {
                return  dataSet.AsQueryable().Where(PropertyNameToCompare + symbol + ValueToCompare);
            }
            catch
            {
                return dataSet;
            }
        }
    }
}
