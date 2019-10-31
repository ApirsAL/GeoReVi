using Caliburn.Micro;
using System.Collections.Generic;
using System.Data;

namespace GeoReVi
{
    interface IMultidimensionalDataSetHolder
    {
        BindableCollection<Mesh> DataSet { get; }

        bool HoldsData { get; }
    }
}
