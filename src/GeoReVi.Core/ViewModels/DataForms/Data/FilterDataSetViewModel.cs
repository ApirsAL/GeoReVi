using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GeoReVi
{
    public class FilterDataSetViewModel<T> : Screen
    {
        #region Public properties

        /// <summary>
        /// A set of filter operations
        /// </summary>
        private ObservableCollection<NumberFilterOperation<double>> numberFilterOperations = new ObservableCollection<NumberFilterOperation<double>>();
        public ObservableCollection<NumberFilterOperation<double>> NumberFilterOperations
        {
            get => this.numberFilterOperations;
            set
            {
                this.numberFilterOperations = value;
            }
        }

        /// <summary>
        /// Text filter variable
        /// </summary>
        private ObservableCollection<FilterElement> textFilters = new ObservableCollection<FilterElement>();
        public ObservableCollection<FilterElement> TextFilters
        {
            get
            {
                return this.textFilters;
            }
            set
            {
                this.textFilters = value;
                NotifyOfPropertyChange(() => this.textFilters);
            }
        }

        /// <summary>
        /// The original data set
        /// </summary>
        private BindableCollection<T> originalDataSet = new BindableCollection<T>();
        public BindableCollection<T> OriginalDataSet
        {
            get => originalDataSet;
            set
            {
                this.originalDataSet = value;
                NotifyOfPropertyChange(() => OriginalDataSet);
            }
        }

        /// <summary>
        /// The filtered data set
        /// </summary>
        private BindableCollection<T> filterDataSet = new BindableCollection<T>();
        public BindableCollection<T> FilterDataSet
        {
            get => filterDataSet;
            set
            {
                this.filterDataSet = value;
                NotifyOfPropertyChange(() => FilterDataSet);
            }
        }

        /// <summary>
        /// The original data table columns
        /// </summary>
        private ObservableCollection<string> dataTableColumnNames = new ObservableCollection<string>();
        public ObservableCollection<string> DataTableColumnNames
        {
            get => this.dataTableColumnNames;
            set
            {
                this.dataTableColumnNames = value;
                NotifyOfPropertyChange(() => DataTableColumnNames);
            }
        }

        /// <summary>
        /// Checks if the headers have to be converted beforehand
        /// </summary>
        public bool FromDatabase { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public FilterDataSetViewModel()
        {

        }

        /// <summary>
        /// Specific constructor
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="headers"></param>
        public FilterDataSetViewModel(BindableCollection<T> dataSet, BindableCollection<T> filteredDataSet, ObservableCollection<string> headers, bool fromDatabase = false)
        {
            DataTableColumnNames = headers;
            FilterDataSet = filteredDataSet;
            OriginalDataSet = dataSet;
            FromDatabase = fromDatabase;
        }

        #endregion

        #region Public methods
        ///Event that gets fired if the filter text was changed
        public async Task Filter()
        {
            if (TextFilters.Any(x => x.Text == ""))
            {
                FilterDataSet = OriginalDataSet;
                return;
            }
            else
            {
                FilterDataSet = new BindableCollection<T>();
            }
            

            //Filtering data based on the selection
            try
            {
                ObservableCollection<FilterElement> selectedFilters = new ObservableCollection<FilterElement>(TextFilters.Where(x => x.Selected == true));
                List<BindableCollection<T>> filteredDataSets = new List<BindableCollection<T>>();

                //Iterating over each filter and select the values
                for (int j = 0; j < selectedFilters.Count(); j++)
                {
                    filteredDataSets.Add(new BindableCollection<T>(CollectionHelper.Filter<T>(OriginalDataSet, selectedFilters[j].Text, selectedFilters[j].Contained).ToList()));
                }

                //Adding each data set to the filtered data set that is not included in it yet
                filteredDataSets.ForEach(x =>
                {
                    for(int i = 0; i<filteredDataSets.Count();i++)
                    {
                        x = new BindableCollection<T>(x.Intersect(filteredDataSets[i]));
                    }

                    FilterDataSet.AddRange(x.Where(y => !FilterDataSet.Contains(y)));
                });
            }
            catch (Exception e)
            {

            }
            finally
            {

            }

        }

        /// <summary>
        /// Adding a new text filter to the collection
        /// </summary>
        public void AddTextFilter()
        {
            try
            {
                TextFilters.Add(new FilterElement("", true, false));
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Removing all selected text filters from the filter list
        /// </summary>
        public void RemoveTextFilter()
        {
            try
            {
                ObservableCollection<FilterElement> selectedFilters = new ObservableCollection<FilterElement>(TextFilters.Where(x => x.Selected == true).ToList());
                for (int i = 0; i < selectedFilters.Count(); i++)
                    TextFilters.Remove(selectedFilters[i]);
            }
            catch
            {

            }
        }
        #endregion
    }

    public class FilterElement : PropertyChangedBase
    {
        #region Properties

        /// <summary>
        /// Filter text
        /// </summary>
        private string text = "";
        public string Text
        {
            get => this.text;
            set
            {
                this.text = value;
                NotifyOfPropertyChange(() => Text);
            }
        }

        /// <summary>
        /// Whether text should be contained in collection or not
        /// </summary>
        private bool contained = false;
        public bool Contained
        {
            get => this.contained;
            set
            {
                this.contained = value;
                NotifyOfPropertyChange(() => Contained);
            }
        }

        /// <summary>
        /// Checking wheter the object is selected or not
        /// </summary>
        private bool selected = false;
        public bool Selected
        {
            get => this.selected;
            set
            {
                this.selected = value;
                NotifyOfPropertyChange(() => Selected);
            }
        }

        /// <summary>
        /// Checks whether the data set should this filter additionally or not additionally
        /// </summary>
        private AndOr andOr = AndOr.And;
        public AndOr AndOr
        {
            get => this.andOr;
            set
            {
                this.andOr = value;
                NotifyOfPropertyChange(() => AndOr);
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="_text"></param>
        /// <param name="_contained"></param>
        /// <param name="_selected"></param>
        public FilterElement(string _text, bool _contained, bool _selected)
        {
            Text = _text;
            Contained = _contained;
            Selected = _selected;
        }

        #endregion
    }

    /// <summary>
    /// And-Or enum
    /// </summary>
    public enum AndOr
    {
        And = 1,
        Or = 2
    }
}
