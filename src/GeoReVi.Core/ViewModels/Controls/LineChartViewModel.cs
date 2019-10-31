using Caliburn.Micro;

namespace GeoReVi
{
    public class LineChartViewModel : ChartViewModelBase<BubbleChartViewModel>
    {

        #region Public properties
        
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

        public LineChartViewModel()
        {
            Lco = new LineChartObject();
        }

        #endregion

        #region Public methods

        public void OpenInWindow()
        {
            new DispatchService().Invoke(
            () =>
            {
                ((ShellViewModel)IoC.Get<IShell>()).OpenGenericWindow<LineChartViewModel>(new LineChartViewModel() { Lco = new LineChartObject(this.Lco) });

            });
        }

        #endregion
    }
}
