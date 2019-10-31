using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace GeoReVi
{
    /// <summary>
    /// Interaktionslogik für ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl
    {

        #region Constructor
        public ColorPicker()
        {
            InitializeComponent();
        }
        #endregion

        #region Custom events
        // Create a custom routed event by first registering a RoutedEventID
        // This event uses the bubbling routing strategy
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorPicker));

        // Provide CLR accessors for the event
        public event RoutedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        // This method raises the Tap event
        void RaiseSelectionChangedEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ColorPicker.SelectionChangedEvent);
            RaiseEvent(newEventArgs);
        }

        //Raising the event if the selection from the combobox changed
        void superCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RaiseSelectionChangedEvent();
        }

        #endregion


        public Brush SelectedColor
        {
            get { return (Brush)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Brush), typeof(ColorPicker), new UIPropertyMetadata(null));
    }
}
