using Caliburn.Micro;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace GeoReVi
{
    /// <summary>
    /// A behaviour to bind the selected items in a listview to a BindableCollection of projects
    /// </summary>
    public class SelectedMeasPointsBehaviour : Behavior<ListView>
    {
        public static readonly DependencyProperty SelectedItemCollectionProperty =
                 DependencyProperty.Register("SelectedItemCollection", typeof(BindableCollection<Mesh>), typeof(SelectedMeasPointsBehaviour), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemsPropertyChanged));

        public BindableCollection<Mesh> SelectedItemCollection
        {
            get { return (BindableCollection<Mesh>)GetValue(SelectedItemCollectionProperty); }
            set { SetValue(SelectedItemCollectionProperty, value); }
        }

        private static ListView _listview;
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.SelectionChanged += OnSelectionChanged;
            _listview = AssociatedObject;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (this.AssociatedObject != null)
                this.AssociatedObject.SelectionChanged -= OnSelectionChanged;
            if (_sourceCollection != null)
                _sourceCollection.CollectionChanged -= faciesbeds_CollectionChanged;
        }

        private static bool _collectionSetFromSource;
        private static ObservableCollection<Mesh> _sourceCollection;
        private static void OnSelectedItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // invoked when the source property,
            //i.e. the entire ObservableCollection<DateTime> property is being set...
            _sourceCollection = e.NewValue as ObservableCollection<Mesh>;
            if (_sourceCollection != null)
            {
                _sourceCollection.CollectionChanged += faciesbeds_CollectionChanged;
                foreach (Mesh dt in _sourceCollection)
                {
                    _listview.SelectedItems.Add(dt);
                }
            }
        }

        private static void faciesbeds_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
      
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                if (e.AddedItems != null && e.AddedItems.Count > 0)
                {
                    IEnumerable<Mesh> addedBeds = e.AddedItems.OfType<Mesh>();
                    foreach (Mesh dt in addedBeds)
                        SelectedItemCollection.Add(dt);
                }

                if (e.RemovedItems != null && e.RemovedItems.Count > 0)
                {
                    IEnumerable<Mesh> removedBeds = e.RemovedItems.OfType<Mesh>();
                    foreach (Mesh dt in removedBeds)
                        SelectedItemCollection.Remove(dt);
                }
        }
    }
}
