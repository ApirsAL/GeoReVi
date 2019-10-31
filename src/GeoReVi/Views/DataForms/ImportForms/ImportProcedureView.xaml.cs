using System.ComponentModel;
using System.Windows.Controls;


namespace GeoReVi
{
    /// <summary>
    /// Interaktionslogik für ImportProcedureView.xaml
    /// </summary>
    public partial class ImportProcedureView : UserControl
    {
        public ImportProcedureView()
        {
            InitializeComponent();
        }

        // This snippet is much safer in terms of preventing unwanted
        // Exceptions because of missing [DisplayNameAttribute].
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
                    e.Column.Width = 0;
                    e.Column.IsReadOnly = true;
                    e.Cancel = true;

                }
            }
        }
    }
}
