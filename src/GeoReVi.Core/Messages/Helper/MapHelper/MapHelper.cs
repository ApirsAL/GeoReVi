using Microsoft.Maps.MapControl.WPF;
using System.Windows;
using System.Windows.Controls;

namespace GeoReVi
{
    /// <summary>
    /// Helper class for microsofts bing map
    /// </summary>
    public static class MapHelper
    {
        public static readonly DependencyProperty CenterProperty = DependencyProperty.RegisterAttached(
            "Center",
            typeof(Location),
            typeof(MapHelper),
            new PropertyMetadata(new Location(0,0), new PropertyChangedCallback(CenterChanged))
        );

        public static void SetCenter(DependencyObject obj, Location value)
        {
            obj.SetValue(CenterProperty, value);
        }

        public static Location GetCenter(DependencyObject obj)
        {
            return (Location)obj.GetValue(CenterProperty);
        }

        private static void CenterChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Map map = (Map)obj;
            if (map != null)
            {
                map.Children.Clear();
                try
                {
                    map.Center = (Location)args.NewValue;
                }
                catch
                {
                    map.Center = new Location(0,0);
                }

                map.ZoomLevel = 12;
                switch(map.Tag)
                {
                    //In case of objects of investigation
                    case "Outcrop":
                        var push = new Pushpin() { Location = (Location)obj.GetValue(CenterProperty) };
                        //Adding a style
                        push.SetResourceReference(Control.StyleProperty, "OutcropPushPin");
                        map.Children.Add(push);
                        break;
                    case "Drilling":
                        var push1 = new Pushpin() { Location = (Location)obj.GetValue(CenterProperty) };
                        //Adding a style
                        push1.SetResourceReference(Control.StyleProperty, "DrillingPushPin");
                        map.Children.Add(push1);
                        break;

                    //In case of rock samples
                    case "RockSample":
                        var push2 = new Pushpin() { Location = (Location)obj.GetValue(CenterProperty) };
                        //Adding a style
                        push2.SetResourceReference(Control.StyleProperty, "RockSamplePushPin");
                        map.Children.Add(push2);
                        break;
                }
            }
                
        }
    }
}
