using Caliburn.Micro;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace GeoReVi
{
    public class PersonsViewModel : Screen
    {
        #region Private members

        //EventAggregator
        public readonly IEventAggregator _events;

        /// <summary>
        /// Selected Person
        /// </summary>
        private tblPerson selectedPerson;

        /// <summary>
        /// Person collection
        /// </summary>
        private BindableCollection<tblPerson> persons;

        /// <summary>
        /// Filtered person collection
        /// </summary>
        private BindableCollection<tblPerson> filteredPersons;

        /// <summary>
        /// The selected project
        /// </summary>
        private tblProject project;

        /// <summary>
        /// Text for filtering the results
        /// </summary>
        private string textFilter;

        /// <summary>
        /// Cancellation token source for cancelling image downloads
        /// </summary>
        private CancellationTokenSource cts;

        #endregion

        #region Public properties

        /// <summary>
        /// Event handler if selected object or its index changes
        /// </summary>
        public event EventHandler FilterTextChanged;

        /// <summary>
        /// Selected Person object
        /// </summary>
        public tblPerson SelectedPerson
        {
            get { return this.selectedPerson; }
            set
            {
                this.selectedPerson = value;
                NotifyOfPropertyChange(() => SelectedPerson);
            }
        }

        /// <summary>
        /// Person collection
        /// </summary>
        public BindableCollection<tblPerson> Persons
        {
            get { return this.persons; }
            set
            {
                this.persons = value;
                NotifyOfPropertyChange(() => Persons);
            }
        }

        /// <summary>
        /// Filtered person collection
        /// </summary>
        public BindableCollection<tblPerson> FilteredPersons
        {
            get { return this.filteredPersons; }
            set
            {
                this.filteredPersons = value;
                NotifyOfPropertyChange(() => FilteredPersons);
            }
        }
        /// <summary>
        /// Text to filter the persons
        /// </summary>
        public string TextFilter
        {
            get
            {
                return this.textFilter;
            }
            set
            {
                this.textFilter = value;
                OnFilterTextChanged();
                NotifyOfPropertyChange(() => TextFilter);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public PersonsViewModel(int projectId)
        {
            this._events = ((ShellViewModel)IoC.Get<IShell>())._events;
            LoadData(projectId);

        }

        #endregion

        #region Methods

        //Loading filtered data
        private void LoadData(int projectId)
        {
            try
            {
                //All persons participating in the actual project
                var query = new ApirsRepository<tblPerson>().GetPersonByProject(projectId);
                //(from p in db.tblPersons
                //             join persprj in db.v_PersonsProject on p.persIdPk equals persprj.persIdFk
                //             where persprj.prjIdFk == projectId
                //             select p);

                //All persons registered
                var persons = new ApirsRepository<tblPerson>().GetModel();

                //Filtering all persons out who already participate
                foreach (var pers in query)
                {
                    persons = persons.Where(p => p.persIdPk != pers.persIdPk);
                }

                //Assign the rest of the people to the variable
                Persons = new BindableCollection<tblPerson>(persons.ToList());
                FilteredPersons = new BindableCollection<tblPerson>(Persons);
            }
            catch (Exception e)
            {
                Persons = new BindableCollection<tblPerson>();
                FilteredPersons = new BindableCollection<tblPerson>();
            }
        }

        /// <summary>
        /// Dynamically filtering the Persons
        /// </summary>
        /// <param name="value"></param>
        private void OnFilterTextChanged()
        {
            if (TextFilter == "")
            {
                FilteredPersons = new BindableCollection<tblPerson>((Persons).ToList());
            }

            if (FilteredPersons.Count == 0)
                return;

            BackgroundWorker bw = new BackgroundWorker();

            bw.DoWork += ((sender1, args) =>
            {

                new DispatchService().Invoke(
                () =>
                {
                    if (cts != null)
                    {
                        cts.Cancel();
                        cts = null;
                    }
                    cts = new CancellationTokenSource();

                    try
                    {
                        FilteredPersons = new BindableCollection<tblPerson>(CollectionHelper.Filter<tblPerson>(Persons, TextFilter));
                    }
                    catch (Exception e)
                    {
                        _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
                    }
                });
            });

            bw.RunWorkerCompleted += ((sender1, args) =>
            {
                if (args.Error != null)  // if an exception occurred during DoWork,
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(args.Error.ToString());

            });

            bw.RunWorkerAsync(); // start the background worker
        }

        /// <summary>
        /// Selecting a person based on the id
        /// </summary>
        /// <param name="id"></param>
        public void SelectPerson(int id)
        {
            try
            {
                SelectedPerson = (from p in FilteredPersons
                                  where p.persIdPk == id
                                  select p).First();
                this.TryClose();
            }
            catch (Exception e)
            {
                SelectedPerson = new tblPerson();
                this.TryClose();
            }
        }
        #endregion
    }
}
