using Caliburn.Micro;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Core;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace GeoReVi
{
    /// <summary>
    /// Interaktionslogik für MapView.xaml
    /// </summary>
    public partial class MapView : UserControl
    {
        public MapView()
        {
            InitializeComponent();
            ApplicationIdCredentialsProvider applicationIdCredentialsProvider = new ApplicationIdCredentialsProvider(GetBingKey());
            myBingMap.CredentialsProvider = applicationIdCredentialsProvider;
            this.DataContext = new MapViewModel(new MapStyle(this.myBingMap, (IEventAggregator)((ShellViewModel)IoC.Get<IShell>())._events), (IEventAggregator)((ShellViewModel)IoC.Get<IShell>())._events);
        }

        /// <summary>
        /// Gets the bing key from the associated file
        /// </summary>
        /// <returns></returns>
        private string GetBingKey()
        {
            string ret = "";

            try
            {
                string dir = System.IO.Path.GetDirectoryName(
    System.Reflection.Assembly.GetExecutingAssembly().Location);

                string file = "";

                file = dir + @"\Media\Data\K.csv";

                var stream = File.OpenRead(file);

                foreach (string line in File.ReadLines(file, Encoding.ASCII))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    else
                        ret = line;
                }
            }
            catch
            {

            }

            return ret;
        }
    }
}
