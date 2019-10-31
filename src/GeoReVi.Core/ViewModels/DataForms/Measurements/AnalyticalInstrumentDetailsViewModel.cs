using Caliburn.Micro;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// View model for analytical instrument details
    /// </summary>
    [Export(typeof(AnalyticalInstrumentDetailsViewModel))]
    public class AnalyticalInstrumentDetailsViewModel : Screen
    {
        #region Members
        //Event aggregator to communicate
        IEventAggregator _events;

        //Instruments collection
        private BindableCollection<tblMeasuringDevice> analyticalInstruments;
        //All instruments collection
        private BindableCollection<tblMeasuringDevice> allAnalyticalInstruments;

        //Selected instrument
        private tblMeasuringDevice selectedAnalyticalInstrument;

        //Text filter variable
        private string textFilter;

        /// <summary>
        /// Cancellation token source for cancelling File downloads
        /// </summary>
        private CancellationTokenSource cts;

        #endregion

        #region Public properties

        public BindableCollection<tblMeasuringDevice> AnalyticalInstruments
        {
            get
            {
                return this.analyticalInstruments;
            }
            set
            {
                this.analyticalInstruments = value;
                NotifyOfPropertyChange(() => AnalyticalInstruments);
            }
        }

        //Selected instrument property
        public tblMeasuringDevice SelectedAnalyticalInstrument
        {
            get
            {
                return this.selectedAnalyticalInstrument;
            }
            set
            {
                this.selectedAnalyticalInstrument = value;
                NotifyOfPropertyChange(() => SelectedAnalyticalInstrument);
            }
        }


        /// <summary>
        /// Text filter variable
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
                OnTextFilterChanged();
                NotifyOfPropertyChange(() => this.textFilter);
            }
        }

        #endregion

        #region Constructor

        public AnalyticalInstrumentDetailsViewModel(IEventAggregator events)
        {
            _events = events;
            LoadData();
        }

        #endregion

        #region Methods

        //Loading filtered data
        private async void LoadData()
        {
            if (!ServerInteractionHelper.IsNetworkAvailable() && !ServerInteractionHelper.TryAccessDatabase())
            {
                AnalyticalInstruments = new BindableCollection<tblMeasuringDevice>();
                SelectedAnalyticalInstrument = new tblMeasuringDevice();
                return;
            }

            try
            {
                await Task.Run(() =>
                {
                    using (var db = new ApirsRepository<tblMeasuringDevice>())
                    {
                        AnalyticalInstruments = new BindableCollection<tblMeasuringDevice>(db.GetModel().ToList());
                    }
                });

                this.allAnalyticalInstruments = AnalyticalInstruments;

                if (AnalyticalInstruments.Count == 0)
                {
                    SelectedAnalyticalInstrument = new tblMeasuringDevice();
                }
                else if (AnalyticalInstruments.Count > 1)
                {
                    SelectedAnalyticalInstrument = AnalyticalInstruments.First();
                }
                else
                { SelectedAnalyticalInstrument = AnalyticalInstruments.First(); }
            }
            catch (Exception e)
            {
                AnalyticalInstruments = new BindableCollection<tblMeasuringDevice>();
                this.allAnalyticalInstruments = AnalyticalInstruments;
                SelectedAnalyticalInstrument = new tblMeasuringDevice();
            }
        }

        // Commit changes from the new rock sample form
        // or edits made to the existing rock sample form.  
        public void Update()
        {
            if (!(bool)((ShellViewModel)IoC.Get<IShell>()).LocalMode)
                if (!DataValidation.CheckPrerequisites(CRUD.Update, SelectedAnalyticalInstrument, (int)SelectedAnalyticalInstrument.mdUploaderId))
                    return;

            try
            {
                using (var db = new ApirsRepository<tblMeasuringDevice>())
                {
                    if (SelectedAnalyticalInstrument.mdIdPk == 0)
                        db.InsertModel(SelectedAnalyticalInstrument);
                    else
                        db.UpdateModel(SelectedAnalyticalInstrument, SelectedAnalyticalInstrument.mdIdPk);
                }

                Refresh();
            }
            catch (SqlException ex)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please provide valid input parameters");
            }
            catch (Exception e)
            {
                if (e.Message.Contains("EntityValidation"))
                    ((ShellViewModel)IoC.Get<IShell>()).ShowError("Please provide a name for the instrument.");
                else
                    _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
            }
            finally
            {
            }
        }


        /// <summary>
        /// Refreshing the dataset
        /// </summary>
        public override void Refresh()
        {
            if (!DataValidation.CheckPrerequisites(CRUD.Load))
                return;

            tblMeasuringDevice current = SelectedAnalyticalInstrument;
            int id = 0;

            try
            {
                if (SelectedAnalyticalInstrument != null)
                    id = SelectedAnalyticalInstrument.mdIdPk;

                LoadData();
                SelectedAnalyticalInstrument = AnalyticalInstruments.Where(p => p.mdIdPk == id).First();
            }
            catch
            {
                try
                {
                    LoadData();
                    SelectedAnalyticalInstrument = new tblMeasuringDevice();
                }
                catch
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpeced error occured.");
                }
            }
        }


        // Sets up the form so that user can enter data. Data is later  
        // saved when user clicks Commit.  
        public void Add()
        {
            if (!DataValidation.CheckPrerequisites(CRUD.Add))
                return;

            SelectedAnalyticalInstrument = new tblMeasuringDevice();
            SelectedAnalyticalInstrument.mdUploaderId = (int)((ShellViewModel)IoC.Get<IShell>()).UserId;

        }

        /// <summary>
        /// Deleting the currently viewed rock sample
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Delete()
        {
            if (!DataValidation.CheckPrerequisites(CRUD.Delete, SelectedAnalyticalInstrument, (int)SelectedAnalyticalInstrument.mdUploaderId))
                return;

            // If existing window is visible, delete the customer and all their orders.  
            // In a real application, you should add warnings and allow the user to cancel the operation.  

            if (((ShellViewModel)IoC.Get<IShell>()).ShowQuestion("Are you sure to delete the record?") == MessageBoxViewResult.No)
            {
                return;
            }

            using (var db = new ApirsRepository<tblMeasuringDevice>())
            {
                try
                {
                    if ((bool)((ShellViewModel)IoC.Get<IShell>()).LocalMode)
                        db.DeleteModelById(SelectedAnalyticalInstrument.mdIdPk);
                    else
                    {
                        db.DeleteModelById(SelectedAnalyticalInstrument.mdIdPk);
                        db.Save();
                    }

                    Refresh();

                }
                catch (Exception ex)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpeced error occured.");
                }
                finally
                {
                }

            }
        }

        ///Event that gets fired if the filter text was changed
        private void OnTextFilterChanged()
        {
            if (TextFilter == "")
            {
                AnalyticalInstruments = this.allAnalyticalInstruments;
            }

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

                    //Filtering data based on the selection
                    try
                    {
                        //Filtering
                        AnalyticalInstruments = new BindableCollection<tblMeasuringDevice>(CollectionHelper.Filter<tblMeasuringDevice>(allAnalyticalInstruments, TextFilter).ToList());
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
                    ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpeced error occured.");

            });

            bw.RunWorkerAsync(); // start the background worker

        }

        /// <summary>
        /// Go to the last dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Last()
        {
            if (AnalyticalInstruments.Count != 0)
                SelectedAnalyticalInstrument = AnalyticalInstruments.Last();
        }


        /// <summary>
        /// Go to the previous dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Previous()
        {
            if (AnalyticalInstruments.Count != 0)
                SelectedAnalyticalInstrument = Navigation.GetPrevious(AnalyticalInstruments, SelectedAnalyticalInstrument);
        }

        /// <summary>
        /// Go to the next dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Next()
        {

            if (AnalyticalInstruments.Count != 0)
                SelectedAnalyticalInstrument = Navigation.GetNext(AnalyticalInstruments, SelectedAnalyticalInstrument);
        }

        /// <summary>
        /// Go to the first dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void First()
        {
            if (AnalyticalInstruments.Count != 0)
                SelectedAnalyticalInstrument = AnalyticalInstruments.First();
        }


        /// <summary>
        /// Checking the uniqueness of the name in the database
        /// </summary>
        public void CheckUniqueness()
        {
            int count;
            if (SelectedAnalyticalInstrument.mdIdPk > 0)
                return;

            try
            {
                using (var db = new ApirsRepository<tblMeasuringDevice>())
                {
                    count = db.GetModelByExpression(rs => rs.mdName == SelectedAnalyticalInstrument.mdName).Count();

                    if (count == 0)
                        return;

                    char a = 'A';

                    while (count > 0)
                    {
                        SelectedAnalyticalInstrument.mdName = SelectedAnalyticalInstrument.mdName + a.ToString();

                        count = db.GetModelByExpression(rs => rs.mdName == SelectedAnalyticalInstrument.mdName).Count();
                        a++;
                    }

                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("The name was already in use. We provided a valid alternative for it:" + Environment.NewLine
                        + SelectedAnalyticalInstrument.mdName);

                    NotifyOfPropertyChange(() => SelectedAnalyticalInstrument);
                }
            }
            catch (Exception e)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpeced error occured.");
            }
        }
        #endregion
    }
}
