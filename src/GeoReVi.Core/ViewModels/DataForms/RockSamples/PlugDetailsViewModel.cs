using Caliburn.Micro;
using System;
using System.Linq;
using System.Linq;
using System.Windows;
using System.Data.SqlClient;

namespace GeoReVi
{
    public class PlugDetailsViewModel : Screen
    {
        #region Private members

        /// <summary>
        /// Selected plug
        /// </summary>
        private tblPlug selectedPlug;

        /// <summary>
        /// Plug collection
        /// </summary>
        private BindableCollection<tblPlug> plugs;
        #endregion

        #region Public properties

        /// <summary>
        /// Selected plug object
        /// </summary>
        public tblPlug SelectedPlug
        {
            get { return this.selectedPlug; }
            set { this.selectedPlug = value; NotifyOfPropertyChange(() => SelectedPlug); }
        }

        /// <summary>
        /// Plug collection
        /// </summary>
        public BindableCollection<tblPlug> Plugs
        {
            get { return this.plugs; }
            set { this.plugs = value; NotifyOfPropertyChange(() => Plugs); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public PlugDetailsViewModel()
        {
            LoadData("");
        }

        /// <summary>
        /// Constructor with specific sample name
        /// </summary>
        public PlugDetailsViewModel(string name)
        {
            LoadData(name);
        }
        #endregion

        #region Methods

        //Loading filtered data
        private void LoadData(string name)
        {

            using (var db = new ApirsRepository<tblPlug>())
            {
                try
                {
                    Plugs = new BindableCollection<tblPlug>(db.GetModelByExpression(plug => plug.plugLabel == name));
                    if (Plugs.Count == 0)
                    {
                        SelectedPlug = new tblPlug();
                        SelectedPlug.plugLabel = name;
                    }
                    else if (Plugs.Count > 1)
                    {
                        SelectedPlug = Plugs.First();
                    }
                    else
                    { SelectedPlug = Plugs.First(); }
                }
                catch
                {
                    Plugs = new BindableCollection<tblPlug>();
                    SelectedPlug = new tblPlug();
                }

            }
        }

        public void Update()
        {
            using (var db = new ApirsRepository<tblPlug>())
            {
                try
                {
                    if (SelectedPlug.plugIdPk == 0)
                    {
                        try
                        {
                            db.InsertModel(SelectedPlug);
                            db.Save();
                            TryClose();
                        }
                        catch
                        {
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Plug can't be added. Please check every field again.");
                            return;
                        }

                    }
                    else
                    {
                        tblPlug result = db.GetModelById(SelectedPlug.plugIdPk);
                        if (result != null)
                        {
                            db.UpdateModel(SelectedPlug, SelectedPlug.plugIdPk);
                            db.Save();
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

