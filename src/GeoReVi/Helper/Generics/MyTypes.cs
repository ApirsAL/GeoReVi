using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    public class MyObject
    {
        public dynamic MyType { get; set; }
    }

    public class MyType<T>
    {
        public dynamic Myvalue { get; set; }

        public MyType(T value)
        {
            Myvalue = value;
        }
    }
}
