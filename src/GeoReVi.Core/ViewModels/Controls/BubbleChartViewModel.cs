
using Caliburn.Micro;

namespace GeoReVi
{
    public class BubbleChartViewModel : ChartViewModelBase<BubbleChartViewModel>
    {
        #region Public properties

        private BubbleChartObject bubco;
        public BubbleChartObject Bubco
        {
            get => this.bubco;
            set
            {
                this.bubco = value;
                NotifyOfPropertyChange(() => Bubco);
            }
        }

        #endregion

        #region Constructor

        public BubbleChartViewModel()
        {
            Bubco = new BubbleChartObject();
        }

        #endregion

        #region Public methods

        public void OpenInWindow()
        {
            new DispatchService().Invoke(
            () =>
            {
                ((ShellViewModel)IoC.Get<IShell>()).OpenGenericWindow<BubbleChartViewModel>(new BubbleChartViewModel() { Bubco = new BubbleChartObject(this.Bubco) });

            });
        }

        #endregion
    }
}
