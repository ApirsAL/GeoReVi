using System;
using System.Reflection;
using System.Windows.Media;

namespace GeoReVi
{
    public static class ColorHelper
    {
        /// <summary>
        /// Returns a random brush
        /// </summary>
        /// <returns></returns>
        public static Brush PickBrush()
        {
            Brush result = Brushes.Transparent;

            Random rnd = new Random();

            Type brushesType = typeof(Brushes);

            PropertyInfo[] properties = brushesType.GetProperties();

            int random = rnd.Next(properties.Length);
            result = (Brush)properties[random].GetValue(null, null);

            return result;
        }

    }
}
