using System.Windows.Controls;
using System.ComponentModel;

namespace GeoReVi
{
    /// <summary>
    /// Interaktionslogik für RockSampleDetailsView.xaml
    /// </summary>
    public partial class LabMeasurementDetailsView : UserControl
    {

        public LabMeasurementDetailsView()
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
