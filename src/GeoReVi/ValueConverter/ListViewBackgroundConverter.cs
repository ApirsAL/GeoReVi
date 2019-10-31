using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace GeoReVi
{
    /// <summary>
    /// Background value converter for listview items
    /// FROM https://blogs.msdn.microsoft.com/atc_avalon_team/2006/03/23/alternate-background-for-listviewitems/
    /// </summary>
    public sealed class BackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ListViewItem item = (ListViewItem)value;


            ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;


            // Get the index of a ListViewItem
            int index = listView.ItemContainerGenerator.IndexFromContainer(item);

            if (index % 2 == 0)
            {
                return Brushes.Red;
            }
            else
            {
                return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}