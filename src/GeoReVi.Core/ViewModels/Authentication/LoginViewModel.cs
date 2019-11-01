using Caliburn.Micro;
using System;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Windows.Input;

namespace GeoReVi
{
    public class LoginViewModel : Screen
    {
        #region Private members

        /// <summary>
        /// Event aggregator for event subscription to communicate with other viewmodels
        /// </summary>
        private IEventAggregator _events;

        private bool isLoggedIn = false;

        private bool isLoginRunning = false;

        #endregion

        #region Public properties

        /// <summary>
        /// The email of the user
        /// </summary>
        public string Email { get; set; } = "";

        /// <summary>
        /// A flag indicating every login thats running
        /// </summary>
        public bool IsLoginRunning
        {
            get
            {
                return this.isLoginRunning;
            }
            set
            {
                this.isLoginRunning = value;
                NotifyOfPropertyChange(() => IsLoginRunning);
                NotifyOfPropertyChange(() => CanLogin);
            }
        }

        public bool CanLogin
        {
            get { return !IsLoginRunning; }
        }

        /// <summary>
        /// A flag indicating if the user is logged in
        /// </summary>
        public bool IsLoggedIn
        {
            get
            {
                return this.isLoggedIn;
            }
            set
            {
                this.isLoggedIn = value;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="window"></param>
        public LoginViewModel(IEventAggregator events)
        {
            _events = events;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Attempts to log the user in
        /// </summary>
        /// <param name="parameter">The SecureString passed in from the view for the users password</param>
        /// <returns></returns>
        public void Login(PasswordBox parameter, KeyEventArgs keyArgs)
        {
            //_events.BeginPublishOnUIThread(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok));

            if (keyArgs.Key != Key.Enter || IsLoginRunning)
                return;

            Login(parameter);

        }

        /// <summary>
        /// Attempts to log the user in
        /// </summary>
        /// <param name="parameter">The SecureString passed in from the view for the users password</param>
        /// <returns></returns>
        public async void Login(PasswordBox parameter)
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => IsLoginRunning, async () =>
            {

                try
                {
                    //var pwBox = (PasswordBox)parameter;
                    string username = this.Email ?? "";

                    using (var db = new ApirsDatabase())
                    {
                        var paramLoginName = new SqlParameter
                        {
                            ParameterName = "pLoginName",
                            Value = username,
                            Direction = ParameterDirection.Input
                        };

                        var paramPass = new SqlParameter
                        {
                            ParameterName = "pPassword",
                            Value = parameter.Password,
                            Direction = ParameterDirection.Input
                        };

                        var paramResponse = new SqlParameter
                        {
                            ParameterName = "responseMessage",
                            Size = 250,
                            SqlDbType = SqlDbType.NVarChar,
                            Direction = ParameterDirection.Output
                        };


                        string par = db.Database.SqlQuery<string>("exec dbo.spUserLogin @pLoginName, @pPassword, @responseMessage", paramLoginName, paramPass, paramResponse).First();

                        //Forward the user to the home view or denying the login based on the response of the server
                        switch (par)
                        {
                            case "Invalid login":
                            case "Incorrect password":
                                _events.PublishOnUIThreadAsync(new MessageBoxMessage("Wrong password. Please try it again", "", MessageBoxViewType.Information, MessageBoxViewButton.Ok));
                                break;
                            case "User successfully logged in":
                                //Get the actual user id and set it as a property in the shellview
                                tblPerson result = (from p in db.tblPersons
                                                    where p.persUserName == username.ToString()
                                                    select p).First();
                                _events.PublishOnUIThreadAsync(new ChangeUserMessage(Convert.ToInt32(result.persIdPk), result.persFullName));
                                //Changing the viewmodel to the homeview
                                _events.PublishOnUIThreadAsync(new ChangeViewModelMessage("HomeView"));
                                break;
                            default:
                                break;
                        }
                    }

                    return;
                }
                catch (Exception e)
                {
                    _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok));
                }

            });

        }
        #endregion

        #region Handler for other Viewmodels

        /// <summary>
        /// Loading the register view
        /// </summary>
        /// <param name="parameter"></param>
        public void LoadRegisterView(string parameter)
        {
            _events.PublishOnUIThreadAsync(new ChangeViewModelMessage("RegisterView"));
        }

        #endregion
    }
}
