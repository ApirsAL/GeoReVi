using Caliburn.Micro;

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
