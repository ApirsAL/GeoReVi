using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.IO;

namespace GeoReVi
{
    public class BoxWhiskerChartViewModel : ChartViewModelBase<BoxWhiskerChartViewModel>
    {
        #region Public properties

        private BoxPlotChartObject bco;
        public BoxPlotChartObject Bco
        {
            get => this.bco;
            set
            {
                this.bco = value;
                NotifyOfPropertyChange(() => Bco);
            }
        }

        #endregion

        #region Constructor

        public BoxWhiskerChartViewModel()
        {
            Bco = new BoxPlotChartObject();
            Bco.CreateChart();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Saving the chart object
        /// </summary>
        public void SaveChartObject()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Box-whisker chart object (*.bco)|*.bco";
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
                        case ".bco":
                            Bco.ToXml(fi.FullName);
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
            openFileDlg.Filter = @"Box-whisker chart object (*.bco)|*.bco";
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
                    case ".bco":
                        Bco = (BoxPlotChartObject)fi.FullName.FromXml<BoxPlotChartObject>();
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
                ((ShellViewModel)IoC.Get<IShell>()).OpenGenericWindow<BoxWhiskerChartViewModel>(new BoxWhiskerChartViewModel() { Bco =new BoxPlotChartObject(this.Bco) });

            });
        }
        
        #endregion
    }
}
