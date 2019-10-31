using Caliburn.Micro;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace GeoReVi.ViewModels.Filter
{
    public class GenericListFilterViewModel<T> : PropertyChangedBase where T : class
    {
        #region Public properties

        /// <summary>
        /// The imported data set
        /// </summary>
        private BindableCollection<T> list = new BindableCollection<T>();
        public BindableCollection<T> List
        {
            get => this.list;
            set
            {
                this.list = value;
                NotifyOfPropertyChange(() => List);
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

        #endregion

        #region Constructor

        [ImportingConstructor]
        public GenericListFilterViewModel(BindableCollection<T> _list)
        {
            List = _list;
        }


        #endregion

        #region Public methods

        #endregion
    }
}
