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
    public class SelectedLaboratoryMeasurementsBehaviour : Behavior<ListView>
    {

        public static readonly DependencyProperty SelectedItemCollectionProperty =
                  DependencyProperty.Register("SelectedItemCollection", typeof(BindableCollection<tblLaboratoryMeasurement>), typeof(SelectedLaboratoryMeasurementsBehaviour), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemsPropertyChanged));

        public BindableCollection<tblLaboratoryMeasurement> SelectedItemCollection
        {
            get { return (BindableCollection<tblLaboratoryMeasurement>)GetValue(SelectedItemCollectionProperty); }
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
                _sourceCollection.CollectionChanged -= selectedLabMeasurement_CollectionChanged;
        }

        private static bool _collectionSetFromSource;
        private static ObservableCollection<tblLaboratoryMeasurement> _sourceCollection;
        private static void OnSelectedItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // invoked when the source property,
            //i.e. the entire ObservableCollection<DateTime> property is being set...
            _sourceCollection = e.NewValue as ObservableCollection<tblLaboratoryMeasurement>;
            if (_sourceCollection != null)
            {
                _sourceCollection.CollectionChanged += selectedLabMeasurement_CollectionChanged;
                foreach (tblLaboratoryMeasurement dt in _sourceCollection)
                {
                    _collectionSetFromSource = true;
                    _listview.SelectedItems.Add(dt);
                    _collectionSetFromSource = false;
                }
            }
        }

        private static void selectedLabMeasurement_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null && e.NewItems.Count > 0)
                    {
                        IEnumerable<tblLaboratoryMeasurement> addedBeds = e.NewItems.OfType<tblLaboratoryMeasurement>();
                        foreach (tblLaboratoryMeasurement dt in addedBeds)
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
                        IEnumerable<tblLaboratoryMeasurement> removedBeds = e.OldItems.OfType<tblLaboratoryMeasurement>();
                        foreach (tblLaboratoryMeasurement dt in removedBeds)
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
                    IEnumerable<tblLaboratoryMeasurement> addedBeds = e.AddedItems.OfType<tblLaboratoryMeasurement>();
                    foreach (tblLaboratoryMeasurement dt in addedBeds)
                        SelectedItemCollection.Add(dt);
                }

                if (e.RemovedItems != null && e.RemovedItems.Count > 0)
                {
                    IEnumerable<tblLaboratoryMeasurement> removedBeds = e.RemovedItems.OfType<tblLaboratoryMeasurement>();
                    foreach (tblLaboratoryMeasurement dt in removedBeds)
                        SelectedItemCollection.Remove(dt);
                }
            }
        }
    }
}
