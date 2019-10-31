using System;
using System.Collections;


namespace GeoReVi
{
    public class TextFilterOperation : ITextFilterOperation
    {
        #region Public properties

        #endregion
        public string PropertyNameToCompare { get; set; }

        public string ValueToCompare { get; set; }

        public TextQueryOperator QueryOperator { get; set; }


    }
}
