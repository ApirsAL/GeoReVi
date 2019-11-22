using Caliburn.Micro;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.ObjectModel;

namespace GeoReVi
{
    public class BoundaryCondition : PropertyChangedBase
    {
        #region Public properties

        /// <summary>
        /// Type of the boundary condition
        /// </summary>
        private BoundaryConditionType type = BoundaryConditionType.Dirichlet;
        public BoundaryConditionType Type
        {
            get => this.type;
            set
            {
                this.type = value;
                NotifyOfPropertyChange(() => Type);
            }
        }

        /// <summary>
        /// Value of the boundary condition
        /// </summary>
        private double value = 0;
        public double Value
        {
            get => this.value;
            set
            {
                this.value = value;
                NotifyOfPropertyChange(() => Value);
            }
        }

        /// <summary>
        /// Building the boundary condition matrix
        /// </summary>
        private Matrix<double> boundaryConditionMatrix = Matrix<double>.Build.Random(1, 1);
        public Matrix<double> BoundaryConditionMatrix
        {
            get => boundaryConditionMatrix;
            set
            {
                this.boundaryConditionMatrix = value;
            }
        }
        /// <summary>
        /// All points that are assigned to the boundary condition
        /// </summary>
        private ObservableCollection<LocationTimeValue> boundaryPoints = new ObservableCollection<LocationTimeValue>();
        public ObservableCollection<LocationTimeValue> BoundaryPoints
        {
            get => this.boundaryPoints;
            set
            {
                this.boundaryPoints = value;
                NotifyOfPropertyChange(() => BoundaryPoints);
            }
        }

        /// <summary>
        /// Getting the indices of the boundary conditions
        /// </summary>
        private Matrix<double> indices;
        public Matrix<double> Indices
        {
            get => this.indices;
            set
            {
                this.indices = value;
                NotifyOfPropertyChange(() => Indices);
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Builds the boundary conditions
        /// </summary>
        /// <param name="columns"></param>
        public void BuildBoundaryConditionMatrix(int columns)
        {
            try
            {
                BoundaryConditionMatrix = Matrix<double>.Build.Dense(1, columns, Value);
            }
            catch
            {
                throw new Exception("Could not build boundary condition");
            }
        }

        public void BuildIndices(Mesh host)
        {

            try
            {
                indices = Matrix<double>.Build.Dense(BoundaryPoints.Count, 1, 0);

                for (int i = 0; i < BoundaryPoints.Count; i++)
                    indices[i, 0] = host.Vertices.IndexOf(BoundaryPoints[i]);
            }
            catch
            {

            }
        }

        #endregion
    }
}
