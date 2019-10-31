using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Data.SqlClient;

namespace GeoReVi
{
    public class CuboidDetailsViewModel : Screen
    {
        #region Private members

        /// <summary>
        /// Selected Cuboid
        /// </summary>
        private tblCuboid selectedCuboid;

        /// <summary>
        /// Cuboid collection
        /// </summary>
        private BindableCollection<tblCuboid> cuboids;
        #endregion

        #region Public properties

        /// <summary>
        /// Selected Cuboid object
        /// </summary>
        public tblCuboid SelectedCuboid
        {
            get { return this.selectedCuboid; }
            set { this.selectedCuboid = value; NotifyOfPropertyChange(() => SelectedCuboid); }
        }

        /// <summary>
        /// Cuboid collection
        /// </summary>
        public BindableCollection<tblCuboid> Cuboids
        {
            get { return this.cuboids; }
            set { this.cuboids = value; NotifyOfPropertyChange(() => Cuboids); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public CuboidDetailsViewModel()
        {
            LoadData("");
        }

        /// <summary>
        /// Constructor with specific sample name
        /// </summary>
        public CuboidDetailsViewModel(string name)
        {
            LoadData(name);
        }
        #endregion

        #region Methods

        //Loading filtered data
        private void LoadData(string name)
        {

            using (var db = new ApirsRepository<tblCuboid>())
            {
                try
                {
                    Cuboids = new BindableCollection<tblCuboid>(db.GetModelByExpression(cub => cub.cubLabel == name));
                    if (Cuboids.Count == 0)
                    {
                        SelectedCuboid = new tblCuboid();
                        SelectedCuboid.cubLabel = name;
                    }
                    else if (Cuboids.Count > 1)
                    {
                        SelectedCuboid = Cuboids.First();
                    }
                    else
                    { SelectedCuboid = Cuboids.First(); }
                }
                catch
                {
                    Cuboids = new BindableCollection<tblCuboid>();
                    SelectedCuboid = new tblCuboid();
                }

            }
        }

        public void Update()
        {
            using (var db = new ApirsRepository<tblCuboid>())
            {
                try
                {
                    if (SelectedCuboid.cubIdPk == 0)
                    {
                        try
                        {
                            db.InsertModel(SelectedCuboid);
                            db.Save();
                            TryClose();
                        }
                        catch
                        {
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Cuboid can't be added. Please check every field again.");
                            return;
                        }

                    }
                    else
                    {
                        tblCuboid result = db.GetModelById(SelectedCuboid.cubIdPk);
                        if (result != null)
                        {
                            db.UpdateModel(SelectedCuboid, SelectedCuboid.cubIdPk);
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
