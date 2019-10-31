using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace GeoReVi
{
    /// <summary>
    /// Converting an object to the type of the object
    /// </summary>
    public class GridViewItemValueConverter : BaseValueConverter<GridViewItemValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (ObservableCollection<tblSectionLithofacy>)value;
            }
            catch
            {

            }

            return value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
