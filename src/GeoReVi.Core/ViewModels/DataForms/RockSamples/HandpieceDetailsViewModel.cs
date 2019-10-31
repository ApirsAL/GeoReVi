using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Data.SqlClient;

namespace GeoReVi
{
    public class HandpieceDetailsViewModel : Screen
    {
        #region Private members

        /// <summary>
        /// Selected Handpiece
        /// </summary>
        private tblHandpiece selectedHandpiece;

        /// <summary>
        /// Handpiece collection
        /// </summary>
        private BindableCollection<tblHandpiece> handpieces;
        #endregion

        #region Public properties

        /// <summary>
        /// Selected Handpiece object
        /// </summary>
        public tblHandpiece SelectedHandpiece
        {
            get { return this.selectedHandpiece; }
            set { this.selectedHandpiece = value; NotifyOfPropertyChange(() => SelectedHandpiece); }
        }

        /// <summary>
        /// Handpiece collection
        /// </summary>
        public BindableCollection<tblHandpiece> Handpieces
        {
            get { return this.handpieces; }
            set { this.handpieces = value; NotifyOfPropertyChange(() => Handpieces); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HandpieceDetailsViewModel()
        {
            LoadData("");
        }

        /// <summary>
        /// Constructor with specific sample name
        /// </summary>
        public HandpieceDetailsViewModel(string name)
        {
            LoadData(name);
        }
        #endregion

        #region Methods

        //Loading filtered data
        private void LoadData(string name)
        {

            using (var db = new ApirsRepository<tblHandpiece>())
            {
                try
                {
                    Handpieces = new BindableCollection<tblHandpiece>(db.GetModelByExpression(hp => hp.hpLabelFk == name));
                    if (Handpieces.Count == 0)
                    {
                        SelectedHandpiece = new tblHandpiece();
                        SelectedHandpiece.hpLabelFk = name;
                    }
                    else if (Handpieces.Count > 1)
                    {
                        SelectedHandpiece = Handpieces.First();
                    }
                    else
                    { SelectedHandpiece = Handpieces.First(); }
                }
                catch
                {
                    Handpieces = new BindableCollection<tblHandpiece>();
                    SelectedHandpiece = new tblHandpiece();
                }

            }
        }

        public void Update()
        {
            using (var db = new ApirsRepository<tblHandpiece>())
            {
                try
                {
                    if (SelectedHandpiece.hpIdPk == 0)
                    {
                        try
                        {
                            db.InsertModel(SelectedHandpiece);
                            db.Save();
                            TryClose();
                        }
                        catch
                        {
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Handpiece can't be added. Please check every field again.");
                            return;
                        }

                    }
                    else
                    {
                        tblHandpiece result = db.GetModelById(SelectedHandpiece.hpIdPk);
                        if (result != null)
                        {
                            db.UpdateModel(SelectedHandpiece, SelectedHandpiece.hpIdPk);
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
