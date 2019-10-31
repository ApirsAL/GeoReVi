using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    public class SendUserMessageMessage
    {
        public SendUserMessageMessage(int fromUser, int toUser, string headerText, string plainText, DateTime date)
        {
            FromUser = fromUser;
            ToUser = toUser;
            HeaderText = headerText;
            PlainText = plainText;
            MessageDate = date;
        }

        public int FromUser
        {
            get;
            private set;
        }

        public int ToUser
        {
            get;
            private set;
        }

        public string HeaderText
        {
            get;
            private set;
        }

        public string PlainText
        {
            get;
            private set;
        }

        public Nullable<System.DateTime> MessageDate
        { get; private set; }
    }
}
