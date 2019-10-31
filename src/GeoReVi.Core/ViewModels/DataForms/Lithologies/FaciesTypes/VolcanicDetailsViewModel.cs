using Caliburn.Micro;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace GeoReVi
{
    public class VolcanicDetailsViewModel : Screen
    {
        #region Private members

        /// <summary>
        /// Selected plug
        /// </summary>
        private tblVolcanicFacy selectedVolcanicFacy;

        /// <summary>
        /// Plug collection
        /// </summary>
        private BindableCollection<tblVolcanicFacy> volcanicFacys;

        #endregion

        #region Public properties

        /// <summary>
        /// Selected plug object
        /// </summary>
        public tblVolcanicFacy SelectedVolcanicFacy
        {
            get { return this.selectedVolcanicFacy; }
            set { this.selectedVolcanicFacy = value; NotifyOfPropertyChange(() => SelectedVolcanicFacy); }
        }

        /// <summary>
        /// Plug collection
        /// </summary>
        public BindableCollection<tblVolcanicFacy> VolcanicFacys
        {
            get { return this.volcanicFacys; }
            set { this.volcanicFacys = value; NotifyOfPropertyChange(() => VolcanicFacys); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public VolcanicDetailsViewModel()
        {
            LoadData(0);
        }

        /// <summary>
        /// Constructor with specific sample name
        /// </summary>
        public VolcanicDetailsViewModel(int id)
        {
            LoadData(id);
        }
        #endregion

        #region Methods

        //Loading filtered data
        private void LoadData(int id)
        {
                try
                {
                    VolcanicFacys = new BindableCollection<tblVolcanicFacy>(new ApirsRepository<tblVolcanicFacy>().GetModelByExpression(lft =>lft.vffacIdFk == id));

                    if (VolcanicFacys.Count == 0)
                    {
                        SelectedVolcanicFacy = new tblVolcanicFacy();
                        SelectedVolcanicFacy.vffacIdFk = id;
                    }
                    else if (VolcanicFacys.Count > 1)
                    {
                        SelectedVolcanicFacy = VolcanicFacys.First();
                    }
                    else
                    {
                        SelectedVolcanicFacy = VolcanicFacys.First();
                    }
                }
                catch (Exception e)
                {
                    VolcanicFacys = new BindableCollection<tblVolcanicFacy>();
                    SelectedVolcanicFacy = new tblVolcanicFacy();
                }
        }

        // Commit changes from the new object form
        // or edits made to the existing object form.  
        public void Update()
        {
            using (var db = new ApirsRepository<tblVolcanicFacy>())
            {
                try
                {
                    if (SelectedVolcanicFacy == null)
                    {
                        try
                        {
                            db.InsertModel(SelectedVolcanicFacy);
                        }
                        catch
                        {
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Object can't be added. Please check every field again.");
                            return;
                        }

                    }
                    else
                    {
                        try
                        {
                            if (new ApirsRepository<tblVolcanicFacy>().GetModelByExpression(x => x.vffacIdFk == SelectedVolcanicFacy.vffacIdFk).Count() > 0)
                                db.UpdateModel(SelectedVolcanicFacy, SelectedVolcanicFacy.vffacIdFk);
                            else
                                db.InsertModel(SelectedVolcanicFacy);
                        }
                        catch
                        {

                                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Object can't be added. Please check every field again.");
                                return;
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
        #endregion
    }
}

