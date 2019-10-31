using LiteDB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Text;

namespace GeoReVi
{
    /// <summary>
    /// Generic data repository interface for a class T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAllRepository<T> : IDisposable
        where T : class, new()
    {
        IEnumerable<T> GetModel();

        IEnumerable<T> GetModelByExpression(Expression<Func<T, bool>> filter);

        T GetModelById(int modelId, int localId = 0);

        void InsertModel(T model);

        void DeleteModelById(int modelId, int localId = 0);

        void DeleteRange(IEnumerable<T> modelsToDelete);

        void Save();
    }

    /// <summary>
    /// Generic data repository for the server apirs database
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApirsRepository<T> : IAllRepository<T> where T : class, new()
    {
        #region Private members

        //server database context
        protected ApirsDatabase _apirsDatabase;
        //Transaction context
        protected DbContextTransaction dbContextTransaction;
        //Service
        //protected GeoReViDatabase georev;

        //server database set
        private DbSet<T> dbEntity;

        //local database file
        private static string dbLiteFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\GeoReVi\Data\LocalDB\GeoReViLocal.db";

        //local lightdb database connection object
        private static LiteDatabase _apirsLocalLiteDatabase = new LiteDatabase(dbLiteFile);

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApirsRepository()
        {
            if ((bool)((ShellViewModel)IoC.Get<IShell>()).LocalMode)
            {
                _apirsLocalLiteDatabase = new LiteDatabase(dbLiteFile);
            }
            else
            {
                _apirsDatabase = new ApirsDatabase();
                dbEntity = _apirsDatabase.Set<T>();
            }
        }

        #endregion

        #region Generic queries

        /// <summary>
        /// Deleting a model by its id
        /// </summary>
        /// <param name="modelId"></param>
        public void DeleteModelById(int modelId, int localId = 0)
        {
            if (_apirsDatabase != null)
            {
                T model = dbEntity.Find(modelId);
                dbEntity.Remove(model);
                _apirsDatabase.SaveChanges();
            }
            else
            {
                _apirsLocalLiteDatabase.GetCollection<T>(typeof(T).Name).Delete(modelId);
            }
        }

        /// <summary>
        /// Deleting a model by its id
        /// </summary>
        /// <param name="modelId"></param>
        public void DeleteModelById(Guid modelId, int localId = 0)
        {
            if (_apirsDatabase != null)
            {
                T model = dbEntity.Find(modelId);
                dbEntity.Remove(model);
                _apirsDatabase.SaveChanges();
            }
            else
            {
                _apirsLocalLiteDatabase.GetCollection<T>(typeof(T).Name).Delete(modelId);
            }
        }

        /// <summary>
        /// Deleting a range of models
        /// </summary>
        /// <param name="modelsToDelete"></param>
        public void DeleteRange(IEnumerable<T> modelsToDelete)
        {
            dbEntity.RemoveRange(modelsToDelete);
        }
        /// <summary>
        /// Getting all models
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetModel()
        {
            if (_apirsDatabase != null)
            {
                return (dbEntity.Select(x => x).ToList());
            }
            else
            {
                return _apirsLocalLiteDatabase.GetCollection<T>(typeof(T).Name).FindAll();
            }
        }

        /// <summary>
        /// Getting all models
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetModelByExpression(Expression<Func<T, bool>> filter)
        {
            if (_apirsDatabase != null)
            {
                return dbEntity.Where(filter);
            }
            else
            {
                return _apirsLocalLiteDatabase.GetCollection<T>(typeof(T).Name).Find(filter);
            }
        }

        /// <summary>
        /// Getting one model by id
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public T GetModelById(int modelId, int localId = 0)
        {
            if (_apirsDatabase != null)
            {
                return dbEntity.Find(modelId);
            }
            else
            {
                var a = _apirsLocalLiteDatabase.GetCollection<T>(typeof(T).Name).FindById(modelId);

                return a;
            }

        }

        /// <summary>
        /// Inserting a model into the db set
        /// </summary>
        /// <param name="model"></param>
        public void InsertModel(T model)
        {
            if (_apirsDatabase != null)
            {
                dbEntity.Add(model);
                _apirsDatabase.SaveChanges();
            }
            else
            {
                _apirsLocalLiteDatabase.GetCollection<T>(typeof(T).Name).Insert(model);
            }

        }

        /// <summary>
        /// Saving changes to the data context
        /// </summary>
        public void Save()
        {
            try
            {
                if (_apirsDatabase != null)
                    _apirsDatabase.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                }
            }
        }

        /// <summary>
        /// Updating a model
        /// </summary>
        /// <param name="model"></param>
        public void UpdateModel(T model, int id, bool local = false)
        {
            if (_apirsDatabase != null)
            {
                T result = GetModelById(id);
                if (result != null)
                {
                    _apirsDatabase.Entry<T>(result).CurrentValues.SetValues(model);
                }

                _apirsDatabase.SaveChanges();
            }
            else
            {
                _apirsLocalLiteDatabase.GetCollection<T>(typeof(T).Name).Update(model);
            }
        }

        /// <summary>
        /// Updating a model
        /// </summary>
        /// <param name="model"></param>
        public void UpdateModelWithoutSave(T model, int id, bool local = false)
        {
            if (_apirsDatabase != null)
            {
                T result = GetModelById(id);
                if (result != null)
                {
                    _apirsDatabase.Entry<T>(result).CurrentValues.SetValues(model);
                }
            }
            else
            {
                _apirsLocalLiteDatabase.GetCollection<T>(typeof(T).Name).Update(model);
            }
        }


        /// <summary>
        /// Activating the transaction context
        /// </summary>
        /// <param name="model"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public void StartTransaction()
        {
            this.dbContextTransaction = _apirsDatabase.Database.BeginTransaction();
        }

        /// <summary>
        /// Commiting the transaction
        /// </summary>
        /// <param name="model"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public void CommitTransaction()
        {
            this.dbContextTransaction.Commit();
        }

        /// <summary>
        /// Rolling the transaction back
        /// </summary>
        /// <param name="model"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public void RollbackTransaction()
        {
            this.dbContextTransaction.Rollback();
        }

        //Check if there is a unique data set based on an input string
        public bool CheckUniqueness(T model, string parameter)
        {
            var type = model.GetType().GetGenericArguments()[0];
            var properties = type.GetProperties();

            return dbEntity.Any(x => properties
                        .Any(p => p.GetValue(x).ToString().Contains(parameter)));
        }
        #endregion

        #region Specific queries

        /// <summary>
        /// Returns a set of laboratory measurements 
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="rockSample"></param>
        /// <param name="laboratoryMeasurement"></param>
        /// <param name="groupClause"></param>
        /// <param name="additionalParameter"></param>
        /// <param name="all"></param>
        /// <returns></returns>
        public List<Mesh> GetLaboratoryMeasurementPoints(IEnumerable<tblRockSample> samples,
            tblRockSample rockSample,
            tblMeasurement laboratoryMeasurement,
            string groupClause,
            string property = "",
            bool all = false,
            bool global = false,
            DateTime? from = null,
            DateTime? to = null,
            string filterProperty = "",
            string filter = "")
        {
            #region Variable declaration

            List<Mesh> MeasPoints = new List<Mesh>();

            List<string> c = samples.Where(samp => samp.sampooiName == rockSample.sampooiName)
            .Select(samp => samp.sampLabel).ToList();

            if (all)
                samples = new List<tblRockSample>(samples.ToList());
            else
                samples = new List<tblRockSample>(samples.Where(samp => samp.sampooiName == rockSample.sampooiName));

            IEnumerable<IGrouping<string, tblRockSample>> groups = new List<IGrouping<string, tblRockSample>>();

            List<Tuple<double, double, double, double, DateTime, string>> values = new List<Tuple<double, double, double, double, DateTime, string>>();
            List<List<Tuple<double, double, double, double, DateTime, string>>> allValues = new List<List<Tuple<double, double, double, double, DateTime, string>>>();

            double minX = 999999999999999999;
            double minY = 999999999999999999;

            #endregion

            try
            {
                switch (groupClause)
                {
                    case "Object of investigation":
                        groups = from x in samples
                                 group x by x.sampooiName into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    case "Petrography":
                        groups = from x in samples
                                 group x by x.sampPetrographicTerm into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    case "Lithostratigraphy":
                        groups = from x in samples
                                 group x by x.sampLithostratigraphyName into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    case "Chronostratigraphy":
                        groups = from x in samples
                                 group x by x.sampChronStratName into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    case "Facies":
                        groups = from x in samples
                                 group x by x.sampFaciesFk into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    case "Architectural element":
                        groups = from x in samples
                                 group x by x.sampArchitecturalElement into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    case "Depositional environment":
                        groups = from x in samples
                                 group x by x.sampDepositionalEnvironment into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    case "Sample type":
                        groups = from x in samples
                                 group x by x.sampType into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    default:
                        break;

                }

                string[] groupString = new string[] { };

                if (groups.Count() > 0)
                    groupString = groups.Select(x => x.Key).ToArray();

                if (groupString.Count() < 1)
                    groupString = new string[] { "All" };

                foreach (var group in groupString)
                {
                    try
                    {
                        switch (groupClause)
                        {
                            case "Object of investigation":
                                c = samples.Where(samp => samp.sampooiName == group)
                                                       .Select(samp => samp.sampLabel)
                                                       .ToList();
                                break;
                            case "Petrography":
                                c = samples.Where(samp => samp.sampPetrographicTerm == group)
                                                       .Select(samp => samp.sampLabel)
                                                       .ToList();
                                break;
                            case "Lithostratigraphy":
                                c = samples.Where(samp => samp.sampLithostratigraphyName == group)
                                                       .Select(samp => samp.sampLabel)
                                                       .ToList();
                                break;
                            case "Chronostratigraphy":
                                c = samples.Where(samp => samp.sampChronStratName == group)
                                                       .Select(samp => samp.sampLabel)
                                                       .ToList();
                                break;
                            case "Facies":
                                c = samples.Where(samp => samp.sampFaciesFk == group)
                                                       .Select(samp => samp.sampLabel)
                                                       .ToList();
                                break;
                            case "Architectural element":
                                c = samples.Where(samp => samp.sampArchitecturalElement == group)
                                                       .Select(samp => samp.sampLabel)
                                                       .ToList();
                                break;
                            case "Depositional environment":
                                c = samples.Where(samp => samp.sampDepositionalEnvironment == group)
                                                       .Select(samp => samp.sampLabel)
                                                       .ToList();
                                break;
                            case "Sample type":
                                c = samples.Where(samp => samp.sampType == group)
                                               .Select(samp => samp.sampLabel)
                                               .ToList();
                                break;
                            default:
                                c = samples.Select(samp => samp.sampLabel)
                                           .ToList();
                                break;

                        }

                        //Getting all measurements belonging to the rock sample collection based on a date range
                        List<tblMeasurement> measurements = new ApirsRepository<tblMeasurement>()
                                                                            .GetModelByExpression(labme => c.Contains(labme.measRockSampleIdFk))
                                                                            .Where(x => (from != null ? x.measDate >= from : 0 == 0) && (to != null ? x.measDate <= to : 0 == 0))
                                                                            .ToList();

                        //measurements related to the selected rock samples with the selected parameter
                        List<int> b = measurements.Where(labme => (string)labme.measParameter == laboratoryMeasurement.measParameter
                                                        && labme.measIdPk != 0 && c.Contains(labme.measRockSampleIdFk))
                                                        .Select(labme => (int)labme.measIdPk)
                                                        .ToList();

                        #region Defining lambda properties

                        ///lambda parameter for rock sample properties
                        var z = System.Linq.Expressions.Expression.Parameter(typeof(tblRockSample), "z");

                        List<string> prop = new List<string>();

                        if (global)
                        {
                            prop.Add("sampLongitude");
                            prop.Add("sampLatitude");
                            prop.Add("sampElevation");
                        }
                        else
                        {
                            prop.Add("sampLocalXCoordinates");
                            prop.Add("sampLocalYCoordinates");
                            prop.Add("sampLocalZCoordinates");
                        }

                        prop.Add("sampLabel");
                        prop.Add(property);

                        var body = System.Linq.Expressions.Expression.PropertyOrField(z, prop[0]);
                        var lambdaX = System.Linq.Expressions.Expression.Lambda<Func<tblRockSample, double?>>(body, z);

                        body = System.Linq.Expressions.Expression.PropertyOrField(z, prop[1]);
                        var lambdaY = System.Linq.Expressions.Expression.Lambda<Func<tblRockSample, double?>>(body, z);

                        body = System.Linq.Expressions.Expression.PropertyOrField(z, prop[2]);
                        var lambdaZ = System.Linq.Expressions.Expression.Lambda<Func<tblRockSample, double?>>(body, z);

                        body = System.Linq.Expressions.Expression.PropertyOrField(z, prop[3]);
                        var lambdaName = System.Linq.Expressions.Expression.Lambda<Func<tblRockSample, string>>(body, z);

                        #endregion

                        //Loading data from the database
                        if (laboratoryMeasurement.measParameter == "Apparent permeability")
                        {
                            values = new ApirsRepository<tblApparentPermeability>()
                                .GetModelByExpression(aperm => b.Contains(aperm.apermIdFk))
                                .Where(x => (filter != "" ? (string)x.GetType().GetProperty(filterProperty).GetValue(x, null) == filter : 0 == 0))
                                .Select(aperm => new Tuple<double, double, double, double, DateTime, string>(
                            Convert.ToDouble(aperm.GetType().GetProperty(property).GetValue(aperm, null)),
                            Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == aperm.apermIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaX.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == aperm.apermIdFk).FirstOrDefault().measLocalCoordinateX))),
                            Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == aperm.apermIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaY.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == aperm.apermIdFk).FirstOrDefault().measLocalCoordinateY))),
                            Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == aperm.apermIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaZ.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == aperm.apermIdFk).FirstOrDefault().measLocalCoordinateZ))),
                            Convert.ToDateTime(measurements.Where(labme => labme.measIdPk == aperm.apermIdFk).Select(labme => labme.measDate).FirstOrDefault()),
                            samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == aperm.apermIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaName.Compile()).First()
                        )).Where(x => x.Item1 != 0).ToList();
                        }
                        else if (laboratoryMeasurement.measParameter == "Axial compression")
                        {
                            values = new ApirsRepository<tblAxialCompression>()
                            .GetModelByExpression(ac => b.Contains(ac.aclabmeIdFk))
                            .Where(x => (filter != "" ? (string)x.GetType().GetProperty(filterProperty).GetValue(x, null) == filter : 0 == 0))
                            .Select(ac => new Tuple<double, double, double, double, DateTime, string>(
                                Convert.ToDouble(ac.GetType().GetProperty(property).GetValue(ac, null)),
                                Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == ac.aclabmeIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaX.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == ac.aclabmeIdFk).FirstOrDefault().measLocalCoordinateX))),
                                Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == ac.aclabmeIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaY.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == ac.aclabmeIdFk).FirstOrDefault().measLocalCoordinateY))),
                                Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == ac.aclabmeIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaZ.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == ac.aclabmeIdFk).FirstOrDefault().measLocalCoordinateZ))),
                                Convert.ToDateTime(measurements.Where(labme => labme.measIdPk == ac.aclabmeIdFk).Select(labme => labme.measDate).FirstOrDefault()),
                                samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == ac.aclabmeIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaName.Compile()).First()
                            )).Where(x => x.Item1 != 0).ToList();
                        }
                        else if (laboratoryMeasurement.measParameter == "Grain size")
                        {
                            values = new ApirsRepository<tblGrainSize>()
                            .GetModelByExpression(gs => b.Contains(gs.gslabmeIdFk))
                            .Where(x => (filter != "" ? (string)x.GetType().GetProperty(filterProperty).GetValue(x, null) == filter : 0 == 0))
                            .Select(gs => new Tuple<double, double, double, double, DateTime, string>(
                                Convert.ToDouble(gs.GetType().GetProperty(property).GetValue(gs, null)),
                                Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == gs.gslabmeIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaX.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == gs.gslabmeIdFk).FirstOrDefault().measLocalCoordinateX))),
                                Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == gs.gslabmeIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaY.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == gs.gslabmeIdFk).FirstOrDefault().measLocalCoordinateY))),
                                Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == gs.gslabmeIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaZ.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == gs.gslabmeIdFk).FirstOrDefault().measLocalCoordinateZ))),
                                Convert.ToDateTime(measurements.Where(labme => labme.measIdPk == gs.gslabmeIdFk).Select(labme => labme.measDate).FirstOrDefault()),
                                samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == gs.gslabmeIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaName.Compile()).First()
                            )).Where(x => x.Item1 != 0).ToList();
                        }
                        else if (laboratoryMeasurement.measParameter == "Intrinsic permeability")
                        {
                            values = new ApirsRepository<tblIntrinsicPermeability>()
                            .GetModelByExpression(inpe => b.Contains(inpe.inpeIdFk))
                            .Where(x => (filter != "" ? (string)x.GetType().GetProperty(filterProperty).GetValue(x, null) == filter : 0 == 0))
                            .Select(inpe => new Tuple<double, double, double, double, DateTime, string>(
                                Convert.ToDouble(inpe.GetType().GetProperty(property).GetValue(inpe, null)),
                                Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == inpe.inpeIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaX.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == inpe.inpeIdFk).FirstOrDefault().measLocalCoordinateX))),
                                Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == inpe.inpeIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaY.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == inpe.inpeIdFk).FirstOrDefault().measLocalCoordinateY))),
                                Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == inpe.inpeIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaZ.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == inpe.inpeIdFk).FirstOrDefault().measLocalCoordinateZ))),
                                Convert.ToDateTime(measurements.Where(labme => labme.measIdPk == inpe.inpeIdFk).Select(labme => labme.measDate).FirstOrDefault()),
                                samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == inpe.inpeIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaName.Compile()).First()
                            )).Where(x => x.Item1 != 0).ToList();
                        }
                        else if (laboratoryMeasurement.measParameter == "Isotope")
                        {
                            values = new ApirsRepository<tblIsotopes>()
                            .GetModelByExpression(iso => b.Contains(iso.islabmeIdFk))
                            .Where(x => (filter != "" ? (string)x.GetType().GetProperty(filterProperty).GetValue(x, null) == filter : 0 == 0))
                            .Select(iso => new Tuple<double, double, double, double, DateTime, string>(
                                Convert.ToDouble(iso.GetType().GetProperty(property).GetValue(iso, null)),
                                Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == iso.islabmeIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaY.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == iso.islabmeIdFk).FirstOrDefault().measLocalCoordinateX))),
                                Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == iso.islabmeIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaZ.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == iso.islabmeIdFk).FirstOrDefault().measLocalCoordinateY))),
                                Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == iso.islabmeIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaX.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == iso.islabmeIdFk).FirstOrDefault().measLocalCoordinateZ))),
                                Convert.ToDateTime(measurements.Where(labme => labme.measIdPk == iso.islabmeIdFk).Select(labme => labme.measDate).FirstOrDefault()),
                                samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == iso.islabmeIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaName.Compile()).First()
                            )).Where(x => x.Item1 != 0).ToList();

                        }
                        else if (laboratoryMeasurement.measParameter == "Bulk density")
                        {
                            values = new ApirsRepository<tblBulkDensity>()
                                .GetModelByExpression(bd => b.Contains(bd.bdIdFk))
                                .Where(x => (filter != "" ? (string)x.GetType().GetProperty(filterProperty).GetValue(x, null) == filter : 0 == 0))
                                .Select(bd => new Tuple<double, double, double, double, DateTime, string>(
                            Convert.ToDouble(bd.GetType().GetProperty(property).GetValue(bd, null)),
                            Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == bd.bdIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaX.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == bd.bdIdFk).FirstOrDefault().measLocalCoordinateX))),
                            Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == bd.bdIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaY.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == bd.bdIdFk).FirstOrDefault().measLocalCoordinateY))),
                            Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == bd.bdIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaZ.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == bd.bdIdFk).FirstOrDefault().measLocalCoordinateZ))),
                            Convert.ToDateTime(measurements.Where(labme => labme.measIdPk == bd.bdIdFk).Select(labme => labme.measDate).FirstOrDefault()),
                            samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == bd.bdIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaName.Compile()).First()
                            )).Where(x => x.Item1 != 0).ToList();
                        }
                        else if (laboratoryMeasurement.measParameter == "Porosity")
                        {
                            values = new ApirsRepository<tblEffectivePorosity>()
                                .GetModelByExpression(por => b.Contains(por.porIdFk))
                                .Where(x => (filter != "" ? (string)x.GetType().GetProperty(filterProperty).GetValue(x, null) == filter : 0 == 0))
                                .Select(por => new Tuple<double, double, double, double, DateTime, string>(
                        Convert.ToDouble(por.GetType().GetProperty(property).GetValue(por, null)),
                            Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == por.porIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaX.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == por.porIdFk).FirstOrDefault().measLocalCoordinateX))),
                            Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == por.porIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaY.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == por.porIdFk).FirstOrDefault().measLocalCoordinateY))),
                            Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == por.porIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaZ.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == por.porIdFk).FirstOrDefault().measLocalCoordinateZ))),
                            Convert.ToDateTime(measurements.Where(labme => labme.measIdPk == por.porIdFk).Select(labme => labme.measDate).FirstOrDefault()),
                            samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == por.porIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaName.Compile()).First()
                            )).Where(x => x.Item1 != 0).ToList();
                        }
                        else if (laboratoryMeasurement.measParameter == "Hydraulic head")
                        {
                            values = new ApirsRepository<tblHydraulicHead>()
                                .GetModelByExpression(hh => b.Contains(hh.hhmeasIdFk))
                                .Where(x => (filter != "" ? (string)x.GetType().GetProperty(filterProperty).GetValue(x, null) == filter : 0 == 0))
                                .Select(hh => new Tuple<double, double, double, double, DateTime, string>(
                        Convert.ToDouble(hh.GetType().GetProperty(property).GetValue(hh, null)),
                            Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == hh.hhmeasIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaX.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == hh.hhmeasIdFk).FirstOrDefault().measLocalCoordinateX))),
                            Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == hh.hhmeasIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaY.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == hh.hhmeasIdFk).FirstOrDefault().measLocalCoordinateY))),
                            Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == hh.hhmeasIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaZ.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == hh.hhmeasIdFk).FirstOrDefault().measLocalCoordinateZ))),
                            Convert.ToDateTime(measurements.Where(labme => labme.measIdPk == hh.hhmeasIdFk).Select(labme => labme.measDate).FirstOrDefault()),
                            samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == hh.hhmeasIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaName.Compile()).First()
                            )).Where(x => x.Item1 != 0).ToList();
                        }
                        else if (laboratoryMeasurement.measParameter == "Grain density")
                        {
                            values = new ApirsRepository<tblGrainDensity>()
                                .GetModelByExpression(gd => b.Contains(gd.gdIdFk))
                                .Where(x => (filter != "" ? (string)x.GetType().GetProperty(filterProperty).GetValue(x, null) == filter : 0 == 0))
                                .Select(gd => new Tuple<double, double, double, double, DateTime, string>(
                            Convert.ToDouble(gd.GetType().GetProperty(property).GetValue(gd, null)),
                            Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == gd.gdIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaX.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == gd.gdIdFk).FirstOrDefault().measLocalCoordinateX))),
                            Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == gd.gdIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaY.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == gd.gdIdFk).FirstOrDefault().measLocalCoordinateY))),
                            Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == gd.gdIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaZ.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == gd.gdIdFk).FirstOrDefault().measLocalCoordinateZ))),
                            Convert.ToDateTime(measurements.Where(labme => labme.measIdPk == gd.gdIdFk).Select(labme => labme.measDate).FirstOrDefault()),
                            samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == gd.gdIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaName.Compile()).First()
                            )).Where(x => x.Item1 != 0).ToList();
                        }
                        else if (laboratoryMeasurement.measParameter == "Thermal conductivity")
                        {
                            values = new ApirsRepository<tblThermalConductivity>()
                                .GetModelByExpression(tc => b.Contains(tc.tcIdFk))
                                .Where(x => (filter != "" ? (string)x.GetType().GetProperty(filterProperty).GetValue(x, null) == filter : 0 == 0))
                                .Select(tc => new Tuple<double, double, double, double, DateTime, string>(
                            Convert.ToDouble(tc.GetType().GetProperty(property).GetValue(tc, null)),
                            Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == tc.tcIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaX.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == tc.tcIdFk).FirstOrDefault().measLocalCoordinateX))),
                            Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == tc.tcIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaY.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == tc.tcIdFk).FirstOrDefault().measLocalCoordinateY))),
                            Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == tc.tcIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaZ.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == tc.tcIdFk).FirstOrDefault().measLocalCoordinateZ))),
                            Convert.ToDateTime(measurements.Where(labme => labme.measIdPk == tc.tcIdFk).Select(labme => labme.measDate).FirstOrDefault()),
                            samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == tc.tcIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaName.Compile()).First()
                            )).Where(x => x.Item1 != 0).ToList();
                        }
                        else if (laboratoryMeasurement.measParameter == "Thermal diffusivity")
                        {
                            values = new ApirsRepository<tblThermalDiffusivity>()
                                .GetModelByExpression(td => b.Contains(td.tdIdFk))
                                .Where(x => (filter != "" ? (string)x.GetType().GetProperty(filterProperty).GetValue(x, null) == filter : 0 == 0))
                                .Select(td => new Tuple<double, double, double, double, DateTime, string>(
                            Convert.ToDouble(td.GetType().GetProperty(property).GetValue(td, null)),
                            Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == td.tdIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaX.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == td.tdIdFk).FirstOrDefault().measLocalCoordinateX))),
                            Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == td.tdIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaY.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == td.tdIdFk).FirstOrDefault().measLocalCoordinateY))),
                            Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == td.tdIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaZ.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == td.tdIdFk).FirstOrDefault().measLocalCoordinateZ))),
                            Convert.ToDateTime(measurements.Where(labme => labme.measIdPk == td.tdAvValue).Select(labme => labme.measDate).FirstOrDefault()),
                            samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == td.tdIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaName.Compile()).First()
                            )).Where(x => x.Item1 != 0).ToList();
                        }
                        else if (laboratoryMeasurement.measParameter == "Resistivity")
                        {
                            values = new ApirsRepository<tblResistivity>()
                                .GetModelByExpression(res => b.Contains(res.reslabmeIdFk))
                                .Where(x => (filter != "" ? (string)x.GetType().GetProperty(filterProperty).GetValue(x, null) == filter : 0 == 0))
                                .Select(res => new Tuple<double, double, double, double, DateTime, string>(
                        Convert.ToDouble(res.GetType().GetProperty(property).GetValue(res, null)),
                        Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == res.reslabmeIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaX.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == res.reslabmeIdFk).FirstOrDefault().measLocalCoordinateX))),
                        Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == res.reslabmeIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaY.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == res.reslabmeIdFk).FirstOrDefault().measLocalCoordinateY))),
                        Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == res.reslabmeIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaZ.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == res.reslabmeIdFk).FirstOrDefault().measLocalCoordinateZ))),
                        Convert.ToDateTime(measurements.Where(labme => labme.measIdPk == res.reslabmeIdFk).Select(labme => labme.measDate).FirstOrDefault()),
                        samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == res.reslabmeIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaName.Compile()).First()
                        )).Where(x => x.Item1 != 0).ToList();
                        }
                        else if (laboratoryMeasurement.measParameter == "Sonic wave velocity")
                        {
                            values = new ApirsRepository<tblSonicWave>()
                                .GetModelByExpression(sw => b.Contains(sw.swIdFk))
                                .Where(x => (filter != "" ? (string)x.GetType().GetProperty(filterProperty).GetValue(x, null) == filter : 0 == 0))
                                .Select(sw => new Tuple<double, double, double, double, DateTime, string>(
                        Convert.ToDouble(sw.GetType().GetProperty(property).GetValue(sw, null)),
                        Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == sw.swIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaX.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == sw.swIdFk).FirstOrDefault().measLocalCoordinateX))),
                        Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == sw.swIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaY.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == sw.swIdFk).FirstOrDefault().measLocalCoordinateY))),
                        Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == sw.swIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaZ.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == sw.swIdFk).FirstOrDefault().measLocalCoordinateZ))),
                        Convert.ToDateTime(measurements.Where(labme => labme.measIdPk == sw.swIdFk).Select(labme => labme.measDate).FirstOrDefault()),
                        samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == sw.swIdFk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaName.Compile()).First()
                        )).Where(x => x.Item1 != 0).ToList();
                        }
                        else if (laboratoryMeasurement.measParameter == "X-Ray Fluorescence")
                        {
                            values = new ApirsRepository<tblXRayFluorescenceSpectroscopy>()
                                .GetModelByExpression(xrf => b.Contains(xrf.xrfIdPk))
                                .Where(x => (filter != "" ? (string)x.GetType().GetProperty(filterProperty).GetValue(x, null) == filter : 0 == 0))
                                .Select(xrf => new Tuple<double, double, double, double, DateTime, string>(
                        Convert.ToDouble(xrf.GetType().GetProperty(property).GetValue(xrf, null)),
                        Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == xrf.xrfIdPk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaX.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == xrf.xrfIdPk).FirstOrDefault().measLocalCoordinateX))),
                        Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == xrf.xrfIdPk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaY.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == xrf.xrfIdPk).FirstOrDefault().measLocalCoordinateY))),
                        Convert.ToDouble(samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == xrf.xrfIdPk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaZ.Compile()).First() + (global ? 0 : Convert.ToDouble(measurements.Where(labme => labme.measIdPk == xrf.xrfIdPk).FirstOrDefault().measLocalCoordinateZ))),
                        Convert.ToDateTime(measurements.Where(labme => labme.measIdPk == xrf.xrfIdPk).Select(labme => labme.measDate).FirstOrDefault()),
                        samples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.measIdPk == xrf.xrfIdPk).Select(labme => labme.measRockSampleIdFk).FirstOrDefault()).Select(lambdaName.Compile()).First()
                        )).Where(x => x.Item1 != 0).ToList();
                        }

                        DataTable dat = (DataTable)CollectionHelper.ConvertTo<Tuple<double, double, double, double, DateTime, string>>(values);
                        dat.Columns[0].ColumnName = "Value";
                        dat.Columns[1].ColumnName = "X";
                        dat.Columns[2].ColumnName = "Y";
                        dat.Columns[3].ColumnName = "Z";
                        dat.Columns[4].ColumnName = "Date";
                        dat.Columns[5].ColumnName = "Name";

                        MeasPoints.Add(new Mesh() { Name = group, Data = (DataTable)CollectionHelper.ConvertTo<Tuple<double, double, double, double, DateTime, string>>(values) });
                        allValues.Add(values);
                    }
                    catch
                    {
                        continue;
                    }
                }


            }
            catch
            {
                return null;
            }

            return MeasPoints;
        }

        /// <summary>
        /// Returns a set of laboratory measurements 
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="rockSample"></param>
        /// <param name="laboratoryMeasurement"></param>
        /// <param name="groupClause"></param>
        /// <param name="additionalParameter"></param>
        /// <param name="all"></param>
        /// <returns></returns>
        public List<Mesh> GetLaboratoryPetrophysics(IEnumerable<tblRockSample> samples, tblRockSample rockSample, string groupClause = "", bool all = false)
        {
            #region Variable declaration

            List<Mesh> Petrophysics = new List<Mesh>();

            if (all)
                samples = new List<tblRockSample>(samples.ToList());
            else
                samples = new List<tblRockSample>(samples.Where(samp => samp.sampooiName == rockSample.sampooiName));

            List<string> c = new List<string>();

            IEnumerable<IGrouping<string, tblRockSample>> groups = new List<IGrouping<string, tblRockSample>>();

            var z = System.Linq.Expressions.Expression.Parameter(typeof(tblRockSample), "z");

            #endregion

            try
            {
                switch (groupClause)
                {
                    case "Object of investigation":
                        groups = from x in samples
                                 group x by x.sampooiName into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    case "Petrography":
                        groups = from x in samples
                                 group x by x.sampPetrographicTerm into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    case "Lithostratigraphy":
                        groups = from x in samples
                                 group x by x.sampLithostratigraphyName into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    case "Chronostratigraphy":
                        groups = from x in samples
                                 group x by x.sampChronStratName into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    case "Facies":
                        groups = from x in samples
                                 group x by x.sampFaciesFk into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    case "Architectural element":
                        groups = from x in samples
                                 group x by x.sampArchitecturalElement into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    case "Depositional environment":
                        groups = from x in samples
                                 group x by x.sampDepositionalEnvironment into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    case "Sample type":
                        groups = from x in samples
                                 group x by x.sampType into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    default:
                        break;

                }

                string[] groupString = new string[] { };

                if (groups.Count() > 0)
                    groupString = groups.Select(x => x.Key).ToArray();

                if (groupString.Count() < 1)
                    groupString = new string[] { "All" };

                foreach (var group in groupString)
                {
                    try
                    {
                        switch (groupClause)
                        {
                            case "Object of investigation":
                                c = samples.Where(samp => samp.sampooiName == group)
                                                       .Select(samp => samp.sampLabel)
                                                       .ToList();
                                break;
                            case "Petrography":
                                c = samples.Where(samp => samp.sampPetrographicTerm == group)
                                                       .Select(samp => samp.sampLabel)
                                                       .ToList();
                                break;
                            case "Lithostratigraphy":
                                c = samples.Where(samp => samp.sampLithostratigraphyName == group)
                                                       .Select(samp => samp.sampLabel)
                                                       .ToList();
                                break;
                            case "Chronostratigraphy":
                                c = samples.Where(samp => samp.sampChronStratName == group)
                                                       .Select(samp => samp.sampLabel)
                                                       .ToList();
                                break;
                            case "Facies":
                                c = samples.Where(samp => samp.sampFaciesFk == group)
                                                       .Select(samp => samp.sampLabel)
                                                       .ToList();
                                break;
                            case "Architectural element":
                                c = samples.Where(samp => samp.sampArchitecturalElement == group)
                                                       .Select(samp => samp.sampLabel)
                                                       .ToList();
                                break;
                            case "Depositional environment":
                                c = samples.Where(samp => samp.sampDepositionalEnvironment == group)
                                                       .Select(samp => samp.sampLabel)
                                                       .ToList();
                                break;
                            case "Sample type":
                                c = samples.Where(samp => samp.sampType == group)
                                               .Select(samp => samp.sampLabel)
                                               .ToList();
                                break;
                            default:
                                c = samples.Select(samp => samp.sampLabel)
                                           .ToList();
                                break;

                        }

                        List<v_PetrophysicsRockSamples> pet = new List<v_PetrophysicsRockSamples>();

                        if (!(bool)((ShellViewModel)IoC.Get<IShell>()).LocalMode)
                            pet = new ApirsRepository<v_PetrophysicsRockSamples>()
                                           .GetModelByExpression(pets => c.Contains(pets.labmeSampleName)).ToList();
                        else
                            pet = new ApirsRepository<v_PetrophysicsRockSamples>().GetPetrophysicsFromLocalDB(c).ToList();

                        DataTable dat = CollectionHelper.ConvertTo<v_PetrophysicsRockSamples>(pet);

                        CollectionHelper.RemoveNanRowsAndColumns(dat);

                        Petrophysics.Add(new Mesh() { Name = group, Data = dat });
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            catch
            {
                return null;
            }

            return Petrophysics;
        }

        public List<Mesh> GetFieldMeasurementPoints(IEnumerable<tblMeasurement> fieldMeasurements,
            tblMeasurement selectedFieldMeasurement,
            IEnumerable<tblObjectOfInvestigation> oois,
            string groupClause,
            string property = "",
            bool all = false,
            bool global = false,
            DateTime? from = null,
            DateTime? to = null,
            string filterProperty = "",
            string filter = "")
        {
            #region Variable declaration

            List<Mesh> MeasPoints = new List<Mesh>();


            List<int> c = new List<int>();

            if (!all)
            {
                fieldMeasurements = new List<tblMeasurement>(fieldMeasurements.ToList());
                fieldMeasurements.Where(fime => fime.measObjectOfInvestigationIdFk == selectedFieldMeasurement.measObjectOfInvestigationIdFk).Select(fime => fime.measIdPk).ToList();
            }
            else
            {
                List<string> ooiNames = oois.Select(ooi => ooi.ooiName).ToList();
                fieldMeasurements = new ApirsRepository<tblMeasurement>().GetModelByExpression(meas => ooiNames.Contains(meas.measObjectOfInvestigationIdFk) && meas.measParameter == selectedFieldMeasurement.measParameter)
                                                                             .Where(x => (from != null ? x.measDate >= from : 0 == 0) && (to != null ? x.measDate <= to : 0 == 0))
                                                                             .ToList();
            }

            IEnumerable<IGrouping<string, tblMeasurement>> groups = new List<IGrouping<string, tblMeasurement>>();

            List<Tuple<double, double, double, double, DateTime, string>> values = new List<Tuple<double, double, double, double, DateTime, string>>();
            List<List<Tuple<double, double, double, double, DateTime, string>>> allValues = new List<List<Tuple<double, double, double, double, DateTime, string>>>();

            double minX = 999999999999999999;
            double minY = 999999999999999999;

            #endregion

            try
            {
                switch (groupClause)
                {
                    case "Object of investigation":
                        groups = from x in fieldMeasurements
                                 group x by x.measObjectOfInvestigationIdFk into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;

                    case "Lithostratigraphy":
                        groups = from x in fieldMeasurements
                                 group x by x.measLithostratigraphy into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    case "Chronostratigraphy":
                        groups = from x in fieldMeasurements
                                 group x by x.measChronostratigraphy into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    case "Facies":
                        groups = from x in fieldMeasurements
                                 group x by x.measFacies into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    case "Architectural element":
                        groups = from x in fieldMeasurements
                                 group x by x.measArchitecturalElement into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    case "Measuring device":
                        groups = from x in fieldMeasurements
                                 group x by x.measDevice into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    default:
                        break;

                }

                string[] groupString = new string[] { };

                if (groups.Count() > 0)
                    groupString = groups.Select(x => x.Key).ToArray();

                if (groupString.Count() < 1)
                    groupString = new string[] { "All" };

                foreach (var group in groupString)
                {
                    try
                    {
                        switch (groupClause)
                        {
                            case "Object of investigation":
                                c = fieldMeasurements.Where(samp => samp.measObjectOfInvestigationIdFk == group)
                                                       .Select(samp => samp.measIdPk)
                                                       .ToList();
                                break;
                            case "Lithostratigraphy":
                                c = fieldMeasurements.Where(samp => samp.measLithostratigraphy == group)
                                                       .Select(samp => samp.measIdPk)
                                                       .ToList();
                                break;
                            case "Chronostratigraphy":
                                c = fieldMeasurements.Where(samp => samp.measChronostratigraphy == group)
                                                       .Select(samp => samp.measIdPk)
                                                       .ToList();
                                break;
                            case "Facies":
                                c = fieldMeasurements.Where(samp => samp.measFacies == group)
                                                       .Select(samp => samp.measIdPk)
                                                       .ToList();
                                break;
                            case "Architectural element":
                                c = fieldMeasurements.Where(samp => samp.measArchitecturalElement == group)
                                                       .Select(samp => samp.measIdPk)
                                                       .ToList();
                                break;
                            case "Measuring device":
                                break;
                            default:
                                c = fieldMeasurements
                                               .Select(samp => samp.measIdPk)
                                               .ToList();
                                break;

                        }

                        #region Defining lambda properties

                        var z = System.Linq.Expressions.Expression.Parameter(typeof(tblMeasurement), "z");
                        List<string> prop = new List<string>();

                        if (global)
                        {
                            prop.Add("measLongitude");
                            prop.Add("measLatitude");
                            prop.Add("measElevation");
                        }
                        else
                        {
                            prop.Add("measLocalCoordinateX");
                            prop.Add("measLocalCoordinateY");
                            prop.Add("measLocalCoordinateZ");
                        }


                        prop.Add("measObjectOfInvestigationIdFk");

                        var body = System.Linq.Expressions.Expression.PropertyOrField(z, prop[0]);
                        var lambdaX = System.Linq.Expressions.Expression.Lambda<Func<tblMeasurement, double?>>(body, z);

                        body = System.Linq.Expressions.Expression.PropertyOrField(z, prop[1]);
                        var lambdaY = System.Linq.Expressions.Expression.Lambda<Func<tblMeasurement, double?>>(body, z);

                        body = System.Linq.Expressions.Expression.PropertyOrField(z, prop[2]);
                        var lambdaZ = System.Linq.Expressions.Expression.Lambda<Func<tblMeasurement, double?>>(body, z);

                        body = System.Linq.Expressions.Expression.PropertyOrField(z, prop[3]);
                        var lambdaName = System.Linq.Expressions.Expression.Lambda<Func<tblMeasurement, string>>(body, z);

                        #endregion

                        if (selectedFieldMeasurement.measParameter == "Total Gamma Ray")
                        {
                            values = new ApirsRepository<tblTotalGammaRay>()
                                .GetModelByExpression(t => c.Contains(t.tgrfimeIdPk))
                                .Where(x => (filter != "" ? (string)x.GetType().GetProperty(filterProperty).GetValue(x, null) == filter : 0 == 0))
                                .Select(t => new Tuple<double, double, double, double, DateTime, string>(
                            Convert.ToDouble(t.GetType().GetProperty(property).GetValue(t, null)),
                            Convert.ToDouble(fieldMeasurements.Where(meas => meas.measIdPk == t.tgrfimeIdPk).Select(lambdaX.Compile()).First() + (global ? 0 : fieldMeasurements.Where(meas => meas.measIdPk == t.tgrfimeIdPk).First().measLocalCoordinateX)),
                            Convert.ToDouble(fieldMeasurements.Where(meas => meas.measIdPk == t.tgrfimeIdPk).Select(lambdaY.Compile()).First() + (global ? 0 : fieldMeasurements.Where(meas => meas.measIdPk == t.tgrfimeIdPk).First().measLocalCoordinateY)),
                            Convert.ToDouble(fieldMeasurements.Where(meas => meas.measIdPk == t.tgrfimeIdPk).Select(lambdaZ.Compile()).First() + (global ? 0 : fieldMeasurements.Where(meas => meas.measIdPk == t.tgrfimeIdPk).First().measLocalCoordinateZ)),
                            Convert.ToDateTime(fieldMeasurements.Where(labme => labme.measIdPk == t.tgrfimeIdPk).Select(labme => labme.measDate).FirstOrDefault()),
                            fieldMeasurements.Where(labme => labme.measIdPk == t.tgrfimeIdPk).Select(labme => labme.measIdPk.ToString()).FirstOrDefault()
                        )).Where(x => x.Item1 != 0).ToList();
                        }
                        else if (selectedFieldMeasurement.measParameter == "Magnetic Susceptibility")
                        {
                            values = new ApirsRepository<tblSusceptibility>()
                                .GetModelByExpression(t => c.Contains(t.susfimeIdPk))
                                .Where(x => (filter != "" ? (string)x.GetType().GetProperty(filterProperty).GetValue(x, null) == filter : 0 == 0))
                                .Select(t => new Tuple<double, double, double, double, DateTime, string>(
                            Convert.ToDouble(t.GetType().GetProperty(property).GetValue(t, null)),
                            Convert.ToDouble(fieldMeasurements.Where(meas => meas.measIdPk == t.susfimeIdPk).Select(lambdaX.Compile()).First() + (global ? 0 : fieldMeasurements.Where(meas => meas.measIdPk == t.susfimeIdPk).First().measLocalCoordinateX)),
                            Convert.ToDouble(fieldMeasurements.Where(meas => meas.measIdPk == t.susfimeIdPk).Select(lambdaY.Compile()).First() + (global ? 0 : fieldMeasurements.Where(meas => meas.measIdPk == t.susfimeIdPk).First().measLocalCoordinateY)),
                            Convert.ToDouble(fieldMeasurements.Where(meas => meas.measIdPk == t.susfimeIdPk).Select(lambdaZ.Compile()).First() + (global ? 0 : fieldMeasurements.Where(meas => meas.measIdPk == t.susfimeIdPk).First().measLocalCoordinateZ)),
                            Convert.ToDateTime(fieldMeasurements.Where(labme => labme.measIdPk == t.susfimeIdPk).Select(labme => labme.measDate).FirstOrDefault()),
                            fieldMeasurements.Where(labme => labme.measIdPk == t.susfimeIdPk).Select(labme => labme.measIdPk.ToString()).FirstOrDefault()
                        )).Where(x => x.Item1 != 0).ToList();
                        }
                        else if (selectedFieldMeasurement.measParameter == "Sonic Log")
                        {
                            values = new ApirsRepository<tblSonicLog>()
                                .GetModelByExpression(t => c.Contains(t.slfimeIdFk))
                                .Where(x => (filter != "" ? (string)x.GetType().GetProperty(filterProperty).GetValue(x, null) == filter : 0 == 0))
                                .Select(t => new Tuple<double, double, double, double, DateTime, string>(
                            Convert.ToDouble(t.GetType().GetProperty(property).GetValue(t, null)),
                            Convert.ToDouble(fieldMeasurements.Where(meas => meas.measIdPk == t.slfimeIdFk).Select(lambdaX.Compile()).First() + (global ? 0 : fieldMeasurements.Where(meas => meas.measIdPk == t.slfimeIdFk).First().measLocalCoordinateX)),
                            Convert.ToDouble(fieldMeasurements.Where(meas => meas.measIdPk == t.slfimeIdFk).Select(lambdaY.Compile()).First() + (global ? 0 : fieldMeasurements.Where(meas => meas.measIdPk == t.slfimeIdFk).First().measLocalCoordinateY)),
                            Convert.ToDouble(fieldMeasurements.Where(meas => meas.measIdPk == t.slfimeIdFk).Select(lambdaZ.Compile()).First() + (global ? 0 : fieldMeasurements.Where(meas => meas.measIdPk == t.slfimeIdFk).First().measLocalCoordinateZ)),
                            Convert.ToDateTime(fieldMeasurements.Where(labme => labme.measIdPk == t.slfimeIdFk).Select(labme => labme.measDate).FirstOrDefault()),
                            fieldMeasurements.Where(labme => labme.measIdPk == t.slfimeIdFk).Select(labme => labme.measIdPk.ToString()).FirstOrDefault()
                        )).Where(x => x.Item1 != 0).ToList();
                        }
                        else if (selectedFieldMeasurement.measParameter == "Hydraulic head")
                        {
                            values = new ApirsRepository<tblHydraulicHead>()
                                .GetModelByExpression(t => c.Contains(t.hhmeasIdFk))
                                .Where(x => (filter != "" ? (string)x.GetType().GetProperty(filterProperty).GetValue(x, null) == filter : 0 == 0))
                                .Select(t => new Tuple<double, double, double, double, DateTime, string>(
                            Convert.ToDouble(t.GetType().GetProperty(property).GetValue(t, null)),
                            Convert.ToDouble(fieldMeasurements.Where(meas => meas.measIdPk == t.hhmeasIdFk).Select(lambdaX.Compile()).First() + (global ? 0 : fieldMeasurements.Where(meas => meas.measIdPk == t.hhmeasIdFk).First().measLocalCoordinateX)),
                            Convert.ToDouble(fieldMeasurements.Where(meas => meas.measIdPk == t.hhmeasIdFk).Select(lambdaY.Compile()).First() + (global ? 0 : fieldMeasurements.Where(meas => meas.measIdPk == t.hhmeasIdFk).First().measLocalCoordinateY)),
                            Convert.ToDouble(fieldMeasurements.Where(meas => meas.measIdPk == t.hhmeasIdFk).Select(lambdaZ.Compile()).First() + (global ? 0 : fieldMeasurements.Where(meas => meas.measIdPk == t.hhmeasIdFk).First().measLocalCoordinateZ)),
                            Convert.ToDateTime(fieldMeasurements.Where(labme => labme.measIdPk == t.hhmeasIdFk).Select(labme => labme.measDate).FirstOrDefault()),
                            fieldMeasurements.Where(labme => labme.measIdPk == t.hhmeasIdFk).Select(labme => labme.measIdPk.ToString()).FirstOrDefault()
                        )).Where(x => x.Item1 != 0).ToList();
                        }
                        else if (selectedFieldMeasurement.measParameter == "Rock Quality Designation Index")
                        {
                            values = new ApirsRepository<tblRockQualityDesignationIndex>()
                                .GetModelByExpression(t => c.Contains(t.rqdfimeIdFk))
                                .Where(x => (filter != "" ? (string)x.GetType().GetProperty(filterProperty).GetValue(x, null) == filter : 0 == 0))
                                .Select(t => new Tuple<double, double, double, double, DateTime, string>(
                            Convert.ToDouble(t.GetType().GetProperty(property).GetValue(t, null)),
                            Convert.ToDouble(fieldMeasurements.Where(meas => meas.measIdPk == t.rqdfimeIdFk).Select(lambdaX.Compile()).First() + (global ? 0 : fieldMeasurements.Where(meas => meas.measIdPk == t.rqdfimeIdFk).First().measLocalCoordinateX)),
                            Convert.ToDouble(fieldMeasurements.Where(meas => meas.measIdPk == t.rqdfimeIdFk).Select(lambdaY.Compile()).First() + (global ? 0 : fieldMeasurements.Where(meas => meas.measIdPk == t.rqdfimeIdFk).First().measLocalCoordinateY)),
                            Convert.ToDouble(fieldMeasurements.Where(meas => meas.measIdPk == t.rqdfimeIdFk).Select(lambdaZ.Compile()).First() + (global ? 0 : fieldMeasurements.Where(meas => meas.measIdPk == t.rqdfimeIdFk).First().measLocalCoordinateZ)),
                            Convert.ToDateTime(fieldMeasurements.Where(labme => labme.measIdPk == t.rqdfimeIdFk).Select(labme => labme.measDate).FirstOrDefault()),
                            fieldMeasurements.Where(labme => labme.measIdPk == t.rqdfimeIdFk).Select(labme => labme.measIdPk.ToString()).FirstOrDefault()
                        )).Where(x => x.Item1 != 0).ToList();
                        }
                        else if (selectedFieldMeasurement.measParameter == "Temperature")
                        {
                            values = new ApirsRepository<tblBoreholeTemperature>()
                                .GetModelByExpression(t => c.Contains(t.btfimeIdFk))
                                .Where(x => (filter != "" ? (string)x.GetType().GetProperty(filterProperty).GetValue(x, null) == filter : 0 == 0))
                                .Select(t => new Tuple<double, double, double, double, DateTime, string>(
                            Convert.ToDouble(t.GetType().GetProperty(property).GetValue(t, null)),
                            Convert.ToDouble(fieldMeasurements.Where(meas => meas.measIdPk == t.btfimeIdFk).Select(lambdaX.Compile()).First() + (global ? 0 : fieldMeasurements.Where(meas => meas.measIdPk == t.btfimeIdFk).First().measLocalCoordinateX)),
                            Convert.ToDouble(fieldMeasurements.Where(meas => meas.measIdPk == t.btfimeIdFk).Select(lambdaY.Compile()).First() + (global ? 0 : fieldMeasurements.Where(meas => meas.measIdPk == t.btfimeIdFk).First().measLocalCoordinateY)),
                            Convert.ToDouble(fieldMeasurements.Where(meas => meas.measIdPk == t.btfimeIdFk).Select(lambdaZ.Compile()).First() + (global ? 0 : fieldMeasurements.Where(meas => meas.measIdPk == t.btfimeIdFk).First().measLocalCoordinateZ)),
                            Convert.ToDateTime(fieldMeasurements.Where(labme => labme.measIdPk == t.btfimeIdFk).Select(labme => labme.measDate).FirstOrDefault()),
                            fieldMeasurements.Where(labme => labme.measIdPk == t.btfimeIdFk).Select(labme => labme.measIdPk.ToString()).FirstOrDefault()
                        )).Where(x => x.Item1 != 0).ToList();
                        }
                        else if (selectedFieldMeasurement.measParameter == "Spectral Gamma Ray")
                            values = new ApirsRepository<tblSpectralGammaRay>()
                                .GetModelByExpression(t => c.Contains(t.sgrIdPk))
                                .Where(x => (filter != "" ? (string)x.GetType().GetProperty(filterProperty).GetValue(x, null) == filter : 0 == 0))
                                .Select(t => new Tuple<double, double, double, double, DateTime, string>(
                            Convert.ToDouble(t.GetType().GetProperty(property).GetValue(t, null)),
                            Convert.ToDouble(fieldMeasurements.Where(meas => meas.measIdPk == t.sgrIdPk).Select(lambdaX.Compile()).First() + (global ? 0 : fieldMeasurements.Where(meas => meas.measIdPk == t.sgrIdPk).First().measLocalCoordinateX)),
                            Convert.ToDouble(fieldMeasurements.Where(meas => meas.measIdPk == t.sgrIdPk).Select(lambdaY.Compile()).First() + (global ? 0 : fieldMeasurements.Where(meas => meas.measIdPk == t.sgrIdPk).First().measLocalCoordinateY)),
                            Convert.ToDouble(fieldMeasurements.Where(meas => meas.measIdPk == t.sgrIdPk).Select(lambdaZ.Compile()).First() + (global ? 0 : fieldMeasurements.Where(meas => meas.measIdPk == t.sgrIdPk).First().measLocalCoordinateZ)),
                            Convert.ToDateTime(fieldMeasurements.Where(labme => labme.measIdPk == t.sgrIdPk).Select(labme => labme.measDate).FirstOrDefault()),
                            fieldMeasurements.Where(labme => labme.measIdPk == t.sgrIdPk).Select(labme => labme.measIdPk.ToString()).FirstOrDefault()
                        )).Where(x => x.Item1 != 0).ToList();

                        MeasPoints.Add(new Mesh() { Name = group, Data = CollectionHelper.ConvertTo<Tuple<double, double, double, double, DateTime, string>>(values) });
                        allValues.Add(values);

                    }
                    catch
                    {
                        continue;
                    }
                }

            }
            catch
            {
                return null;
            }

            return MeasPoints;
        }

        /// <summary>
        /// Getting a list of field measurements
        /// </summary>
        /// <param name="fieldMeasurements"></param>
        /// <param name="selectedFieldMeasurement"></param>
        /// <param name="groupClause"></param>
        /// <param name="all"></param>
        /// <returns></returns>
        public List<Mesh> GetFieldPetrophysics(IEnumerable<tblMeasurement> fieldMeasurements, tblMeasurement selectedFieldMeasurement, string groupClause, bool all = false)
        {
            #region Variable declaration

            List<Mesh> Petrophysics = new List<Mesh>();

            fieldMeasurements = new List<tblMeasurement>(fieldMeasurements.ToList());

            List<tblMeasurement> c = new List<tblMeasurement>();

            IEnumerable<IGrouping<string, tblMeasurement>> groups = new List<IGrouping<string, tblMeasurement>>();

            var z = System.Linq.Expressions.Expression.Parameter(typeof(tblMeasurement), "z");

            #endregion

            try
            {
                switch (groupClause)
                {
                    case "Object of investigation":
                        groups = from x in fieldMeasurements
                                 group x by x.measObjectOfInvestigationIdFk into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;

                    case "Lithostratigraphy":
                        groups = from x in fieldMeasurements
                                 group x by x.measLithostratigraphy into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    case "Chronostratigraphy":
                        groups = from x in fieldMeasurements
                                 group x by x.measChronostratigraphy into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    case "Facies":
                        groups = from x in fieldMeasurements
                                 group x by x.measFacies into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    case "Architectural element":
                        groups = from x in fieldMeasurements
                                 group x by x.measArchitecturalElement into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    case "Measuring device":
                        groups = from x in fieldMeasurements
                                 group x by x.measDevice into newGroup
                                 orderby newGroup.Key
                                 select newGroup;
                        break;
                    default:
                        break;

                }

                string[] groupString = new string[] { };

                if (groups.Count() > 0)
                    groupString = groups.Select(x => x.Key).ToArray();

                if (groupString.Count() < 1)
                    groupString = new string[] { "All" };

                foreach (var group in groupString)
                {
                    try
                    {
                        switch (groupClause)
                        {
                            case "Object of investigation":
                                c = fieldMeasurements.Where(samp => samp.measObjectOfInvestigationIdFk == group)
                                                       .ToList();
                                break;
                            case "Lithostratigraphy":
                                c = fieldMeasurements.Where(samp => samp.measLithostratigraphy == group)
                                                       .ToList();
                                break;
                            case "Chronostratigraphy":
                                c = fieldMeasurements.Where(samp => samp.measChronostratigraphy == group)
                                                       .ToList();
                                break;
                            case "Facies":
                                c = fieldMeasurements.Where(samp => samp.measFacies == group)
                                                       .ToList();
                                break;
                            case "Architectural element":
                                c = fieldMeasurements.Where(samp => samp.measArchitecturalElement == group)
                                                       .ToList();
                                break;
                            case "Measuring device":
                                break;
                            default:
                                c = fieldMeasurements.ToList();
                                break;

                        }


                        List<double> c_localx = c.Select(x => Convert.ToDouble(x.measLocalCoordinateX)).Distinct().ToList();
                        List<double> c_localy = c.Select(x => Convert.ToDouble(x.measLocalCoordinateY)).Distinct().ToList();
                        List<double> c_localz = c.Select(x => Convert.ToDouble(x.measLocalCoordinateZ)).Distinct().ToList();
                        List<int> c_project = c.Select(x => Convert.ToInt32(x.measprjIdFk)).Distinct().ToList();
                        string ooi = c.First().measObjectOfInvestigationIdFk.ToString();

                        List<v_PetrophysicsFieldMeasurements> pet = new List<v_PetrophysicsFieldMeasurements>();

                        if (!(bool)((ShellViewModel)IoC.Get<IShell>()).LocalMode)
                            pet = new ApirsRepository<v_PetrophysicsFieldMeasurements>()
                                        .GetModelByExpression(pets => c_localx.Contains((double)pets.Local_x)
                                                                      && c_localy.Contains((double)pets.Local_y)
                                                                      && c_localz.Contains((double)pets.Local_z)
                                                                      && c_project.Contains((int)pets.Project_ID)
                                                                      && pets.Object_of_investigation == ooi).ToList();

                        DataTable dat = CollectionHelper.ConvertTo<v_PetrophysicsFieldMeasurements>(pet);

                        Petrophysics.Add(new Mesh() { Name = group, Data = dat });
                    }
                    catch
                    {
                        continue;
                    }
                }

            }
            catch
            {
                return null;
            }

            return Petrophysics;
        }

        /// <summary>
        /// A local query for petrophysical measurements
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public IEnumerable<v_PetrophysicsRockSamples> GetPetrophysicsFromLocalDB(IEnumerable<string> c)
        {
            List<tblMeasurement> labs = _apirsLocalLiteDatabase.GetCollection<tblMeasurement>().Find(x => c.Contains(x.measRockSampleIdFk)).ToList();

            return (from rs in _apirsLocalLiteDatabase.GetCollection<tblRockSample>(typeof(tblRockSample).Name).Find(x => c.Contains(x.sampLabel))
                    select new v_PetrophysicsRockSamples()
                    {
                        Local_x = Convert.ToDouble(rs.sampLocalXCoordinates),
                        Local_y = Convert.ToDouble(rs.sampLocalYCoordinates),
                        Local_z = Convert.ToDouble(rs.sampLocalZCoordinates),
                        labmeSampleName = rs.sampLabel,
                        Sample_type = rs.sampType,
                        Petrography = rs.sampPetrographicTerm,
                        Chronostratigraphy = rs.sampChronStratName,
                        Lithofacies = rs.sampFaciesFk,
                        Architectural_element = rs.sampArchitecturalElement,
                        Depositional_environment = rs.sampDepositionalEnvironment,
                        Lithostratigraphy = rs.sampLithostratigraphyName,
                        Latitude = rs.sampLatitude,
                        Longitude = rs.sampLongitude,
                        Object_of_investigation = rs.sampooiName,
                        Project_ID = rs.sampprjIdFk,
                        Apparent_permeability = _apirsLocalLiteDatabase.GetCollection<tblApparentPermeability>(typeof(tblApparentPermeability).Name).Find(x => labs.Where(y => y.measParameter == "Apparent permeability" && y.measRockSampleIdFk == rs.sampLabel).Select(z => z.measIdPk).Contains(x.apermIdFk)).Select(x => x.apermValueM2).ToList().Average(),
                        Intrinsic_permeability = _apirsLocalLiteDatabase.GetCollection<tblIntrinsicPermeability>(typeof(tblIntrinsicPermeability).Name).Find(x => labs.Where(y => y.measParameter == "Intrinsic permeability" && y.measRockSampleIdFk == rs.sampLabel).Select(z => z.measIdPk).Contains(x.inpeIdFk)).Select(x => x.inpeValuem2).ToList().Average(),
                        Grain_density = _apirsLocalLiteDatabase.GetCollection<tblGrainDensity>(typeof(tblGrainDensity).Name).Find(x => labs.Where(y => y.measParameter == "Grain density" && y.measRockSampleIdFk == rs.sampLabel).Select(z => z.measIdPk).Contains(x.gdIdFk)).Select(x => x.gdMeanDensity).ToList().Average(),
                        Bulk_density = _apirsLocalLiteDatabase.GetCollection<tblBulkDensity>(typeof(tblBulkDensity).Name).Find(x => labs.Where(y => y.measParameter == "Bulk density" && y.measRockSampleIdFk == rs.sampLabel).Select(z => z.measIdPk).Contains(x.bdIdFk)).Select(x => x.bdValue).ToList().Average(),
                        Porosity = _apirsLocalLiteDatabase.GetCollection<tblEffectivePorosity>(typeof(tblEffectivePorosity).Name).Find(x => labs.Where(y => y.measParameter == "Porosity" && y.measRockSampleIdFk == rs.sampLabel).Select(z => z.measIdPk).Contains(x.porIdFk)).Select(x => x.porValuePerc).ToList().Average(),
                        Thermal_conductivity = _apirsLocalLiteDatabase.GetCollection<tblThermalConductivity>(typeof(tblThermalConductivity).Name).Find(x => labs.Where(y => y.measParameter == "Thermal conductivity" && y.measRockSampleIdFk == rs.sampLabel).Select(z => z.measIdPk).Contains(x.tcIdFk)).Select(x => x.tcAvValue).ToList().Average(),
                        Thermal_diffusivity = _apirsLocalLiteDatabase.GetCollection<tblThermalDiffusivity>(typeof(tblThermalDiffusivity).Name).Find(x => labs.Where(y => y.measParameter == "Thermal diffusivity" && y.measRockSampleIdFk == rs.sampLabel).Select(z => z.measIdPk).Contains(x.tdIdFk)).Select(x => x.tdAvValue).ToList().Average(),
                    });
        }

        //Getting all lithostratigraphic units in a union query
        public IEnumerable<LithostratigraphyUnion> GetCompleteLithostratigraphy()
        {
            if (_apirsDatabase != null)
                return (from gr in _apirsDatabase.tblGroups
                        select new LithostratigraphyUnion()
                        {
                            Id = (from strat in _apirsDatabase.tblUnionLithostratigraphies where strat.grName == gr.grName select strat.ID).FirstOrDefault(),
                            ParentName = "",
                            UploaderId = (from strat in _apirsDatabase.tblUnionLithostratigraphies where strat.grName == gr.grName select (int)strat.unionLithUploaderIdFk).FirstOrDefault(),
                            Chronostratigraphy = (from strat in _apirsDatabase.tblUnionLithostratigraphies where strat.grName == gr.grName select (string)strat.chronostratNameFk).FirstOrDefault(),
                            grName = gr.grName,
                            Hierarchy = "Group",
                            LithologicDescriptionShort = gr.grLithologicDescriptionShort,
                            TopBoundary = gr.grTopBoundary,
                            BaseBoundary = gr.grBaseBoundary,
                            TypeLocality = gr.grTypeLocality,
                            DateDocumentation = gr.grDateDocumentation,
                            Countries = gr.grCountries,
                            States = gr.grStates,
                            Literature = gr.grLiterature,
                            MaxThickness = gr.grMaxThickness,
                            MeanThickness = gr.grMeanThickness,
                            Notes = gr.grNotes
                        }).Concat(from sg in _apirsDatabase.tblSubgroups
                                  select new LithostratigraphyUnion()
                                  {
                                      Id = (from strat in _apirsDatabase.tblUnionLithostratigraphies where strat.grName == sg.sgName select strat.ID).FirstOrDefault(),
                                      ParentName = (from strat in _apirsDatabase.tblGroups where strat.grIdPk == sg.sggrIdFk select strat.grName).FirstOrDefault(),
                                      UploaderId = (from strat in _apirsDatabase.tblUnionLithostratigraphies where strat.grName == sg.sgName select (int)strat.unionLithUploaderIdFk).FirstOrDefault(),
                                      Chronostratigraphy = (from strat in _apirsDatabase.tblUnionLithostratigraphies where strat.grName == sg.sgName select (string)strat.chronostratNameFk).FirstOrDefault(),
                                      grName = sg.sgName,
                                      Hierarchy = "Subgroup",
                                      LithologicDescriptionShort = sg.sgLithologicDescriptionShort,
                                      TopBoundary = sg.sgTopBoundary,
                                      BaseBoundary = sg.sgBaseBoundary,
                                      TypeLocality = sg.sgTypeLocality,
                                      DateDocumentation = sg.sgDateOfDocumentation,
                                      Countries = sg.sgCountries,
                                      States = sg.sgStates,
                                      Literature = sg.sgLiterature,
                                      MaxThickness = sg.sgMaxThickness,
                                      MeanThickness = sg.sgMeanThickness,
                                      Notes = sg.sgNotes
                                  }).Concat(from fm in _apirsDatabase.tblFormations
                                            select new LithostratigraphyUnion()
                                            {
                                                Id = (from strat in _apirsDatabase.tblUnionLithostratigraphies where strat.grName == fm.fmName select strat.ID).FirstOrDefault(),
                                                ParentName = (from strat in _apirsDatabase.tblSubgroups where strat.sgIdPk == fm.fmsgIdFk select strat.sgName).FirstOrDefault(),
                                                UploaderId = (from strat in _apirsDatabase.tblUnionLithostratigraphies where strat.grName == fm.fmName select (int)strat.unionLithUploaderIdFk).FirstOrDefault(),
                                                Chronostratigraphy = (from strat in _apirsDatabase.tblUnionLithostratigraphies where strat.grName == fm.fmName select (string)strat.chronostratNameFk).FirstOrDefault(),
                                                grName = fm.fmName,
                                                Hierarchy = "Formation",
                                                LithologicDescriptionShort = fm.fmDescription,
                                                TopBoundary = fm.fmTopBoundary,
                                                BaseBoundary = fm.fmBaseBoundary,
                                                TypeLocality = fm.fmTypeLocality,
                                                DateDocumentation = fm.fmDateOfDocumentation,
                                                Countries = fm.fmCountries,
                                                States = fm.fmStates,
                                                Literature = fm.fmLiterature,
                                                MaxThickness = fm.fmMaxThickness,
                                                MeanThickness = fm.fmMeanThickness,
                                                Notes = fm.fmNotes
                                            }).Concat(from sf in _apirsDatabase.tblSubformations
                                                      select new LithostratigraphyUnion()
                                                      {
                                                          Id = (from strat in _apirsDatabase.tblUnionLithostratigraphies where strat.grName == sf.sfName select strat.ID).FirstOrDefault(),
                                                          ParentName = (from strat in _apirsDatabase.tblFormations where strat.fmIdPk == sf.sffmId select strat.fmName).FirstOrDefault(),
                                                          UploaderId = (from strat in _apirsDatabase.tblUnionLithostratigraphies where strat.grName == sf.sfName select (int)strat.unionLithUploaderIdFk).FirstOrDefault(),
                                                          Chronostratigraphy = (from strat in _apirsDatabase.tblUnionLithostratigraphies where strat.grName == sf.sfName select (string)strat.chronostratNameFk).FirstOrDefault(),
                                                          grName = sf.sfName,
                                                          Hierarchy = "Subformation",
                                                          LithologicDescriptionShort = sf.sfTypeLocality,
                                                          TopBoundary = sf.sfTopBoundary,
                                                          BaseBoundary = sf.sfBaseBoundary,
                                                          TypeLocality = sf.sfDescription,
                                                          DateDocumentation = sf.sfDateOfDocumentation,
                                                          Countries = sf.sfCountries,
                                                          States = sf.sfStates,
                                                          Literature = sf.sfLiterature,
                                                          MaxThickness = sf.sfMaxThickness,
                                                          MeanThickness = sf.sfMeanThickness,
                                                          Notes = sf.sfNotes
                                                      });
            else
                return (from gr in _apirsLocalLiteDatabase.GetCollection<tblGroup>(typeof(tblGroup).Name).FindAll()
                        select new LithostratigraphyUnion()
                        {
                            Id = _apirsLocalLiteDatabase.GetCollection<tblUnionLithostratigraphy>(typeof(tblUnionLithostratigraphy).Name).Find(x => x.grName == gr.grName).Select(x => x.ID).FirstOrDefault(),
                            ParentName = "",
                            UploaderId = _apirsLocalLiteDatabase.GetCollection<tblUnionLithostratigraphy>(typeof(tblUnionLithostratigraphy).Name).Find(x => x.grName == gr.grName).Select(x => (int)x.unionLithUploaderIdFk).FirstOrDefault(),
                            Chronostratigraphy = _apirsLocalLiteDatabase.GetCollection<tblUnionLithostratigraphy>(typeof(tblUnionLithostratigraphy).Name).Find(x => x.grName == gr.grName).Select(x => (string)x.chronostratNameFk).FirstOrDefault(),
                            grName = gr.grName,
                            Hierarchy = "Group",
                            LithologicDescriptionShort = gr.grLithologicDescriptionShort,
                            TopBoundary = gr.grTopBoundary,
                            BaseBoundary = gr.grBaseBoundary,
                            TypeLocality = gr.grTypeLocality,
                            DateDocumentation = gr.grDateDocumentation,
                            Countries = gr.grCountries,
                            States = gr.grStates,
                            Literature = gr.grLiterature,
                            MaxThickness = gr.grMaxThickness,
                            MeanThickness = gr.grMeanThickness,
                            Notes = gr.grNotes
                        }).Concat(from sg in _apirsLocalLiteDatabase.GetCollection<tblSubgroup>(typeof(tblSubgroup).Name).FindAll()
                                  select new LithostratigraphyUnion()
                                  {
                                      Id = _apirsLocalLiteDatabase.GetCollection<tblUnionLithostratigraphy>(typeof(tblUnionLithostratigraphy).Name).Find(x => x.grName == sg.sgName).Select(x => x.ID).FirstOrDefault(),
                                      ParentName = _apirsLocalLiteDatabase.GetCollection<tblGroup>(typeof(tblGroup).Name).Find(x => x.grIdPk == sg.sggrIdFk).Select(x => x.grName).FirstOrDefault(),
                                      UploaderId = _apirsLocalLiteDatabase.GetCollection<tblUnionLithostratigraphy>(typeof(tblUnionLithostratigraphy).Name).Find(x => x.grName == sg.sgName).Select(x => (int)x.unionLithUploaderIdFk).FirstOrDefault(),
                                      Chronostratigraphy = _apirsLocalLiteDatabase.GetCollection<tblUnionLithostratigraphy>(typeof(tblUnionLithostratigraphy).Name).Find(x => x.grName == sg.sgName).Select(x => (string)x.chronostratNameFk).FirstOrDefault(),
                                      grName = sg.sgName,
                                      Hierarchy = "Subgroup",
                                      LithologicDescriptionShort = sg.sgLithologicDescriptionShort,
                                      TopBoundary = sg.sgTopBoundary,
                                      BaseBoundary = sg.sgBaseBoundary,
                                      TypeLocality = sg.sgTypeLocality,
                                      DateDocumentation = sg.sgDateOfDocumentation,
                                      Countries = sg.sgCountries,
                                      States = sg.sgStates,
                                      Literature = sg.sgLiterature,
                                      MaxThickness = sg.sgMaxThickness,
                                      MeanThickness = sg.sgMeanThickness,
                                      Notes = sg.sgNotes
                                  }).Concat(from fm in _apirsLocalLiteDatabase.GetCollection<tblFormation>(typeof(tblFormation).Name).FindAll()
                                            select new LithostratigraphyUnion()
                                            {
                                                Id = _apirsLocalLiteDatabase.GetCollection<tblUnionLithostratigraphy>(typeof(tblUnionLithostratigraphy).Name).Find(x => x.grName == fm.fmName).Select(x => x.ID).FirstOrDefault(),
                                                ParentName = _apirsLocalLiteDatabase.GetCollection<tblSubgroup>(typeof(tblSubgroup).Name).Find(x => x.sgIdPk == fm.fmsgIdFk).Select(x => x.sgName).FirstOrDefault(),
                                                UploaderId = _apirsLocalLiteDatabase.GetCollection<tblUnionLithostratigraphy>(typeof(tblUnionLithostratigraphy).Name).Find(x => x.grName == fm.fmName).Select(x => (int)x.unionLithUploaderIdFk).FirstOrDefault(),
                                                Chronostratigraphy = _apirsLocalLiteDatabase.GetCollection<tblUnionLithostratigraphy>(typeof(tblUnionLithostratigraphy).Name).Find(x => x.grName == fm.fmName).Select(x => (string)x.chronostratNameFk).FirstOrDefault(),
                                                grName = fm.fmName,
                                                Hierarchy = "Formation",
                                                LithologicDescriptionShort = fm.fmDescription,
                                                TopBoundary = fm.fmTopBoundary,
                                                BaseBoundary = fm.fmBaseBoundary,
                                                TypeLocality = fm.fmTypeLocality,
                                                DateDocumentation = fm.fmDateOfDocumentation,
                                                Countries = fm.fmCountries,
                                                States = fm.fmStates,
                                                Literature = fm.fmLiterature,
                                                MaxThickness = fm.fmMaxThickness,
                                                MeanThickness = fm.fmMeanThickness,
                                                Notes = fm.fmNotes
                                            }).Concat(from sf in _apirsLocalLiteDatabase.GetCollection<tblSubformation>(typeof(tblSubformation).Name).FindAll()
                                                      select new LithostratigraphyUnion()
                                                      {
                                                          Id = _apirsLocalLiteDatabase.GetCollection<tblUnionLithostratigraphy>(typeof(tblUnionLithostratigraphy).Name).Find(x => x.grName == sf.sfName).Select(x => x.ID).FirstOrDefault(),
                                                          ParentName = _apirsLocalLiteDatabase.GetCollection<tblFormation>(typeof(tblFormation).Name).Find(x => x.fmIdPk == sf.sffmId).Select(x => (string)x.fmName).FirstOrDefault(),
                                                          UploaderId = _apirsLocalLiteDatabase.GetCollection<tblUnionLithostratigraphy>(typeof(tblUnionLithostratigraphy).Name).Find(x => x.grName == sf.sfName).Select(x => (int)x.unionLithUploaderIdFk).FirstOrDefault(),
                                                          Chronostratigraphy = _apirsLocalLiteDatabase.GetCollection<tblUnionLithostratigraphy>(typeof(tblUnionLithostratigraphy).Name).Find(x => x.grName == sf.sfName).Select(x => (string)x.chronostratNameFk).FirstOrDefault(),
                                                          grName = sf.sfName,
                                                          Hierarchy = "Subformation",
                                                          LithologicDescriptionShort = sf.sfTypeLocality,
                                                          TopBoundary = sf.sfTopBoundary,
                                                          BaseBoundary = sf.sfBaseBoundary,
                                                          TypeLocality = sf.sfDescription,
                                                          DateDocumentation = sf.sfDateOfDocumentation,
                                                          Countries = sf.sfCountries,
                                                          States = sf.sfStates,
                                                          Literature = sf.sfLiterature,
                                                          MaxThickness = sf.sfMaxThickness,
                                                          MeanThickness = sf.sfMeanThickness,
                                                          Notes = sf.sfNotes
                                                      });
        }
        /// <summary>
        /// Getting a set of all drillings
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DrillingJoin> GetAllDrillings()
        {
            if (_apirsDatabase != null)
                return (from ooi in _apirsDatabase.tblObjectOfInvestigations
                        join drill in _apirsDatabase.tblDrillings on ooi.ooiName equals drill.drillName
                        where ooi.ooiName != null
                        select new DrillingJoin()
                        {
                            DrillingID = ooi.ooiIdPk,
                            Name = ooi.ooiName,
                            Type = ooi.ooiType,
                            Latitude = (double)ooi.ooiLatitude,
                            Longitude = (double)ooi.ooiLongitude,
                            DrillingProcess = drill.drillDrillingProcess,
                            Length = (double)drill.drillLength
                        });
            else
                return (from ooi in _apirsLocalLiteDatabase.GetCollection<tblObjectOfInvestigation>().FindAll()
                        join drill in _apirsLocalLiteDatabase.GetCollection<tblDrilling>().FindAll()
                        on ooi.ooiName equals drill.drillName
                        where ooi.ooiName != null
                        select new DrillingJoin()
                        {
                            DrillingID = ooi.ooiIdPk,
                            Name = ooi.ooiName,
                            Type = ooi.ooiType,
                            Latitude = (double)ooi.ooiLatitude,
                            Longitude = (double)ooi.ooiLongitude,
                            DrillingProcess = drill.drillDrillingProcess,
                            Length = (double)drill.drillLength
                        });
        }
        /// <summary>
        /// Getting a set of all outcrops
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OutcropJoin> GetAllOutcrops()
        {
            if (_apirsDatabase != null)
                return (from ooi in _apirsDatabase.tblObjectOfInvestigations
                        join outc in _apirsDatabase.tblOutcrops on ooi.ooiName equals outc.outLocalName
                        where ooi.ooiName != null
                        select new OutcropJoin()
                        {
                            OutcropID = ooi.ooiIdPk,
                            Name = ooi.ooiName,
                            Type = ooi.ooiType,
                            Latitude = (double)ooi.ooiLatitude,
                            Longitude = (double)ooi.ooiLongitude,
                            Owner = ooi.ooiOwner,
                            ActualConditions = outc.outLastCondition
                        });
            else
                return (from ooi in _apirsLocalLiteDatabase.GetCollection<tblObjectOfInvestigation>().FindAll()
                        join outc in _apirsLocalLiteDatabase.GetCollection<tblOutcrop>().FindAll()
                        on ooi.ooiName equals outc.outLocalName
                        where ooi.ooiName != null
                        select new OutcropJoin()
                        {
                            OutcropID = ooi.ooiIdPk,
                            Name = ooi.ooiName,
                            Type = ooi.ooiType,
                            Latitude = (double)ooi.ooiLatitude,
                            Longitude = (double)ooi.ooiLongitude,
                            Owner = ooi.ooiOwner,
                            ActualConditions = outc.outLastCondition
                        });
        }

        //Returning all projects, the user participates at
        public IEnumerable<tblProject> GetProjectByParticipation(int id)
        {
            if ((bool)((ShellViewModel)IoC.Get<IShell>()).LocalMode)
                return new List<tblProject>();
            else
                return from p in _apirsDatabase.tblProjects
                       join pers in _apirsDatabase.v_PersonsProject on p.prjIdPk equals pers.prjIdFk
                       where p.prjCreatorIdFk != id && pers.persIdFk == id
                       select p;
        }

        //Returning all projects, the user participates at
        public IEnumerable<tblProject> GetUserProjects(int id)
        {
            if ((bool)((ShellViewModel)IoC.Get<IShell>()).LocalMode)
                return _apirsLocalLiteDatabase.GetCollection<tblProject>(typeof(tblProject).Name).Find(x => x.prjCreatorIdFk == id);
            else
                return from p in _apirsDatabase.tblProjects
                       join pers in _apirsDatabase.v_PersonsProject on p.prjIdPk equals pers.prjIdFk
                       where p.prjCreatorIdFk == id && pers.persIdFk == id
                       select p;
        }

        public IEnumerable<tblPerson> GetPersonByProject(int id)
        {
            if ((bool)((ShellViewModel)IoC.Get<IShell>()).LocalMode)
                return new List<tblPerson>();
            else
                return from pers in _apirsDatabase.tblPersons
                       join persprj in _apirsDatabase.v_PersonsProject on pers.persIdPk equals persprj.persIdFk
                       where persprj.prjIdFk == id
                       select pers;
        }

        //Getting all facies types based on a project in a foreach query
        public IEnumerable<tblFacy> GetFaciesByProject(IList<tblProject> projects)
        {
            IEnumerable<tblFacy> facyQuery;
            List<tblFacy> facyReturn = new List<tblFacy>();

            //Iterating through all participating projects and select all related rock samples and facies types
            foreach (tblProject prj in projects)
            {
                //querying all facies types which belong to the certain rock type and project
                if ((bool)((ShellViewModel)IoC.Get<IShell>()).LocalMode)
                {
                    facyQuery = _apirsLocalLiteDatabase.GetCollection<tblFacy>(typeof(tblFacy).Name).Find(fac => fac.facprjIdFk == prj.prjIdPk);
                }
                else
                {
                    facyQuery = (from fac in _apirsDatabase.tblFacies
                                 where fac.facprjIdFk == prj.prjIdPk
                                 select fac).ToList();
                }

                foreach (tblFacy facy in facyQuery)
                {
                    facyReturn.Add(facy);
                }
            }

            return facyReturn;
        }

        /// <summary>
        /// Aff facies types based on one project
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public IEnumerable<tblFacy> GetFaciesTypeByProject(IList<tblProject> projects, string parameter)
        {
            IEnumerable<tblFacy> query;
            List<tblFacy> output = new List<tblFacy>();

            foreach (tblProject prj in projects)
            {
                //querying all facies types which belong to the certain rock type and project
                if ((bool)((ShellViewModel)IoC.Get<IShell>()).LocalMode)
                {
                    query = _apirsLocalLiteDatabase.GetCollection<tblFacy>(typeof(tblFacy).Name).Find(fac => fac.facprjIdFk == prj.prjIdPk && fac.facType == parameter);
                }
                else
                {
                    query = (from fac in _apirsDatabase.tblFacies
                             where fac.facprjIdFk == prj.prjIdPk && fac.facType == parameter
                             select fac).ToList();
                }

                //Adding each facies type to the list
                foreach (tblFacy f in query)
                {
                    output.Add(f);
                }
            }

            return output;
        }

        /// <summary>
        /// All lithostratigraphies where a set of facies types was observed
        /// </summary>
        /// <param name="fcs"></param>
        /// <returns></returns>
        public IEnumerable<tblFaciesLithostrat> GetLithostratigtaphyByFaciest(IList<tblFacy> fcs)
        {
            IEnumerable<tblFaciesLithostrat> query;
            List<tblFaciesLithostrat> output = new List<tblFaciesLithostrat>();

            foreach (tblFacy fc in fcs)
            {
                if (_apirsDatabase != null)
                    //querying all facies types which belong to the certain rock type and project
                    query = ((from fac in _apirsDatabase.tblFaciesLithostrats
                              where fac.facIdFk == fc.facIdPk
                              select fac).ToList());
                else
                    query = _apirsLocalLiteDatabase.GetCollection<tblFaciesLithostrat>().Find(fac => fac.facIdFk == fc.facIdPk);

                //Adding each facies type to the list
                foreach (tblFaciesLithostrat f in query)
                {
                    output.Add(f);
                }
            }

            return output;
        }
        /// <summary>
        /// All occurences of a list of facies types
        /// </summary>
        /// <param name="fcs"></param>
        /// <returns></returns>
        public IEnumerable<tblFaciesObservation> GetOccurenceByFacies(IList<tblFacy> fcs)
        {
            IEnumerable<tblFaciesObservation> query;
            List<tblFaciesObservation> output = new List<tblFaciesObservation>();

            foreach (tblFacy fc in fcs)
            {
                if (_apirsDatabase != null)
                    //querying all facies types which belong to the certain rock type and project
                    query = ((from fac in _apirsDatabase.tblFaciesObservations
                              where fac.fofacIdFk == fc.facIdPk
                              select fac).ToList());
                else
                    query = _apirsLocalLiteDatabase.GetCollection<tblFaciesObservation>().Find(fac => fac.fofacIdFk == fc.facIdPk);

                //Adding each facies type to the list
                foreach (tblFaciesObservation f in query)
                {
                    output.Add(f);
                }
            }

            return output;
        }
        /// <summary>
        /// All rock samples of a set of projects with a certain parameter
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public IEnumerable<tblRockSample> GetRockSamplesByProject(IList<tblProject> projects, string parameter)
        {
            IEnumerable<tblRockSample> query;
            List<tblRockSample> output = new List<tblRockSample>();

            //Iterating through all participating projects and select all related rock samples and facies types
            foreach (tblProject prj in projects)
            {
                if (_apirsDatabase != null)
                    if (parameter != "All")
                        query = (from rs in _apirsDatabase.tblRockSamples
                                 where rs.sampType.Contains(parameter) && rs.sampprjIdFk == prj.prjIdPk
                                 select rs).ToList();
                    else
                        query = (from rs in _apirsDatabase.tblRockSamples
                                 where rs.sampprjIdFk == prj.prjIdPk
                                 select rs).ToList();
                else
                    if (parameter != "All")
                    query = _apirsLocalLiteDatabase.GetCollection<tblRockSample>().Find(rs => rs.sampType.Contains(parameter) && rs.sampprjIdFk == prj.prjIdPk);
                else
                    query = _apirsLocalLiteDatabase.GetCollection<tblRockSample>().Find(rs => rs.sampprjIdFk == prj.prjIdPk);


                foreach (tblRockSample rsp in query)
                {
                    output.Add(rsp);
                }
            }
            return output;
        }

        /// <summary>
        /// All rock samples of a set of projects
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        public IEnumerable<tblRockSample> GetAllRockSamplesByProject(IList<tblProject> projects)
        {
            IEnumerable<tblRockSample> query;
            List<tblRockSample> output = new List<tblRockSample>();

            //Iterating through all participating projects and select all related rock samples and facies types
            foreach (tblProject prj in projects)
            {
                if (_apirsDatabase != null)
                    query = (from rs in _apirsDatabase.tblRockSamples
                             where rs.sampprjIdFk == prj.prjIdPk
                             select rs).ToList();
                else
                    query = _apirsLocalLiteDatabase.GetCollection<tblRockSample>().Find(rs => rs.sampprjIdFk == prj.prjIdPk);


                foreach (tblRockSample rsp in query)
                {
                    output.Add(rsp);
                }
            }
            return output;
        }


        /// <summary>
        /// All architectural elements observed in the scope of a set of projects
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public IEnumerable<tblArchitecturalElement> GetArchitecturalElementsByProject(IList<tblProject> projects, string parameter)
        {
            IEnumerable<tblArchitecturalElement> query;
            List<tblArchitecturalElement> output = new List<tblArchitecturalElement>();

            foreach (tblProject prj in projects)
            {
                //querying all facies types which belong to the certain rock type and project
                if (_apirsDatabase != null)
                    query = ((from fac in _apirsDatabase.tblArchitecturalElements
                              where fac.aeprjIdFk == prj.prjIdPk && fac.aeType == parameter
                              select fac).ToList());
                else
                    query = _apirsLocalLiteDatabase.GetCollection<tblArchitecturalElement>().Find(ae => ae.aeprjIdFk == prj.prjIdPk && ae.aeType == parameter);

                //Adding each facies type to the list
                foreach (tblArchitecturalElement f in query)
                {
                    output.Add(f);
                }
            }

            return output;
        }
        /// <summary>
        /// All architectural elements observed in the scope of a set of projects
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public IEnumerable<tblArchitecturalElement> GetArchitecturalElementsByProject(IList<tblProject> projects)
        {
            IEnumerable<tblArchitecturalElement> query;
            List<tblArchitecturalElement> output = new List<tblArchitecturalElement>();

            foreach (tblProject prj in projects)
            {
                //querying all facies types which belong to the certain rock type and project
                if (_apirsDatabase != null)
                    query = ((from fac in _apirsDatabase.tblArchitecturalElements
                              where fac.aeprjIdFk == prj.prjIdPk
                              select fac).ToList());
                else
                    query = _apirsLocalLiteDatabase.GetCollection<tblArchitecturalElement>().Find(ae => ae.aeprjIdFk == prj.prjIdPk);

                //Adding each facies type to the list
                foreach (tblArchitecturalElement f in query)
                {
                    output.Add(f);
                }
            }

            return output;
        }
        /// <summary>
        /// All lithostratigraphic units where a set of architectural elements was observed in
        /// </summary>
        /// <param name="aes"></param>
        /// <returns></returns>
        public IEnumerable<tblArchitecturalElementLithostrat> GetLithostratigtaphyByArchitecturalElement(IList<tblArchitecturalElement> aes)
        {
            IEnumerable<tblArchitecturalElementLithostrat> query;
            List<tblArchitecturalElementLithostrat> output = new List<tblArchitecturalElementLithostrat>();

            foreach (tblArchitecturalElement ae in aes)
            {
                //querying all facies types which belong to the certain rock type and project
                if (_apirsDatabase != null)
                    query = ((from fac in _apirsDatabase.tblArchitecturalElementLithostrats
                              where fac.aeIdFk == ae.aeIdPk
                              select fac).ToList());
                else
                    query = _apirsLocalLiteDatabase.GetCollection<tblArchitecturalElementLithostrat>().Find(fac => fac.aeIdFk == ae.aeIdPk);

                //Adding each facies type to the list
                foreach (tblArchitecturalElementLithostrat f in query)
                {
                    output.Add(f);
                }
            }

            return output;
        }
        /// <summary>
        /// All occurences of a set of architectural elements
        /// </summary>
        /// <param name="aes"></param>
        /// <returns></returns>
        public IEnumerable<tblArchEleOccurence> GetOccurenceByArchitecturalElement(IList<tblArchitecturalElement> aes)
        {
            IEnumerable<tblArchEleOccurence> query;
            List<tblArchEleOccurence> output = new List<tblArchEleOccurence>();

            foreach (tblArchitecturalElement ae in aes)
            {
                //querying all facies types which belong to the certain rock type and project
                if (_apirsDatabase != null)
                    query = ((from fac in _apirsDatabase.tblArchEleOccurences
                              where fac.aeIdFk == ae.aeIdPk
                              select fac).ToList());
                else
                    query = _apirsLocalLiteDatabase.GetCollection<tblArchEleOccurence>().Find(fac => fac.aeIdFk == ae.aeIdPk);

                //Adding each facies type to the list
                foreach (tblArchEleOccurence f in query)
                {
                    output.Add(f);
                }
            }

            return output;
        }
        /// <summary>
        /// Aff facies types based on one project
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public IEnumerable<tblSection> GetSectionByProject(IList<tblProject> projects, string parameter)
        {
            IEnumerable<tblSection> query;
            List<tblSection> output = new List<tblSection>();

            foreach (tblProject prj in projects)
            {
                //querying all facies types which belong to the certain rock type and project
                if (_apirsDatabase != null)
                    query = ((from sec in _apirsDatabase.tblSections
                              where sec.secprjIdFk == prj.prjIdPk && sec.secType == parameter
                              select sec).ToList());
                else
                    query = _apirsLocalLiteDatabase.GetCollection<tblSection>().Find(sec => sec.secprjIdFk == prj.prjIdPk && sec.secType == parameter);

                //Adding each facies type to the list
                foreach (tblSection f in query)
                {
                    output.Add(f);
                }
            }

            return output;
        }

        public IEnumerable<tblMeasurement> GetFieldMeasurementByProject(IList<tblProject> projects, string name)
        {

            IEnumerable<tblMeasurement> query = new List<tblMeasurement>();
            var fieldMeasurements = new BindableCollection<tblMeasurement>();

            if (_apirsDatabase != null)
            {
                //Iterating through all participating projects and select all related rock samples
                foreach (tblProject prj in projects)
                {
                    query = (from fime in _apirsDatabase.tblMeasurements
                             where fime.measObjectOfInvestigationIdFk == name
                             && fime.measprjIdFk == prj.prjIdPk
                             && fime.measType == "Field measurement"
                             orderby fime.measParameter
                             select fime).ToList();

                    foreach (tblMeasurement rsp in query)
                    {
                        fieldMeasurements.Add(rsp);
                    }

                    fieldMeasurements.OrderBy(x => x.measParameter).OrderBy(y => y.measIdPk);

                }

                return fieldMeasurements;
            }
            else
            {
                //Iterating through all participating projects and select all related rock samples
                foreach (tblProject prj in projects)
                {
                    query = _apirsLocalLiteDatabase.GetCollection<tblMeasurement>(typeof(tblMeasurement).Name).Find(fime => fime.measObjectOfInvestigationIdFk == name && fime.measprjIdFk == prj.prjIdPk).OrderBy(fime => fime.measParameter);

                    foreach (tblMeasurement rsp in query)
                    {
                        fieldMeasurements.Add(rsp);
                    }
                    fieldMeasurements.OrderBy(x => x.measParameter).OrderBy(y => y.measIdPk);
                }

                return fieldMeasurements;
            }
        }
        /// <summary>
        /// A list of images related to a rock sample with the id "id"
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<v_PictureStore>> GetRockSampleImagesAsync(int id)
        {
            try
            {
                if (_apirsDatabase != null)
                    return await (from pic in _apirsDatabase.v_PictureStore
                                  join picsamp in _apirsDatabase.tblPictureRockSamples on pic.stream_id equals picsamp.picStreamIdFk
                                  where picsamp.sampIdFk == id
                                  select pic).ToListAsync().ConfigureAwait(false);
                else
                {
                    List<Guid> a = _apirsLocalLiteDatabase.GetCollection<tblPictureRockSample>(typeof(tblPictureRockSample).Name).Find(x => x.sampIdFk == id).Select(x => x.picStreamIdFk).ToList();

                    return _apirsLocalLiteDatabase.GetCollection<v_PictureStore>(typeof(v_PictureStore).Name).Find(x => a.Contains(x.stream_id)).ToList();
                }
            }
            catch
            {
                return new List<v_PictureStore>();
            }
        }
        /// <summary>
        /// A list of images related to an object of investigation with the id "id"
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<v_PictureStore>> GetObjectImagesAsync(int id)
        {

            try
            {
                if (_apirsDatabase != null)
                    return await (from pic in _apirsDatabase.v_PictureStore
                                  join picsamp in _apirsDatabase.tblPictureObjectOfInvestigations on pic.stream_id equals picsamp.picStreamIdFk
                                  where picsamp.ooiIdFk == id
                                  select pic).ToListAsync().ConfigureAwait(false);
                else
                {
                    List<Guid> a = _apirsLocalLiteDatabase.GetCollection<tblPictureObjectOfInvestigation>(typeof(tblPictureObjectOfInvestigation).Name).Find(x => x.ooiIdFk == id).Select(x => x.picStreamIdFk).ToList();

                    return _apirsLocalLiteDatabase.GetCollection<v_PictureStore>(typeof(v_PictureStore).Name).Find(x => a.Contains(x.stream_id)).ToList();
                }
            }
            catch
            {
                return new List<v_PictureStore>();
            }
        }
        /// <summary>
        /// A list of images related to a facies type with the id "id"
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<v_PictureStore>> GetFaciesImagesAsync(int id)
        {
            try
            {
                if (_apirsDatabase != null)
                    return await (from pic in _apirsDatabase.v_PictureStore
                                  join picsamp in _apirsDatabase.tblPictureLithofacies on pic.stream_id equals picsamp.picStreamIdFk
                                  where picsamp.lftIdFk == id
                                  select pic).ToListAsync().ConfigureAwait(false);
                else
                {
                    List<Guid> a = _apirsLocalLiteDatabase.GetCollection<tblPictureLithofacy>(typeof(tblPictureLithofacy).Name).Find(x => x.lftIdFk == id).Select(x => (Guid)x.picStreamIdFk).ToList();

                    return _apirsLocalLiteDatabase.GetCollection<v_PictureStore>(typeof(v_PictureStore).Name).Find(x => a.Contains(x.stream_id)).ToList();
                }

            }
            catch
            {
                return new List<v_PictureStore>();
            }
        }
        /// <summary>
        /// A list of images related to an architectural element with the id "id"
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<v_PictureStore>> GetArchitecturalElementImagesAsync(int id)
        {
            try
            {
                if (_apirsDatabase != null)
                    return await (from pic in _apirsDatabase.v_PictureStore
                                  join picsamp in _apirsDatabase.tblPictureArchitecturalElements on pic.stream_id equals picsamp.picStreamIdFk
                                  where picsamp.aeIdFk == id
                                  select pic).ToListAsync().ConfigureAwait(false);
                else
                {
                    List<Guid> a = _apirsLocalLiteDatabase.GetCollection<tblPictureArchitecturalElement>(typeof(tblPictureArchitecturalElement).Name).Find(x => x.aeIdFk == id).Select(x => (Guid)x.picStreamIdFk).ToList();

                    return _apirsLocalLiteDatabase.GetCollection<v_PictureStore>(typeof(v_PictureStore).Name).Find(x => a.Contains(x.stream_id)).ToList();
                }

            }
            catch
            {
                return new List<v_PictureStore>();
            }
        }

        /// <summary>
        ///A list of files related to laboratory measurements with the id "id"
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<v_FileStore>> GetLabMeasurementFileAsync(int id)
        {
            try
            {
                if (_apirsDatabase != null)
                    return await (from pic in _apirsDatabase.v_FileStore
                                  join picsamp in _apirsDatabase.tblFileLabMeasurements on pic.stream_id equals picsamp.picStreamIdFk
                                  where picsamp.labmeIdFk == id
                                  select pic).ToListAsync().ConfigureAwait(false);
                else
                {
                    List<Guid> a = _apirsLocalLiteDatabase.GetCollection<tblFileLabMeasurement>(typeof(tblFileLabMeasurement).Name).Find(x => x.labmeIdFk == id).Select(x => (Guid)x.picStreamIdFk).ToList();

                    return _apirsLocalLiteDatabase.GetCollection<v_FileStore>(typeof(v_FileStore).Name).Find(x => a.Contains(x.stream_id)).ToList();
                }

            }
            catch
            {
                return new List<v_FileStore>();
            }
        }
        /// <summary>
        ///A list of files related to laboratory measurements with the id "id"
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<v_FileStore>> GetFieldMeasurementFileAsync(int id)
        {
            try
            {
                if (_apirsDatabase != null)
                    return await (from pic in _apirsDatabase.v_FileStore
                                  join picsamp in _apirsDatabase.tblFileFieldMeasurements on pic.stream_id equals picsamp.filStreamIdFk
                                  where picsamp.fimeIdFk == id
                                  select pic).ToListAsync().ConfigureAwait(false);
                else
                {
                    List<Guid> a = _apirsLocalLiteDatabase.GetCollection<tblFileFieldMeasurement>(typeof(tblFileFieldMeasurement).Name).Find(x => x.fimeIdFk == id).Select(x => (Guid)x.filStreamIdFk).ToList();

                    return _apirsLocalLiteDatabase.GetCollection<v_FileStore>(typeof(v_FileStore).Name).Find(x => a.Contains(x.stream_id)).ToList();
                }
            }
            catch
            {
                return new List<v_FileStore>();
            }
        }
        /// <summary>
        ///A list of files related to laboratory measurements with the id "id"
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<v_FileStore>> GetSectionFileAsync(int id)
        {
            try
            {
                if (_apirsDatabase != null)
                    return await (from pic in _apirsDatabase.v_FileStore
                                  join picsamp in _apirsDatabase.tblFileSections on pic.stream_id equals picsamp.filStreamIdFk
                                  where picsamp.secIdFk == id
                                  select pic).ToListAsync().ConfigureAwait(false);
                else
                {
                    List<Guid> a = _apirsLocalLiteDatabase.GetCollection<tblFileFieldMeasurement>(typeof(tblFileFieldMeasurement).Name).Find(x => x.fimeIdFk == id).Select(x => (Guid)x.filStreamIdFk).ToList();

                    return _apirsLocalLiteDatabase.GetCollection<v_FileStore>(typeof(v_FileStore).Name).Find(x => a.Contains(x.stream_id)).ToList();
                }

            }
            catch
            {
                return new List<v_FileStore>();
            }
        }

        /// <summary>
        /// Mapping a field measurement lithofacies type based on the depth, latitude and longitude of a layer in a lithological section
        /// </summary>
        /// <param name="fieldMeasurements"></param>
        /// <param name="section"></param>
        /// <param name="projects"></param>
        /// <returns></returns>
        public Tuple<int, List<tblMeasurement>> MapFieldMeasurementToLithologicalSection(List<tblMeasurement> fieldMeasurements, tblSection section, IList<tblProject> projects)
        {

            var fims = new List<tblMeasurement>(fieldMeasurements);
            List<tblSectionLithofacy> layers = new List<tblSectionLithofacy>();

            int i = 0;

            try
            {

                if (_apirsDatabase != null)
                    layers = _apirsDatabase.tblSectionLithofacies
                             .Where(lit => lit.litsecIdFk == section.secIdPk).ToList();
                else
                    layers = _apirsLocalLiteDatabase.GetCollection<tblSectionLithofacy>(typeof(tblSectionLithofacy).Name)
                             .Find(lit => lit.litsecIdFk == section.secIdPk).ToList();

                foreach (tblMeasurement fime in fims)
                {
                    tblSectionLithofacy layer = layers
                        .Where(lay => -1 * lay.litsecBase <= fime.measLocalCoordinateZ
                                      && -1 * lay.litsecTop > fime.measLocalCoordinateZ)
                        .FirstOrDefault();

                    if (layer == null)
                        layer = layers
                                .Where(lay => lay.litsecBase <= fime.measLocalCoordinateZ
                                              && lay.litsecTop > fime.measLocalCoordinateZ)
                                              .FirstOrDefault();

                    if (layer != null)
                    {
                        fime.measFaciesIdFk = layer.litseclftId;
                        i++;
                    }
                }

            }
            catch
            {
                return null;
            }

            return new Tuple<int, List<tblMeasurement>>(i, fims);
        }

        /// <summary>
        /// Loads data from CSV files in the local database
        /// </summary>
        /// <returns></returns>
        public async Task UpdateLocalDatabase()
        {
            try
            {
                string dir = System.IO.Path.GetDirectoryName(
System.Reflection.Assembly.GetExecutingAssembly().Location);

                string file = "";

                //Cleaning all tables
                _apirsLocalLiteDatabase.GetCollection<tblUnionPetrography>(typeof(tblUnionPetrography).Name).Delete(x => x == x);
                _apirsLocalLiteDatabase.GetCollection<tblUnionChronostratigraphy>(typeof(tblUnionChronostratigraphy).Name).Delete(x => x == x);
                _apirsLocalLiteDatabase.GetCollection<tblFaciesCode>(typeof(tblFaciesCode).Name).Delete(x => x == x);
                _apirsLocalLiteDatabase.GetCollection<tblAlia>(typeof(tblAlia).Name).Delete(x => x == x);

                //Inserting all entities from the CSV lists to the tables
                List<string> names = GetNames("Petrography");

                for (int i = 0; i < names.Count(); i++)
                    _apirsLocalLiteDatabase.GetCollection<tblUnionPetrography>(typeof(tblUnionPetrography).Name).Insert(new tblUnionPetrography() { Petrography = names[i] });

                names = GetNames("Chronostratigraphy");

                for (int i = 0; i < names.Count(); i++)
                    _apirsLocalLiteDatabase.GetCollection<tblUnionChronostratigraphy>(typeof(tblUnionChronostratigraphy).Name).Insert(new tblUnionChronostratigraphy() { eonName = names[i] });

                file = dir + @"\Media\Data\Alias.csv";

                //Getting file information
                FileInfo fi = new FileInfo(file);

                DataTable table = new DataTable() { TableName = "MyTableName" };

                table = FileHelper.CsvToDataTable(fi.FullName,true);

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    try
                    {
                        tblAlia al = new tblAlia();
                        al.alColumnName = table.Rows[i][0].ToString();
                        al.alAlias = table.Rows[i][1].ToString();
                        al.alTableName = table.Rows[i][2].ToString();
                        al.alTableAlias = table.Rows[i][3].ToString();
                        al.alImport = Convert.ToBoolean(table.Rows[i][4]);
                        al.alDataType = table.Rows[i][5].ToString();

                        _apirsLocalLiteDatabase.GetCollection<tblAlia>(typeof(tblAlia).Name).Insert(al);
                    }
                    catch
                    {
                        continue;
                    }

                }

                file = dir + @"\Media\Data\Facies.csv";

                //Getting file information
                fi = new FileInfo(file);

                table = new DataTable() { TableName = "MyTableName" };

                table = FileHelper.CsvToDataTable(fi.FullName, true);

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    try
                    {
                        tblFaciesCode fc = new tblFaciesCode();
                        fc.fcCode = table.Rows[i][0].ToString();
                        fc.fcDecoding = table.Rows[i][1].ToString();
                        fc.fcDescription = table.Rows[i][2].ToString();
                        fc.fcFaciesType = table.Rows[i][3].ToString();
                        fc.fcHierarchy = table.Rows[i][4].ToString();

                        _apirsLocalLiteDatabase.GetCollection<tblFaciesCode>(typeof(tblFaciesCode).Name).Insert(fc);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Gets all names of entities in a csv file
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<string> GetNames(string entity = null, int id = 0)
        {
            List<string> names = new List<string>();

            try
            {
                string dir = System.IO.Path.GetDirectoryName(
                    System.Reflection.Assembly.GetExecutingAssembly().Location);

                string file = "";

                switch (entity)
                {
                    case "Chronostratigraphy":
                        file = dir + @"\Media\Data\Chronostratigraphy.csv";
                        break;
                    case "Petrography":
                        file = dir + @"\Media\Data\Petrography.csv";
                        break;
                    case "Alias":
                        file = dir + @"\Media\Data\Alias.csv";
                        break;
                    case "Facies":
                        file = dir + @"\Media\Data\Facies.csv";
                        break;
                }

                var stream = File.OpenRead(file);

                foreach (string line in File.ReadLines(file, Encoding.ASCII))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    names.Add(line);
                }
            }
            catch
            {

            }

            return names;
        }

        #region Images

        public Tuple<Guid, string> UploadImage(string imagePath)
        {
            try
            {

                //Getting file information
                FileInfo fi = new FileInfo(imagePath);

                //Implementing a new filestream
                FileStream fs = new FileStream(fi.FullName, System.IO.FileMode.Open, FileAccess.Read);

                //Transfering filestream into binary format
                BinaryReader rdr = new BinaryReader(fs);
                byte[] fileData = rdr.ReadBytes((int)fs.Length);

                //Scaling factor
                double scale = 0.6;

                //Resizing bitmap to reduce image size
                Bitmap startBitmap;
                //Converting fileStream data into bitmap
                using (var ms = new MemoryStream(fileData))
                {
                    startBitmap = new Bitmap(ms);
                }

                //new size
                int newHeight = (int)Math.Round(startBitmap.Height * scale, 0);
                int newWidth = (int)Math.Round(startBitmap.Width * scale, 0);

                // write CreateBitmapFromBytes  
                Bitmap newBitmap = new Bitmap(newWidth, newHeight);

                //Resizing the bitmap
                using (Graphics graphics = Graphics.FromImage(newBitmap))
                {
                    graphics.DrawImage(startBitmap, new Rectangle(0, 0, newWidth, newHeight), new Rectangle(0, 0, startBitmap.Width, startBitmap.Height), GraphicsUnit.Pixel);
                }

                //Back conversion
                fileData = ImageHelper.BitmapToByte(newBitmap); // write CreateBytesFromBitmap 

                //Closing filestream
                rdr.Close();
                fs.Close();

                if (_apirsDatabase != null)
                {

                    try
                    {
                        ApirsDatabase ap = new ApirsDatabase();
                        //Retrieving file meta data
                        string fileName = fi.Name;
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                        string extension = fi.Extension;
                        char charac = 'a';

                        //Changing image name based on the count of occurences
                        while (ap.v_PictureStore.Where(x => x.name == fileName).Count() > 0)
                        {
                            fileName = fileNameWithoutExtension + charac + extension;
                            charac++;
                        }

                        ap = new ApirsDatabase();
                        //Establishing a sql connection
                        using (SqlConnection SqlConn = new SqlConnection(ap.Database.Connection.ConnectionString.ToString()))
                        {
                            SqlCommand spAddImage = new SqlCommand("dbo.spAddImage", SqlConn);

                            //Testing if a connection is established
                            if (ServerInteractionHelper.IsNetworkAvailable())
                            {
                                //Preparing the stored procedure
                                spAddImage.CommandType = System.Data.CommandType.StoredProcedure;

                                //Adding the parameters
                                spAddImage.Parameters.Add("@pName", SqlDbType.NVarChar, 255).Value = fileName;
                                spAddImage.Parameters.Add("@pFile_Stream", SqlDbType.Image, fileData.Length).Value = fileData;

                                //Opening the connection
                                SqlConn.Open();

                                Guid result = (Guid)spAddImage.ExecuteScalar();

                                SqlConn.Close();

                                return new Tuple<Guid, string>(result, fileName);
                            }
                        }
                    }
                    catch
                    {
                        ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(UserMessageValueConverter.ConvertBack(1));
                    }

                    // Create a GUID with all zeros and compare it to Empty.
                    Byte[] bytes = new Byte[16];
                    return new Tuple<Guid, string>(new Guid(bytes), null);
                }
                else
                {
                    try
                    {
                        //Retrieving file meta data
                        string fileName = fi.Name;
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                        string extension = fi.Extension;
                        char charac = 'a';

                        //Changing image name based on the count of occurences
                        while (_apirsLocalLiteDatabase.GetCollection<v_PictureStore>().Find(x => x.name == fileName).Count() > 0)
                        {
                            fileName = fileNameWithoutExtension + charac + extension;
                            charac++;
                        }

                        v_PictureStore pic = new v_PictureStore() { file_stream = fileData, name = fileName };

                        _apirsLocalLiteDatabase.GetCollection<v_PictureStore>(typeof(v_PictureStore).Name).Insert(pic);

                        return new Tuple<Guid, string>(pic.stream_id, fileName);

                    }
                    catch
                    {
                        ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(UserMessageValueConverter.ConvertBack(1));
                    }

                    // Create a GUID with all zeros and compare it to Empty.
                    Byte[] bytes = new Byte[16];
                    return new Tuple<Guid, string>(new Guid(bytes), null);
                }
            }
            catch
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(UserMessageValueConverter.ConvertBack(1));

                // Create a GUID with all zeros and compare it to Empty.
                Byte[] bytes = new Byte[16];
                return new Tuple<Guid, string>(new Guid(bytes), null);
            }
        }

        #endregion Images

        #endregion

        #region Helpers

        /// <summary>
        /// Filter a data set based on a parameter
        /// </summary>
        /// <param name="models"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public IEnumerable<T> Filter(IEnumerable<T> models, string parameter)
        {

            var type = models.GetType().GetGenericArguments()[0];
            var properties = type.GetProperties();
            try
            {
                var result = models.Where(x => properties
                            .Any(p => p.GetValue(x) != null && p.GetValue(x).ToString().ToLower().Contains(parameter.ToLower())));
                return result;
            }
            catch
            {
                return Enumerable.Empty<T>();
            }
        }

        public void DeleteFile(string name)
        {
            try
            {
                if (_apirsDatabase != null)
                    //Establishing a sql connection
                    using (SqlConnection SqlConn = new SqlConnection(_apirsDatabase.Database.Connection.ConnectionString.ToString()))
                    {
                        SqlCommand spDeleteFile = new SqlCommand("dbo.spDeleteFile", SqlConn);

                        //Testing if a connection is established
                        if (ServerInteractionHelper.IsNetworkAvailable())
                        {
                            //Preparing the stored procedure
                            spDeleteFile.CommandType = System.Data.CommandType.StoredProcedure;

                            //Adding the parameters
                            spDeleteFile.Parameters.Add("@file_name", SqlDbType.NVarChar, 255).Value = name;

                            //Opening the connection
                            SqlConn.Open();

                            //Deleting the File in the database
                            spDeleteFile.ExecuteNonQuery();

                            SqlConn.Close();
                        }
                    }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Task which synchronizes the online and offline database
        /// </summary>
        /// <returns></returns>
        public void SynchronizeDatabases()
        {
            if (_apirsDatabase == null)
            {
                return;
            }

            _apirsLocalLiteDatabase = new LiteDatabase(dbLiteFile);

            List<tblObjectOfInvestigation> localObjects = _apirsLocalLiteDatabase.GetCollection<tblObjectOfInvestigation>().FindAll().ToList();
            foreach (tblObjectOfInvestigation locOb in localObjects)
            {
                try
                {
                    _apirsDatabase.tblObjectOfInvestigations.Add(locOb);
                    _apirsDatabase.SaveChanges();
                    _apirsLocalLiteDatabase.GetCollection<tblObjectOfInvestigation>().Delete(locOb.ooiIdPk);

                }
                catch (Exception e)
                {
                    continue;
                }
            }
        }

        public void SynchronizeLocalDatabase()
        {
            if (_apirsDatabase == null)
            {
                return;
            }

            _apirsLocalLiteDatabase = new LiteDatabase(dbLiteFile);

            try
            {
                if (_apirsLocalLiteDatabase.GetCollection<tblUnionChronostratigraphy>().Count() == 0)
                {
                    //Getting Chronostratigraphy
                    List<tblEonothem> eon = _apirsDatabase.tblEonothems.ToList();
                    List<tblErathem> era = _apirsDatabase.tblErathems.ToList();
                    List<tblPeriod> per = _apirsDatabase.tblPeriods.ToList();
                    List<tblSery> ser = _apirsDatabase.tblSeries.ToList();
                    List<tblSystem> sys = _apirsDatabase.tblSystems.ToList();
                    List<tblStage> sta = _apirsDatabase.tblStages.ToList();
                    List<tblUnionChronostratigraphy> chron = _apirsDatabase.tblUnionChronostratigraphies.ToList();

                    try
                    {
                        if (_apirsLocalLiteDatabase.GetCollection<tblEonothem>().Count() == 0)
                            foreach (var e in eon)
                            {
                                try
                                {
                                    _apirsLocalLiteDatabase.GetCollection<tblEonothem>().Insert(new tblEonothem { eonIdPk = e.eonIdPk, eonchronIdFk = e.eonchronIdFk, eonName = e.eonName, eonNumericalAgeLowerBoundary = e.eonNumericalAgeLowerBoundary, eonPlusMinus = e.eonPlusMinus });
                                }
                                catch (Exception exc)
                                {
                                    continue;
                                }
                            };
                        if (_apirsLocalLiteDatabase.GetCollection<tblErathem>().Count() == 0)
                            foreach (var e in era)
                            {
                                try
                                {
                                    _apirsLocalLiteDatabase.GetCollection<tblErathem>().Insert(new tblErathem { eraIdPk = e.eraIdPk, erachronIdFk = e.erachronIdFk, eraName = e.eraName, eraNumericalAgeLowerBoundary = e.eraNumericalAgeLowerBoundary, eraPlusMinus = e.eraPlusMinus, eraeonIdFk = e.eraeonIdFk });
                                }
                                catch
                                {
                                    continue;
                                }
                            };
                        if (_apirsLocalLiteDatabase.GetCollection<tblPeriod>().Count() == 0)
                            foreach (var e in per)
                            {
                                try
                                {
                                    _apirsLocalLiteDatabase.GetCollection<tblPeriod>().Insert(new tblPeriod { perIdPk = e.perIdPk, perchronIdFk = e.perchronIdFk, perName = e.perName, perNumericalAgeLowerBoundary = e.perNumericalAgeLowerBoundary, perPlusMinus = e.perPlusMinus, pereonIdFk = e.pereonIdFk, pereraIdFk = e.pereraIdFk });
                                }
                                catch
                                {
                                    continue;
                                }
                            };
                        if (_apirsLocalLiteDatabase.GetCollection<tblSery>().Count() == 0)
                            foreach (var e in ser)
                            {
                                try
                                {
                                    _apirsLocalLiteDatabase.GetCollection<tblSery>().Insert(new tblSery { serIdPk = e.serIdPk, serchronIdFk = e.serchronIdFk, serName = e.serName, serNumericalAgeLowerBoundary = e.serNumericalAgeLowerBoundary, serPlusMinus = e.serPlusMinus, sereonIdFk = e.sereonIdFk, sereraIdFk = e.sereraIdFk, serperIdFk = e.serperIdFk, sersysIdFk = e.sersysIdFk });
                                }
                                catch
                                {
                                    continue;
                                }
                            };
                        if (_apirsLocalLiteDatabase.GetCollection<tblSystem>().Count() == 0)
                            foreach (var e in sys)
                            {
                                try
                                {
                                    _apirsLocalLiteDatabase.GetCollection<tblSystem>().Insert(new tblSystem { sysIdPk = e.sysIdPk, syschronIdFk = e.syschronIdFk, sysName = e.sysName, sysNumericalAgeLowerBoundary = e.sysNumericalAgeLowerBoundary, sysPlusMinus = e.sysPlusMinus, syseonIdFk = e.syseonIdFk, syseraIdFk = e.syseraIdFk, sysperIdFk = e.sysperIdFk });
                                }
                                catch
                                {
                                    continue;
                                }
                            };
                        if (_apirsLocalLiteDatabase.GetCollection<tblStage>().Count() == 0)
                            foreach (var e in sta)
                            {
                                try
                                {
                                    _apirsLocalLiteDatabase.GetCollection<tblStage>().Insert(new tblStage { stageIdPk = e.stageIdPk, stagechronIdFk = e.stagechronIdFk, stageName = e.stageName, stageNumericalAgeLowerBoundary = e.stageNumericalAgeLowerBoundary, stagePlusMinus = e.stagePlusMinus, stageeonIdFk = e.stageeonIdFk, stageeraIdFk = e.stageeraIdFk, stageperIdFk = e.stageperIdFk });
                                }
                                catch
                                {
                                    continue;
                                }
                            };
                        if (_apirsLocalLiteDatabase.GetCollection<tblUnionChronostratigraphy>().Count() == 0)
                            foreach (var e in chron) { try { _apirsLocalLiteDatabase.GetCollection<tblUnionChronostratigraphy>().Insert(new tblUnionChronostratigraphy() { eonName = e.eonName }); } catch { continue; } };
                    }
                    catch (Exception ex)
                    {

                    }

                    //Getting Petrography
                    List<tblPetrography1> pet1 = _apirsDatabase.tblPetrography1.ToList();
                    List<tblPetrography2> pet2 = _apirsDatabase.tblPetrography2.ToList();
                    List<tblPetrography3> pet3 = _apirsDatabase.tblPetrography3.ToList();
                    List<tblPetrography4> pet4 = _apirsDatabase.tblPetrography4.ToList();
                    List<tblPetrography5> pet5 = _apirsDatabase.tblPetrography5.ToList();
                    List<tblPetrography6> pet6 = _apirsDatabase.tblPetrography6.ToList();
                    List<tblPetrography7> pet7 = _apirsDatabase.tblPetrography7.ToList();
                    List<tblPetrography8> pet8 = _apirsDatabase.tblPetrography8.ToList();
                }

                if (_apirsLocalLiteDatabase.GetCollection<tblUnionPetrography>().Count() == 0)
                {
                    List<tblUnionPetrography> petunion = _apirsDatabase.tblUnionPetrographies.ToList();

                    if (_apirsLocalLiteDatabase.GetCollection<tblUnionPetrography>().Count() != _apirsDatabase.tblUnionPetrographies.Count())
                    {
                        _apirsLocalLiteDatabase.GetCollection<tblFaciesCode>().Delete(x => x == x);

                        foreach (var e in petunion)
                        {
                            try
                            {
                                _apirsLocalLiteDatabase.GetCollection<tblUnionPetrography>().Insert(new tblUnionPetrography() { Petrography = e.Petrography });
                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                        }
                    }
                }

                if (_apirsLocalLiteDatabase.GetCollection<tblFaciesCode>().Count() == 0)
                {
                    //Getting facies codes
                    List<tblFaciesCode> faco = _apirsDatabase.tblFaciesCodes.ToList();

                    if (_apirsLocalLiteDatabase.GetCollection<tblFaciesCode>().Count() != _apirsDatabase.tblFaciesCodes.Count())
                    {
                        _apirsLocalLiteDatabase.GetCollection<tblFaciesCode>().Delete(x => x == x);

                        foreach (var e in faco)
                        {
                            try
                            {
                                _apirsLocalLiteDatabase.GetCollection<tblFaciesCode>().Insert(e);
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }

                if (_apirsLocalLiteDatabase.GetCollection<tblAlia>().Count() == 0)
                {
                    //Getting facies codes
                    List<tblAlia> faco = _apirsDatabase.tblAlias.ToList();

                    if (_apirsLocalLiteDatabase.GetCollection<tblAlia>().Count() != _apirsDatabase.tblAlias.Count())
                    {
                        _apirsLocalLiteDatabase.GetCollection<tblAlia>().Delete(x => x == x);

                        foreach (var e in faco)
                        {
                            try
                            {
                                _apirsLocalLiteDatabase.GetCollection<tblAlia>().Insert(e);
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }


        }

        //public void MergeMeasurements()
        //{
        //    List<tblFieldMeasurement> fieldMeasurements = _apirsDatabase.tblFieldMeasurements.ToList();

        //    foreach(tblFieldMeasurement fime in fieldMeasurements)
        //    {
        //        try
        //        {
        //            tblMeasurement measurement = new tblMeasurement()
        //            {
        //                measAnalyticalDeviceId = fime.fimeAnalyticalDeviceId,
        //                measArchitecturalElement = fime.fimeArchitecturalElement,
        //                measArchitecturalElementIdFk = fime.fimeArchitecturalElementIdFk,
        //                measChronostratigraphy = fime.fimeChronostratigraphy,
        //                measDate = fime.fimeDate,
        //                measDepositionalEnvironmentIdFk = fime.fimeDepositionalEnvironmentIdFk,
        //                measDevice = fime.fimeDevice,
        //                measDirection = fime.fimeDirection,
        //                measElevation = fime.fimeElevation,
        //                measFacies = fime.fimeFacies,
        //                measFaciesIdFk = fime.fimeFaciesIdFk,
        //                measLatitude = fime.fimeLatitude,
        //                measLithostratigraphy = fime.fimeLithostratigraphy,
        //                measLocalCoordinateX = fime.fimeLocalCoordinateX,
        //                measLocalCoordinateY = fime.fimeLocalCoordinateY,
        //                measLocalCoordinateZ = fime.fimeLocalCoordinateZ,
        //                measLongitude = fime.fimeLongitude,
        //                measObjectOfInvestigationIdFk = fime.fimeObjectOfInvestigation,
        //                measParameter = fime.fimType,
        //                measprjIdFk = fime.fimeprjIdFk,
        //                measProject = fime.fimeProject,
        //                measResultType = "Scalar",
        //                measType = "Field measurement",
        //                measUploadDate = DateTime.Now,
        //                measUploader = fime.fimeUploader,
        //                measUploaderId = fime.fimeUploaderId
        //            };

        //            _apirsDatabase.tblMeasurements.Add(measurement);
        //            _apirsDatabase.SaveChanges();

        //            switch(fime.fimType)
        //            {
        //                case "Total Gamma Ray":
        //                    tblTotalGammaRay total = _apirsDatabase.tblTotalGammaRays.Where(x => x.tgrfimeIdPk == fime.fimeIdPk).First();
        //                    _apirsDatabase.Database.ExecuteSqlCommand("UPDATE tblTotalGammaRay SET tgrfimeIdPk = " + measurement.measIdPk.ToString() + " WHERE tgrfimeIdPk =" + fime.fimeIdPk + ";");
        //                    break;
        //                case "Spectral Gamma Ray":
        //                    tblSpectralGammaRay total2 = _apirsDatabase.tblSpectralGammaRays.Where(x => x.sgrIdPk == fime.fimeIdPk).First();
        //                    _apirsDatabase.Database.ExecuteSqlCommand("UPDATE tblSpectralGammaRay SET sgrIdPk = " + measurement.measIdPk.ToString() + " WHERE sgrIdPk =" + fime.fimeIdPk + ";");
        //                    break;
        //                case "Magnetic Susceptibility":
        //                    tblSusceptibility total4 = _apirsDatabase.tblSusceptibilities.Where(x => x.susfimeIdPk == fime.fimeIdPk).First();
        //                    _apirsDatabase.Database.ExecuteSqlCommand("UPDATE tblSusceptibility SET susfimeIdPk = " + measurement.measIdPk.ToString() + " WHERE susfimeIdPk =" + fime.fimeIdPk + ";");
        //                    break;
        //                case "Sonic Log":

        //                    break;
        //                case "Temperature":
        //                    tblBoreholeTemperature total6 = _apirsDatabase.tblBoreholeTemperature.Where(x => x.btfimeIdFk == fime.fimeIdPk).First();
        //                    _apirsDatabase.Database.ExecuteSqlCommand("UPDATE tblBoreholeTemperature SET btfimeIdFk = " + measurement.measIdPk.ToString() + " WHERE btfimeIdFk =" + fime.fimeIdPk + ";");
        //                    break;
        //                case "Rock Quality Designation Index":
        //                    tblRockQualityDesignationIndex total8 = _apirsDatabase.tblRockQualityDesignationIndex.Where(x => x.rqdfimeIdFk == fime.fimeIdPk).First();
        //                    _apirsDatabase.Database.ExecuteSqlCommand("UPDATE tblRockQualityDesignationIndex SET rqdfimeIdFk = " + measurement.measIdPk.ToString() + " WHERE rqdfimeIdFk =" + fime.fimeIdPk + ";");
        //                    break;
        //                case "Structure Orientation":
        //                    tblStructureOrientation total10 = _apirsDatabase.tblStructureOrientations.Where(x => x.sofimeIdFk == fime.fimeIdPk).First();
        //                    _apirsDatabase.Database.ExecuteSqlCommand("UPDATE tblStructureOrientation SET sofimeIdFk = " + measurement.measIdPk.ToString() + " WHERE sofimeIdFk =" + fime.fimeIdPk + ";");

        //                    break;
        //            }

        //            _apirsDatabase.SaveChanges();

        //        }
        //        catch
        //        {

        //        }
        //    }

        //}

        //public void MergeLabMeasurements()
        //{
        //    List<tblLaboratoryMeasurement> laboratoryMeasurements = _apirsDatabase.tblLaboratoryMeasurements.Where(x => x.labmeIdPk >=6109).ToList();

        //    foreach (tblLaboratoryMeasurement labme in laboratoryMeasurements)
        //    {
        //        try
        //        {
        //            tblMeasurement measurement = new tblMeasurement()
        //            {
        //                measAnalyticalDeviceId = labme.labmeDevice == null ? 0 : _apirsDatabase.tblMeasuringDevices.Where(x => x.mdName == labme.labmeDevice).FirstOrDefault().mdIdPk,
        //                measDate = labme.labmeDate,
        //                measDevice = labme.labmeDevice,
        //                measDirection = "X",
        //                measParameter = labme.labmeParameter,
        //                measprjIdFk = labme.labmeprjIdFk,
        //                measProject = labme.labmeProjectName,
        //                measResultType = "Scalar",
        //                measType = "Laboratory measurement",
        //                measUploadDate = DateTime.Now,
        //                measUploader = labme.labmeUploader,
        //                measUploaderId = labme.labmeUploaderId,
        //                measRockSampleIdFk = labme.labmeSampleName
        //            };

        //            _apirsDatabase.tblMeasurements.Add(measurement);
        //            _apirsDatabase.SaveChanges();

        //            switch (labme.labmeParameter)
        //            {
        //                case "Axial compression":
        //                    _apirsDatabase.Database.ExecuteSqlCommand("UPDATE tblAxialCompression SET aclabmeIdFk = " + measurement.measIdPk.ToString() + " WHERE aclabmeIdFk =" + labme.labmeIdPk + ";");
        //                    break;
        //                case "Grain density":
        //                    _apirsDatabase.Database.ExecuteSqlCommand("UPDATE tblGrainDensity SET gdIdFk = " + measurement.measIdPk.ToString() + " WHERE gdIdFk =" + labme.labmeIdPk + ";");
        //                    break;
        //                case "Bulk density":
        //                    _apirsDatabase.Database.ExecuteSqlCommand("UPDATE tblBulkDensity SET bdIdFk = " + measurement.measIdPk.ToString() + " WHERE bdIdFk =" + labme.labmeIdPk + ";");
        //                    break;
        //                case "Porosity":
        //                    _apirsDatabase.Database.ExecuteSqlCommand("UPDATE tblEffectivePorosity SET porIdFk = " + measurement.measIdPk.ToString() + " WHERE porIdFk =" + labme.labmeIdPk + ";");
        //                    break;
        //                case "Grain size":
        //                    _apirsDatabase.Database.ExecuteSqlCommand("UPDATE tblGrainSize SET gslabmeIdFk = " + measurement.measIdPk.ToString() + " WHERE gslabmeIdFk =" + labme.labmeIdPk + ";");
        //                    break;
        //                case "Apparent permeability":
        //                    _apirsDatabase.Database.ExecuteSqlCommand("UPDATE tblApparentPermeability SET apermIdFk = " + measurement.measIdPk.ToString() + " WHERE apermIdFk =" + labme.labmeIdPk + ";");
        //                    break;
        //                case "Intrinsic permeability":
        //                    _apirsDatabase.Database.ExecuteSqlCommand("UPDATE tblIntrinsicPermeability SET inpeIdFk = " + measurement.measIdPk.ToString() + " WHERE inpeIdFk =" + labme.labmeIdPk + ";");
        //                    break;
        //                case "Isotope":
        //                    _apirsDatabase.Database.ExecuteSqlCommand("UPDATE tblIsotopes SET islabmeIdFk = " + measurement.measIdPk.ToString() + " WHERE islabmeIdFk =" + labme.labmeIdPk + ";");
        //                    break;
        //                case "Thermal conductivity":
        //                    _apirsDatabase.Database.ExecuteSqlCommand("UPDATE tblThermalConductivity SET tcIdFk = " + measurement.measIdPk.ToString() + " WHERE tcIdFk =" + labme.labmeIdPk + ";");
        //                    break;
        //                case "Thermal diffusivity":
        //                    _apirsDatabase.Database.ExecuteSqlCommand("UPDATE tblThermalDiffusivity SET tdIdFk = " + measurement.measIdPk.ToString() + " WHERE tdIdFk =" + labme.labmeIdPk + ";");
        //                    break;
        //                case "Resistivity":
        //                    _apirsDatabase.Database.ExecuteSqlCommand("UPDATE tblResistivity SET reslabmeIdFk = " + measurement.measIdPk.ToString() + " WHERE reslabmeIdFk =" + labme.labmeIdPk + ";");
        //                    break;
        //                case "Sonic wave velocity":
        //                    _apirsDatabase.Database.ExecuteSqlCommand("UPDATE tblSonicWave SET swIdFk = " + measurement.measIdPk.ToString() + " WHERE swIdFk =" + labme.labmeIdPk + ";");
        //                    break;
        //                case "X-Ray Fluorescence":
        //                    _apirsDatabase.Database.ExecuteSqlCommand("UPDATE tblXRayFluorescenceSpectroscopy SET xrfIdPk = " + measurement.measIdPk.ToString() + " WHERE xrfIdPk =" + labme.labmeIdPk + ";");
        //                    break;
        //            }

        //            _apirsDatabase.SaveChanges();

        //        }
        //        catch
        //        {

        //        }
        //    }

        //}

        void IDisposable.Dispose() { }
        public void Dispose() { }

        //Converts a null value to string
        private string NullToString(object Value)
        {

            // Value.ToString() allows for Value being DBNull, but will also convert int, double, etc.
            return Value == null ? "" : Value.ToString();
        }

        #endregion
    }

    //A class for a table join between object of investigation and drilling
    public class DrillingJoin
    {
        public int DrillingID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Length { get; set; }
        public string DrillingProcess { get; set; }
    }

    //A class for a table join between object of investigation and outcrops
    public class OutcropJoin
    {
        public int OutcropID { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Owner { get; set; }
        public string ActualConditions { get; set; }
    }

    public class LithostratigraphyUnion
    {
        public int Id { get; set; }
        public string ParentName { get; set; }
        public string Chronostratigraphy { get; set; }
        public string grName { get; set; }
        public string Hierarchy { get; set; }
        public string LithologicDescriptionShort { get; set; }
        public string BaseBoundary { get; set; }
        public string TopBoundary { get; set; }
        public Nullable<double> MeanThickness { get; set; }
        public Nullable<double> MaxThickness { get; set; }
        public string TypeLocality { get; set; }
        public string Countries { get; set; }
        public string States { get; set; }
        public string Notes { get; set; }
        public string Literature { get; set; }
        public Nullable<System.DateTime> DateDocumentation { get; set; }
        public int UploaderId { get; set; }
    }

    public class Chronostratigraphy
    {
        public int IdPk { get; set; }
        public string Name { get; set; }
        public Nullable<int> NumericalAgeLowerBoundary { get; set; }
        public Nullable<int> PlusMinus { get; set; }
    }
}