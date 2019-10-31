using Caliburn.Micro;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace GeoReVi
{
    public class BiochemicalDetailsViewModel : Screen
    {
        #region Private members

        /// <summary>
        /// Selected plug
        /// </summary>
        private tblBiochemicalFacy selectedBiochemicalFacy;

        /// <summary>
        /// Plug collection
        /// </summary>
        private BindableCollection<tblBiochemicalFacy> biochemicalFacys;

        /// <summary>
        /// Sedimentary structures collection
        /// </summary>
        private BindableCollection<tblSedimentaryStructure> primarySedimentaryStructures;

        /// <summary>
        /// Sedimentary structures collection
        /// </summary>
        private BindableCollection<tblSedimentaryStructure> secondarySedimentaryStructures;
        #endregion

        #region Public properties

        /// <summary>
        /// Selected plug object
        /// </summary>
        public tblBiochemicalFacy SelectedBiochemicalFacy
        {
            get { return this.selectedBiochemicalFacy; }
            set { this.selectedBiochemicalFacy = value; NotifyOfPropertyChange(() => SelectedBiochemicalFacy); }
        }

        /// <summary>
        /// Plug collection
        /// </summary>
        public BindableCollection<tblBiochemicalFacy> BiochemicalFacys
        {
            get { return this.biochemicalFacys; }
            set { this.biochemicalFacys = value; NotifyOfPropertyChange(() => BiochemicalFacys); }
        }

        /// <summary>
        /// Collection of primary sedimentary structures
        /// </summary>
        public BindableCollection<tblSedimentaryStructure> PrimarySedimentaryStructures
        {
            get
            {
                return this.primarySedimentaryStructures;
            }
            set
            {
                this.primarySedimentaryStructures = value;
                NotifyOfPropertyChange(() => PrimarySedimentaryStructures);
            }
        }

        /// <summary>
        /// Collection of primary sedimentary structures
        /// </summary>
        public BindableCollection<tblSedimentaryStructure> SecondarySedimentaryStructures
        {
            get
            {
                return this.secondarySedimentaryStructures;
            }
            set
            {
                this.secondarySedimentaryStructures = value;
                NotifyOfPropertyChange(() => SecondarySedimentaryStructures);
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BiochemicalDetailsViewModel()
        {
            LoadData(0);
        }

        /// <summary>
        /// Constructor with specific sample name
        /// </summary>
        public BiochemicalDetailsViewModel(int id)
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
                BiochemicalFacys = new BindableCollection<tblBiochemicalFacy>(new ApirsRepository<tblBiochemicalFacy>().GetModelByExpression(lft => lft.bffacIdFk == id));

                if (BiochemicalFacys.Count == 0)
                {
                    SelectedBiochemicalFacy = new tblBiochemicalFacy();
                    SelectedBiochemicalFacy.bffacIdFk = id;
                }
                else if (BiochemicalFacys.Count > 1)
                {
                    SelectedBiochemicalFacy = BiochemicalFacys.First();
                }
                else
                {
                    SelectedBiochemicalFacy = BiochemicalFacys.First();
                }
            }
            catch (Exception e)
            {
                BiochemicalFacys = new BindableCollection<tblBiochemicalFacy>();
                SelectedBiochemicalFacy = new tblBiochemicalFacy();
            }

            try
            {
                PrimarySedimentaryStructures = new BindableCollection<tblSedimentaryStructure>(new ApirsRepository<tblSedimentaryStructure>().GetModelByExpression(sed => sed.sedRockType.Contains("Biochemical") && sed.sedPrimary == true));

                SecondarySedimentaryStructures = new BindableCollection<tblSedimentaryStructure>(new ApirsRepository<tblSedimentaryStructure>().GetModelByExpression(sed => sed.sedRockType.Contains("Biochemical") && sed.sedPrimary == false));
            }
            catch
            {
                PrimarySedimentaryStructures = new BindableCollection<tblSedimentaryStructure>();
                SecondarySedimentaryStructures = new BindableCollection<tblSedimentaryStructure>();
            }
        }

        // Commit changes from the new object form
        // or edits made to the existing object form.  
        public void Update()
        {
            using (var db = new ApirsRepository<tblBiochemicalFacy>())
            {
                try
                {
                    if (SelectedBiochemicalFacy == null)
                    {
                        try
                        {
                            db.InsertModel(SelectedBiochemicalFacy);
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
                            if (new ApirsRepository<tblBiochemicalFacy>().GetModelByExpression(x => x.bffacIdFk == SelectedBiochemicalFacy.bffacIdFk).Count() > 0)
                                db.UpdateModel(SelectedBiochemicalFacy, SelectedBiochemicalFacy.bffacIdFk);
                            else
                                db.InsertModel(SelectedBiochemicalFacy);
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

