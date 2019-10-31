using Accord.Math;
using Caliburn.Micro;
using System;
using System.Linq;

namespace GeoReVi
{
    /// <summary>
    /// Numerical solver for differential equations
    /// </summary>
    public abstract class NumericalSolver : PropertyChangedBase, Solver
    {
        #region Public Properties

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
        /// Previous step in the simulation
        /// </summary>
        private Mesh previousStep = new Mesh();
        public Mesh PreviousStep
        {
            get => previousStep;
            set
            {
                this.previousStep = value;
                NotifyOfPropertyChange(() => PreviousStep);
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
        /// All points that are assigned Neumann boundary conditions
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

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public NumericalSolver()
        {

        }

        #endregion

        #region Public Methods

        #endregion

        /// <summary>
        /// Computing the solution
        /// </summary>
        public void Compute()
        {
            try
            {
                //Preparing the mesh for the physical problem
                switch(PhysicalProblem)
                {
                    case PhysicalProblem.HeatConduction:
                        for (int i = 0; i < Fields.Count(); i++)
                        {
                            if(Fields[i].IsConstant)
                                InitialConditions.AddParameter(Fields[i].Value);
                            else
                                InitialConditions.AddParameter(Fields[i].Mesh);
                        }
                        break;
                }

                //Initializing the solution
                Solution = new Mesh(InitialConditions);

                double timeStep = SimulationTime / Convert.ToDouble((TimeSteps - 1));

                //Time discretization
                for (double t = 0; t<SimulationTime; t += timeStep)
                {
                    PreviousStep = new Mesh(Solution);

                    //Iterating over all vertices
                    for (int i = 0; i<Solution.Vertices.Count();i++)
                    {
                        switch (PhysicalProblem)
                        {
                            //Solving the heat conduction problem
                            case PhysicalProblem.HeatConduction:
                                if (Solution.Vertices[i].IsExterior)
                                {
                                    Solution.Vertices[i].Value[0] = BoundaryConditions.Where(x => x.BoundaryPoints.Contains(Solution.Vertices[i])).First().Value;
                                    continue;
                                }

                                //Solution.Vertices[i].Value[0] += PreviousStep.CalculateGradient(PreviousStep.Vertices[i]) * ((PreviousStep.CalculateGradientFunction(PreviousStep.Vertices[i]).Multiply(Solution.Vertices[i].Value[1])));
                                break;
                        }
                    }
                }
            }
            catch
            {

            }
        }
    }
}
