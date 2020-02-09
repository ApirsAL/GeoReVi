using System.Windows.Media.Media3D;
using System.Xml.Serialization;

namespace GeoReVi
{
    public class ThreeDEditor : EditorBaseClass
    {
        #region Properties

        /// <summary>
        /// The position of the cursor on the element
        /// </summary>
        private Point3D? cursorOnElementPosition = new Point3D();
        [XmlIgnore]
        public Point3D? CursorOnElementPosition
        {
            get => this.cursorOnElementPosition;
            set
            {
                this.cursorOnElementPosition = value;
                NotifyOfPropertyChange(() => CursorOnElementPosition);
            }
        }

        /// <summary>
        /// Number of vertical cells that should be produced when extracting a feature
        /// </summary>
        private int verticalCellCount = 100;
        public int VerticalCellCount
        {
            get => this.verticalCellCount;
            set
            {
                this.verticalCellCount = value;
                NotifyOfPropertyChange(() => VerticalCellCount);
            }
        }

        /// <summary>
        /// Number of horizontal cells that should be produced when extracting a feature
        /// </summary>
        private int horizontalCellCount = 100;
        public int HorizontalCellCount
        {
            get => this.horizontalCellCount;
            set
            {
                this.horizontalCellCount = value;
                NotifyOfPropertyChange(() => HorizontalCellCount);
            }
        }

        #endregion

        #region Constructor

        #endregion
    }
}
