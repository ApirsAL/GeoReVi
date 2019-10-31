using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    public class BarChartViewModel : ChartViewModelBase<BarChartViewModel>
    {
        #region Public properties

        private BarChartObject barco;
        public BarChartObject Barco
        {
            get => this.barco;
            set
            {
                this.barco = value;
                NotifyOfPropertyChange(() => Barco);
            }
        }

        #endregion

        #region Constructor

        public BarChartViewModel()
        {
            Barco = new BarChartObject();
        }

        #endregion

        #region Public methods

        public void OpenInWindow()
        {
            new DispatchService().Invoke(
            () =>
            {
                ((ShellViewModel)IoC.Get<IShell>()).OpenGenericWindow<BarChartViewModel>(new BarChartViewModel() { Barco = new BarChartObject(this.Barco)});
            
            });
        }
        #endregion
    }
}
