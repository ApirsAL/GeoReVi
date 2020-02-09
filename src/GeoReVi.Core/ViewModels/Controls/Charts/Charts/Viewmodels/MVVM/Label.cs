using Caliburn.Micro;
using System.Windows.Media;
using System.Xml.Serialization;

namespace GeoReVi
{
    public class Label : PropertyChangedBase
    {
        #region Public properties

        //X coordinate
        private double x = 0;
        public double X
        {
            get => this.x;
            set
            {
                this.x = value;
                NotifyOfPropertyChange(() => X);
            }
        }

        //Y coordinate
        private double y = 0;
        public double Y
        {
            get => this.y;
            set
            {
                this.y = value;
                NotifyOfPropertyChange(() => Y);
            }
        }

        //Y coordinate
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
        /// Label brush
        /// </summary>
        private Brush labelColor = Brushes.Black;
        [XmlIgnore()]
        public Brush LabelColor
        {
            get => labelColor;
            set
            {
                this.labelColor = value;
                NotifyOfPropertyChange(() => LabelColor);

            }
        }


        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public Label()
        {

        }

        #endregion
    }
}
