using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GeoReVi
{
    public class ImportLocationValuesViewModel : Screen
    {
        #region Private members

        /// <summary>
        /// Event aggregator for event subscription to communicate with other viewmodels
        /// </summary>
        private IEventAggregator _events;

        /// <summary>
        /// The project, the data sets will be imported into
        /// </summary><
        private int _projectId;

        #endregion Private members

        #region Public properties

        #region Constructor


        #endregion

        #region Data

        /// <summary>
        /// The imported data table
        /// </summary>
        private DataTable importedTable;
        public DataTable ImportedTable
        {
            get => this.importedTable;
            set
            {
                this.importedTable = value;

                if (value != null)
                    OnImportedTableChanged();

                NotifyOfPropertyChange(() => TotalImorts);
                NotifyOfPropertyChange(() => ImportedTable);
            }
        }

        /// <summary>
        /// Headers of the imported table
        /// </summary>
        public string SelectedHeader { get; set; }

        private BindableCollection<string> headers;
        public BindableCollection<string> Headers
        {
            get => this.headers;
            set
            {
                this.headers = value;
                NotifyOfPropertyChange(() => Headers);
            }
        }

        /// <summary>
        /// Alias collection of the imported table equivalent from the database
        /// </summary>
        private List<tblAlia> allAlias;
        public tblAlia SelectedAlias { get; set; }

        private BindableCollection<tblAlia> alias =
            new BindableCollection<tblAlia>()
            {
                new tblAlia()
                {
                    alColumnName = "X",
                    alAlias = "X"
                },
                                new tblAlia()
                {
                    alColumnName = "Y",
                    alAlias = "Y"
                },
                new tblAlia()
                {
                    alColumnName = "Z",
                    alAlias = "Z"
                },
                new tblAlia()
                {
                    alColumnName = "Value1",
                    alAlias = "Value1"
                },
                new tblAlia()
                {
                    alColumnName = "Value2",
                    alAlias = "Value2"
                },
                new tblAlia()
                {
                    alColumnName = "Value3",
                    alAlias = "Value3"
                },
                new tblAlia()
                {
                    alColumnName = "Value4",
                    alAlias = "Value4"
                },
                new tblAlia()
                {
                    alColumnName = "Value5",
                    alAlias = "Value5"
                },
                new tblAlia()
                {
                    alColumnName = "Value6",
                    alAlias = "Value6"
                },
                new tblAlia()
                {
                    alColumnName = "Value7",
                    alAlias = "Value7"
                },
                new tblAlia()
                {
                    alColumnName = "Value8",
                    alAlias = "Value8"
                },
                new tblAlia()
                {
                    alColumnName = "Value9",
                    alAlias = "Value9"
                },
                new tblAlia()
                {
                    alColumnName = "Value10",
                    alAlias = "Value10"
                },
                new tblAlia()
                {
                    alColumnName = "DateTime",
                    alAlias = "DateTime"
                },
                 new tblAlia()
                {
                    alColumnName = "Name",
                    alAlias = "Name"
                }
            };
        public BindableCollection<tblAlia> Alias
        {
            get
            {
                return this.alias;
            }
            set
            {
                this.alias = value;
                NotifyOfPropertyChange(() => Alias);
            }
        }

        /// <summary>
        /// Mapping dictionary
        /// </summary>
        public StringPair SelectedMapping { get; set; }
        private BindableCollection<StringPair> mapping;
        public BindableCollection<StringPair> Mapping
        {
            get => this.mapping;
            set
            {
                this.mapping = value;
                NotifyOfPropertyChange(() => Mapping);
            }
        }

        /// <summary>
        /// Class one import collection
        /// </summary>
        private DataTable importOneEntities;
        public DataTable ImportOneEntities
        {
            get => this.importOneEntities;
            set
            {
                this.importOneEntities = value;
                NotifyOfPropertyChange(() => ImportOneEntities);
            }
        }

        /// <summary>
        /// The failed import data sets
        /// </summary>
        private BindableCollection<LocationTimeValue> importFailures;
        public BindableCollection<LocationTimeValue> ImportFailures
        {
            get => this.importFailures;
            set
            {
                this.importFailures = value;
                NotifyOfPropertyChange(() => ImportFailures);
            }
        }

        /// <summary>
        /// Number of total imports
        /// </summary>
        private int totalImorts = 0;
        public int TotalImorts
        {
            get => this.ImportedTable.Rows.Count;
        }
        #endregion Data

        #region Procedures

        /// <summary>
        /// Steps of the import procedure
        /// </summary>
        private bool isStep1 = true;
        public bool IsStep1 { get => this.isStep1; set { this.isStep1 = value; NotifyOfPropertyChange(() => IsStep1); } }
        private bool isStep2 = false;
        public bool IsStep2 { get => this.isStep2; set { this.isStep2 = value; NotifyOfPropertyChange(() => IsStep2); } }
        private bool isCancelled = false;
        public bool IsCancelled { get => this.isCancelled; set { this.isCancelled = value; NotifyOfPropertyChange(() => IsCancelled); } }

        private bool loading;
        public bool Loading
        {
            get => this.loading;
            set
            {
                this.loading = value;
                NotifyOfPropertyChange(() => Loading);
            }
        }
        #endregion Procedures

        #endregion Public properties

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="events"></param>
        public ImportLocationValuesViewModel(IEventAggregator events, DataTable dt, string type = "")
        {
            this._events = events;
            this._events.Subscribe(this);
            this._projectId = (int)((ShellViewModel)IoC.Get<IShell>()).SelectedProject.prjIdPk;
            ImportedTable = dt;
            Mapping = new BindableCollection<StringPair>();
            allAlias = new List<tblAlia>(Alias);
        }

        #endregion

        #region Methods

        //Auto connecting the properties
        public void AutoConnect()
        {
            if (!Loading)
                foreach (tblAlia al in allAlias)
                {
                    try
                    {
                        if (Headers.Where(x => x == al.alAlias).First() != null)
                        {
                            SelectedHeader = Headers.Where(x => x == al.alAlias).First();
                            SelectedAlias = Alias.Where(x => x.alAlias == al.alAlias).First();
                            Connect();
                        }
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
        }

        //Connecting an alias with a header
        public void Connect()
        {
            if (!Loading)
                try
                {
                    //Adding key value to dictionary
                    Mapping.Add(new StringPair() { String1 = SelectedHeader, String2 = SelectedAlias.alAlias });
                    //Removing selected header
                    Headers.Remove(SelectedHeader);
                    //Removing selected alias
                    Alias.Remove(SelectedAlias);

                }
                catch (Exception e)
                {
                    throw e;
                }
        }

        //Removing an alias-header-connection
        public void RemoveConnection()
        {
            if (!Loading)
                try
                {
                    StringPair tempMap = SelectedMapping;

                    //Removing the connection
                    Mapping.Remove(SelectedMapping);
                    //Adding the header
                    Headers.Add(tempMap.String1);
                    //Adding the alias
                    Alias.Add(this.allAlias.Where(x => x.alAlias == tempMap.String2).First());
                }
                catch (Exception e)
                {
                    throw e;
                }
        }

        /// <summary>
        /// Cancelling import
        /// </summary>
        public void Cancel()
        {
            IsCancelled = true;

            if(ImportOneEntities != null)
                ImportOneEntities.Clear();

            if (!Loading)
                ((AdditionalShellViewModel)this.Parent).TryClose();
        }

        //Extracting headers
        private void OnImportedTableChanged()
        {
            Headers = new BindableCollection<string>(ImportedTable.Columns.Cast<DataColumn>()
                                                     .Select(x => x.ColumnName)
                                                     .ToList());
        }

        //Next step
        public async void Next()
        {
            try
            {
                if (IsStep1)
                {
                    if (!(Alias.Count == 0 || Mapping.Count > 0))
                    {
                        ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("All attributes have to be assigned to an imported header.");
                        return;
                    }

                    int numberOfColumns = ImportedTable.Columns.Count;

                    // Iterating over each column of the imported table and assigning the associated GeoReVi header
                    for (int i = 0; i < numberOfColumns; i++)
                    {
                        try
                        {
                            string name = ImportedTable.Columns[i].ColumnName;
                            string columnDb = allAlias.Where(x => x.alAlias == Mapping.Where(y => y.String1 == name).Select(y => y.String2).FirstOrDefault()).Select(x => x.alColumnName).FirstOrDefault();

                            if (String.IsNullOrEmpty(columnDb))
                                continue;

                            ImportedTable.Columns[name].ColumnName = columnDb;
                        }
                        catch
                        {
                            throw new Exception("Data import failed.");
                        }
                    }

                    try
                    {
                        List<string> columnHeaders = new List<string>();

                        foreach (DataColumn dc in ImportedTable.Columns)
                            columnHeaders.Add(dc.ColumnName);

                        if (!columnHeaders.Contains("Value1"))
                            ImportedTable.Columns.Add("Value1", typeof(double));
                        if (!columnHeaders.Contains("Value2"))
                            ImportedTable.Columns.Add("Value2", typeof(double));
                        if (!columnHeaders.Contains("Value3"))
                            ImportedTable.Columns.Add("Value3", typeof(double));
                        if (!columnHeaders.Contains("Value4"))
                            ImportedTable.Columns.Add("Value4", typeof(double));
                        if (!columnHeaders.Contains("Value5"))
                            ImportedTable.Columns.Add("Value5", typeof(double));
                        if (!columnHeaders.Contains("Value6"))
                            ImportedTable.Columns.Add("Value6", typeof(double));
                        if (!columnHeaders.Contains("Value7"))
                            ImportedTable.Columns.Add("Value7", typeof(double));
                        if (!columnHeaders.Contains("Value8"))
                            ImportedTable.Columns.Add("Value8", typeof(double));
                        if (!columnHeaders.Contains("Value9"))
                            ImportedTable.Columns.Add("Value9", typeof(double));
                        if (!columnHeaders.Contains("Value10"))
                            ImportedTable.Columns.Add("Value10", typeof(double));
                        if (!columnHeaders.Contains("X"))
                            ImportedTable.Columns.Add("X", typeof(double));
                        if (!columnHeaders.Contains("Y"))
                            ImportedTable.Columns.Add("Y", typeof(double));
                        if (!columnHeaders.Contains("Z"))
                            ImportedTable.Columns.Add("Z", typeof(double));
                        if (!columnHeaders.Contains("DateTime"))
                            ImportedTable.Columns.Add("DateTime", typeof(DateTime));
                        if (!columnHeaders.Contains("Name"))
                            ImportedTable.Columns.Add("Name", typeof(string));

                        ImportedTable.Columns["Value1"].SetOrdinal(0);
                        ImportedTable.Columns["Value2"].SetOrdinal(1);
                        ImportedTable.Columns["Value3"].SetOrdinal(2);
                        ImportedTable.Columns["Value4"].SetOrdinal(3);
                        ImportedTable.Columns["Value5"].SetOrdinal(4);
                        ImportedTable.Columns["Value6"].SetOrdinal(5);
                        ImportedTable.Columns["Value7"].SetOrdinal(6);
                        ImportedTable.Columns["Value8"].SetOrdinal(7);
                        ImportedTable.Columns["Value9"].SetOrdinal(8);
                        ImportedTable.Columns["Value10"].SetOrdinal(9);
                        ImportedTable.Columns["X"].SetOrdinal(10);
                        ImportedTable.Columns["Y"].SetOrdinal(11);
                        ImportedTable.Columns["Z"].SetOrdinal(12);
                        ImportedTable.Columns["DateTime"].SetOrdinal(13);
                        ImportedTable.Columns["DateTime"].DefaultValue = DateTime.Now;
                        ImportedTable.Columns["Name"].SetOrdinal(14);
                    }
                    catch
                    {
                        throw new Exception("Data import failed.");
                    }
                }

            }
            catch
            {
                throw new Exception("Data import failed.");
            }

            if (!Loading)
                ((AdditionalShellViewModel)this.Parent).TryClose();
        }


        /// <summary>
        /// Finishes the import
        /// </summary>
        public void FinishImport()
        {
            if (!Loading)
                ((AdditionalShellViewModel)this.Parent).TryClose();
        }
        #endregion

        #region Helper

        /// <summary>
        /// Trying to set a property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        private bool TrySetProperty(object obj, string property, object value)
        {
            try
            {
                var prop = obj.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
                if (prop != null && prop.CanWrite)
                    prop.SetValue(obj, value, null);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #endregion
    }
}
