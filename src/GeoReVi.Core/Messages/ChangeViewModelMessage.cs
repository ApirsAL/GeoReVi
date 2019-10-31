namespace GeoReVi
{
    /// <summary>
    /// A model event class to change to another view model
    /// </summary>
    public class ChangeViewModelMessage
    {
        public string View
        {
            get;
            private set;
        }

        public ChangeViewModelMessage(string view)
        {
            View = view;
        }
    }
}
