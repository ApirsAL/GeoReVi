
using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.IO;

namespace GeoReVi
{
    /// <summary>
    /// Viewmodel for variogram chart objects
    /// </summary>
    public class VariogramChartViewModel : ChartViewModelBase<VariogramChartViewModel>
    {

        #region Public properties

        /// <summary>
        /// The corresponding chart object
        /// </summary>
        private VariogramChartObject vco;
        public VariogramChartObject Vco
        {
            get => this.vco;
            set
            {
                this.vco = value;
                NotifyOfPropertyChange(() => Vco);
            }
        }

        #endregion

        #region Constructor

        public VariogramChartViewModel()
        {
            Vco = new VariogramChartObject();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Saving the chart object
        /// </summary>
        public void SaveChartObject()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Variogram chart object (*.vco)|*.vco";
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
                        case ".vco":
                            Vco.ToXml(fi.FullName);
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
            openFileDlg.Filter = @"Variogram chart object (*.vco)|*.vco";
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
                    case ".vco":
                        Vco = (VariogramChartObject)fi.FullName.FromXml<VariogramChartObject>();
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
                ((ShellViewModel)IoC.Get<IShell>()).OpenGenericWindow<VariogramChartViewModel>(new VariogramChartViewModel() { Vco = new VariogramChartObject(this.Vco) });

            });
        }
        #endregion
    }
}
