using Caliburn.Micro;

namespace GeoReVi
{
    public class LineChart3DViewModel : ChartViewModelBase<LineChart3DViewModel>
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

        //A view model for variogram interaction
        private VariogramChartViewModel variogramChartViewModel = new VariogramChartViewModel();
        public VariogramChartViewModel VariogramChartViewModel
        {
            get => this.variogramChartViewModel;
            set
            {
                this.variogramChartViewModel = value;
                NotifyOfPropertyChange(() => VariogramChartViewModel);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public LineChart3DViewModel()
        {
            VariogramChartViewModel = new VariogramChartViewModel();
            Ch3d = new Chart3DObject();
            Ch3d.SpatialInterpolationHelper.Vh = VariogramChartViewModel.Vco.Vh;
            VariogramChartViewModel.Vco.DataSet = Ch3d.DataSet;
        }

        #endregion

        #region Public methods

        public void OpenInWindow()
        {
            new DispatchService().Invoke(
            () =>
            {
                ((ShellViewModel)IoC.Get<IShell>()).OpenGenericWindow<LineChart3DViewModel>(new LineChart3DViewModel() { Ch3d = new Chart3DObject(this.Ch3d) });

            });
        }

        #endregion
    }
}
