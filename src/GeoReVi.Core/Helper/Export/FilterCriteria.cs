using Caliburn.Micro;
using Rh.DateRange.Picker.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GeoReVi
{
    public class FilterCriteria : PropertyChangedBase
    {
        #region Properties

        /// <summary>
        /// variable to check to show all objects
        /// </summary>
        private bool all;
        public bool All
        {
            get => this.all;
            set
            {
                this.all = value;
                NotifyOfPropertyChange(() => All);
            }
        }

        /// <summary>
        /// Variable the data set will be grouped by
        /// </summary>
        private string groupBy;
        public string GroupBy
        {
            get => this.groupBy;
            set
            {
                this.groupBy = value;
                NotifyOfPropertyChange(() => GroupBy);
            }
        }

        //Checks if values should be filtered by a date range
        private bool filterByDate = false;
        public bool FilterByDate
        {
            get => this.filterByDate;
            set
            {
                this.filterByDate = value;
                NotifyOfPropertyChange(() => FilterByDate);
            }
        }

        /// <summary>
        /// Lower time limit
        /// </summary>
        private DateTime? from = new DateTime(1900, 1, 1, 0, 0, 0);
        public DateTime? From
        {
            get => this.from;
            set
            {
                this.from = value;
                NotifyOfPropertyChange(() => From);
            }
        }

        /// <summary>
        /// Upper time limit
        /// </summary>
        private DateTime? to = DateTime.Now;
        public DateTime? To
        {
            get => this.to;
            set
            {
                this.to = value;
                NotifyOfPropertyChange(() => To);
            }
        }

        /// <summary>
        /// The kind of date time range
        /// </summary>
        private DateRangeKind? kind = DateRangeKind.Custom;
        public DateRangeKind? Kind
        {
            get => this.kind;
            set
            {
                this.kind = value;
                NotifyOfPropertyChange(() => Kind);
            }
        }

        /// <summary>
        /// Variable to select the global or local reference system
        /// </summary>
        private bool global;
        public bool Global
        {
            get => this.global;
            set
            {
                this.global = value;
                NotifyOfPropertyChange(() => Global);
            }
        }

        #endregion
    }
}