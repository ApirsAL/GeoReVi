using Caliburn.Micro;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace GeoReVi
{
    public class IgneousDetailsViewModel : Screen
    {
        #region Private members

        /// <summary>
        /// Selected plug
        /// </summary>
        private tblIgneousFacy selectedIgneousFacy;

        /// <summary>
        /// Plug collection
        /// </summary>
        private BindableCollection<tblIgneousFacy> igneousFacys;

        #endregion

        #region Public properties

        /// <summary>
        /// Selected plug object
        /// </summary>
        public tblIgneousFacy SelectedIgneousFacy
        {
            get { return this.selectedIgneousFacy; }
            set { this.selectedIgneousFacy = value; NotifyOfPropertyChange(() => SelectedIgneousFacy); }
        }

        /// <summary>
        /// Plug collection
        /// </summary>
        public BindableCollection<tblIgneousFacy> IgneousFacys
        {
            get { return this.igneousFacys; }
            set { this.igneousFacys = value; NotifyOfPropertyChange(() => IgneousFacys); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public IgneousDetailsViewModel()
        {
            LoadData(0);
        }

        /// <summary>
        /// Constructor with specific sample name
        /// </summary>
        public IgneousDetailsViewModel(int id)
        {
            LoadData(id);
        }
        #endregion

        #region Methods

        //Loading filtered data
        private void LoadData(int id)
        {

            using (var db = new ApirsRepository<tblIgneousFacy>())
            {
                try
                {
                    IgneousFacys = new BindableCollection<tblIgneousFacy>(db.GetModelByExpression(lft => lft.iffacIdFk == id));

                    if (IgneousFacys.Count == 0)
                    {
                        SelectedIgneousFacy = new tblIgneousFacy();
                        SelectedIgneousFacy.iffacIdFk = id;
                    }
                    else if (IgneousFacys.Count > 1)
                    {
                        SelectedIgneousFacy = IgneousFacys.First();
                    }
                    else
                    {
                        SelectedIgneousFacy = IgneousFacys.First();
                    }
                }
                catch (Exception e)
                {
                    IgneousFacys = new BindableCollection<tblIgneousFacy>();
                    SelectedIgneousFacy = new tblIgneousFacy();
                }
            }
        }

        // Commit changes from the new object form
        // or edits made to the existing object form.  
        public void Update()
        {
            using (var db = new ApirsRepository<tblIgneousFacy>())
            {
                try
                {
                    if (SelectedIgneousFacy == null)
                    {
                        try
                        {
                            db.InsertModel(SelectedIgneousFacy);
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
                            if (new ApirsRepository<tblIgneousFacy>().GetModelByExpression(x => x.iffacIdFk == SelectedIgneousFacy.iffacIdFk).Count() > 0)
                                db.UpdateModel(SelectedIgneousFacy, SelectedIgneousFacy.iffacIdFk);
                            else
                                db.InsertModel(SelectedIgneousFacy);
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

        /// <summary>
        /// Opening a hyperlink
        /// </summary>
        /// <param name="uri"></param>
        public void OpenHyperlink(string uri)
        {
            Process.Start(uri);
        }

        #endregion
    }
}

