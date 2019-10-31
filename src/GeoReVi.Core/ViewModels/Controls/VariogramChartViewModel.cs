
using Caliburn.Micro;

namespace GeoReVi
{
    /// <summary>
    /// Viewmodel for variogram chart objects
    /// </summary>
    public class VariogramChartViewModel : ChartViewModelBase<VariogramChartViewModel>
    {

        #region Public properties

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
