using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace GeoReVi
{
    /// <summary>
    /// Multi value converter
    /// </summary>
    public class GetPropertyValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var theCollection = values[0] as ObservableCollection<Tuple<string, string>>;
                return (theCollection?[(int)values[1]])?.Item1; //Item1 is the column name, Item2 is the column's ocmbobox's selectedItem
            }
            catch (Exception)
            {
                //use a better implementation!
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
