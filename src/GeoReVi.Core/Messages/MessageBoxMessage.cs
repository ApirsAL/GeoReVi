using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GeoReVi
{
    public class MessageBoxMessage
    {
        #region Properties

        /// <summary>
        /// Messages beeing displayed
        /// </summary>
        public string MessageText { get; set; }
        public string DetailsText { get; set; }


        /// <summary>
        /// Message box view image
        /// </summary>
        public MessageBoxViewType MessageBoxType
        {
            get;set;
        }

        /// <summary>
        /// Message box buttons
        /// </summary>
        public MessageBoxViewButton MessageBoxButton
        {
            get;set;
        }

        #endregion

        public MessageBoxMessage(string message="", string details="", MessageBoxViewType type=MessageBoxViewType.None, MessageBoxViewButton button = MessageBoxViewButton.Ok)
        {
            MessageText = message;
            DetailsText = details;
            MessageBoxType = type;
            MessageBoxButton = button;
        }

    }
}
