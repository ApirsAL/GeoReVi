using Caliburn.Micro;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace GeoReVi
{
    public class SiliciclasticDetailsViewModel : Screen
    {
        #region Private members

        /// <summary>
        /// Selected plug
        /// </summary>
        private tblLithofacy selectedLithofacy;

        /// <summary>
        /// Plug collection
        /// </summary>
        private BindableCollection<tblLithofacy> lithofacys;

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
        public tblLithofacy SelectedLithofacy
        {
            get { return this.selectedLithofacy; }
            set { this.selectedLithofacy = value; NotifyOfPropertyChange(() => SelectedLithofacy); }
        }

        /// <summary>
        /// Plug collection
        /// </summary>
        public BindableCollection<tblLithofacy> Lithofacys
        {
            get { return this.lithofacys; }
            set { this.lithofacys = value; NotifyOfPropertyChange(() => Lithofacys); }
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
        public SiliciclasticDetailsViewModel()
        {
            LoadData(0);
        }

        /// <summary>
        /// Constructor with specific sample name
        /// </summary>
        public SiliciclasticDetailsViewModel(int id)
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
                Lithofacys = new BindableCollection<tblLithofacy>(new ApirsRepository<tblLithofacy>().GetModelByExpression(lft => lft.facIdFk == id));

                if (Lithofacys.Count == 0)
                {
                    SelectedLithofacy = new tblLithofacy();
                    SelectedLithofacy.facIdFk = id;
                }
                else if (Lithofacys.Count > 1)
                {
                    SelectedLithofacy = Lithofacys.First();
                }
                else
                {
                    SelectedLithofacy = Lithofacys.First();
                }
            }
            catch (Exception e)
            {
                Lithofacys = new BindableCollection<tblLithofacy>();
                SelectedLithofacy = new tblLithofacy();
            }

            try
            {
                PrimarySedimentaryStructures = new BindableCollection<tblSedimentaryStructure>(new ApirsRepository<tblSedimentaryStructure>().GetModelByExpression(sed => sed.sedRockType.Contains("Siliciclastic") && sed.sedPrimary == true));
                SecondarySedimentaryStructures = new BindableCollection<tblSedimentaryStructure>(new ApirsRepository<tblSedimentaryStructure>().GetModelByExpression(sed => sed.sedRockType.Contains("Siliciclastic") && sed.sedPrimary == false));
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
            using (var db = new ApirsRepository<tblLithofacy>())
            {
                try
                {
                    if (SelectedLithofacy.facIdFk == 0)
                    {
                        try
                        {
                            db.InsertModel(SelectedLithofacy);
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
                            if (new ApirsRepository<tblLithofacy>().GetModelByExpression(x => x.facIdFk == SelectedLithofacy.facIdFk).Count() > 0)
                                db.UpdateModel(SelectedLithofacy, SelectedLithofacy.facIdFk);
                            else
                                db.InsertModel(SelectedLithofacy);
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
    }

    #endregion
}

