using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.IO;

namespace GeoReVi
{
    public class LineAndScatterChartViewModel : ChartViewModelBase<LineAndScatterChartViewModel>
    {

        #region Public properties

        /// <summary>
        /// Line chart object
        /// </summary>
        private LineChartObject lco;
        public LineChartObject Lco
        {
            get => this.lco;
            set
            {
                this.lco = value;
                NotifyOfPropertyChange(() => Lco);
            }
        }

        #endregion

        #region Constructor

        public LineAndScatterChartViewModel()
        {
            Lco = new LineChartObject();
            Lco.CreateLineChart();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Saving the chart object
        /// </summary>
        public void SaveChartObject()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Line chart object (*.lco)|*.lco";
            saveFileDialog1.RestoreDirectory = true;

            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                try
                {
                    //Getting file information
                    FileInfo fi = new FileInfo(saveFileDialog1.FileName);

                    switch(fi.Extension)
                    {
                        case ".lco":
                            Lco.ToXml(fi.FullName);
                            break;
                    }
                }
                catch(Exception e)
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
            openFileDlg.Filter = @"Line chart object (*.lco)|*.lco";
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
                    case ".lco":
                        Lco = (LineChartObject)fi.FullName.FromXml<LineChartObject>();
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
                ((ShellViewModel)IoC.Get<IShell>()).OpenGenericWindow<LineAndScatterChartViewModel>(new LineAndScatterChartViewModel() { Lco = new LineChartObject(this.Lco) });

            });
        }

        #endregion
    }
}
