namespace GeoReVi
{
    /// <summary>
    /// A model event to change to another user
    /// </summary>
    public class ChangeUserMessage
    {
        public ChangeUserMessage(int userId, string fullName)
        {
            UserId = userId;
            FullName = fullName;
        }

        public int UserId
        {
            get;
            private set;
        }

        public string FullName
        {
            get;
            private set;
        }

    }
}
