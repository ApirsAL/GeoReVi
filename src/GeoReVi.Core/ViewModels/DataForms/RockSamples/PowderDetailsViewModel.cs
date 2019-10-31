using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Data.SqlClient;

namespace GeoReVi
{
    public class PowderDetailsViewModel : Screen
    {
        #region Private members

        /// <summary>
        /// Selected Powder
        /// </summary>
        private tblPowder selectedPowder;

        /// <summary>
        /// Powder collection
        /// </summary>
        private BindableCollection<tblPowder> Powders;
        #endregion

        #region Public properties

        /// <summary>
        /// Selected Powder object
        /// </summary>
        public tblPowder SelectedPowder
        {
            get { return this.selectedPowder; }
            set { this.selectedPowder = value; NotifyOfPropertyChange(() => SelectedPowder); }
        }

        /// <summary>
        /// Powder collection
        /// </summary>
        public BindableCollection<tblPowder> powders
        {
            get { return this.powders; }
            set { this.powders = value; NotifyOfPropertyChange(() => Powders); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public PowderDetailsViewModel()
        {
            LoadData("");
        }

        /// <summary>
        /// Constructor with specific sample name
        /// </summary>
        public PowderDetailsViewModel(string name)
        {
            LoadData(name);
        }
        #endregion

        #region Methods

        //Loading filtered data
        private void LoadData(string name)
        {

            using (var db = new ApirsRepository<tblPowder>())
            {
                try
                {
                    Powders = new BindableCollection<tblPowder>(db.GetModelByExpression(pow => pow.powFromSampleName == name));
                    if (Powders.Count == 0)
                    {
                        SelectedPowder = new tblPowder();
                        SelectedPowder.powFromSampleName = name;
                    }
                    else if (Powders.Count > 1)
                    {
                        SelectedPowder = Powders.First();
                    }
                    else
                    { SelectedPowder = Powders.First(); }
                }
                catch
                {
                    Powders = new BindableCollection<tblPowder>();
                    SelectedPowder = new tblPowder();
                }

            }
        }

        public void Update()
        {
            using (var db = new ApirsRepository<tblPowder>())
            {
                try
                {
                    if (SelectedPowder.powIdPk == 0)
                    {
                        try
                        {
                            db.InsertModel(SelectedPowder);
                            db.Save();
                            TryClose();
                        }
                        catch
                        {
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Powder can't be added. Please check every field again.");
                            return;
                        }

                    }
                    else
                    {
                        tblPowder result = db.GetModelById(SelectedPowder.powIdPk);
                        if (result != null)
                        {
                            db.UpdateModel(SelectedPowder, SelectedPowder.powIdPk);
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
        #endregion

    }

}
