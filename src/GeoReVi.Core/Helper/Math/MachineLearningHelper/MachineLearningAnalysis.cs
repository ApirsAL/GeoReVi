using Accord.Math;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GeoReVi
{
    /// <summary>
    /// Abstract class for machine learning algorithms
    /// </summary>
    public abstract class MachineLearningAnalysis : PropertyChangedBase, 
        IMultidimensionalDataSetHolder, IMachineLearningAnalysis
    {
        #region Public properties
        /// <summary>
        /// The calculation DataSet
        /// </summary>
        private double[,] calculationDataSet = new double[,] { };
        public double[,] CalculationDataSet
        {
            get => this.calculationDataSet;
            set
            {
                this.calculationDataSet = value;
                NotifyOfPropertyChange(() => CalculationDataSetView);
            }
        }

        /// <summary>
        /// The view of the calculation data set
        /// </summary>
        public DataTable CalculationDataSetView
        {
            get
            {
                return this.CalculationDataSet.ToTable(DataSet.First().Properties.Select(x => x.Value).ToArray());
            }
        }

        /// <summary>
        /// The data set of this class
        /// </summary>
        private BindableCollection<Mesh> dataSet = new BindableCollection<Mesh>();
        public BindableCollection<Mesh> DataSet
        {
            get => this.dataSet;
            set
            {
                this.dataSet = value;
                NotifyOfPropertyChange(() => DataSet);
            }
        }

        /// <summary>
        /// Checks if the class holds a data set
        /// </summary>
        public bool HoldsData
        {
            get
            {
                if (this.DataSet != null)
                    if (this.DataSet.Count > 1)
                        return true;

                return false;
            }
        }

        /// <summary>
        /// Checks if a computation takes place
        /// </summary>
        private bool isComputing = false;
        public bool IsComputing
        {
            get => isComputing;
            set
            {
                this.isComputing = value;
                NotifyOfPropertyChange(() => IsComputing);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Learn a certain pattern
        /// </summary>
        public void Learn()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Predicts values
        /// </summary>
        public void Predict()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
