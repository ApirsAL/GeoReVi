
namespace GeoReVi
{
    public class OpenDataWindowMessage
    {
        public OpenDataWindowMessage(object dataObject, string message)
        {
            DataObject = dataObject;
            Message = message;
        }

        public object DataObject
        { get; private set; }

        public string Message
        { get; private set; }
    }
}
