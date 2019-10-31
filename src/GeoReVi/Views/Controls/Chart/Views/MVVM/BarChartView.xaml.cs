using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace GeoReVi
{
    /// <summary>
    /// Interaktionslogik für BarChartView.xaml
    /// </summary>
    public partial class BarChartView : UserControl
    {
        public BarChartView()
        {
            InitializeComponent();
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "PNG (*.png)|*.png|BMP (*.bmp)|*.bmp|EMF (*.emf)|*.emf|PDF (*.pdf)|*.pdf|XAML (*.xaml)|*.xaml";
            saveFileDialog1.RestoreDirectory = true;

            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                //Getting the extension
                var ext = saveFileDialog1.FileName.Substring(saveFileDialog1.FileName.LastIndexOf(".")).ToLower();

                try
                {
                    //Getting the extension
                    var ext1 = saveFileDialog1.FileName.Substring(saveFileDialog1.FileName.LastIndexOf(".")).ToLower();

                    switch (ext1.ToString())
                    {
                        case ".png":
                            //Downloading to the specific folder
                            ImageCapturer.SaveToPng((FrameworkElement)ChartGrid, saveFileDialog1.FileName);
                            break;
                        case ".bmp":
                            ImageCapturer.SaveToBmp((FrameworkElement)ChartGrid, saveFileDialog1.FileName);
                            break;
                        case ".emf":
                            ImageCapturer.SaveToEmf(ChartGrid, saveFileDialog1.FileName);
                            break;
                        case ".xaml":
                            ImageCapturer.SaveToXaml(ChartGrid, saveFileDialog1.FileName);
                            break;
                        case ".pdf":
                            ImageCapturer.SaveToPdf((FrameworkElement)ChartGrid, saveFileDialog1.FileName);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).LogError(ex);
                }
            }
        }
    }
}
