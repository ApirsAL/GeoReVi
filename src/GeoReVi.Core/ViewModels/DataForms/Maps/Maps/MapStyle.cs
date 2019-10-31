using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Design;
using System;
using System.Collections.Generic;
using Caliburn.Micro;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using Microsoft.SqlServer.Types;
using System.Data;

namespace GeoReVi
{
    public class MapStyle : Screen
    {
        #region Private members

        /// <summary>
        /// Event aggregator for event subscription to communicate with other viewmodels
        /// </summary>
        private IEventAggregator _events;

        private List<Pushpin> pushpinList = new List<Pushpin>();
        private List<Pushpin> pushpinListRemove = new List<Pushpin>();
        private List<MapPolyline> polylineList = new List<MapPolyline>();
        private List<MapPolyline> polylineListRemove = new List<MapPolyline>();

        /// <summary>
        /// The actual bing map object
        /// </summary>
        private Map mMap;

        /// <summary>
        /// A tile overlay layer
        /// </summary>
        private MapTileLayer tileLayer = new MapTileLayer();

        /// <summary>
        /// A layer for gridlines
        /// </summary>
        private MapLayer gridLineLayer = new MapLayer();

        //Coordinates string
        private string coordinates;

        /// <summary>
        /// Updating and selection booleans
        /// </summary>
        private bool updating = false;
        private bool selectionMode = false;
        private bool selectionRunning = false;
        private bool zoomingMode = false;
        private bool zoomingRunning = false;
        private bool areGridLinesVisible = false;

        /// <summary>
        /// Gridline properties
        /// </summary>
        private double stepWidth = 1;
        private double thickness = 1;
        private double opacity = 1;

        #endregion

        #region Public properties

        /// <summary>
        /// A pushpin geometry
        /// </summary>
        public Pushpin Pushpin
        {
            get;
            set;
        }

        /// <summary>
        /// A List of pushpins we want to add to a map
        /// </summary>
        public List<Pushpin> PushpinList
        {
            get
            {
                return this.pushpinList;
            }

            set
            {
                this.pushpinList = value;
            }
        }

        /// <summary>
        /// A pushpin geometry
        /// </summary>
        public MapPolyline PolyLine
        {
            get;
            set;
        }

        /// <summary>
        /// A List of pushpins we want to add to a map
        /// </summary>
        public List<MapPolyline> PolylineList
        {
            get
            {
                return this.polylineList;
            }

            set
            {
                this.polylineList = value;
            }
        }

        /// <summary>
        /// The map control
        /// </summary>
        public Map BingMap
        {
            get
            {
                return this.mMap;
            }
            set
            {
                this.mMap = value;
                NotifyOfPropertyChange(() => BingMap);
            }
        }

        /// <summary>
        /// Checks if zooming is running
        /// </summary>
        public bool ZoomingRunning
        {
            get { return this.zoomingRunning; }
            set
            {
                this.zoomingRunning = value;
                NotifyOfPropertyChange(() => ZoomingRunning);
            }
        }

        /// <summary>
        /// Checks if a selection is running and if the selection mode is enabled
        /// </summary>
        public bool SelectionMode
        {
            get => this.selectionMode;
            set
            {
                this.selectionMode = value;
                if (SelectionMode)
                    Mouse.OverrideCursor = Cursors.Cross;
                else
                    Mouse.OverrideCursor = Cursors.Arrow;
                NotifyOfPropertyChange(() => SelectionMode);
            }
        }
        public bool SelectionRunning
        {
            get { return this.selectionRunning; }
            set
            {
                this.selectionRunning = value;
                NotifyOfPropertyChange(() => SelectionRunning);
            }
        }

        /// <summary>
        /// Gridline properties
        /// </summary>
        /// 

        /// <summary>
        /// Gridline properties
        /// </summary>
        public double StepWidth
        {
            get => this.stepWidth;
            set
            {
                if (value > 0 && value < 180)
                    this.stepWidth = value;
                else
                    return;

                NotifyOfPropertyChange(() => StepWidth);
                DrawGridLines();
            }
        }
        public double Thickness
        {
            get => this.thickness;
            set { this.thickness = value; NotifyOfPropertyChange(() => Thickness); DrawGridLines(); } }
        public double Opacity { get => this.opacity; set { this.opacity = value; NotifyOfPropertyChange(() => Opacity); DrawGridLines(); } }
        public bool AreGridLinesVisible
        {
            get => this.areGridLinesVisible;
            set
            {
                this.areGridLinesVisible = value;
                NotifyOfPropertyChange(() => AreGridLinesVisible);
                AddOrRemoveGridLines();
            }
        } 

        /// <summary>
        /// Checks if a zooming is running and if the selection mode is enabled
        /// </summary>
        public bool ZoomingMode
        {
            get { return this.zoomingMode; }
            set
            {
                this.zoomingMode = value;
                if (ZoomingMode)
                    Mouse.OverrideCursor = Cursors.Cross;
                else
                    Mouse.OverrideCursor = Cursors.Arrow;

                NotifyOfPropertyChange(() => ZoomingMode);
            }
        }


        //Coordinates string
        public string Coordinates
        {
            get
            {
                return this.coordinates;
            }
            set
            {
                this.coordinates = value;
                NotifyOfPropertyChange(() => Coordinates);
            }
        }

        /// <summary>
        /// Selection properties
        /// </summary>
        public Location SelectionStartLocation
        { get; set; }
        public Location SelectionEndLocation
        { get; set; }

        //A selection rectangle drawn at runtime
        public MapPolygon SelectionRectangle { get; set; }

        /// <summary>
        /// Current ZoomLevel of the map
        /// </summary>
        public double MapZoomLevel
        {
            get
            {
                return this.BingMap.ZoomLevel;
            }
            set
            {
                this.BingMap.ZoomLevel = value;
                NotifyOfPropertyChange(() => MapZoomLevel);
            }
        }


        /// <summary>
        /// Showing if the interface is updating
        /// </summary>
        public bool Updating
        {
            get => this.updating;
            set
            {
                this.updating = value;
                NotifyOfPropertyChange(() => Updating);
            }
        }

        public MapPolygon Polygon { get; set; }

        /// <summary>
        /// The location converter we need for the map element
        /// </summary>
        public LocationConverter LocationConverter { get; set; }

        /// <summary>
        /// Eventhandler for mouse events of the map
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void MouseButtonEventHandler(object sender, EventArgs e);
        public delegate void MouseEventHandler(object sender, EventArgs e);
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MapStyle(Map _map, IEventAggregator events)
        {
            _events = events;

            BingMap = _map;
            BingMap.Children.Clear();

            BingMap.MouseLeftButtonDown += SelectionStart;
            BingMap.MouseMove += SelectionMove;
            BingMap.MouseLeftButtonUp += SelectionEnd;

            BingMap.Center = new Location(47.751569, 1.675063);
            MapZoomLevel = 5;
            Polygon = new MapPolygon();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Defining pushpin locations, styles and tooltips and adding them to the map
        /// </summary>
        private void DefinePushpinLocation(string elementName, IEnumerable<tblRockSample> rockSamples)
        {
            PushpinList.RemoveAll(pushpin => pushpin.Name == elementName);

            foreach (var samp in rockSamples)
            {
                try
                {
                    Pushpin = new Pushpin();
                    Pushpin.Location = new Location((double)samp.sampLatitude, (double)(samp.sampLongitude));
                    Pushpin.Name = elementName;
                    Pushpin.Title = samp.sampLabel;
                    Pushpin.Description = "Sample label: " + samp.sampLabel + Environment.NewLine +
                                            "Latitude: " + samp.sampLatitude + Environment.NewLine +
                                            "Longitude: " + samp.sampLongitude + Environment.NewLine +
                                            "Petrography: " + samp.sampPetrographicTerm;

                    if (Pushpin.Location.Latitude == 0 && Pushpin.Location.Longitude == 0
                        && pushpinList.Where(x => x.Location.Latitude == samp.sampLatitude
                        && x.Location.Longitude == samp.sampLongitude
                        && x.Name == elementName)
                        .Count() > 0)
                    {
                        continue;
                    }

                    //Adding a style
                    Pushpin.SetResourceReference(Control.StyleProperty, "RockSamplePushPin");

                    Pushpin.ToolTip = new ToolTip()
                    {
                        DataContext = Pushpin,
                        Style = Application.Current.Resources["CustomInfoboxStyle"] as Style
                    };

                    Pushpin.MouseDown += ClickOnPushpin;

                    PushpinList.Add(Pushpin);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

            }
        }

        /// <summary>
        /// Defining pushpin locations, styles and tooltips and adding them to the map
        /// </summary>
        private void DefinePushpinLocation(string elementName, IEnumerable<DrillingJoin> drillings)
        {
            PushpinList.RemoveAll(pushpin => pushpin.Name == elementName);

            foreach (var drill in drillings)
            {
                try
                {
                    Pushpin = new Pushpin();
                    Pushpin.Location = new Location((double)drill.Latitude, (double)(drill.Longitude));
                    Pushpin.Name = elementName;
                    Pushpin.Title = drill.Name;
                    Pushpin.Description = "Name: " + drill.Name + Environment.NewLine +
                                            "Latitude: " + drill.Latitude + Environment.NewLine +
                                            "Longitude: " + drill.Longitude + Environment.NewLine +
                                            "Length: " + drill.Length;

                    if (Pushpin.Location.Latitude == 0 && this.Pushpin.Location.Longitude == 0)
                    {
                        continue;
                    }

                    //Adding a style
                    Pushpin.SetResourceReference(Control.StyleProperty, "DrillingPushPin");
                    Pushpin.Height = Pushpin.Height + drill.Length / 1000;
                    Pushpin.Width = Pushpin.Width + drill.Length / 1000;

                    Pushpin.ToolTip = new ToolTip()
                    {
                        DataContext = Pushpin,
                        Style = Application.Current.Resources["CustomInfoboxStyle"] as Style
                    };

                    PushpinList.Add(Pushpin);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
            }
        }

        /// <summary>
        /// Defining pushpin locations, styles and tooltips and adding them to the map
        /// </summary>
        private void DefinePushpinLocation(string elementName, IEnumerable<OutcropJoin> outcrops)
        {
            PushpinList.RemoveAll(pushpin => pushpin.Name == elementName);

            foreach (var outc in outcrops)
            {
                try
                {
                    Pushpin = new Pushpin();
                    Pushpin.Location = new Location((double)outc.Latitude, (double)(outc.Longitude));
                    Pushpin.Name = elementName;

                    Pushpin.Title = outc.Name;
                    Pushpin.Description = "Name: " + outc.Name + Environment.NewLine +
                                            "Latitude: " + outc.Latitude + Environment.NewLine +
                                            "Longitude: " + outc.Longitude + Environment.NewLine +
                                            "Owner: " + outc.Owner + Environment.NewLine +
                                            "Actual conditions: " + outc.ActualConditions;

                    if (this.Pushpin.Location.Latitude == 0 && Pushpin.Location.Longitude == 0)
                    {
                        continue;
                    }
                    //Adding a style
                    Pushpin.SetResourceReference(Control.StyleProperty, "OutcropPushPin");

                    Pushpin.ToolTip = new ToolTip()
                    {
                        DataContext = Pushpin,
                        Style = Application.Current.Resources["CustomInfoboxStyle"] as Style
                    };

                    PushpinList.Add(this.Pushpin);
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
            }
        }

        /// <summary>
        /// Defining pushpin locations, styles and tooltips and adding them to the map
        /// </summary>
        private void DefinePushpinLocation(string elementName, IEnumerable<tblTransect> transects)
        {
            PolylineList.RemoveAll(polyline => polyline.Name == elementName);

            foreach (var tran in transects)
            {
                try
                {
                    PolyLine = new MapPolyline();
                    PolyLine.Locations = new LocationCollection() { new Location((double)tran.traLatNorthEnd, (double)(tran.traLongNorthEnd)),
                                                                      new Location((double)tran.traLatSouthEnd, (double)(tran.traLongSouthEnd)) };
                    PolyLine.Stroke = Brushes.Red;
                    PolyLine.StrokeThickness = 2;
                    PolyLine.Name = elementName;

                    PolyLine.Title = tran.traName;
                    PolyLine.Description = "Name: " + tran.traName + Environment.NewLine +
                                           "Type: " + tran.traType + Environment.NewLine +
                                           "Date: " + tran.traProductionDate + Environment.NewLine;

                    PolyLine.ToolTip = new ToolTip()
                    {
                        DataContext = PolyLine,
                        Style = Application.Current.Resources["CustomInfoboxStyle"] as Style
                    };

                    PolylineList.Add(PolyLine);

                }

                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
            }
        }

        /// <summary>
        /// Defining pushpin locations, styles and tooltips and adding them to the map
        /// </summary>
        private void DefinePushpinLocation(string elementName, IEnumerable<tblMeasurement> measurement)
        {
            PushpinList.RemoveAll(pushpin => pushpin.Name == elementName);

            foreach (var meas in measurement)
            {
                try
                {
                    Pushpin = new Pushpin();
                    Pushpin.Location = new Location((double)meas.measLatitude, (double)(meas.measLongitude));
                    Pushpin.Name = elementName;

                    Pushpin.Title = elementName;
                    Pushpin.Description = "Type: " + meas.measType + Environment.NewLine +
                                            "Latitude: " + (double)meas.measLatitude + Environment.NewLine +
                                            "Longitude: " + (double)meas.measLongitude + Environment.NewLine;
                                            //"Dip direction [°]: " + (double)meas.fimeValue1 + Environment.NewLine +
                                            //"Dip angle [°]: " + (double)meas.fimeValue2;

                    if (this.Pushpin.Location.Latitude == 0 && Pushpin.Location.Longitude == 0)
                    {
                        continue;
                    }
                    switch(elementName)
                    {
                        case "PalaeoFlow":
                            //Adding a style
                            Pushpin.SetResourceReference(Control.StyleProperty, "PalaeoFlowPushPin");
                            break;
                    }

                    Pushpin.ToolTip = new ToolTip()
                    {
                        DataContext = Pushpin,
                        Style = Application.Current.Resources["CustomInfoboxStyle"] as Style
                    };

                    //Pushpin.LayoutTransform = new RotateTransform((double)meas.fimeValue1);

                    PushpinList.Add(this.Pushpin);
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
            }
        }

        /// <summary>
        /// Adding the pushpins in the pushpinlist to the map
        /// </summary>
        public void AddElementToMap(IEnumerable<tblRockSample> rockSamples)
        {
            try
            {
                new DispatchService().Invoke(() =>
                {
                    DefinePushpinLocation("RockSample", rockSamples);

                    PushpinList.ForEach(i =>
                    {
                        if (i.Name == "RockSample")
                            BingMap.Children.Add(i);
                    });
                });
            }
            catch
            {

            }
        }
        /// <summary>
        /// Adding the pushpins in the pushpinlist to the map
        /// </summary>
        public void AddElementToMap(IEnumerable<DrillingJoin> drillings)
        {
            try
            {
                new DispatchService().Invoke(() =>
                {

                    DefinePushpinLocation("Drillings", drillings);

                    PushpinList.ForEach(i =>
                    {
                        if (i.Name == "Drillings")
                            BingMap.Children.Add(i);
                    });

                });
            }
            catch
            {

            }
        }
        /// <summary>
        /// Adding the pushpins in the pushpinlist to the map
        /// </summary>
        public void AddElementToMap(IEnumerable<tblTransect> transects)
        {
            try
            {
                new DispatchService().Invoke(() =>
                {

                    DefinePushpinLocation("Transects", transects);

                    PolylineList.ForEach(i =>
                    {
                        if (i.Name == "Transects")
                            BingMap.Children.Add(i);
                    });

                });
            }
            catch(Exception e)
            {

            }
        }
        /// <summary>
        /// Adding the pushpins in the pushpinlist to the map
        /// </summary>
        public void AddElementToMap(IEnumerable<OutcropJoin> outcrops)
        {
            try
            {
                new DispatchService().Invoke(() =>
                {
                    DefinePushpinLocation("Outcrops", outcrops);

                    PushpinList.ForEach(i =>
                    {
                        if (i.Name == "Outcrops")
                            BingMap.Children.Add(i);
                    });
                });
            }
            catch
            { }
        }
        /// <summary>
        /// Adding the pushpins in the pushpinlist to the map
        /// </summary>
        public void AddElementToMap(IEnumerable<tblMeasurement> fieldMeasurements, string type)
        {
            try
            {
                new DispatchService().Invoke(() =>
                {
                    DefinePushpinLocation(type, fieldMeasurements);

                    PushpinList.ForEach(i =>
                    {
                        if (i.Name == "PalaeoFlow")
                            BingMap.Children.Add(i);
                    });
                });
            }
            catch
            {

            }
        }

        //Removes Pushpins from the map object by its name
        public void RemoveMapPushpinByName(string elementRemove)
        {
            try
            {
                new DispatchService().Invoke(() =>
                {
                    //removes all children of the map control that are pushpins with a name of "element"
                    var pushpin = new Pushpin();

                    //removes all children of the map control that are pushpins with a name of "RockSample"
                    foreach (UIElement e in BingMap.Children)
                    {
                        try
                        {
                            pushpin = (Pushpin)e;

                            if (pushpin.Name.Equals(elementRemove))
                            {
                                pushpinListRemove.Add(pushpin);
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    foreach (Pushpin p in pushpinListRemove)
                    {
                        try
                        {
                            BingMap.Children.Remove(p);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                });
            }
            catch
            {
            }

        }

        //Removes Pushpins from the map object by its name
        public void RemoveMapPolylineByName(string elementRemove)
        {
            try
            {
                new DispatchService().Invoke(() =>
                {
                    //removes all children of the map control that are pushpins with a name of "element"
                    var polyline = new MapPolyline();

                    //removes all children of the map control that are pushpins with a name of "RockSample"
                    foreach (UIElement e in BingMap.Children)
                    {
                        try
                        {
                            polyline = (MapPolyline)e;

                            if (polyline.Name.Equals(elementRemove))
                            {
                                polylineListRemove.Add(polyline);
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    foreach (MapPolyline p in polylineListRemove)
                    {
                        try
                        {
                            BingMap.Children.Remove(p);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                });
            }
            catch
            {
            }

        }

        /// <summary>
        /// Dynamically changning the map mode
        /// </summary>
        /// <param name="mapMode"></param>
        public void ChangeMapMode(string mapMode)
        {
            switch (mapMode)
            {
                case "Road":
                    BingMap.Mode = new RoadMode();
                    break;
                case "Aerial":
                    BingMap.Mode = new AerialMode();
                    break;
                case "AerialWithLabels":
                    BingMap.Mode = new AerialMode(true);
                    break;
                case "Mercator":
                    BingMap.Mode = new MercatorMode();
                    break;
            }
        }

        /// <summary>
        /// Starting the selection at the selection start point
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectionStart(object sender, MouseButtonEventArgs e)
        {
            //Zooming if zooming mode is enabled
            if (ZoomingMode == true)
            {
                //Starting the selection
                ZoomingRunning = true;
                //Defining the first selection point
                SelectionStartLocation = new Location(BingMap.ViewportPointToLocation(e.GetPosition(BingMap)));

                //Configuring the selection polygon
                SelectionRectangle = new MapPolygon();
                SelectionRectangle.Locations = new LocationCollection()
                {
                    SelectionStartLocation,
                    SelectionStartLocation,
                    SelectionStartLocation,
                    SelectionStartLocation,
                };
                SelectionRectangle.Fill = Brushes.Transparent;
                SelectionRectangle.Stroke = Brushes.Blue;
                SelectionRectangle.StrokeDashArray = new DoubleCollection() { 2 };
                SelectionRectangle.StrokeThickness = 2;

                //Adding the Selection Polygon to the map object
                BingMap.Children.Add(SelectionRectangle);

                e.Handled = true;
            }

            if (SelectionMode == false)
            {
                return;
            }
            else
            {
                //Starting the selection
                SelectionRunning = true;
                //Defining the first selection point
                SelectionStartLocation = new Location(BingMap.ViewportPointToLocation(e.GetPosition(BingMap)));

                //Configuring the selection polygon
                SelectionRectangle = new MapPolygon();
                SelectionRectangle.Locations = new LocationCollection()
                {
                    SelectionStartLocation,
                    SelectionStartLocation,
                    SelectionStartLocation,
                    SelectionStartLocation,
                };
                SelectionRectangle.Fill = Brushes.Transparent;
                SelectionRectangle.Stroke = Brushes.Red;
                SelectionRectangle.StrokeThickness = 3;

                //Adding the Selection Polygon to the map object
                BingMap.Children.Add(SelectionRectangle);

                e.Handled = true;
            }

        }

        /// <summary>
        /// Drawing a rectangle during selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectionMove(object sender, MouseEventArgs e)
        {

            Location currentLoc = GetMouseTouchLocation(e);

            Coordinates = "Latitude: " + Math.Round(currentLoc.Latitude, 6).ToString()
                + ", Longitude: " + Math.Round(currentLoc.Longitude, 6).ToString()
                + " (WGS84)";

            if (SelectionRunning || ZoomingRunning)
            {
                if (currentLoc != null)
                {
                    var firstLoc = SelectionRectangle.Locations[0];

                    //Update locations 1 - 3 of polygon so as to create a rectangle. 
                    SelectionRectangle.Locations[1] = new Location(firstLoc.Latitude, currentLoc.Longitude);
                    SelectionRectangle.Locations[2] = currentLoc;
                    SelectionRectangle.Locations[3] = new Location(currentLoc.Latitude, firstLoc.Longitude);
                }
            }
        }

        /// <summary>
        /// Ending the selection at the selection end point
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectionEnd(object sender, MouseButtonEventArgs e)
        {
            if (ZoomingMode)
            {
                //Defining the endpoint of the selection
                SelectionEndLocation = new Location(BingMap.ViewportPointToLocation(e.GetPosition(BingMap)));
                //ending selection mode and removing the rectangle from the map
                SelectionRunning = false;
                BingMap.Children.Remove(SelectionRectangle);
                SelectionMode = false;

                //Normalizing the selection endpoints
                NormalizeSelectionLocations();

                BingMap.SetView(new LocationRect(new Location(SelectionEndLocation.Latitude, SelectionStartLocation.Longitude), new Location(SelectionStartLocation.Latitude, SelectionEndLocation.Longitude)));

                ZoomingMode = false;
            }

            if (SelectionMode == false)
                return;

            //Defining the endpoint of the selection
            SelectionEndLocation = new Location(BingMap.ViewportPointToLocation(e.GetPosition(BingMap)));
            //ending selection mode and removing the rectangle from the map
            SelectionRunning = false;
            BingMap.Children.Remove(SelectionRectangle);
            SelectionMode = false;

            //Normalizing the selection endpoints
            NormalizeSelectionLocations();

            _events.PublishOnUIThreadAsync(new FilterByLocationMessage(SelectionStartLocation, SelectionEndLocation));
        }

        /// <summary>
        /// Enable the rectangle selection
        /// </summary>
        public void RectangleSelection()
        {
            if (ZoomingMode)
                ZoomingMode = false;

            SelectionMode = !SelectionMode;
        }

        /// <summary>
        /// Enable the rectangle selection
        /// </summary>
        public void ZoomToExtend()
        {
            if (SelectionMode)
                SelectionMode = false;

            ZoomingMode = !ZoomingMode;
        }

        /// <summary>
        /// Zoom to global mode
        /// </summary>
        public void GlobeZoom()
        {
            MapZoomLevel = 2;
        }

        /// <summary>
        /// Drawing vertical gridlines
        /// </summary>
        /// <param name="stepWidth"></param>
        private void DrawGridLines()
        {
            if(this.gridLineLayer.Children.Count>0)
                this.gridLineLayer.Children.Clear();

            MapPolyline pl;

            //Draw vertical grid lines (Longitudes)
            for (double x = -180; x < 180; x += StepWidth)
            {
                pl = new MapPolyline()
                {
                    Locations = new LocationCollection() { new Location(-85, x), new Location(85, x) },
                    Stroke = Brushes.Gray,
                    StrokeThickness = Thickness,
                    Opacity = Opacity

                };
                gridLineLayer.Children.Add(pl);

            }

            //Draw horizontal grid lines (Latitudes)
            //The wrap around effect in Bing Maps causes shapes that wide to sometimes loop around out of view. 
            //To prevent this we can break the lines into 4 segments. so that these grid lines aways stay in view
            for (double y = -85; y <= 85; y += stepWidth)
            {
                pl = new MapPolyline()
                {
                    Locations = new LocationCollection() { new Location(y, -180), new Location(y, -90) },
                    Stroke = Brushes.Gray,
                    StrokeThickness = Thickness,
                    Opacity = Opacity

                };
                gridLineLayer.Children.Add(pl);

                pl = new MapPolyline()
                {
                    Locations = new LocationCollection() { new Location(y, -90), new Location(y, 0) },
                    Stroke = Brushes.Gray,
                    StrokeThickness = Thickness,
                    Opacity = Opacity

                };

                gridLineLayer.Children.Add(pl);

                pl = new MapPolyline()
                {
                    Locations = new LocationCollection() { new Location(y, 0), new Location(y, 90) },
                    Stroke = Brushes.Gray,
                    StrokeThickness = Thickness,
                    Opacity = Opacity

                };

                gridLineLayer.Children.Add(pl);


                pl = new MapPolyline()
                {
                    Locations = new LocationCollection() { new Location(y, 90), new Location(y, 180) },
                    Stroke = Brushes.Gray,
                    StrokeThickness = Thickness,
                    Opacity = Opacity

                };

                gridLineLayer.Children.Add(pl);
            }
        }

        /// <summary>
        /// Adding to the map
        /// </summary>
        public void AddOrRemoveGridLines()
        {
            if(gridLineLayer.Children.Count<1)
                DrawGridLines();

            if (BingMap.Children.Contains(gridLineLayer))
            {
                BingMap.Children.Remove(gridLineLayer);
                return;
            }
            else
            {
                BingMap.Children.Add(gridLineLayer);
                return;
            }
        }

        /// <summary>
        /// Adding a mapbox tile overlay to the map
        /// </summary>
        private void AddTileOverlay()
        {

            // Create a new map layer to add the tile overlay to.
            tileLayer = new MapTileLayer();

            // The source of the overlay.
            TileSource tileSource = new TileSource();
            tileSource.UriFormat = "{UriScheme}://api.tiles.mapbox.com/v4/mapbox.streets/" + 
                MapZoomLevel.ToString() + "/" + 
                BingMap.Center.Latitude.ToString() +"/" + 
                BingMap.Center.Latitude.ToString() + 
                ".png?access_token=pk.eyJ1IjoiYXBpcnNhbCIsImEiOiJjamZrYmFzZ2YwNXZwMzNvMWoyY3RseDgwIn0.LjWqo81d-mRaPoKao54PbQ";

            // Add the tile overlay to the map layer
            tileLayer.TileSource = tileSource;

            // Add the map layer to the map
            if (!BingMap.Children.Contains(tileLayer))
            {
                BingMap.Children.Add(tileLayer);
            }
            //tileLayer.Opacity = tileOpacity;
        }
        #endregion

        #region Eventhandler

        /// <summary>
        /// Event that is triggered if a pushpin is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name=""></param>
        private void ClickOnPushpin(object sender, MouseButtonEventArgs e)
        {
            ((Pushpin)sender).Focusable = true;
            //((ToolTip)ToolTipService.GetToolTip((Pushpin)sender)).IsOpen = true;
            e.Handled = true;

            //Creating a popup for the pushpin element
            Popup codePopup = new Popup();

            TextBlock popupText = new TextBlock();
            popupText.Text = "Test";
            popupText.Background = Brushes.LightGray;
            popupText.Foreground = Brushes.Black;

            Button close = new Button();

            codePopup.Child = popupText;
            codePopup.PlacementTarget = ((Pushpin)sender);
            codePopup.StaysOpen = false;
            codePopup.IsOpen = true;
            codePopup.Focus();
        }

        #endregion

        #region Helper

        /// <summary>
        /// Getting the current location of the cursor on the map
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private Location GetMouseTouchLocation(object e)
        {
            Location loc = null;

            if (e is MouseEventArgs)
            {
                BingMap.TryViewportPointToLocation((e as MouseEventArgs).GetPosition(BingMap), out loc);
            }
            else if (e is TouchEventArgs)
            {
                BingMap.TryViewportPointToLocation((e as TouchEventArgs).GetTouchPoint(BingMap).Position, out loc);
            }

            return loc;
        }

        /// <summary>
        /// Normalizing the selection polygon locations that the start point is always south west of the end point
        /// </summary>
        private void NormalizeSelectionLocations()
        {
            double transferVariable;

            //Switching if end latitude is lower than start latitude
            if (SelectionStartLocation.Latitude > SelectionEndLocation.Latitude)
            {
                transferVariable = SelectionStartLocation.Latitude;

                SelectionStartLocation.Latitude = SelectionEndLocation.Latitude;
                SelectionEndLocation.Latitude = transferVariable;
            }
            if (SelectionStartLocation.Longitude > SelectionEndLocation.Longitude)
            {
                transferVariable = SelectionStartLocation.Longitude;

                SelectionStartLocation.Longitude = SelectionEndLocation.Longitude;
                SelectionEndLocation.Longitude = transferVariable;
            }
        }

        /// <summary>
        /// This method converts a SqlGeography object and converts it into a Bing Maps shape that can be displayed on the map.
        /// </summary>
        /// <param name="layer">MapLayer to add shape to.</param>
        /// <param name="geography">SqlGeography object to add to map.</param>
        /// <param name="tooltip">Tooltip string to display.</param>
        private void AddSqlGeographyToMapLayer(MapLayer layer, SqlGeography geography, string tooltip)
        {
            UIElement shape = null;

            switch (geography.STGeometryType().Value.ToUpper())
            {
                case "POINT":
                    shape = new Pushpin()
                    {
                        Location = new Location(geography.Lat.Value, geography.Long.Value)
                    };
                    break;
                case "LINESTRING":
                    shape = new MapPolyline()
                    {
                        Locations = GeographyRingToLocationCollection(geography),
                        Stroke = new SolidColorBrush(Color.FromArgb(150, 255, 0, 0))
                    };
                    break;
                case "POLYGON":
                    //Only render the exterior ring of the polygon for now.
                    shape = new MapPolygon()
                    {
                        Locations = GeographyRingToLocationCollection(geography.RingN(1)),
                        Fill = new SolidColorBrush(Color.FromArgb(150, 0, 0, 255)),
                        Stroke = new SolidColorBrush(Color.FromArgb(150, 255, 0, 0))
                    };
                    break;
                case "MULTIPOINT":
                case "MULTILINESTRING":
                case "MULTIPOLYGON":
                case "GEOMETRYCOLLECTION":
                    int numGeoms = geography.STNumGeometries().Value;
                    for (int i = 1; i <= numGeoms; i++)
                    {
                        AddSqlGeographyToMapLayer(layer, geography.STGeometryN(i), tooltip);
                    }
                    break;
                default:
                    break;
            }

            if (shape != null)
            {
                //Add tooltip
                ToolTipService.SetToolTip(shape, tooltip);

                //Add shape to layer
                layer.Children.Add(shape);
            }
        }

        /// <summary>
        /// Converts a ring of points from an SQLGeography into a LocationCollection.
        /// </summary>
        /// <param name="ring"></param>
        /// <returns></returns>
        private LocationCollection GeographyRingToLocationCollection(SqlGeography ring)
        {
            LocationCollection locations = new LocationCollection();
            int numPoints = ring.STNumPoints().Value;
            for (int i = 1; i <= numPoints; i++)
            {
                locations.Add(new Location(ring.STPointN(i).Lat.Value, ring.STPointN(i).Long.Value));
            }
            return locations;
        }

        //case "Countries":
        //    //if (value)
        //    //{
        //    //    RemoveMapPushpinByName(element);
        //    //    break;
        //    //}

        //    double x;
        //    double y;

        //    //Adding each country in the database to the maplayer
        //    foreach (var count in Countries)
        //    {
        //        try
        //        {
        //            LocationCollection loc = new LocationCollection();

        //            switch (count.countrReducedGeom.SpatialTypeName.ToUpper())
        //            {
        //                case "POLYGON":
        //                    //Only render the exterior ring of the polygon for now.
        //                    Polygon = new MapPolygon()
        //                    {
        //                        Fill = Brushes.Gray,
        //                        Opacity = 0.2,
        //                        Stroke = Brushes.Black,
        //                        StrokeThickness = 1
        //                    };

        //                    for (int i = 1; i <= count.countrReducedGeom.PointCount; i+=4)
        //                    {
        //                        x = (double)count.countrReducedGeom.PointAt(i).XCoordinate.Value;
        //                        y = (double)count.countrReducedGeom.PointAt(i).YCoordinate.Value;
        //                        loc.Add(new Location(y, x));
        //                    }

        //                    if (loc.First() != loc.Last())
        //                        loc.Add(loc.First());

        //                    Polygon.Locations = loc;
        //                    Polygon.ToolTip = count.NAME;
        //                    BingMap.Children.Add(Polygon);

        //                    break;
        //                case "MULTIPOINT":
        //                case "MULTILINESTRING":
        //                case "MULTIPOLYGON":
        //                case "GEOMETRYCOLLECTION":
        //                    int numGeoms = (int)count.countrReducedGeom.ElementCount;

        //                    for (int i = 1; i <= numGeoms; i++)
        //                    {
        //                        Polygon = new MapPolygon()
        //                    {
        //                        Fill = Brushes.Gray,
        //                        Opacity = 0.2,
        //                        Stroke = Brushes.Black,
        //                        StrokeThickness = 1
        //                    };

        //                    for (int j = 1; j <= count.countrReducedGeom.ElementAt(i).PointCount; j += 4)
        //                    {
        //                        x = (double)count.countrReducedGeom.ElementAt(i).PointAt(j).XCoordinate.Value;
        //                        y = (double)count.countrReducedGeom.ElementAt(i).PointAt(j).YCoordinate.Value;
        //                        loc.Add(new Location(y, x));
        //                    }

        //                    if (loc.First() != loc.Last())
        //                        loc.Add(loc.First());

        //                    Polygon.Locations = loc;
        //                    Polygon.ToolTip = count.NAME;
        //                    BingMap.Children.Add(Polygon);

        //                    }
        //                    break;
        //                default:
        //                    break;
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            continue;
        //        }
        //        finally
        //        {
        //            Polygon = new MapPolygon();
        //        }
        //    }
        //    break;

        #endregion
    }

    /// <summary>
    /// Overriding the pushpin class to attach a data table
    /// </summary>
    public partial class Pushpin : Microsoft.Maps.MapControl.WPF.Pushpin
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsClicked { get; set; }
        public DataTable Data { get; set; }
    }

    /// <summary>
    /// Overriding the pushpin class to attach a data table
    /// </summary>
    public partial class MapPolyline : Microsoft.Maps.MapControl.WPF.MapPolyline
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsClicked { get; set; }
        public DataTable Data { get; set; }
    }
}
