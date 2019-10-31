using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace GeoReVi
{
    public class DropDownButton : ToggleButton
    {
        // *** Dependency Properties *** 

        // *** Constructors *** 
        public DropDownButton()
        {
            // Bind the ToogleButton.IsChecked property to the drop-down's IsOpen property 
            Binding binding = new Binding("Menu.IsOpen");
            binding.Source = this;
            this.SetBinding(IsCheckedProperty, binding);
            DataContextChanged += (sender, args) =>
            {
                if (Menu != null)
                    Menu.DataContext = DataContext;
            };
        }

        // *** Properties *** 
        public ContextMenu Menu
        {
            get { return (ContextMenu)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }
        public static readonly DependencyProperty MenuProperty = DependencyProperty.Register("Menu", typeof(ContextMenu), typeof(DropDownButton), new UIPropertyMetadata(null, OnMenuChanged));

        private static void OnMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dropDownButton = (DropDownButton)d;
            var contextMenu = (ContextMenu)e.NewValue;
            contextMenu.DataContext = dropDownButton.DataContext;
        }


        // *** Overridden Methods *** 
        protected override void OnClick()
        {
            if (Menu != null)
            {
                // If there is a drop-down assigned to this button, then position and display it 
                Menu.PlacementTarget = this;
                Menu.Placement = PlacementMode.Bottom;
                Menu.IsOpen = true;
            }
        }
    }
}