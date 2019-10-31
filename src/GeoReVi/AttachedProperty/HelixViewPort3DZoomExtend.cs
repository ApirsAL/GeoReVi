using HelixToolkit.Wpf;
using System;
using System.Windows;

namespace GeoReVi
{
    /// <summary>
    /// An attached property that triggers the ZoomExtends method of a helix viewport 3D
    /// </summary>
    public static class HelixViewport3DZoomExtent
    {
        private static readonly Type OwnerType = typeof(HelixViewport3DZoomExtent);
        public static readonly DependencyProperty ZoomExtentsOnUpdateProperty = DependencyProperty.RegisterAttached("ZoomExtentsOnUpdate", typeof(bool), OwnerType, new PropertyMetadata(false, OnDataContextChanged));

        public static bool GetZoomExtentsOnUpdate(DependencyObject obj)
        {
            return (bool)obj.GetValue(ZoomExtentsOnUpdateProperty);
        }
        public static void SetZoomExtentsOnUpdate(DependencyObject obj, bool value)
        {
            obj.SetValue(ZoomExtentsOnUpdateProperty, value);
        }
        private static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewport = d as HelixViewport3D;
            if (viewport == null) return;
            if (viewport.DataContext == null) return;
            viewport.ZoomExtents();
        }
    }
}
