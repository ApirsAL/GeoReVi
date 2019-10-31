using Caliburn.Micro;
using System;
using System.ComponentModel.Composition;
using System.Data.SqlClient;
using System.Linq;

namespace GeoReVi
{
    /// <summary>
    /// Geophysical transect view model
    /// </summary>
    [Export(typeof(TransectDetailsViewModel))]
    public class TransectDetailsViewModel : Screen
    {
        #region Private members

        /// <summary>
        /// Selected plug
        /// </summary>
        private tblTransect selectedTransect;

        /// <summary>
        /// Plug collection
        /// </summary>
        private BindableCollection<tblTransect> transects;
        #endregion

        #region Public properties

        /// <summary>
        /// Selected plug object
        /// </summary>
        public tblTransect SelectedTransect
        {
            get { return this.selectedTransect; }
            set { this.selectedTransect = value; NotifyOfPropertyChange(() => SelectedTransect); }
        }

        /// <summary>
        /// Plug collection
        /// </summary>
        public BindableCollection<tblTransect> Transects
        {
            get { return this.transects; }
            set { this.transects = value; NotifyOfPropertyChange(() => Transects); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TransectDetailsViewModel()
        {
            LoadData("");
        }

        /// <summary>
        /// Constructor with specific sample name
        /// </summary>
        public TransectDetailsViewModel(string name)
        {
            LoadData(name);
        }
        #endregion

        #region Methods

        //Loading filtered data
        private void LoadData(string name)
        {

            using (var db = new ApirsRepository<tblTransect>())
            {
                try
                {
                    Transects = new BindableCollection<tblTransect>(db.GetModelByExpression(tra=>tra.traName == name));
                    if (Transects.Count == 0)
                    {
                        SelectedTransect = new tblTransect();
                        SelectedTransect.traName = name;
                    }
                    else if (Transects.Count > 1)
                    {
                        SelectedTransect = Transects.First();
                    }
                    else
                    {
                        SelectedTransect = Transects.First();
                    }
                }
                catch
                {
                    Transects = new BindableCollection<tblTransect>();
                    SelectedTransect = new tblTransect() { traName = name };
                }

            }
        }

        // Commit changes from the new object form
        // or edits made to the existing object form.  
        public void Update()
        {
            using (var db = new ApirsRepository<tblTransect>())
            {
                try
                {
                    if (SelectedTransect.traIdPk == 0)
                    {
                        try
                        {
                            db.InsertModel(SelectedTransect);
                        }
                        catch(Exception e)
                        {
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Object can't be added. Please check every field again.");
                            return;
                        }
                    }
                    else
                    {
                        tblTransect result = db.GetModelById(SelectedTransect.traIdPk);
                        if (result != null)
                        {
                            db.UpdateModel(SelectedTransect, SelectedTransect.traIdPk);
                        }
                    }

                }
                catch (SqlException ex)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please provide valid input parameters");
                }
                catch (Exception e)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Something went wrong");
                }
                finally
                {
                }

            }
        }
    }

    #endregion
}

