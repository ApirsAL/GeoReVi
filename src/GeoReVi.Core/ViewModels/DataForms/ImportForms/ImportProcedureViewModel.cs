using Caliburn.Micro;
using System.Data;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace GeoReVi
{
    /// <summary>
    /// View model for a one-class import procedure
    /// </summary>
    /// <typeparam name="T">Class to be imported</typeparam>
    public class ImportProcedureViewModel<T> : Screen
    where T : class, new()
    {
        #region Private members

        /// <summary>
        /// Event aggregator for event subscription to communicate with other viewmodels
        /// </summary>
        private IEventAggregator _events;

        /// <summary>
        /// The project, the data sets will be imported into
        /// </summary>
        private int _projectId;

        #endregion Private members

        #region Public properties

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
        private BindableCollection<tblAlia> alias;
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
        private BindableCollection<T> importOneEntities;
        public BindableCollection<T> ImportOneEntities
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
        private BindableCollection<T> importFailures;
        public BindableCollection<T> ImportFailures
        {
            get => this.importFailures;
            set
            {
                this.importFailures = value;
                NotifyOfPropertyChange(() => ImportFailures);
            }
        }

        /// <summary>
        /// Number of remaining imports
        /// </summary>
        private int remainingImports = 0;
        public int RemainingImports
        {
            get => this.remainingImports;
            set
            {
                this.remainingImports = value;
                NotifyOfPropertyChange(() => RemainingImports);
            }
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
        private bool isStep3 = false;
        public bool IsStep3 { get => this.isStep3; set { this.isStep3 = value; NotifyOfPropertyChange(() => IsStep3); } }

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
        public ImportProcedureViewModel(IEventAggregator events, DataTable dt, string type = "")
        {
            this._events = events;
            this._events.Subscribe(this);
            this._projectId = (int)((ShellViewModel)IoC.Get<IShell>()).SelectedProject.prjIdPk;
            ImportedTable = dt;
            ImportOneEntities = new BindableCollection<T>();
            Mapping = new BindableCollection<StringPair>();
            GetAlias();
        }

        #endregion

        #region Methods

        //Getting the alias of a table
        public void GetAlias()
        {
            Alias = new BindableCollection<tblAlia>();
            string tableName1 = typeof(T).Name;

            try
            {
                var alias1 = new BindableCollection<tblAlia>(new ApirsRepository<tblAlia>()
                    .GetModelByExpression(x =>
                    x.alTableName == tableName1)
                    .ToList());


                Alias.AddRange(alias1);

                this.allAlias = Alias.ToList();
            }
            catch (Exception w)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowError(UserMessageValueConverter.ConvertBack(1));
                Cancel();
            }
        }

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

                }
        }

        /// <summary>
        /// Cancelling import
        /// </summary>
        public void Cancel()
        {
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

            CommandHelper ch = new CommandHelper();

            await ch.RunCommand(() => Loading, async () =>
            {
                await Task.Delay(1000);

                if (IsStep1)
                {
                    if (!(Alias.Count == 0 || Mapping.Count > 0))
                    {
                        ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("All attributes have to be assigned to an imported header.");
                        return;
                    }

                    string tableName1 = typeof(T).Name;

                    ImportOneEntities.Clear();
                    ImportOneEntities = new BindableCollection<T>();

                    int numberOfColumns = ImportedTable.Columns.Count;

                    //Data table iteration
                    foreach (DataRow dr in ImportedTable.Rows)
                    {
                        T a = GetObjectA();

                        for (int i = 0; i < numberOfColumns - 1; i++)
                        {
                            try
                            {
                                string name = ImportedTable.Columns[i].ColumnName;
                                string columnDb = allAlias.Where(x => x.alAlias == Mapping.Where(y => y.String1 == name).Select(y => y.String2).First()).Select(x => x.alColumnName).First();

                                TrySetProperty(a, columnDb, dr[i]);
                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    string name = ImportedTable.Columns[i].ColumnName;
                                    string columnDb = allAlias.Where(x => x.alAlias == Mapping.Where(y => y.String1 == name).Select(y => y.String2).First()).Select(x => x.alColumnName).First();

                                    TrySetProperty(a, columnDb, dr[i]);
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }

                        ImportOneEntities.Add(a);
                    }

                    IsStep2 = true;
                    IsStep1 = false;
                    return;

                }
            });
        }

        //Previous step
        public void Previous()
        {
            if (IsStep2 && !Loading)
            {
                IsStep1 = true;
                IsStep2 = false;
            }
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

        //T Instantiation
        protected T GetObjectA()
        {
            return new T();
        }

        /// <summary>
        /// Importing the connected rows into the DB
        /// </summary>
        public async void StartImportAsync()
        {
            //Instantiate local variables
            Type entityType1 = typeof(T);
            PropertyDescriptorCollection properties1 = TypeDescriptor.GetProperties(entityType1);

            //Finding specific columns
            string userIdColumn = "";
            string projectIdColumn = "";
            string parameterColumn = "";

            //Id value of parent
            int idValue = 0;

            int importcounts = 0;
            ImportFailures = new BindableCollection<T>();

            CommandHelper ch = new CommandHelper();

            await ch.RunCommand(() => Loading, async () =>
            {
                await Task.Delay(3000);

                foreach (PropertyDescriptor prop in properties1)
                {
                    //Trying to find a user id column
                    try
                    {
                        if (prop.Name.Contains("UserId"))
                            userIdColumn = prop.Name;
                        else if (prop.Name.Contains("UploaderId"))
                            userIdColumn = prop.Name;
                        else if (prop.Name.Contains("InterpreterId"))
                            userIdColumn = prop.Name;
                    }
                    catch (Exception e)
                    {

                    }
                    //Trying to find a project id column
                    try
                    {
                        if (prop.Name.Contains("ProjectId"))
                            projectIdColumn = prop.Name;
                        else if (prop.Name.Contains("prjId"))
                            projectIdColumn = prop.Name;
                    }
                    catch (Exception e)
                    {

                    }
                    //Trying to find a project id column
                    try
                    {
                        if (prop.Name.Contains("fimeType"))
                            parameterColumn = prop.Name;
                        else if (prop.Name.Contains("labmeParameter"))
                            parameterColumn = prop.Name;
                    }
                    catch (Exception e)
                    {

                    }
                }

                //Importing second entity collection
                importcounts = ImportOneEntities.Count;

                //Using transaction mode to transmit the inserts
                using (var db1 = new ApirsRepository<T>())
                {
                    //Interating through the import entities
                    for (int i = 0; i < importcounts; i++)
                    {

                        try
                        {
                            //trying to assign the previously found user id column
                            if (userIdColumn != "")
                            {
                                TrySetProperty(ImportOneEntities[i], userIdColumn, (int)((ShellViewModel)IoC.Get<IShell>()).UserId);

                            }

                            if (projectIdColumn != "")
                                TrySetProperty(ImportOneEntities[i], projectIdColumn, _projectId);

                            try
                            {
                                db1.InsertModel(ImportOneEntities[i]);
                            }
                            catch (Exception e)
                            {
                                //Adding the failed data set to a collection
                                ImportFailures.Add(ImportOneEntities[i]);
                                continue;
                            }

                        }
                        catch (Exception ex)
                        {
                            //Adding the failed data set to a collection
                            ImportFailures.Add(ImportOneEntities[i]);
                        }
                        finally
                        {
                            RemainingImports = ImportOneEntities.Count() - i;
                        }
                    }
                }
            });

            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(String.Format("{0} data sets were imported. {1} data sets could not be imported and will be exported as a .csv file for error correction.", importcounts - importFailures.Count(), ImportFailures.Count()));
            ImportOneEntities = new BindableCollection<T>(ImportFailures.ToList());
        }


        #endregion
    }

    /// <summary>
    /// View model for a two-class import procedure
    /// </summary>
    /// <typeparam name="T">First class to be imported (parent)</typeparam>
    /// <typeparam name="U">Second class to be imported (child)</typeparam>
    public class ImportProcedureViewModel<T, U> : Screen
        where T : class, new()
        where U : class, new()
    {
        #region Private members

        /// <summary>
        /// Event aggregator for event subscription to communicate with other viewmodels
        /// </summary>
        private IEventAggregator _events;

        private int _projectId;

        #endregion Private members

        #region Public properties

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
        private BindableCollection<tblAlia> alias;
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
        private BindableCollection<T> importOneEntities;
        public BindableCollection<T> ImportOneEntities
        {
            get => this.importOneEntities;
            set
            {
                this.importOneEntities = value;
                NotifyOfPropertyChange(() => ImportOneEntities);
            }
        }

        /// <summary>
        /// Class one import collection
        /// </summary>
        private BindableCollection<U> importTwoEntities;
        public BindableCollection<U> ImportTwoEntities
        {
            get => this.importTwoEntities;
            set
            {
                this.importTwoEntities = value;
                NotifyOfPropertyChange(() => ImportTwoEntities);
            }
        }

        private BindableCollection<GenericPair<T, U>> importEntities;
        public BindableCollection<GenericPair<T, U>> ImportEntities
        {
            get => this.importEntities;
            set
            {
                this.importEntities = value;
                NotifyOfPropertyChange(() => ImportEntities);
            }
        }

        /// <summary>
        /// The failed import data sets
        /// </summary>
        private BindableCollection<GenericPair<T, U>> importFailures;
        public BindableCollection<GenericPair<T, U>> ImportFailures
        {
            get => this.importFailures;
            set
            {
                this.importFailures = value;
                NotifyOfPropertyChange(() => ImportFailures);
            }
        }

        /// <summary>
        /// Number of remaining imports
        /// </summary>
        private int remainingImports = 0;
        public int RemainingImports
        {
            get => this.remainingImports;
            set
            {
                this.remainingImports = value;
                NotifyOfPropertyChange(() => RemainingImports);
            }
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
        private bool isStep3 = false;
        public bool IsStep3 { get => this.isStep3; set { this.isStep3 = value; NotifyOfPropertyChange(() => IsStep3); } }

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
        public ImportProcedureViewModel(IEventAggregator events, DataTable dt, string type = "")
        {
            this._events = events;
            this._events.Subscribe(this);
            this._projectId = (int)((ShellViewModel)IoC.Get<IShell>()).SelectedProject.prjIdPk;
            ImportedTable = dt;
            Mapping = new BindableCollection<StringPair>();
            ImportEntities = new BindableCollection<GenericPair<T, U>>();
            GetAlias();
        }

        #endregion

        #region Methods

        /// <summary>
        ///Getting the alias of a table
        /// </summary>
        public void GetAlias()
        {
            Alias = new BindableCollection<tblAlia>();
            string tableName1 = typeof(T).Name;
            string tableName2 = typeof(U).Name;

            try
            {
                var alias1 = new BindableCollection<tblAlia>(new ApirsRepository<tblAlia>()
                    .GetModelByExpression(x =>
                    x.alTableName == tableName1
                    && x.alImport == true)
                    .ToList());

                var alias2 = new BindableCollection<tblAlia>(new ApirsRepository<tblAlia>()
                    .GetModelByExpression(x =>
                    x.alTableName == tableName2
                    && x.alImport == true)
                    .ToList());

                Alias.AddRange(alias1);
                Alias.AddRange(alias2);

                this.allAlias = Alias.ToList();
            }
            catch (Exception w)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowError(UserMessageValueConverter.ConvertBack(1));
                Cancel();
            }
        }

        /// <summary>
        ///Getting the alias of a table
        /// </summary>
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

                }
        }

        /// <summary>
        /// Auto connecting the data base headers to the import table headers
        /// </summary>
        public void AutoConnect()
        {
            if (!Loading)
                foreach (tblAlia al in allAlias)
                {
                    try
                    {
                        if (Headers.Where(x => x == al.alAlias).FirstOrDefault() != null)
                        {
                            SelectedHeader = Headers.Where(x => x == al.alAlias).First();
                            SelectedAlias = Alias.Where(x => x.alAlias == al.alAlias).First();
                            Connect();
                        }
                        else if(Headers.Where(x => al.alAlias.ToLower().Contains(x.ToLower()) || x.ToLower().Contains(al.alAlias.ToLower())).FirstOrDefault() != null)
                        {
                            SelectedHeader = Headers.Where(x=> al.alAlias.Contains(x)).First();

                            if(SelectedHeader == null)
                                SelectedHeader = Headers.Where(x => x.Contains(al.alAlias)).First();

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
        /// <summary>
        ///Getting the alias of a table
        /// </summary>
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

                }
        }

        /// <summary>
        /// Cancelling import
        /// </summary>
        public void Cancel()
        {
            if (!Loading)
                ((AdditionalShellViewModel)this.Parent).TryClose();
        }

        /// <summary>
        ///Getting the alias of a table
        /// </summary>
        private void OnImportedTableChanged()
        {
            Headers = new BindableCollection<string>(ImportedTable.Columns.Cast<DataColumn>()
                                                     .Select(x => x.ColumnName)
                                                     .ToList());
        }

        //Next step
        public async void Next()
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => Loading, async () =>
             {
                 if (IsStep1)
                 {
                     if (!(Alias.Count == 0 || Mapping.Count > 0))
                     {
                         ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("All attributes have to be assigned to an imported header.");
                         return;
                     }

                     string tableName1 = typeof(T).Name;
                     string tableName2 = typeof(U).Name;

                     ImportEntities.Clear();
                     ImportOneEntities = new BindableCollection<T>();
                     ImportTwoEntities = new BindableCollection<U>();

                     int numberOfColumns = ImportedTable.Columns.Count;

                     //Data table iteration
                     foreach (DataRow dr in ImportedTable.Rows)
                     {
                         T a = GetObjectA();
                         U b = GetObjectB();

                         for (int i = 0; i < numberOfColumns - 1; i++)
                         {
                             if(Mapping.Select(x=>x.String1).Contains(dr.Table.Columns[i].ColumnName))
                             try
                             {
                                 string name = ImportedTable.Columns[i].ColumnName;
                                 string columnDb = allAlias.Where(x => x.alAlias == Mapping.Where(y => y.String1 == name).Select(y => y.String2).First()).Select(x => x.alColumnName).First();
                                   
                                 if (dr[i] != null)
                                     try
                                     {
                                         TrySetProperty(a, columnDb, dr[i]);
                                         TrySetProperty(b, columnDb, dr[i]);
                                     }
                                     catch
                                     {
                                         TrySetProperty(a, columnDb, (int)dr[i]);
                                         TrySetProperty(b, columnDb, (int)dr[i]);
                                     }

                             }
                             catch (Exception ex)
                             {
                                 continue;
                             }

                         }

                         ImportOneEntities.Add(a);
                         ImportTwoEntities.Add(b);
                         GenericPair<T, U> ab = new GenericPair<T, U>() { First = a, Second = b };
                         ImportEntities.Add(ab);
                     }

                     IsStep2 = true;
                     IsStep1 = false;
                     return;

                 }
             });
        }

        //Previous step
        public void Previous()
        {
            if (IsStep2 && !Loading)
            {
                IsStep1 = true;
                IsStep2 = false;
            }
        }

        /// <summary>
        /// Importing the connected rows into the DB
        /// </summary>
        public async void StartImportAsync()
        {
            //Instantiate local variables
            Type entityType1 = typeof(T);
            PropertyDescriptorCollection properties1 = TypeDescriptor.GetProperties(entityType1);
            Type entityType2 = typeof(U);
            PropertyDescriptorCollection properties2 = TypeDescriptor.GetProperties(entityType2);

            //Finding specific columns
            string userIdColumn = "";
            string projectIdColumn = "";
            string idColumn = "";
            string foreignIdColumn = "";

            //Id value of parent
            int idValue = 0;

            int importcounts = 0;
            ImportFailures = new BindableCollection<GenericPair<T, U>>();

            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => Loading, async () =>
            {
                foreach (PropertyDescriptor prop in properties1)
                {
                    //Trying to find a user id column
                    try
                    {
                        if (prop.Name.Contains("UserId"))
                            userIdColumn = prop.Name;
                        else if (prop.Name.Contains("UploaderId"))
                            userIdColumn = prop.Name;
                        else if (prop.Name.Contains("InterpreterId"))
                            userIdColumn = prop.Name;
                    }
                    catch (Exception e)
                    {

                    }
                    //Trying to find a project id column
                    try
                    {
                        if (prop.Name.Contains("ProjectId"))
                            projectIdColumn = prop.Name;
                        else if (prop.Name.Contains("prjId"))
                            projectIdColumn = prop.Name;
                    }
                    catch (Exception e)
                    {

                    }
                    //Trying to find the id column
                    try
                    {
                        if (prop.Name.Contains("labmeIdPk"))
                            idColumn = prop.Name;
                        else if (prop.Name.Contains("IdPk"))
                            idColumn = prop.Name;
                    }
                    catch (Exception e)
                    {

                    }
                }
                foreach (PropertyDescriptor prop in properties2)
                {
                    //Trying to find the foreign id column
                    try
                    {
                        if (prop.Name.Contains("labmeIdFk")
                        || prop.Name.Contains("gdIdFk")
                        || prop.Name.Contains("gdIdFk")
                        || prop.Name.Contains("apermIdFk")
                        || prop.Name.Contains("porIdFk")
                        || prop.Name.Contains("inpeIdFk")
                        || prop.Name.Contains("bdIdFk")
                        || prop.Name.Contains("swIdFk")
                        || prop.Name.Contains("xrfIdPk")
                        || (prop.Name.Contains("tcIdFk") && !prop.Name.Contains("tdtcIdFk"))
                        || prop.Name.Contains("measIdFk")
                        || prop.Name.Contains("tdIdFk")
                        )
                            foreignIdColumn = prop.Name;

                        else if (prop.Name.Contains("fimeIdPk")
                        || prop.Name.Contains("sgrIdPk")
                        || prop.Name.Contains("fimeIdFk")
                        || prop.Name.Contains("sofimeIdFk"))
                            foreignIdColumn = prop.Name;
                    }
                    catch (Exception e)
                    {

                    }
                }

                //Importing second entity collection
                importcounts = ImportOneEntities.Count;

                //Using transaction mode to transmit the inserts
                using (var db1 = new ApirsRepository<T>())
                {
                    using (var db2 = new ApirsRepository<U>())
                    {
                        //Interating through the import entities
                        for (int i = 0; i < importcounts; i++)
                        {

                            try
                            {
                                //trying to assign the previously found user id column
                                if (userIdColumn != "")
                                {
                                    TrySetProperty(ImportOneEntities[i], userIdColumn, (int)((ShellViewModel)IoC.Get<IShell>()).UserId);
                                    TrySetProperty(ImportTwoEntities[i], userIdColumn, (int)((ShellViewModel)IoC.Get<IShell>()).UserId);

                                }

                                if (projectIdColumn != "")
                                    TrySetProperty(ImportOneEntities[i], projectIdColumn, _projectId);

                                try
                                {
                                    db1.InsertModel(ImportOneEntities[i]);

                                    idValue = (int)TryGetProperty(ImportOneEntities[i], idColumn);

                                    if (idValue != 0 && foreignIdColumn != "")
                                    {
                                        TrySetProperty(ImportTwoEntities[i], foreignIdColumn, idValue);
                                    }

                                }
                                catch (Exception e)
                                {
                                    //Adding the failed data set to a collection
                                    ImportFailures.Add(new GenericPair<T, U>() { First = ImportOneEntities[i], Second = ImportTwoEntities[i] });
                                    continue;
                                }
                                //Adding first and second entity to the database

                                try
                                {
                                    db2.InsertModel(ImportTwoEntities[i]);
                                }
                                catch (DbEntityValidationException dbEx)
                                {
                                    //foreach (var validationErrors in dbEx.EntityValidationErrors)
                                    //{
                                    //    foreach (var validationError in validationErrors.ValidationErrors)
                                    //    {
                                    //        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                                    //    }
                                    //}
                                }
                                catch (Exception e)
                                {
                                    if (idValue != 0)
                                        db1.DeleteModelById(idValue);

                                    //Adding the failed data set to a collection
                                    ImportFailures.Add(new GenericPair<T, U>() { First = ImportOneEntities[i], Second = ImportTwoEntities[i] });
                                }

                            }
                            catch (Exception ex)
                            {
                                //Adding the failed data set to a collection
                                ImportFailures.Add(new GenericPair<T, U>() { First = ImportOneEntities[i], Second = ImportTwoEntities[i] });
                            }
                            finally
                            {
                                RemainingImports = ImportEntities.Count() - i;
                            }
                        }
                    }
                }
            });

            RemainingImports = 0;

            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(String.Format("{0} data sets were imported. {1} data sets could not be imported and will be exported as a .csv file for error correction.", importcounts - importFailures.Count(), ImportFailures.Count()));
            ImportOneEntities = new BindableCollection<T>(ImportFailures.Select(x => x.First).ToList());
            ImportTwoEntities = new BindableCollection<U>(ImportFailures.Select(x => x.Second).ToList());
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

        /// <summary>
        /// Trying to set a property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        private object TryGetProperty(object obj, string property)
        {
            try
            {
                var prop = obj.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
                if (prop != null && prop.CanWrite)
                    return prop.GetValue(obj);

                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        //T Instantiation
        protected T GetObjectA()
        {
            return new T();
        }

        //T Instantiation
        protected U GetObjectB()
        {
            return new U();
        }

        #endregion
    }

    /// <summary>
    /// A class with a string pair attribute
    /// </summary>
    public class StringPair
    {
        public string String1 { get; set; }
        public string String2 { get; set; }
    }
}
