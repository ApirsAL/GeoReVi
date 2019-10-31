using Caliburn.Micro;

namespace GeoReVi
{
    /// <summary>
    /// Viewmodel for the messagebox popup service
    /// </summary>
    public class MessageBoxViewModel : Screen
    {
        #region Private members

        //EventAggregator
        private readonly IEventAggregator _events;
        //The message box type
        private MessageBoxViewType messageBoxType;
        //The message buttons types
        private MessageBoxViewButton messageBoxButton;

        //Messages beeing displayed
        private string messageText = "Message";
        private string detailsText = "";
        #endregion

        #region Public properties

        /// <summary>
        /// Checks if messagebox has certain buttons
        /// </summary>
        public bool HasCancel
        {
            get
            {
                return (MessageBoxButton == MessageBoxViewButton.OkCancel
                        || MessageBoxButton == MessageBoxViewButton.YesNoCancel);
            }
            set
            {
                NotifyOfPropertyChange(() => HasOk);
            }
        }
        public bool HasOk
        {
            get
            {
                return (MessageBoxButton == MessageBoxViewButton.OkCancel
                        || MessageBoxButton == MessageBoxViewButton.Ok);
            }
            set
            {
                NotifyOfPropertyChange(() => HasOk);
            }
        }
        public bool HasYes
        {
            get
            {
                return (MessageBoxButton == MessageBoxViewButton.YesNoCancel
                        || MessageBoxButton == MessageBoxViewButton.YesNo);
            }
            set
            {
                NotifyOfPropertyChange(() => HasYes);
            }
        }
        public bool HasNo
        {
            get
            {
                return (MessageBoxButton == MessageBoxViewButton.YesNoCancel
                        || MessageBoxButton == MessageBoxViewButton.YesNo);
            }
            set
            {
                NotifyOfPropertyChange(() => HasNo);
            }
        }

        /// <summary>
        /// Checks if the messagebox contains details
        /// </summary>
        public bool HasDetails
        {
            get
            {
                return !string.IsNullOrEmpty(DetailsText);
            }
            set
            {
                NotifyOfPropertyChange(() => HasDetails);
            }
        }

        /// <summary>
        /// Message box view image
        /// </summary>
        public MessageBoxViewType MessageBoxType
        {
            get => this.messageBoxType;
            set
            {
                this.messageBoxType = value;
                NotifyOfPropertyChange(() => MessageBoxType);
                NotifyOfPropertyChange(() => IsInformation);
                NotifyOfPropertyChange(() => IsQuestion);
                NotifyOfPropertyChange(() => IsWarning);
                NotifyOfPropertyChange(() => IsError);
            }
        }

        /// <summary>
        /// Message box buttons
        /// </summary>
        public MessageBoxViewButton MessageBoxButton
        {
            get => this.messageBoxButton;
            set
            {
                this.messageBoxButton = value;
                NotifyOfPropertyChange(() => MessageBoxButton);
                NotifyOfPropertyChange(() => HasCancel);
                NotifyOfPropertyChange(() => HasOk);
                NotifyOfPropertyChange(() => HasYes);
                NotifyOfPropertyChange(() => HasNo);
            }
        }

        /// <summary>
        /// MessageBoxViewResult
        /// </summary>
        public MessageBoxViewResult Result { get; private set; }

        /// <summary>
        /// Visibilities
        /// </summary>
        public bool IsInformation
        {
            get { return this.MessageBoxType == MessageBoxViewType.Information; }
            set { NotifyOfPropertyChange(() => IsInformation); }
        }
        public bool IsQuestion
        {
            get { return this.MessageBoxType == MessageBoxViewType.Question; }
            set { NotifyOfPropertyChange(() => IsInformation); }
        }
        public bool IsWarning
        {
            get { return this.MessageBoxType == MessageBoxViewType.Warning; }
            set { NotifyOfPropertyChange(() => IsInformation); }
        }
        public bool IsError
        {
            get { return this.MessageBoxType == MessageBoxViewType.Error; }
            set { NotifyOfPropertyChange(() => IsInformation); }
        }

        /// <summary>
        /// Messages beeing displayed
        /// </summary>
        public string MessageText { get => this.messageText; set { this.messageText = value; NotifyOfPropertyChange(() => MessageText); } }
        public string DetailsText { get => this.detailsText; set { this.detailsText = value; NotifyOfPropertyChange(() => DetailsText); NotifyOfPropertyChange(() => HasDetails); } }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="events"></param>
        public MessageBoxViewModel(IEventAggregator events,
                                    string message = "",
                                    string details = "",
                                    MessageBoxViewType type=MessageBoxViewType.None, 
                                    MessageBoxViewButton button = MessageBoxViewButton.Ok)
        {
            this._events = events;
            MessageText = message;
            DetailsText = details;
            MessageBoxType = type;
            MessageBoxButton = button;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Method to close the dialog
        /// </summary>
        /// <param name="tag"></param>
        public void Close(string tag)
        {
            switch(tag)
            {
                case ("Ok"):
                    Result = MessageBoxViewResult.Ok;
                    break;
                case ("Cancel"):
                    Result = MessageBoxViewResult.Cancel;
                    break;
                case ("Yes"):
                    Result = MessageBoxViewResult.Yes;
                    break;
                case ("No"):
                    Result = MessageBoxViewResult.No;
                    break;
            }

            TryClose();
        }

        #endregion

    }
}
