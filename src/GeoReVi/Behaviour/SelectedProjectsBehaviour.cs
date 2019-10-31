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
    public class SelectedProjectsBehaviour : Behavior<ListView>
    {
        public static readonly DependencyProperty SelectedItemCollectionProperty =
                 DependencyProperty.Register("SelectedItemCollection", typeof(BindableCollection<tblProject>), typeof(SelectedProjectsBehaviour), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemsPropertyChanged));

        public BindableCollection<tblProject> SelectedItemCollection
        {
            get { return (BindableCollection<tblProject>)GetValue(SelectedItemCollectionProperty); }
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
        private static ObservableCollection<tblProject> _sourceCollection;
        private static void OnSelectedItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // invoked when the source property,
            //i.e. the entire ObservableCollection<DateTime> property is being set...
            _sourceCollection = e.NewValue as ObservableCollection<tblProject>;
            if (_sourceCollection != null)
            {
                _sourceCollection.CollectionChanged += faciesbeds_CollectionChanged;
                foreach (tblProject dt in _sourceCollection)
                {
                    _collectionSetFromSource = true;
                    _listview.SelectedItems.Add(dt);
                    _collectionSetFromSource = false;
                }
            }
        }

        private static void faciesbeds_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null && e.NewItems.Count > 0)
                    {
                        IEnumerable<tblProject> addedBeds = e.NewItems.OfType<tblProject>();
                        foreach (tblProject dt in addedBeds)
                        {
                            if (!_listview.SelectedItems.Contains(dt))
                            {
                                _collectionSetFromSource = true;
                                _listview.SelectedItems.Add(dt);
                                _collectionSetFromSource = false;
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null && e.OldItems.Count > 0)
                    {
                        IEnumerable<tblProject> removedBeds = e.OldItems.OfType<tblProject>();
                        foreach (tblProject dt in removedBeds)
                        {
                            if (_listview.SelectedItems.Contains(dt))
                            {
                                _collectionSetFromSource = true;
                                _listview.SelectedItems.Remove(dt);
                                _collectionSetFromSource = false;
                            }
                        }
                    }
                    break;
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_collectionSetFromSource)
            {
                if (e.AddedItems != null && e.AddedItems.Count > 0)
                {
                    IEnumerable<tblProject> addedBeds = e.AddedItems.OfType<tblProject>();
                    foreach (tblProject dt in addedBeds)
                        SelectedItemCollection.Add(dt);
                }

                if (e.RemovedItems != null && e.RemovedItems.Count > 0)
                {
                    IEnumerable<tblProject> removedBeds = e.RemovedItems.OfType<tblProject>();
                    foreach (tblProject dt in removedBeds)
                        SelectedItemCollection.Remove(dt);
                }
            }
        }
    }
}
