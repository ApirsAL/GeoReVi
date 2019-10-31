using Caliburn.Micro;
using SQLite;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// Generic data repository interface for a class T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAllRepository<T> : IDisposable where T : class, new()
    {
        IEnumerable<T> GetModel();

        IEnumerable<T> GetModelByExpression(Expression<Func<T, bool>> filter);

        T GetModelById(int modelId);

        void InsertModel(T model);

        void DeleteModelById(int modelId);

        void DeleteRange(IEnumerable<T> modelsToDelete);

        void UpdateModel(T model, bool local);

        void UpdateOrAddModel(T model, bool local);

        void Save();
    }

    /// <summary>
    /// Generic data repository for the server apirs database
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApirsRepository<T> : IAllRepository<T> where T : class, new()
    {
        private ApirsDatabase _apirsDatabase;
        private DbSet<T> dbEntity;
        private static string dbFile = Path.Combine(Environment.CurrentDirectory, @"LocalDB\APIRSlocal.db");
        private SQLiteConnection _apirsLocalDatabase;


        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApirsRepository()
        {
            _apirsDatabase = new ApirsDatabase();
            dbEntity = _apirsDatabase.Set<T>();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApirsRepository(bool local)
        {
            _apirsLocalDatabase = new SQLiteConnection(dbFile);
            try
            {
                _apirsLocalDatabase.CreateTable<T>();
            }
            catch(Exception e)
            {

            }
        }
        #endregion

        #region Generic queries
        /// <summary>
        /// Deleting a model by its id
        /// </summary>
        /// <param name="modelId"></param>
        public void DeleteModelById(int modelId)
        {
            if (_apirsDatabase != null)
            {
                T model = dbEntity.Find(modelId);
                dbEntity.Remove(model);
            }
            else
            {
                _apirsLocalDatabase.Delete(_apirsLocalDatabase.Get<T>(modelId));
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
                return _apirsLocalDatabase.Table<T>();
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
                return _apirsLocalDatabase.Table<T>().Where(filter);
            }

        }

        /// <summary>
        /// Getting one model by id
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public T GetModelById(int modelId)
        {
            if (_apirsDatabase != null)
            {
                return dbEntity.Find(modelId);
            }
            else
            {
                return _apirsLocalDatabase.Get<T>(modelId);
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
            }
            else
            {
                _apirsLocalDatabase.Insert(model);
            }

        }

        /// <summary>
        /// Saving changes to the data context
        /// </summary>
        public void Save()
        {
            try
            {
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
        public void UpdateModel(T model, bool local = false)
        {
            if (!local)
            {
                _apirsDatabase.Entry(model).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                _apirsLocalDatabase.Update(model);
            }
        }

        /// <summary>
        /// Updating a model
        /// </summary>
        /// <param name="model"></param>
        public void UpdateModel(T model, int id, bool local = false)
        {
            if (!local)
            {
                T result = GetModelById(id);
                if (result != null)
                {
                    _apirsDatabase.Entry<T>(result).CurrentValues.SetValues(model);
                }
                //_apirsDatabase.Entry(model).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                _apirsLocalDatabase.Update(model);
            }
        }

        public void UpdateOrAddModel(T model, bool insert = false)
        {
            try
            {
                if (_apirsDatabase != null)
                {
                    try
                    {
                        UpdateModel(model);
                        Save();
                    }
                    catch
                    {
                        InsertModel(model);
                        Save();
                    }
                }
                else
                {
                    if (insert)
                        InsertModel(model);
                    else
                        UpdateModel(model, true);
                }

            }
            catch
            {

            }
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

        //Getting all lithostratigraphic units in a union query
        public IEnumerable<LithostratigraphyUnion> GetCompleteLithostratigraphy()
        {
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
        }

        //Getting all facies types based on a project in a foreach query
        public IEnumerable<tblFacy> GetFaciesByProject(IList<tblProject> projects)
        {
            IEnumerable<tblFacy> facyQuery;
            List<tblFacy> facyReturn = new List<tblFacy>();

            //Iterating through all participating projects and select all related rock samples and facies types
            foreach (tblProject prj in projects)
            {
                facyQuery = (from fac in _apirsDatabase.tblFacies
                             where fac.facprjIdFk == prj.prjIdPk
                             select fac).ToList();

                foreach (tblFacy facy in facyQuery)
                {
                    facyReturn.Add(facy);
                }
            }

            return facyReturn;
        }

        public IEnumerable<tblFacy> GetFaciesTypeByProject(IList<tblProject> projects, string parameter)
        {
            IEnumerable<tblFacy> query;
            List<tblFacy> output = new List<tblFacy>();

            foreach (tblProject prj in projects)
            {
                //querying all facies types which belong to the certain rock type and project
                query = ((from fac in _apirsDatabase.tblFacies
                          where fac.facprjIdFk == prj.prjIdPk && fac.facType == parameter
                          select fac).ToList());

                //Adding each facies type to the list
                foreach (tblFacy f in query)
                {
                    output.Add(f);
                }
            }

            return output;
        }

        public IEnumerable<tblFaciesLithostrat> GetLithostratigtaphyByFaciest(IList<tblFacy> fcs)
        {
            IEnumerable<tblFaciesLithostrat> query;
            List<tblFaciesLithostrat> output = new List<tblFaciesLithostrat>();

            foreach (tblFacy fc in fcs)
            {
                //querying all facies types which belong to the certain rock type and project
                query = ((from fac in _apirsDatabase.tblFaciesLithostrats
                          where fac.facIdFk == fc.facIdPk
                          select fac).ToList());

                //Adding each facies type to the list
                foreach (tblFaciesLithostrat f in query)
                {
                    output.Add(f);
                }
            }

            return output;
        }

        public IEnumerable<tblFaciesObservation> GetOccurenceByFacies(IList<tblFacy> fcs)
        {
            IEnumerable<tblFaciesObservation> query;
            List<tblFaciesObservation> output = new List<tblFaciesObservation>();

            foreach (tblFacy fc in fcs)
            {
                //querying all facies types which belong to the certain rock type and project
                query = ((from fac in _apirsDatabase.tblFaciesObservations
                          where fac.fofacIdFk == fc.facIdPk
                          select fac).ToList());

                //Adding each facies type to the list
                foreach (tblFaciesObservation f in query)
                {
                    output.Add(f);
                }
            }

            return output;
        }

        public IEnumerable<tblRockSample> GetRockSamplesByProject(IList<tblProject> projects, string parameter)
        {
            IEnumerable<tblRockSample> query;
            List<tblRockSample> output = new List<tblRockSample>();

            //Iterating through all participating projects and select all related rock samples and facies types
            foreach (tblProject prj in projects)
            {
                query = (from rs in _apirsDatabase.tblRockSamples
                         where rs.sampType.Contains(parameter) && rs.sampprjIdFk == prj.prjIdPk
                         select rs).ToList();

                foreach (tblRockSample rsp in query)
                {
                    output.Add(rsp);
                }
            }
            return output;
        }

        public IEnumerable<tblArchitecturalElement> GetArchitecturalElementsByProject(IList<tblProject> projects, string parameter)
        {
            IEnumerable<tblArchitecturalElement> query;
            List<tblArchitecturalElement> output = new List<tblArchitecturalElement>();

            foreach (tblProject prj in projects)
            {
                //querying all facies types which belong to the certain rock type and project
                query = ((from fac in _apirsDatabase.tblArchitecturalElements
                          where fac.aeprjIdFk == prj.prjIdPk && fac.aeType == parameter
                          select fac).ToList());

                //Adding each facies type to the list
                foreach (tblArchitecturalElement f in query)
                {
                    output.Add(f);
                }
            }

            return output;
        }

        public IEnumerable<tblArchitecturalElementLithostrat> GetLithostratigtaphyByArchitecturalElement(IList<tblArchitecturalElement> aes)
        {
            IEnumerable<tblArchitecturalElementLithostrat> query;
            List<tblArchitecturalElementLithostrat> output = new List<tblArchitecturalElementLithostrat>();

            foreach (tblArchitecturalElement ae in aes)
            {
                //querying all facies types which belong to the certain rock type and project
                query = ((from fac in _apirsDatabase.tblArchitecturalElementLithostrats
                          where fac.aeIdFk == ae.aeIdPk
                          select fac).ToList());

                //Adding each facies type to the list
                foreach (tblArchitecturalElementLithostrat f in query)
                {
                    output.Add(f);
                }
            }

            return output;
        }

        public IEnumerable<tblArchEleOccurence> GetOccurenceByArchitecturalElement(IList<tblArchitecturalElement> aes)
        {
            IEnumerable<tblArchEleOccurence> query;
            List<tblArchEleOccurence> output = new List<tblArchEleOccurence>();

            foreach (tblArchitecturalElement ae in aes)
            {
                //querying all facies types which belong to the certain rock type and project
                query = ((from fac in _apirsDatabase.tblArchEleOccurences
                          where fac.aeIdFk == ae.aeIdPk
                          select fac).ToList());

                //Adding each facies type to the list
                foreach (tblArchEleOccurence f in query)
                {
                    output.Add(f);
                }
            }

            return output;
        }

        public List<v_PictureStore> GetRockSampleImagesAsync(int id)
        {
            try
            {
                var query = (from pic in _apirsDatabase.v_PictureStore
                             join picsamp in _apirsDatabase.tblPictureRockSamples on pic.stream_id equals picsamp.picStreamIdFk
                             where picsamp.sampIdFk == id
                             select pic).ToList();

                return query;
            }
            catch
            {
                return new List<v_PictureStore>();
            }
        }

        public List<v_PictureStore> GetObjectImagesAsync(int id)
        {
            try
            {
                var query = (from pic in _apirsDatabase.v_PictureStore
                             join picsamp in _apirsDatabase.tblPictureObjectOfInvestigations on pic.stream_id equals picsamp.picStreamIdFk
                             where picsamp.ooiIdFk == id
                             select pic).ToList();

                return query;
            }
            catch
            {
                return new List<v_PictureStore>();
            }
        }

        public List<v_PictureStore> GetFaciesImagesAsync(int id)
        {
            try
            {
                var query = (from pic in _apirsDatabase.v_PictureStore
                             join picsamp in _apirsDatabase.tblPictureLithofacies on pic.stream_id equals picsamp.picStreamIdFk
                             where picsamp.lftIdFk == id
                             select pic).ToList();

                return query;
            }
            catch
            {
                return new List<v_PictureStore>();
            }
        }

        public List<v_PictureStore> GetArchitecturalElementImagesAsync(int id)
        {
            try
            {
                var query = (from pic in _apirsDatabase.v_PictureStore
                             join picsamp in _apirsDatabase.tblPictureArchitecturalElements on pic.stream_id equals picsamp.picStreamIdFk
                             where picsamp.aeIdFk == id
                             select pic).ToList();

                return query;
            }
            catch
            {
                return new List<v_PictureStore>();
            }
        }

        public List<v_FileStore> GetLabMeasurementFile(int id)
        {
            try
            {
                var query = (from pic in _apirsDatabase.v_FileStore
                             join picsamp in _apirsDatabase.tblFileLabMeasurements on pic.stream_id equals picsamp.picStreamIdFk
                             where picsamp.labmeIdFk == id
                             select pic).ToList();

                return query;
            }
            catch
            {
                return new List<v_FileStore>();
            }
        }
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

        void IDisposable.Dispose() { }
        public void Dispose() { }

        //Converts a null value to string
        private string NullToString(object Value)
        {

            // Value.ToString() allows for Value being DBNull, but will also convert int, double, etc.
            return Value == null ? "" : Value.ToString();

            // If this is not what you want then this form may suit you better, handles 'Null' and DBNull otherwise tries a straight cast
            // which will throw if Value isn't actually a string object.
            //return Value == null || Value == DBNull.Value ? "" : (string)Value;
        }

        #endregion
    }

    //A class for a table join between object of investigation and drilling
    public class DrillingJoin
    {
        public int DrillingID;
        public string Name;
        public string Type;
        public double Latitude;
        public double Longitude;
        public double Length;
        public string DrillingProcess;
    }

    //A class for a table join between object of investigation and outcrops
    public class OutcropJoin
    {
        public int OutcropID;
        public string Type;
        public string Name;
        public double Latitude;
        public double Longitude;
        public string Owner;
        public string ActualConditions;
    }
}