using Caliburn.Micro;
using System;

namespace GeoReVi
{
    public class AdditionalShellViewModel : Conductor<object>, IScreen
    {

        //The currently active item
        private string currentActiveItem = "ChartWrapView";

        //EventAggregator
        private readonly IEventAggregator _events;

        /// <summary>
        /// The current active item
        /// </summary>
        public string CurrentActiveItem
        {
            get
            {
                return this.currentActiveItem;
            }
            set
            {
                this.currentActiveItem = value;
                NotifyOfPropertyChange(() => CurrentActiveItem);
            }
        }
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="events"></param>
        /// <param name="type"></param>
        public AdditionalShellViewModel(IEventAggregator events, string type = "")
        {
            this._events = events;

            _events.Subscribe(this);

            LoadView(type);
        }

        /// <summary>
        /// Constructor with additional message
        /// </summary>
        /// <param name="events"></param>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public AdditionalShellViewModel(IEventAggregator events, string message, string type = "")
        {
            this._events = events;

            _events.Subscribe(this);

            LoadView(type, message);
        }

        /// <summary>
        /// Constructor for loading a 3d viewpanel
        /// </summary>
        /// <param name="events"></param>
        /// <param name="importProcedureViewModel"></param>
        public AdditionalShellViewModel(IEventAggregator events, ImportLocationValuesViewModel locationViewModel)
        {
            this._events = events;

            _events.Subscribe(this);

            LoadImportView(locationViewModel);
        }


        #endregion
        /// <summary>
        /// Loading a viewmodel based on an input parameter
        /// </summary>
        /// <param name="activeItem"></param>
        public void LoadView(string activeItem, string message = "")
        {
            this.CurrentActiveItem = activeItem;

            switch (activeItem)
            {
                //Objects
                case "Outcrop":
                    OutcropDetailsViewModel outcropDetailsViewModel = new OutcropDetailsViewModel(message);
                    ActivateItem(outcropDetailsViewModel);
                    break;
                case "Drilling":
                    DrillingDetailsViewModel drillingDetailsViewModel = new DrillingDetailsViewModel(message);
                    ActivateItem(drillingDetailsViewModel);
                    break;
                case "Transect":
                    TransectDetailsViewModel transectDetailsViewModel = new TransectDetailsViewModel(message);
                    ActivateItem(transectDetailsViewModel);
                    break;

                //Lithologies
                case "Siliciclastic":
                    SiliciclasticDetailsViewModel siliciclasticDetailsViewModel = new SiliciclasticDetailsViewModel(Convert.ToInt32(message));
                    ActivateItem(siliciclasticDetailsViewModel);
                    break;
                case "Biochemical":
                    BiochemicalDetailsViewModel biochemicalDetailsViewModel = new BiochemicalDetailsViewModel(Convert.ToInt32(message));
                    ActivateItem(biochemicalDetailsViewModel);
                    break;
                case "Volcanic":
                    VolcanicDetailsViewModel volcanicDetailsViewModel = new VolcanicDetailsViewModel(Convert.ToInt32(message));
                    ActivateItem(volcanicDetailsViewModel);
                    break;
                case "Igneous":
                    IgneousDetailsViewModel igneousDetailsViewModel = new IgneousDetailsViewModel(Convert.ToInt32(message));
                    ActivateItem(igneousDetailsViewModel);
                    break;

                //Rock sample forms
                case "Plug":
                    PlugDetailsViewModel plugDetailsViewModel = new PlugDetailsViewModel(message);
                    ActivateItem(plugDetailsViewModel);
                    break;
                case "Cuboid":
                    CuboidDetailsViewModel cuboidDetailsViewModel = new CuboidDetailsViewModel(message);
                    ActivateItem(cuboidDetailsViewModel);
                    break;
                case "Handpiece":
                    HandpieceDetailsViewModel handpieceDetailsViewModel = new HandpieceDetailsViewModel(message);
                    ActivateItem(handpieceDetailsViewModel);
                    break;
                case "ThinSection":
                    ThinSectionDetailsViewModel thinSectionDetailsViewModel = new ThinSectionDetailsViewModel(message);
                    ActivateItem(thinSectionDetailsViewModel);
                    break;
                case "Powder":
                    PowderDetailsViewModel powderDetailsViewModel = new PowderDetailsViewModel(message);
                    ActivateItem(powderDetailsViewModel);
                    break;

                //Stratigraphy
                case "Basin":
                    BasinDetailsViewModel basinDetailsViewModel = new BasinDetailsViewModel(this._events);
                    ActivateItem(basinDetailsViewModel);
                    break;
                case "Lithostratigraphy":
                    LithostratigraphyDetailsViewModel lithostratigraphyDetailsViewModel = new LithostratigraphyDetailsViewModel(this._events);
                    ActivateItem(lithostratigraphyDetailsViewModel);
                    break;
                default:
                    ActivateItem(new HomeViewModel());
                    break;
            }
        }

        /// <summary>
        /// Loading a 3d view panel
        /// </summary>
        /// <param name="zoom"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        private void LoadImportView(ImportLocationValuesViewModel viewModel)
        {
            ActivateItem(viewModel);
        }

        /// <summary>
        /// Closing the additional shell window
        /// </summary>
        public void Close()
        {
            TryClose();
        }
    }

    /// <summary>
    /// Generic viewmodel for handling generic types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AdditionalShellViewModel<T, U> : AdditionalShellViewModel
        where T : class, new()
        where U : class, new()
    {
        /// <summary>
        /// Constructor for loading an import procedure view
        /// </summary>
        /// <param name="events"></param>
        /// <param name="importProcedureViewModel"></param>
        public AdditionalShellViewModel(IEventAggregator events, ImportProcedureViewModel<T, U> importProcedureViewModel) : base(events)
        {
            LoadImportProcedureView(importProcedureViewModel);
        }

        /// <summary>
        /// Loading an import procedure view
        /// </summary>
        /// <param name="zoom"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        private void LoadImportProcedureView(ImportProcedureViewModel<T, U> importProcedureViewModel)
        {
            ActivateItem(importProcedureViewModel);
        }
    }

    /// <summary>
    /// Generic viewmodel for handling generic types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AdditionalShellViewModel<T> : AdditionalShellViewModel
        where T : class, new()
    {
        /// <summary>
        /// Constructor for loading an import procedure view
        /// </summary>
        /// <param name="events"></param>
        /// <param name="importProcedureViewModel"></param>
        public AdditionalShellViewModel(IEventAggregator events, ImportProcedureViewModel<T> importProcedureViewModel) : base(events)
        {
            LoadImportProcedureView(importProcedureViewModel);
        }

        /// <summary>
        /// Constructor for loading an import procedure view
        /// </summary>
        /// <param name="events"></param>
        /// <param name="importProcedureViewModel"></param>
        public AdditionalShellViewModel(IEventAggregator events, T viewModel) : base(events)
        {
            ActivateItem(viewModel);
        }

        /// <summary>
        /// Loading an import procedure view
        /// </summary>
        /// <param name="zoom"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        private void LoadImportProcedureView(ImportProcedureViewModel<T> importProcedureViewModel)
        {
            ActivateItem(importProcedureViewModel);
        }
    }
}
