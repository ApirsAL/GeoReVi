using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;

namespace GeoReVi
{
    /// <summary>
    /// A view model for lithological section visualization
    /// </summary>
    public class SectionChartViewModel : Screen
    {
        #region Public properties

        /// <summary>
        /// chart object for the litho log
        /// </summary>
        private LithoLogChartObject liLo = new LithoLogChartObject();
        public LithoLogChartObject LiLo
        {
            get => this.liLo;
            set
            {
                this.liLo = value;
                NotifyOfPropertyChange(() => LiLo);
            }
        }


        public string Scale { get => "1 m = " + Environment.NewLine + Math.Round(ChartHeight / (LiLo.Ymax - LiLo.Ymin), 2).ToString() + " px"; set { NotifyOfPropertyChange(() => Scale); } }

        /// <summary>
        /// A view model that holds a multi parametric data set of the laboratory MultiParameterViewModel.MeasPoints
        /// </summary>
        private LoadParameterDataViewModel laboratoryParameterViewModel = new LoadParameterDataViewModel();
        public LoadParameterDataViewModel LaboratoryParameterViewModel
        {
            get => this.laboratoryParameterViewModel;
            set
            {
                this.laboratoryParameterViewModel = value;
                NotifyOfPropertyChange(() => LaboratoryParameterViewModel);
            }
        }

        /// <summary>
        /// Height of the chart
        /// </summary>
        private double chartHeight = 500;

        public double ChartHeight
        {
            get
            {
                return this.chartHeight;
            }
            set { this.chartHeight = value;
                NotifyOfPropertyChange(() => ChartHeight);
                NotifyOfPropertyChange(() => Scale); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        [ImportingConstructor]
        public SectionChartViewModel()
        {

        }

        #endregion

    }
}
