using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    public class BarChartViewModel : ChartViewModelBase<BarChartViewModel>
    {
        #region Public properties

        private BarChartObject barco;
        public BarChartObject Barco
        {
            get => this.barco;
            set
            {
                this.barco = value;
                NotifyOfPropertyChange(() => Barco);
            }
        }

        #endregion

        #region Constructor

        public BarChartViewModel()
        {
            Barco = new BarChartObject();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Saving the chart object
        /// </summary>
        public void SaveChartObject()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Bar object (*.baco)|*.baco";
            saveFileDialog1.RestoreDirectory = true;

            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                try
                {
                    //Getting file information
                    FileInfo fi = new FileInfo(saveFileDialog1.FileName);

                    switch (fi.Extension)
                    {
                        case ".baco":
                            Barco.ToXml(fi.FullName);
                            break;
                    }
                }
                catch (Exception e)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).LogError(e);
                }
            }
        }

        /// <summary>
        /// Loading the chart object
        /// </summary>
        public void LoadChartObject()
        {
            //File dialog for opening a jpeg, png or bmp file
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Filter = @"Bar chart object (*.baco)|*.baco";
            openFileDlg.RestoreDirectory = true;
            openFileDlg.ShowDialog();

            if (openFileDlg.FileName == "")
            {
                return;
            }

            //Getting file information
            FileInfo fi = new FileInfo(openFileDlg.FileName);

            try
            {
                switch (fi.Extension)
                {
                    case ".baco":
                        Barco = (BarChartObject)fi.FullName.FromXml<BarChartObject>();
                        break;
                }
            }
            catch (Exception e)
            {
                ((ShellViewModel)IoC.Get<IShell>()).LogError(e);
            }
        }

        public void OpenInWindow()
        {
            new DispatchService().Invoke(
            () =>
            {
                ((ShellViewModel)IoC.Get<IShell>()).OpenGenericWindow<BarChartViewModel>(new BarChartViewModel() { Barco = new BarChartObject(this.Barco)});
            
            });
        }
        #endregion
    }
}
