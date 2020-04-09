using Accord.Math;
using Caliburn.Micro;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// Numerical solver for differential equations
    /// </summary>
    public class NumericalSolver : PropertyChangedBase, Solver
    {
        #region Private members

        /// <summary>
        /// Point coordinates
        /// </summary>
        private Matrix<double> coordinates;

        /// <summary>
        /// Point coordinates
        /// </summary>
        private Matrix<double> connectivity;

        /// <summary>
        /// Global load vector
        /// </summary>
        private Vector<double> globalLoadVector;

        /// <summary>
        /// Global load vector
        /// </summary>
        private Vector<double> weights;

        /// <summary>
        /// Global rhs vector
        /// </summary>
        private Vector<double> globalRHSVector;

        /// <summary>
        /// Global solution vector
        /// </summary>
        private Vector<double> globalSolutionVector;

        /// <summary>
        /// LHS matrix
        /// </summary>
        private Matrix<double> lhsMatrix;

        /// <summary>
        /// RHS matrix
        /// </summary>
        private Matrix<double> rhsMatrix;

        /// <summary>
        /// Gauss integration points
        /// </summary>
        private Matrix<double> gaussIntegrationPoints;

        #endregion

        #region Public Properties

        /// <summary>
        /// Checks if the class is performing a computation right now
        /// </summary>
        private bool isComputing = false;
        public bool IsComputing
        {
            get => this.isComputing;
            set
            {
                this.isComputing = value;
                NotifyOfPropertyChange(() => IsComputing);
            }
        }

        /// <summary>
        /// Dimensionality of the problem
        /// </summary>
        public Dimensionality DomainDimensionality
        {
            get => InitialConditions.Dimensionality;
        }


        /// <summary>
        /// The solving method that should be applied
        /// </summary>
        private SolvingMethod solvingMethod = SolvingMethod.FiniteDifferenceMethod;
        public SolvingMethod SolvingMethod
        {
            get => this.solvingMethod;
            set
            {
                this.solvingMethod = value;
                NotifyOfPropertyChange(() => SolvingMethod);
            }
        }


        /// <summary>
        /// End time of the simulation in seconds
        /// </summary>
        private double simulationTime = 1;
        public double SimulationTime
        {
            get => this.simulationTime;
            set
            {
                this.simulationTime = value;
                NotifyOfPropertyChange(() => SimulationTime);
            }
        }

        /// <summary>
        /// Total number of time steps
        /// </summary>
        private int timeSteps = 10;
        public int TimeSteps
        {
            get => this.timeSteps;
            set
            {
                this.timeSteps = value;
                NotifyOfPropertyChange(() => TimeSteps);
            }
        }

        /// <summary>
        /// Initial conditions
        /// </summary>
        private Mesh initialConditions = new Mesh();
        public Mesh InitialConditions
        {
            get => initialConditions;
            set
            {
                this.initialConditions = value;
                NotifyOfPropertyChange(() => InitialConditions);
            }
        }

        /// <summary>
        /// Resulting field
        /// </summary>
        private Mesh solution = new Mesh();
        public Mesh Solution
        {
            get => solution;
            set
            {
                this.solution = value;
                NotifyOfPropertyChange(() => Solution);
            }
        }

        /// <summary>
        /// All boundary conditions
        /// </summary>
        private BindableCollection<BoundaryCondition> boundaryConditions = new BindableCollection<BoundaryCondition>();
        public BindableCollection<BoundaryCondition> BoundaryConditions
        {
            get => this.boundaryConditions;
            set
            {
                this.boundaryConditions = value;
                NotifyOfPropertyChange(() => BoundaryConditions);
            }
        }

        /// <summary>
        /// The physical problem to be solved
        /// </summary>
        private PhysicalProblem physicalProblem = PhysicalProblem.HeatConduction;
        public PhysicalProblem PhysicalProblem
        {
            get => this.physicalProblem;
            set
            {
                this.physicalProblem = value;
                NotifyOfPropertyChange(() => PhysicalProblem);
            }
        }

        /// <summary>
        /// Fields required to solve the differential equation
        /// </summary>
        private BindableCollection<SpatialField> fields = new BindableCollection<SpatialField>();
        public BindableCollection<SpatialField> Fields
        {
            get => this.fields;
            set
            {
                this.fields = value;
                NotifyOfPropertyChange(() => Fields);
            }
        }

        /// <summary>
        /// Helper class for thermal conduction problems
        /// </summary>
        private ThermalConductionHelper thermalConductionHelper = new ThermalConductionHelper();
        public ThermalConductionHelper ThermalConductionHelper
        {
            get => this.thermalConductionHelper;
            set
            {
                this.thermalConductionHelper = value;
                NotifyOfPropertyChange(() => ThermalConductionHelper);
            }
        }


        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public NumericalSolver()
        {
            AddBoundaryConditions();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Computes the numerical solution of the problem
        /// </summary>
        /// <returns></returns>
        public async Task<Mesh> ComputeNumericalSolution()
        {
            CommandHelper ch = new CommandHelper();

            await Task.WhenAll(ch.RunBackgroundWorkerWithFlagHelperAsync(() => IsComputing, async () =>
            {
                Compute();
            }));

            return Solution;
        }

        /// <summary>
        /// Computing the solution
        /// </summary>
        public void Compute()
        {
            try
            {
                //Validation variables
                DataTable dat = new DataTable();
                dat.Columns.Add(new DataColumn("Time", typeof(double)));
                dat.Columns.Add(new DataColumn("Value", typeof(double)));
                int validationPointIndex = InitialConditions.Vertices.IndexOf(InitialConditions.Vertices.Where(x => x.MeshIndex[0] == 1 && x.MeshIndex[1] == 10 && x.MeshIndex[2] == 10).First());

                //Initializing all elements needed for the solution
                globalLoadVector = Vector<double>.Build.Dense(InitialConditions.Vertices.Count(), 0);

                globalRHSVector = Vector<double>.Build.Dense(InitialConditions.Vertices.Count(), 0);

                globalSolutionVector = Vector<double>.Build.Dense(InitialConditions.Vertices.Count(), 0);

                alglib.sparsematrix lhsMatrix;
                alglib.sparsecreate(InitialConditions.Vertices.Count(), InitialConditions.Vertices.Count(), out lhsMatrix);

                //lhsMatrix = Matrix<double>.Build.Sparse(InitialConditions.Vertices.Count(), InitialConditions.Vertices.Count());

                rhsMatrix = Matrix<double>.Build.Sparse(InitialConditions.Vertices.Count(), InitialConditions.Vertices.Count());

                int nodesPerElement = InitialConditions.MeshCellType == MeshCellType.Hexahedral ? 8 : 4;

                //Getting dimensionality of the problem
                int dimensionality = DomainDimensionality == Dimensionality.OneD ? 1 : (DomainDimensionality == Dimensionality.TwoD ? 2 : (DomainDimensionality == Dimensionality.ThreeD ? 3 : 0));

                //Getting the number of integration points
                int numberOfIntegrationPoints = GetNumberOfIntegrationPoints(dimensionality);

                //Building the boundary conditions
                BuildBoundaryConditions();

                //Intitialising Gauss integration points
                BuildGaussIntegrationPoints(dimensionality);

                //Intitialising weights
                weights = Vector<double>.Build.Dense(numberOfIntegrationPoints, 1.0);

                double timeStep = SimulationTime / Convert.ToDouble((TimeSteps - 1));

                //Preparing the mesh for the physical problem
                switch (PhysicalProblem)
                {
                    case PhysicalProblem.HeatConduction:
                        for (int i = 0; i < InitialConditions.Cells.Count(); i++)
                        {
                            Vector<double> num = Vector<double>.Build.DenseOfArray(InitialConditions.Cells[i].GetNodeIndices(InitialConditions));
                            Matrix<double> coords = Matrix<double>.Build.Dense(nodesPerElement, dimensionality);

                            for (int j = 0; j<num.Count;j++)
                            {
                                int index = Convert.ToInt32(num[j]);

                                LocationTimeValue loc = InitialConditions.Vertices[Convert.ToInt32(num[j])];

                                if(dimensionality == 3)
                                {
                                    coords[j, 0] = loc.X;
                                    coords[j, 1] = loc.Y;
                                    coords[j, 2] = loc.Z;
                                }
                            }

                            //Initialize elements for global matrix
                            Matrix<double> KM = Matrix<double>.Build.Dense(nodesPerElement, nodesPerElement, 0);
                            Matrix<double> MM = Matrix<double>.Build.Dense(nodesPerElement, nodesPerElement, 0);
                            Vector<double> F = Vector<double>.Build.Dense(nodesPerElement, 0);

                            for (int j = 0; j < numberOfIntegrationPoints; j++)
                            {
                                //Building shape functions and shape function derivates
                                Vector<double> shapeFunction = GetShapeFunctions(numberOfIntegrationPoints, j);
                                Matrix<double> shapeDerivates = GetShapeFunctionDerivatives(numberOfIntegrationPoints, j, dimensionality);

                                //Building jacobi matrix, its determinant and inverse
                                Matrix<double> jacobiMatrix = shapeDerivates.Multiply(coords);
                                double determinantJacobi = jacobiMatrix.Determinant();
                                Matrix<double> inverseJacobi = jacobiMatrix.Inverse();

                                //Derivate of shape functions in physical coordinates
                                Matrix<double> shapeDerivatesPhysical = inverseJacobi.Multiply(shapeDerivates);

                                //Building matrix from shape function vector
                                Matrix<double> shapeFuntionMatrix = shapeFunction.ToColumnMatrix();
                                Matrix<double> shapeFuntionRowMatrix = shapeFunction.ToRowMatrix();

                                //Element stiffness matrix
                                KM = KM.Add(shapeDerivatesPhysical.Transpose().Multiply(ThermalConductionHelper.ThermalDiffusivityMatrix).Multiply(shapeDerivatesPhysical).Multiply(determinantJacobi).Multiply(weights[j]));
                                //Element mass matrix
                                MM = MM.Add(shapeFuntionMatrix.Multiply(shapeFuntionRowMatrix).Multiply(determinantJacobi).Multiply(weights[j]));
                                //Element load vector
                                F = F.Add(shapeFunction.Multiply(ThermalConductionHelper.HeatSoruce).Multiply(determinantJacobi).Multiply(weights[j]));
                            }

                            Matrix<double> elementLhsMatrix = MM.Divide(SimulationTime).Add(KM);
                            Matrix<double> elementRhsMatrix = MM.Divide(SimulationTime);

                            //Inserting the element matrix values in the global sparse matrices
                            for (int j = 0; j < num.Count; j++)
                            {
                                for(int k = 0; k<num.Count;k++)
                                {
                                    int coordinateIndex1 = Convert.ToInt32(num[j]);
                                    int coordinateIndex2 = Convert.ToInt32(num[k]);
                                    alglib.sparseset(lhsMatrix, coordinateIndex1, coordinateIndex2, alglib.sparseget(lhsMatrix, coordinateIndex1, coordinateIndex2) + elementLhsMatrix[j, k]);
                                    rhsMatrix[coordinateIndex1, coordinateIndex2] += elementRhsMatrix[j, k];
                                }
                                //lhsMatrix[coordinateIndex, coordinateIndex] += elementLhsMatrix[j, j];
                            }
                        }

                        //Setting initial conditions
                        globalSolutionVector = Vector<double>.Build.DenseOfEnumerable(InitialConditions.Vertices.Select(x => x.Value[0]));

                        //Impose boundary conditions
                        for (int i = 0; i < BoundaryConditions.Count(); i++)
                        {
                            BoundaryConditions[i].BuildIndices(InitialConditions);

                            for (int j = 0; j < BoundaryConditions[i].Indices.RowCount; j++)
                            {
                                int index = Convert.ToInt32(BoundaryConditions[i].Indices[j, 0]);

                                for (int k = 0; k < lhsMatrix.innerobj.m; k++)
                                {
                                    if(k!=index)
                                        alglib.sparseset(lhsMatrix, index, k, 0);
                                    else
                                        alglib.sparseset(lhsMatrix, k, k, 1);
                                }

                            }
                        }
                        
                        //Initializing actual time
                        double actualTime = 0;

                        //Time discretization
                        for (actualTime = 0; actualTime < SimulationTime; actualTime += timeStep)
                        {
                            //Form rhs global vector
                            globalRHSVector = rhsMatrix.Multiply(globalSolutionVector).Add(globalLoadVector);

                            //Impose boundary conditions
                            for(int i =0; i<BoundaryConditions.Count();i++)
                            {
                                BoundaryConditions[i].BuildIndices(InitialConditions);

                                for (int j = 0; j<BoundaryConditions[i].Indices.RowCount;j++)
                                {
                                    if(BoundaryConditions[i].Type == BoundaryConditionType.Dirichlet)
                                        globalRHSVector[Convert.ToInt32(BoundaryConditions[i].Indices[j, 0])] = BoundaryConditions[i].Value;
                                    else if(BoundaryConditions[i].Type == BoundaryConditionType.Neumann)
                                        globalRHSVector[Convert.ToInt32(BoundaryConditions[i].Indices[j, 0])] += BoundaryConditions[i].Value;
                                }
                            }

                            //Solving the LSE with the alglib sparse linear algebra library
                            alglib.sparseconverttocrs(lhsMatrix);

                            double[] solution;

                            alglib.lincgstate s;
                            alglib.lincgreport rep;
                            alglib.lincgcreate(globalRHSVector.Count, out s);
                            alglib.lincgsolvesparse(s, lhsMatrix, true, globalRHSVector.ToArray());
                            alglib.lincgresults(s, out solution, out rep);

                            //Reassign values back to the global solution vector
                            globalSolutionVector = Vector<double>.Build.DenseOfArray(solution);

                            dat.Rows.Add(actualTime, globalSolutionVector[validationPointIndex]);
                        }

                        //Initializing the solution mesh
                        Solution = new Mesh(InitialConditions);

                        for (int i = 0; i < globalSolutionVector.Count; i++)
                            Solution.Vertices[i].Value[0] = globalSolutionVector[i];

                        //Building the mesh
                        Solution.CellsFromPointCloud();

                        break;
                }
            }
            catch
            {
                throw new Exception("Numerical solution failed.");
            }
        }

        /// <summary>
        /// Adding the base classes of boundary conditions
        /// </summary>
        private void AddBoundaryConditions()
        {
            try
            {
                BoundaryConditions.Clear();

                BoundaryConditions.Add(new BoundaryCondition()
                {
                    Region = "Min x",
                    Type = BoundaryConditionType.Dirichlet,
                    Value = 0
                });

                BoundaryConditions.Add(new BoundaryCondition()
                {
                    Region = "Max x",
                    Type = BoundaryConditionType.Dirichlet,
                    Value = 0
                });

                BoundaryConditions.Add(new BoundaryCondition()
                {
                    Region = "Min y",
                    Type = BoundaryConditionType.Dirichlet,
                    Value = 0
                });

                BoundaryConditions.Add(new BoundaryCondition()
                {
                    Region = "Max y",
                    Type = BoundaryConditionType.Dirichlet,
                    Value = 0
                });

                BoundaryConditions.Add(new BoundaryCondition()
                {
                    Region = "Min z",
                    Type = BoundaryConditionType.Dirichlet,
                    Value = 0
                });

                BoundaryConditions.Add(new BoundaryCondition()
                {
                    Region = "Max z",
                    Type = BoundaryConditionType.Dirichlet,
                    Value = 0
                });

            }
            catch
            {
                throw new Exception("The construction of boundary conditions failed.");
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Building the Shape functions and derivatives
        /// </summary>
        private Vector<double> GetShapeFunctions(int numberIntegrationPoints, int integrationPoint)
        {
            try
            {
                //Constructing local coordinates
                double xi = gaussIntegrationPoints[integrationPoint, 0];
                double eta = gaussIntegrationPoints[integrationPoint, 1];
                double zeta = gaussIntegrationPoints[integrationPoint, 2];
                double etam = 1 - eta;
                double xim = 1 - xi;
                double zetam = 1 - zeta;
                double etap = eta + 1;
                double xip = xi + 1;
                double zetap = zeta + 1;

                //Returning the shape functions for all points
                if (numberIntegrationPoints == 8)
                {
                    double[] shapeFunctions = new double[8]
                    {
                            0.125 * xim * etam * zetam,
                            0.125 * xim * etam * zetap,
                            0.125 * xip * etam * zetap,
                            0.125 * xip * etam * zetam,
                            0.125 * xim * etap * zetam,
                            0.125 * xim * etap * zetap,
                            0.125 * xip * etap * zetap,
                            0.125 * xip * etap * zetam

                    };

                    return Vector<double>.Build.DenseOfArray(shapeFunctions);
                }
                else
                    return null;
            }
            catch
            {
                throw new Exception("Shape function construction failed.");
            }
        }

        /// <summary>
        /// Building the Shape function derivatives
        /// </summary>
        private Matrix<double> GetShapeFunctionDerivatives(int numberIntegrationPoints, int integrationPoint, int dimensionality)
        {
            try
            {
                //Constructing local coordinates
                double xi = gaussIntegrationPoints[integrationPoint, 0];
                double eta = gaussIntegrationPoints[integrationPoint, 1];
                double zeta = gaussIntegrationPoints[integrationPoint, 2];
                double etam = 1 - eta;
                double xim = 1 - xi;
                double zetam = 1 - zeta;
                double etap = eta + 1;
                double xip = xi + 1;
                double zetap = zeta + 1;

                double[,] derivatives = new double[dimensionality, numberIntegrationPoints];

                //Returning the derivatives of the shape functions for all points
                if (numberIntegrationPoints == 8 && dimensionality == 3)
                {
                    derivatives[0, 0] = -.125 * etam * zetam;
                    derivatives[0, 1] = -.125 * etam * zetap;
                    derivatives[0, 2] = .125 * etam * zetap;
                    derivatives[0, 3] = .125 * etam * zetam;
                    derivatives[0, 4] = -.125 * etap * zetam;
                    derivatives[0, 5] = -.125 * etap * zetap;
                    derivatives[0, 6] = .125 * etap * zetap;
                    derivatives[0, 7] = .125 * etap * zetam;
                    derivatives[1, 0] = -.125 * xim * zetam;
                    derivatives[1, 1] = -.125 * xim * zetap;
                    derivatives[1, 2] = -.125 * xip * zetap;
                    derivatives[1, 3] = -.125 * xip * zetam;
                    derivatives[1, 4] = .125 * xim * zetam;
                    derivatives[1, 5] = .125 * xim * zetap;
                    derivatives[1, 6] = .125 * xip * zetap;
                    derivatives[1, 7] = .125 * xip * zetam;
                    derivatives[2, 0] = -.125 * xim * etam;
                    derivatives[2, 1] = .125 * xim * etam;
                    derivatives[2, 2] = .125 * xip * etam;
                    derivatives[2, 3] = -.125 * xip * etam;
                    derivatives[2, 4] = -.125 * xim * etap;
                    derivatives[2, 5] = .125 * xim * etap;
                    derivatives[2, 6] = .125 * xip * etap;
                    derivatives[2, 7] = -.125 * xip * etap;
                }
                else
                {
                    return null;
                }

                return Matrix<double>.Build.DenseOfArray(derivatives);
            }
            catch
            {
                throw new Exception("Shape derivative matrix construction failed.");
            }
        }

        /// <summary>
        /// Building the boundary conditions
        /// </summary>
        private void BuildBoundaryConditions()
        {
            try
            {
                int xMinIndex = InitialConditions.Vertices.Min(x => x.MeshIndex[0]);
                int xMaxIndex = InitialConditions.Vertices.Max(x => x.MeshIndex[0]);
                int yMinIndex = InitialConditions.Vertices.Min(x => x.MeshIndex[1]);
                int yMaxIndex = InitialConditions.Vertices.Max(x => x.MeshIndex[1]);
                int zMinIndex = InitialConditions.Vertices.Min(x => x.MeshIndex[2]);
                int zMaxIndex = InitialConditions.Vertices.Max(x => x.MeshIndex[2]);

                for (int i =0;i<BoundaryConditions.Count();i++)
                {
                    BoundaryConditions[i].BoundaryPoints.Clear();

                    switch(BoundaryConditions[i].Region)
                    {
                        case "Min x":
                            BoundaryConditions[i].BoundaryPoints.AddRange(InitialConditions.Vertices.Where(x => x.MeshIndex[0] == xMinIndex));
                            break;
                        case "Max x":
                            BoundaryConditions[i].BoundaryPoints.AddRange(InitialConditions.Vertices.Where(x => x.MeshIndex[0] == xMaxIndex));
                            break;
                        case "Min y":
                            BoundaryConditions[i].BoundaryPoints.AddRange(InitialConditions.Vertices.Where(x => x.MeshIndex[1] == yMinIndex));
                            break;
                        case "Max y":
                            BoundaryConditions[i].BoundaryPoints.AddRange(InitialConditions.Vertices.Where(x => x.MeshIndex[1] == yMaxIndex));
                            break;
                        case "Min z":
                            BoundaryConditions[i].BoundaryPoints.AddRange(InitialConditions.Vertices.Where(x => x.MeshIndex[2] == zMinIndex));
                            break;
                        case "Max z":
                            BoundaryConditions[i].BoundaryPoints.AddRange(InitialConditions.Vertices.Where(x => x.MeshIndex[2] == zMaxIndex));
                            break;
                    }
                }
            }
            catch
            {
                throw new Exception("Building boundary conditions was not successful");
            }

        }

        /// <summary>
        /// Build the gauss integration points
        /// </summary>
        private void BuildGaussIntegrationPoints(int dimensionality)
        {
            try
            {
                //building intregration point matrix
                double[,] pts = new double[GetNumberOfIntegrationPoints(dimensionality), dimensionality];

                switch (InitialConditions.MeshCellType)
                {
                    case MeshCellType.Hexahedral:
                        double root3 = 1.0 / Math.Sqrt(3);

                        pts[0, 0] = root3;
                        pts[0, 1] = root3;
                        pts[0, 2] = root3;

                        pts[1, 0] = root3;
                        pts[1, 1] = root3;
                        pts[1, 2] = -root3;

                        pts[2, 0] = root3;
                        pts[2, 1] = -root3;
                        pts[2, 2] = root3;

                        pts[3, 0] = root3;
                        pts[3, 1] = root3;
                        pts[3, 2] = root3;

                        pts[4, 0] = -root3;
                        pts[4, 1] = root3;
                        pts[4, 2] = root3;

                        pts[5, 0] = -root3;
                        pts[5, 1] = -root3;
                        pts[5, 2] = root3;

                        pts[6, 0] = -root3;
                        pts[6, 1] = root3;
                        pts[6, 2] = -root3;

                        pts[7, 0] = -root3;
                        pts[7, 1] = -root3;
                        pts[7, 2] = -root3;
                        break;
                }

                gaussIntegrationPoints = Matrix<double>.Build.DenseOfArray(pts);
            }
            catch
            {
                throw new Exception("Building gauss integration matrix was not successful");
            }
        }

        /// <summary>
        /// Returns the number of integration points
        /// </summary>
        /// <param name="dimensionality"></param>
        /// <returns></returns>
        public int GetNumberOfIntegrationPoints(int dimensionality)
        {
            try
            {
                return Convert.ToInt32(Math.Pow(2, dimensionality));
            }
            catch
            {
                return int.MinValue;
            }
        }

        #endregion
    }
}
