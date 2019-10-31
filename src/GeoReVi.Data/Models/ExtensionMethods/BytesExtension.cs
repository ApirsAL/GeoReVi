using System.Collections.Generic;
using System.Linq;

namespace GeoReVi
{
    public static class BytesExtension
    {
        public static string ToHexadecimalString(this IEnumerable<byte> bytes)
        {
            return "0x" + string.Concat(bytes.Select(b => b.ToString("X2")));
        }
    }
}
