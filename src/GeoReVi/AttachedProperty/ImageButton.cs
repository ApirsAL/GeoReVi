using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GeoReVi
{

    public class ImageButton : Button
    {
        public ImageButton() : base() { }

        public ImageSource ImageSource
        {
            get { return base.GetValue(ImageSourceProperty) as ImageSource; }
            set { base.SetValue(ImageSourceProperty, value); }
        }
        public static readonly DependencyProperty ImageSourceProperty =
          DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageButton));
    }
}
