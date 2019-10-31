using Caliburn.Micro;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace GeoReVi
{
    public class MetamorphicDetailsViewModel : Screen
    {
        #region Private members

        /// <summary>
        /// Selected plug
        /// </summary>
        private tblDrilling selectedDrilling;

        /// <summary>
        /// Plug collection
        /// </summary>
        private BindableCollection<tblDrilling> drillings;
        #endregion

        #region Public properties

        /// <summary>
        /// Selected plug object
        /// </summary>
        public tblDrilling SelectedDrilling
        {
            get { return this.selectedDrilling; }
            set { this.selectedDrilling = value; NotifyOfPropertyChange(() => SelectedDrilling); }
        }

        /// <summary>
        /// Plug collection
        /// </summary>
        public BindableCollection<tblDrilling> Drillings
        {
            get { return this.drillings; }
            set { this.drillings = value; NotifyOfPropertyChange(() => Drillings); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MetamorphicDetailsViewModel()
        {
            LoadData("");
        }

        /// <summary>
        /// Constructor with specific sample name
        /// </summary>
        public MetamorphicDetailsViewModel(string name)
        {
            LoadData(name);
        }
        #endregion

        #region Methods

        //Loading filtered data
        private void LoadData(string name)
        {

            using (var db = new ApirsDatabase())
            {
                try
                {
                    Drillings = new BindableCollection<tblDrilling>(from drill in db.tblDrillings
                                                                  where drill.drillName == name
                                                                  select drill);
                    if (Drillings.Count == 0)
                    {
                        SelectedDrilling = new tblDrilling();
                        SelectedDrilling.drillName = name;
                    }
                    else if (Drillings.Count > 1)
                    {
                        SelectedDrilling = Drillings.First();
                    }
                    else
                    {
                        SelectedDrilling = Drillings.First();
                    }
                }
                catch
                {
                    Drillings = new BindableCollection<tblDrilling>();
                    SelectedDrilling = new tblDrilling();
                }

            }
        }

        // Commit changes from the new object form
        // or edits made to the existing object form.  
        public void Update()
        {
            using (var db = new ApirsDatabase())
            {
                try
                {
                    if (SelectedDrilling.drillIdPk == 0)
                    {
                        try
                        {
                            db.tblDrillings.Add(SelectedDrilling);
                            db.SaveChanges();
                            TryClose();
                        }
                        catch
                        {
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Object can't be added. Please check every field again.");
                            return;
                        }

                    }
                    else
                    {
                        tblDrilling result = db.tblDrillings.Where(drill => drill.drillName == SelectedDrilling.drillName).First();
                        if (result != null)
                        {
                            db.Entry<tblDrilling>(result).CurrentValues.SetValues(SelectedDrilling);
                            db.SaveChanges();
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

