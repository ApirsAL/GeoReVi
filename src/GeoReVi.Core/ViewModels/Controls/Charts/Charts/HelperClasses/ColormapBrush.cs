using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using System.Xml.Serialization;

namespace GeoReVi
{
    [Serializable]
    public class ColormapBrush : PropertyChangedBase
    {
        #region Properties

        private int colormapLength = 64;
        private byte alphaValue = 255;
        private double ymin = 0;
        private double ymax = 10;
        private int ydivisions = 64;
        private ColormapBrushEnum colormapBrushType = ColormapBrushEnum.Jet;
        private string title = "Default";

        /// <summary>
        /// The type of colormap brush
        /// </summary>
        public ColormapBrushEnum ColormapBrushType
        {
            get { return colormapBrushType; }
            set
            {
                if (colormapBrushType != value)
                {
                    colormapBrushType = value;
                    CalculateColormapBrushes();
                }
                NotifyOfPropertyChange(() => ColormapBrushType);

            }
        }

        /// <summary>
        /// Title of the colormap
        /// </summary>
        public string Title
        {
            get => title;
            set
            {
                title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }

        /// <summary>
        /// An observable collection of the brushes used for the colormap
        /// </summary>
        private ObservableCollection<Rectangle2D> colormapBrushes = new ObservableCollection<Rectangle2D>();
        public ObservableCollection<Rectangle2D> ColormapBrushes
        {
            get => this.colormapBrushes;
            set
            {
                this.colormapBrushes = value;
                NotifyOfPropertyChange(() => ColormapBrushes);
            }
        }

        /// <summary>
        /// Length of the colormap
        /// </summary>
        public int ColormapLength
        {
            get => colormapLength;
            set
            {
                colormapLength = value;
                NotifyOfPropertyChange(() => ColormapLength);
            }
        }

        /// <summary>
        /// Count of subdivision labels
        /// </summary>
        private int labelSubdivisions = 6;
        public int LabelSubdivisions
        {
            get => labelSubdivisions;
            set
            {
                labelSubdivisions = value;

                NotifyOfPropertyChange(() => LabelSubdivisions);
            }
        }

        /// <summary>
        /// The labels for the Colorbar
        /// </summary>
        private ObservableCollection<Label> labels = new ObservableCollection<Label>();
        public ObservableCollection<Label> Labels
        {
            get => this.labels;
            set
            {
                this.labels = value;
                NotifyOfPropertyChange(() => Labels);
            }
        }

        /// <summary>
        /// Alpha value
        /// </summary>
        public byte AlphaValue
        {
            get => alphaValue;
            set
            {
                alphaValue = value;
                NotifyOfPropertyChange(() => AlphaValue);
            }
        }

        /// <summary>
        /// Min value
        /// </summary>
        public double Ymin
        {
            get => ymin;
            set
            {
                ymin = value;

                if (ymin != ymax)
                    CalculateColormapBrushes();

                NotifyOfPropertyChange(() => Ymin);
            }
        }

        /// <summary>
        /// Max value
        /// </summary>
        public double Ymax
        {
            get => ymax;
            set
            {
                ymax = value;

                if (ymin != ymax)
                    CalculateColormapBrushes();

                NotifyOfPropertyChange(() => Ymax);
            }
        }

        /// <summary>
        /// Count of Y divisions
        /// </summary>
        public int Ydivisions
        {
            get => ydivisions;
            set
            {
                ydivisions = value;

                CalculateColormapBrushes();
                NotifyOfPropertyChange(() => Ydivisions);
            }
        }

        /// <summary>
        /// Interpolate colormap
        /// </summary>
        private bool isLog = false;
        public bool IsLog
        {
            get => isLog;
            set
            {
                isLog = value;
                NotifyOfPropertyChange(() => IsLog);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ColormapBrush()
        {

        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="brush">ColormapBrush to copy</param>
        public ColormapBrush(ColormapBrush brush)
        {
            this.AlphaValue = Convert.ToByte(brush.AlphaValue);
            this.ColormapBrushes = brush.ColormapBrushes;
            this.ColormapBrushType = brush.ColormapBrushType;
            this.ColormapLength = Convert.ToInt32(brush.ColormapLength);
            this.Ydivisions = Convert.ToInt32(brush.Ydivisions);
            this.Ymax = Convert.ToDouble(brush.Ymax);
            this.Ymin = Convert.ToDouble(brush.Ymin);
        }

        #endregion

        public byte[,] CalculateColormapBrushes(double opacity = 1)
        {
            byte[,] cmap = new byte[ColormapLength, 4];
            double[] array = new double[ColormapLength];

            switch (ColormapBrushType)
            {
                case ColormapBrushEnum.Spring:
                    for (int i = 0; i < ColormapLength; i++)
                    {
                        array[i] = 1.0 * i / (ColormapLength - 1);
                        cmap[i, 0] = AlphaValue;
                        cmap[i, 1] = 255;
                        cmap[i, 2] = (byte)(255 * array[i]);
                        cmap[i, 3] = (byte)(255 - cmap[i, 2]);
                    }
                    break;

                case ColormapBrushEnum.Summer:
                    for (int i = 0; i < ColormapLength; i++)
                    {
                        array[i] = 1.0 * i / (ColormapLength - 1);
                        cmap[i, 0] = AlphaValue;
                        cmap[i, 1] = (byte)(255 * array[i]);
                        cmap[i, 2] = (byte)(255 * 0.5 * (1 + array[i]));
                        cmap[i, 3] = (byte)(255 * 0.4);
                    }
                    break;

                case ColormapBrushEnum.Autumn:
                    for (int i = 0; i < ColormapLength; i++)
                    {
                        array[i] = 1.0 * i / (ColormapLength - 1);
                        cmap[i, 0] = AlphaValue;
                        cmap[i, 1] = 255;
                        cmap[i, 2] = (byte)(255 * array[i]);
                        cmap[i, 3] = 0;
                    }
                    break;

                case ColormapBrushEnum.Winter:
                    for (int i = 0; i < ColormapLength; i++)
                    {
                        array[i] = 1.0 * i / (ColormapLength - 1);
                        cmap[i, 0] = AlphaValue;
                        cmap[i, 1] = 0;
                        cmap[i, 2] = (byte)(255 * array[i]);
                        cmap[i, 3] = (byte)(255 * (1.0 - 0.5 * array[i]));
                    }
                    break;

                case ColormapBrushEnum.Gray:
                    for (int i = 0; i < ColormapLength; i++)
                    {
                        array[i] = 1.0 * i / (ColormapLength - 1);
                        cmap[i, 0] = AlphaValue;
                        cmap[i, 1] = (byte)(255 * array[i]);
                        cmap[i, 2] = (byte)(255 * array[i]);
                        cmap[i, 3] = (byte)(255 * array[i]);
                    }
                    break;

                case ColormapBrushEnum.Jet:
                    int n = (int)Math.Ceiling(ColormapLength / 4.0);

                    double[,] cMatrix = new double[ColormapLength, 3];

                    int nMod = 0;

                    double[] array1 = new double[3 * n - 1];

                    int[] red = new int[array1.Length];

                    int[] green = new int[array1.Length];

                    int[] blue = new int[array1.Length];

                    if (ColormapLength % 4 == 1)
                        nMod = 1;

                    for (int i = 0; i < array1.Length; i++)
                    {
                        if (i < n)
                            array1[i] = (i + 1.0) / n;
                        else if (i >= n && i < 2 * n - 1)
                            array1[i] = 1.0;
                        else if (i >= 2 * n - 1)
                            array1[i] = (3.0 * n - 1.0 - i) / n;
                        green[i] = (int)Math.Ceiling(n / 2.0) - nMod + i;

                        red[i] = green[i] + n;

                        blue[i] = green[i] - n;
                    }

                    int nb = 0;
                    for (int i = 0; i < blue.Length; i++)
                    {
                        if (blue[i] > 0)
                            nb++;
                    }

                    for (int i = 0; i < ColormapLength; i++)
                    {
                        for (int j = 0; j < red.Length; j++)
                        {
                            if (i == red[j] && red[j] < ColormapLength)
                                cMatrix[i, 0] = array1[i - red[0]];
                        }
                        for (int j = 0; j < green.Length; j++)
                        {
                            if (i == green[j] && green[j] < ColormapLength)
                                cMatrix[i, 1] = array1[i - green[0]];
                        }
                        for (int j = 0; j < blue.Length; j++)
                        {
                            if (i == blue[j] && blue[j] >= 0)
                                cMatrix[i, 2] = array1[array1.Length - 1 - nb + i];
                        }
                    }

                    for (int i = 0; i < ColormapLength; i++)
                    {
                        cmap[i, 0] = AlphaValue;
                        for (int j = 0; j < 3; j++)
                            cmap[i, j + 1] = (byte)(cMatrix[i, j] * 255);
                    }
                    break;

                case ColormapBrushEnum.Hot:
                    int n1 = (int)3 * ColormapLength / 8;
                    double[] red1 = new double[ColormapLength];
                    double[] green1 = new double[ColormapLength];
                    double[] blue1 = new double[ColormapLength];
                    for (int i = 0; i < ColormapLength; i++)
                    {
                        if (i < n1)
                            red1[i] = 1.0 * (i + 1.0) / n1;
                        else
                            red1[i] = 1.0;
                        if (i < n1)
                            green1[i] = 0.0;
                        else if (i >= n1 && i < 2 * n1)
                            green1[i] = 1.0 * (i + 1 - n1) / n1;
                        else
                            green1[i] = 1.0;
                        if (i < 2 * n1)
                            blue1[i] = 0.0;
                        else
                            blue1[i] = 1.0 * (i + 1 - 2 * n1) / (ColormapLength - 2 * n1);

                        cmap[i, 0] = AlphaValue;
                        cmap[i, 1] = (byte)(255 * red1[i]);
                        cmap[i, 2] = (byte)(255 * green1[i]);
                        cmap[i, 3] = (byte)(255 * blue1[i]);
                    }
                    break;

                case ColormapBrushEnum.Cool:
                    for (int i = 0; i < ColormapLength; i++)
                    {
                        array[i] = 1.0 * i / (ColormapLength - 1);
                        cmap[i, 0] = AlphaValue;
                        cmap[i, 1] = (byte)(255 * array[i]);
                        cmap[i, 2] = (byte)(255 * (1 - array[i]));
                        cmap[i, 3] = 255;
                    }
                    break;
            }

            return cmap;
        }
    }


    [Serializable]
    public enum ColormapBrushEnum
    {
        [Description("Spring")]
        Spring = 0,
        [Description("Summer")]
        Summer = 1,
        [Description("Autumn")]
        Autumn = 2,
        [Description("Winter")]
        Winter = 3,
        [Description("Gray")]
        Gray = 4,
        [Description("Jet")]
        Jet = 5,
        [Description("Hot")]
        Hot = 6,
        [Description("Cool")]
        Cool = 7
    }

    public class Rectangle2D : PropertyChangedBase
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

        /// <summary>
        /// Height of the rectangle
        /// </summary>
        private double height = 0;
        public double Height
        {
            get => this.height;
            set
            {
                this.height = value;
                NotifyOfPropertyChange(() => Height);
            }
        }

        /// <summary>
        /// Width of the rectangle
        /// </summary>
        private double width = 0;
        public double Width
        {
            get => this.width;
            set
            {
                this.width = value;
                NotifyOfPropertyChange(() => Width);
            }
        }

        //Brush
        private Brush brush = Brushes.Black;
        [XmlIgnore]
        public Brush Brush
        {
            get => this.brush;
            set
            {
                this.brush = value;
                this.brush.Freeze();
                NotifyOfPropertyChange(() => Brush);
            }
        }


        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public Rectangle2D()
        {

        }

        #endregion
    }
}