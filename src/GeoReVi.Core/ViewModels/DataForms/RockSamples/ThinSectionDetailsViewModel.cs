
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Data.SqlClient;

namespace GeoReVi
{
    public class ThinSectionDetailsViewModel : Screen
    {
        #region Private members

        /// <summary>
        /// Selected ThinSection
        /// </summary>
        private tblThinSection selectedThinSection;

        /// <summary>
        /// ThinSection collection
        /// </summary>
        private BindableCollection<tblThinSection> thinSections;
        #endregion

        #region Public properties

        /// <summary>
        /// Selected ThinSection object
        /// </summary>
        public tblThinSection SelectedThinSection
        {
            get { return this.selectedThinSection; }
            set { this.selectedThinSection = value; NotifyOfPropertyChange(() => SelectedThinSection); }
        }

        /// <summary>
        /// ThinSection collection
        /// </summary>
        public BindableCollection<tblThinSection> ThinSections
        {
            get { return this.thinSections; }
            set { this.thinSections = value; NotifyOfPropertyChange(() => ThinSections); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ThinSectionDetailsViewModel()
        {
            LoadData("");
        }

        /// <summary>
        /// Constructor with specific sample name
        /// </summary>
        public ThinSectionDetailsViewModel(string name)
        {
            LoadData(name);
        }
        #endregion

        #region Methods

        //Loading filtered data
        private void LoadData(string name)
        {

            using (var db = new ApirsRepository<tblThinSection>())
            {
                try
                {
                    ThinSections = new BindableCollection<tblThinSection>(db.GetModelByExpression(ts => ts.tsFromSample == name));
                    if (ThinSections.Count == 0)
                    {
                        SelectedThinSection = new tblThinSection();
                        SelectedThinSection.tsFromSample = name;
                    }
                    else if (ThinSections.Count > 1)
                    {
                        SelectedThinSection = ThinSections.First();
                    }
                    else
                    { SelectedThinSection = ThinSections.First(); }
                }
                catch
                {
                    ThinSections = new BindableCollection<tblThinSection>();
                    SelectedThinSection = new tblThinSection();
                    SelectedThinSection.tsFromSample = name;
                }

            }
        }

        public void Update()
        {
            using (var db = new ApirsRepository<tblThinSection>())
            {
                try
                {
                    if (SelectedThinSection.tsIdPk == 0)
                    {
                        try
                        {
                            db.InsertModel(SelectedThinSection);
                            db.Save();
                            TryClose();
                        }
                        catch
                        {
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Thin section can't be added. Please check every field again.");
                            return;
                        }

                    }
                    else
                    {
                        tblThinSection result = db.GetModelById(SelectedThinSection.tsIdPk);
                        if (result != null)
                        {
                            db.UpdateModel(SelectedThinSection, SelectedThinSection.tsIdPk);
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