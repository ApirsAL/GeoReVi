namespace GeoReVi
{
    /// <summary>
    /// A model event to change to forward short cut events
    /// </summary>
    public class ShortCutMessage
    {
        public ShortCutMessage(string characters)
        {
            Characters = characters;
        }

        public string Characters
        {
            get;
            private set;
        }
    }
}
