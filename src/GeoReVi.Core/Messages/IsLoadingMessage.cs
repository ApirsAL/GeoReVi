using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    public class IsLoadingMessage
    {
        public IsLoadingMessage(object process)
        {
            ProcessId = process;
        }

        public object ProcessId
        {
            get;
            private set;
        }
    }
}
