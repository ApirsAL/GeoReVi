using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.IO;

namespace GeoReVi
{
    public class TernaryChartViewModel : ChartViewModelBase<TernaryChartViewModel>
    {

        #region Public properties

        private TernaryChartObject tco;
        public TernaryChartObject Tco
        {
            get => this.tco;
            set
            {
                this.tco = value;
                NotifyOfPropertyChange(() => Tco);
            }
        }

        #endregion

        #region Constructor

        public TernaryChartViewModel()
        {
            Tco = new TernaryChartObject();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Saving the chart object
        /// </summary>
        public void SaveChartObject()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Ternary object (*.tco)|*.tco";
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
                        case ".tco":
                            Tco.ToXml(fi.FullName);
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
            openFileDlg.Filter = @"Ternary chart object (*.tco)|*.tco";
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
                    case ".tco":
                        Tco = (TernaryChartObject)fi.FullName.FromXml<TernaryChartObject>();
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
                ((ShellViewModel)IoC.Get<IShell>()).OpenGenericWindow<TernaryChartViewModel>(new TernaryChartViewModel() { Tco = new TernaryChartObject(this.Tco) });

            });
        }
        #endregion
    }
}
