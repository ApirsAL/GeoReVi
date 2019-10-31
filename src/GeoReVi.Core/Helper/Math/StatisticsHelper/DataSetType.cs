using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    public enum DataSetType
    {
        UnivariateScalar = 1,
        UnivariateVector = 2,
        UnivariateTensor = 3,
        MultivariateScalar = 4,
        MultivariateVector = 5,
        MultivariateTensor = 6,
        SpatialUnivariateScalar = 7,
        SpatialUnivariateVector = 8,
        SpatialUnivariateTensor = 9,
        SpatialMultivariateScalar = 10,
        SpatialMultivariateVector = 11,
        SpatialMutlivariateTensor = 12,
    }
}
