using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// A view model for the helix 3D chart
    /// </summary>
    public class HelixChart3DViewModel : Screen
    {

        #region Public properties


        //A chart 3D object
        private Chart3DObject ch3d;
        public Chart3DObject Ch3d
        {
            get => this.ch3d;
            set
            {
                this.ch3d = value;
                NotifyOfPropertyChange(() => Ch3d);
            }
        }

        /// <summary>
        /// Triggers the ZoomExtends method of the viewport
        /// </summary>
        private bool previewUpdatedReZoom = true;
        public bool PreviewUpdatedReZoom
        {
            get => this.previewUpdatedReZoom;
            set
            {
                this.previewUpdatedReZoom = value;
                NotifyOfPropertyChange(() => PreviewUpdatedReZoom);
            }
        }

        #endregion

        #region Constructor

        public HelixChart3DViewModel()
        {
            //Instantiating new chart 3D object
            Ch3d = new Chart3DObject();

        }

        #endregion

        #region Public properties

        /// <summary>
        /// Saving the chart object
        /// </summary>
        public void SaveChartObject()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Chart 3D object (*.ch3d)|*.ch3d";
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
                        case ".ch3d":
                            Ch3d.ToXml(fi.FullName);
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
            openFileDlg.Filter = @"Chart 3D object (*.ch3d)|*.ch3d";
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
                    case ".ch3d":
                        Ch3d = (Chart3DObject)fi.FullName.FromXml<Chart3DObject>();
                        break;
                }
            }
            catch (Exception e)
            {
                ((ShellViewModel)IoC.Get<IShell>()).LogError(e);
            }
        }

        /// <summary>
        /// Triggers viewport in the view to zoom back to extends
        /// </summary>
        public void ZoomToExtends()
        {
            PreviewUpdatedReZoom = false;
            PreviewUpdatedReZoom = true;
        }

        /// <summary>
        /// Adds a point to the collection
        /// </summary>
        public async Task PerformEditing()
        {
            await Ch3d.PerformEditing();
        }
            #endregion


        }
}
