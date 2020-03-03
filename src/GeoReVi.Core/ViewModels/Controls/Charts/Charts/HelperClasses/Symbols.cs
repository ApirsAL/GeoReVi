using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System;

namespace GeoReVi
{
    /// <summary>
    /// A class to define symbols used in the application
    /// </summary>
    [Serializable]
    public class Symbols<T> : PropertyChangedBase 
        where T : class
    {
        #region Private members

        /// Type of the symbol
        private SymbolTypeEnum symbolType;

        /// Size of the sybmol
        private double symbolSize;

        //Border color of the symbol
        private Brush borderColor = Brushes.Black;

        //fill color of the symbol
        private Brush fillColor = Brushes.DarkBlue;

        //Thickness of the symbols border
        private double borderThickness;

        private BindableCollection<T> _a;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public Symbols()
        {
            FillColor = Brushes.Blue;
            SymbolType = SymbolTypeEnum.Dot;
            SymbolSize = 5;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Symbols(BindableCollection<T> a)
        {
            FillColor = Brushes.Blue;
            SymbolType = SymbolTypeEnum.Dot;
            SymbolSize = 5;
            _a = a;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The border thickness of the symbol
        /// </summary>
        public double BorderThickness
        {
            get
            {
                return borderThickness;
            }
            set
            {
                borderThickness = value;

                NotifyOfPropertyChange(() => BorderThickness);

            }
        }

        /// <summary>
        /// Border color of the symbol
        /// </summary>
        [XmlIgnore()]
        public Brush BorderColor
        {
            get
            {
                return borderColor;
            }
            set
            {
                value.Freeze();
                borderColor = value;
                NotifyOfPropertyChange(() => BorderColor);

            }
        }

        /// <summary>
        /// Fill color of the symbol
        /// </summary>
        [XmlIgnore()]
        public Brush FillColor
        {
            get
            {
                return fillColor;
            }
            set
            {
                value.Freeze();
                fillColor = value;
                NotifyOfPropertyChange(() => FillColor);
            }
        }
        
        /// <summary>
        /// Size of the symbol
        /// </summary>
        public double SymbolSize
        {
            get
            {
                return symbolSize;
            }
            set
            {
                symbolSize = value;

                NotifyOfPropertyChange(() => SymbolSize);

            }
        }

        //Opacity of the symbols
        private double opacity = 1;
        public double Opacity
        {
            get => this.opacity;
            set
            {
                if(value <= 1 && value >=0)
                    this.opacity = value;

                NotifyOfPropertyChange(() => Opacity);
            }
        }

        /// <summary>
        /// Type of the symbol
        /// </summary>
        [XmlIgnore]
        public SymbolTypeEnum SymbolType
        {
            get
            {
                return symbolType;
            }
            set
            {
                symbolType = value;
                
                NotifyOfPropertyChange(() => SymbolType);
            }
        }

        #endregion

        #region Public methods
        
        public void AddLabel(Canvas chartCanvas, LocationTimeValue pt)
        {
            TextBox tb = new TextBox();
            tb.Text = pt.Name;
            tb.Background = Brushes.Transparent;
            tb.Margin = new Thickness(0);
            tb.BorderThickness = new Thickness(0);
            //defines the z index for the ui element
            Canvas.SetZIndex(tb, 5);
            chartCanvas.Children.Add(tb);
            Canvas.SetLeft(tb, pt.X + 5);
            Canvas.SetTop(tb, pt.Y - 5);

        }

        #endregion

    }

    #region Enums

    #endregion
}

