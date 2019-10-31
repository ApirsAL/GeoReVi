using Caliburn.Micro;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace GeoReVi
{
    /// <summary>
    /// Interaktionslogik für ViewPanel3D.xaml
    /// </summary>
    public partial class ViewPanel3D : UserControl
    {

        #region Dependency properties

        //3D collection property as dependency property
        public BindableCollection<GeometryModel3D> ThreeDCollection { get { return (BindableCollection<GeometryModel3D>)GetValue(ThreeDCollectionProperty); } set { SetValue(ThreeDCollectionProperty, value); } }
        public static DependencyProperty ThreeDCollectionProperty = DependencyProperty.Register("ThreeDCollection", typeof(BindableCollection<GeometryModel3D>), typeof(ViewPanel3D), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDataChanged));

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ViewPanel3D()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        //private void OnViewportMouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    sliderZoom.Value -= (double)e.Delta / 1000;
        //}

        /// <summary>
        /// Event that is fired if the collection changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnDataChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var lc = sender as ViewPanel3D;
            var dc = e.NewValue as BindableCollection<GeometryModel3D>;

            if (dc != null) dc.CollectionChanged += lc.dc_CollectionChanged;
        }

        private void dc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(ThreeDCollection != null)
                foreach(GeometryModel3D threeDObject in this.ThreeDCollection)
                {
                    group.Children.Add(threeDObject);
                }
        }
        
        #endregion
    }
}
