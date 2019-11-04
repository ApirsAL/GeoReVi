using Caliburn.Micro;
using System.Windows.Media;

namespace GeoReVi
{
    public class LegendObject : PropertyChangedBase
    {
        /// <summary>
        /// Label of the legend object
        /// </summary>
        private Label label = new Label();
        public Label Label
        {
            get => this.label;
            set
            {
                this.label = value;
                NotifyOfPropertyChange(() => Label);
            }
        }

        /// <summary>
        /// Line color
        /// </summary>
        private Rectangle2D rectangle = new Rectangle2D();
        public Rectangle2D Rectangle
        {
            get => this.rectangle;
            set
            {
                this.rectangle = value;
                NotifyOfPropertyChange(() => Rectangle);
            }
        }

        /// <summary>
        /// Fill color
        /// </summary>
        private Brush lineColor = Brushes.Black;
        public Brush LineColor
        {
            get => this.lineColor;
            set
            {
                this.lineColor = value;
                NotifyOfPropertyChange(() => LineColor);
            }
        }

        /// <summary>
        /// Type of symbol
        /// </summary>
        private SymbolTypeEnum symbol = SymbolTypeEnum.Dot;
        public SymbolTypeEnum Symbol
        {
            get => this.symbol;
            set
            {
                this.symbol = value;
                NotifyOfPropertyChange(() => Symbol);
            }
        }

    }
}
