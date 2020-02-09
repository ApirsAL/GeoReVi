using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    public abstract class EditorBaseClass : PropertyChangedBase
    {
        #region Properties

        /// <summary>
        /// Checks if the edit mode is running
        /// </summary>
        private bool editing = false;
        public bool Editing
        {
            get => this.editing;
            set
            {
                this.editing = value;

                if (value == false)
                    AddedPoints.Clear();

                NotifyOfPropertyChange(() => Editing);
            }
        }

        /// <summary>
        /// The type of editing
        /// </summary>
        private EditingTypeEnum editingType = EditingTypeEnum.AddPoints;
        public EditingTypeEnum EditingType
        {
            get => this.editingType;
            set
            {
                this.editingType = value;
                NotifyOfPropertyChange(() => EditingType);
            }
        }

        /// <summary>
        /// During editing mode added points
        /// </summary>
        private List<LocationTimeValue> addedPoints = new List<LocationTimeValue>();
        public List<LocationTimeValue> AddedPoints
        {
            get => this.addedPoints;
            set
            {
                this.addedPoints = value;
                NotifyOfPropertyChange(() => AddedPoints);
            }
        }

        #endregion

    }
}
