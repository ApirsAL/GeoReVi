using System.ComponentModel;
using System.Windows.Controls;

namespace GeoReVi
{
    /// <summary>
    /// Interaktionslogik für ObjectDetailsView.xaml
    /// </summary>
    public partial class FieldMeasurementDetailsView : UserControl
    {
        public FieldMeasurementDetailsView()
        {
            InitializeComponent();
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyDescriptor is PropertyDescriptor descriptor)
            {
                string a = CollectionHelper.GetPropertyDisplayName(descriptor);

                if (a != null)
                {
                    e.Column.Header = a;
                }
                else
                {
                    if (e.Column.Header.ToString() == "Parameter")
                        return;

                    e.Column.Width = 0;
                    e.Column.IsReadOnly = true;
                    e.Cancel = true;

                }
            }
        }
    }
}
