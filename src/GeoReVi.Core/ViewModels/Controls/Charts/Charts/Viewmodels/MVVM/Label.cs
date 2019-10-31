using Caliburn.Micro;

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
