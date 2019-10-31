using Caliburn.Micro;
using System;

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
        }

        #endregion

        #region Public methods

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
