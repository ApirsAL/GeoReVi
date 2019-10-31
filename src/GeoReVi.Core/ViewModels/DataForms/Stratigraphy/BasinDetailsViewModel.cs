using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GeoReVi
{
    public class BasinDetailsViewModel : Screen
    {
        #region Members
        //Event aggregator to communicate
        IEventAggregator _events;

        //Instruments collection
        private BindableCollection<tblBasin> basins;
        //All instruments collection
        private BindableCollection<tblBasin> allBasins;

        //Selected instrument
        private tblBasin selectedBasin;

        //Object lithostrat collection
        private BindableCollection<tblBasinLithoUnit> basLithostrat;

        //Lithostrat collection
        private BindableCollection<LithostratigraphyUnion> lithostratigraphy;

        //Text filter variable
        private string textFilter;

        /// <summary>
        /// Cancellation token source for cancelling File downloads
        /// </summary>
        private CancellationTokenSource cts;

        #endregion

        #region Public properties

        public BindableCollection<tblBasin> Basins
        {
            get
            {
                return this.basins;
            }
            set
            {
                this.basins = value;
                NotifyOfPropertyChange(() => Basins);
            }
        }

        //Selected instrument property
        public tblBasin SelectedBasin
        {
            get
            {
                return this.selectedBasin;
            }
            set
            {
                this.selectedBasin = value;

                if (value != null)
                    OnSelectedBasinChanged();
                NotifyOfPropertyChange(() => SelectedBasin);
            }
        }

        /// <summary>
        /// Rock sample collection for the form
        /// </summary>
        public BindableCollection<tblBasinLithoUnit> BasLithostrat
        {
            get { return this.basLithostrat; }
            set
            {
                this.basLithostrat = value;
                NotifyOfPropertyChange(() => BasLithostrat);
            }
        }

        /// <summary>
        /// Collection of all lithostratigraphic units
        /// </summary>
        public BindableCollection<LithostratigraphyUnion> Lithostratigraphy
        {
            get { return this.lithostratigraphy; }
            set { this.lithostratigraphy = value; NotifyOfPropertyChange(() => Lithostratigraphy); }
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

        public BasinDetailsViewModel(IEventAggregator events)
        {
            _events = events;
            LoadData();
        }

        #endregion

        #region Methods

        //Loading filtered data
        private void LoadData()
        {
            try
            {
                using (var db = new ApirsRepository<tblBasin>())
                {
                    try
                    {
                        Basins = new BindableCollection<tblBasin>(db.GetModel().ToList());
                        this.allBasins = Basins;

                        if (Basins.Count == 0)
                        {
                            SelectedBasin = new tblBasin() { basUserIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
                        }
                        else if (Basins.Count > 1)
                        {
                            SelectedBasin = Basins.First();
                        }
                        else
                        { SelectedBasin = Basins.First(); }
                    }
                    catch
                    {
                        Basins = new BindableCollection<tblBasin>();
                        this.allBasins = Basins;
                        SelectedBasin = new tblBasin() { basUserIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
                    }
                }
                using (var db = new ApirsRepository<LithostratigraphyUnion>())
                {
                    try
                    {
                        Lithostratigraphy = new BindableCollection<LithostratigraphyUnion>(db.GetCompleteLithostratigraphy().ToList());
                    }
                    catch
                    {
                        Lithostratigraphy = new BindableCollection<LithostratigraphyUnion>();
                    }
                }
            }
            catch
            {
                Basins = new BindableCollection<tblBasin>();
                SelectedBasin = new tblBasin() { basUserIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
            }
        }

        // Commit changes from the new rock sample form
        // or edits made to the existing rock sample form.  
        public void Update()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Update, SelectedBasin, (int)SelectedBasin.basUserIdFk))
                {
                    return;
                }
            }
            catch(Exception e)
            {
                return;
            }

            using (var db = new ApirsRepository<tblBasin>())
            {
                try
                {
                    if (SelectedBasin.basIdPk == 0)
                    {
                        SelectedBasin.basUserIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId;
                        db.InsertModel(SelectedBasin);

                        Basins.Add(SelectedBasin);
                    }
                    else
                    {
                        db.UpdateModel(SelectedBasin, SelectedBasin.basIdPk);
                        db.Save();

                        using (var db1 = new ApirsRepository<tblBasinLithoUnit>())
                        {
                            foreach (tblBasinLithoUnit litunit in BasLithostrat)
                            {
                                if (litunit.baslitIdPk == 0)
                                {
                                    db1.InsertModel(litunit);
                                }
                                else
                                {
                                    db1.UpdateModel(litunit, litunit.baslitIdPk);
                                }

                            }
                        }
                    }

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
                        ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpected error occured");
                }
                finally
                {
                }

            }
        }

        /// <summary>
        /// Refreshing the dataset
        /// </summary>
        public override void Refresh()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Add))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            tblBasin current = SelectedBasin;
            int id = 0;

            try
            {
                if (SelectedBasin != null)
                    id = SelectedBasin.basIdPk;

                LoadData();
                SelectedBasin = Basins.Where(p => p.basIdPk == id).First();
            }
            catch
            {
                try
                {
                    LoadData();
                    SelectedBasin = new tblBasin() { basUserIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
                }
                catch
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpected error occurred.");
                }
            }
        }


        // Sets up the form so that user can enter data. Data is later  
        // saved when user clicks Commit.  
        public void Add()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Add))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            SelectedBasin = new tblBasin() { basUserIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };

        }

        /// <summary>
        /// Deleting the currently viewed rock sample
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Delete()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Delete, SelectedBasin, (int)SelectedBasin.basUserIdFk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            // If existing window is visible, delete the customer and all their orders.  
            // In a real application, you should add warnings and allow the user to cancel the operation.  

            if (((ShellViewModel)IoC.Get<IShell>()).ShowQuestion("Are you sure to delete the record?") == MessageBoxViewResult.No)
            {
                return;
            }

            using (var db = new ApirsRepository<tblBasin>())
            {
                try
                {
                    db.DeleteModelById(SelectedBasin.basIdPk);

                    Basins.Remove(SelectedBasin);
                    allBasins.Remove(SelectedBasin);

                }
                catch (Exception ex)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpected error occurred.");
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
                Basins = this.allBasins;
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
                        //Filtering outcrops
                        Basins = new BindableCollection<tblBasin>(CollectionHelper.Filter<tblBasin>(allBasins, TextFilter));
                    }
                    catch (Exception e)
                    {
                        ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpeced error occured.");
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
            if (Basins.Count != 0)
                SelectedBasin = Basins.Last();
        }


        /// <summary>
        /// Go to the previous dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Previous()
        {
            if (Basins.Count != 0)
                SelectedBasin = Navigation.GetPrevious(Basins, SelectedBasin);
        }

        /// <summary>
        /// Go to the next dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Next()
        {

            if (Basins.Count != 0)
                SelectedBasin = Navigation.GetNext(Basins, SelectedBasin);
        }

        /// <summary>
        /// Go to the first dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void First()
        {
            if (Basins.Count != 0)
                SelectedBasin = Basins.First();
        }


        /// <summary>
        /// Checking the uniqueness of the name in the database
        /// </summary>
        public void CheckUniqueness()
        {
            int count;

            try
            {
                using (var db = new ApirsRepository<tblBasin>())
                {
                    count = db.GetModelByExpression(rs => rs.basName == SelectedBasin.basName
                             && rs.basIdPk != SelectedBasin.basIdPk).Count();

                    if (count == 0)
                        return;

                    char a = 'A';

                    while (count > 0)
                    {
                        SelectedBasin.basName = SelectedBasin.basName + a.ToString();

                        count = db.GetModelByExpression(rs => rs.basName == SelectedBasin.basName).Count();
                        a++;
                    }

                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("The name was already in use. We provided a valid alternative for it:" + Environment.NewLine
                        + SelectedBasin.basName);
                }
            }
            catch (Exception e)
            {
                _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok));;
            }
        }

        //Activating a background worker to select and download Files asynchronously
        private void OnSelectedBasinChanged()
        {
            using (var db = new ApirsRepository<tblBasinLithoUnit>())
            {
                try
                {
                    BasLithostrat = new BindableCollection<tblBasinLithoUnit>((db.GetModelByExpression(x => x.basIdFk == SelectedBasin.basIdPk).OrderBy(x => x.baslitIdPk).ToList()));
                }
                catch
                {
                    BasLithostrat = new BindableCollection<tblBasinLithoUnit>();
                }
            }
        }

        //Adding a default lithostratigraphic unit
        public void AddLithostratigraphicUnit()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedBasin, (int)SelectedBasin.basUserIdFk, SelectedBasin.basIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            //Only accessible if current user uploaded the object
            try
            {
                if (SelectedBasin == null || SelectedBasin.basUserIdFk != (int)((ShellViewModel)IoC.Get<IShell>()).UserId)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Only the uploader can make changes to the object. Please contact him via message service.");
                    return;
                }
            }
            catch
            {
                return;
            }

            try
            {
                using (var db = new ApirsRepository<tblBasinLithoUnit>())
                {
                    db.InsertModel(new tblBasinLithoUnit() { basIdFk = SelectedBasin.basIdPk, lithID = 55 });
                    db.Save();
                    BasLithostrat = new BindableCollection<tblBasinLithoUnit>((db.GetModelByExpression(x => x.basIdFk == SelectedBasin.basIdPk).OrderBy(x => x.baslitIdPk).ToList()));
                }
            }
            catch
            {
                _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok));;
            }
            finally
            {
                Refresh();
            }
        }

        /// <summary>
        /// Removes a certain lithounit of ID == id
        /// </summary>
        /// <param name="id"></param>
        public void RemoveLithostratigraphicUnit(int id)
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedBasin, (int)SelectedBasin.basUserIdFk, SelectedBasin.basIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            //Only accessible if current user uploaded the object
            try
            {
                if (SelectedBasin == null || SelectedBasin.basUserIdFk != (int)((ShellViewModel)IoC.Get<IShell>()).UserId)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Only the uploader can make changes to the object. Please contact him via message service.");
                    return;
                }
            }
            catch
            {
                return;
            }

            try
            {
                using (var db = new ApirsRepository<tblBasinLithoUnit>())
                {
                    tblBasinLithoUnit result = db.GetModelById(id);
                    db.DeleteModelById(result.baslitIdPk);
                    db.Save();
                    BasLithostrat = new BindableCollection<tblBasinLithoUnit>((db.GetModelByExpression(x => x.basIdFk == SelectedBasin.basIdPk).OrderBy(x => x.baslitIdPk).ToList()));
                }
            }
            catch (Exception e)
            {
                _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok));;
            }
            finally
            {
                Refresh();
            }
        }

        #endregion
    }
}
