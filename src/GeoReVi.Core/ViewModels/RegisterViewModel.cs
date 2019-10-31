using Caliburn.Micro;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GeoReVi
{
    public class RegisterViewModel : Screen
    {
        #region Private members
        /// <summary>
        /// Event aggregator for event subscription to communicate with other viewmodels
        /// </summary>
        private IEventAggregator _events;

        private string userName = "TestUser2";
        private string email = "Test@Test";
        private string affiliation = "Test";
        private string firstName = "Test";
        private string lastName = "Test";

        private bool isRegisterRunning;

        #endregion

        public bool IsRegisterRunning
        {
            get { return this.isRegisterRunning; }
            private set { this.isRegisterRunning = value; NotifyOfPropertyChange(() => IsRegisterRunning); }
        }


        public bool CanRegister
        {
            get
            {
                if (DataValidation.IsValidEmail(this.Email) &&
                    this.UserName.Length > 6 &&
                    this.FirstName.Length > 0 &&
                    this.LastName.Length > 0 &&
                    this.Affiliation.Length > 0)
                {
                    return true;
                }
                else
                    return false;
            }
            set
            {
                NotifyOfPropertyChange(() => CanRegister); ;
            }
        }

        /// <summary>
        /// The inserted user name
        /// </summary>
        public string UserName
        {
            get
            {
                return this.userName;
            }
            set
            {
                this.userName = value;
                NotifyOfPropertyChange(() => UserName);
                NotifyOfPropertyChange(() => CanRegister);
            }

        }
        public string Email { get { return this.email; } set { this.email = value; NotifyOfPropertyChange(() => Email); NotifyOfPropertyChange(() => CanRegister); } }
        public string Affiliation { get { return this.affiliation; } set { this.affiliation = value; NotifyOfPropertyChange(() => Affiliation); NotifyOfPropertyChange(() => CanRegister); } }
        public string FirstName { get { return this.firstName; } set { this.firstName = value; NotifyOfPropertyChange(() => FirstName); NotifyOfPropertyChange(() => CanRegister); } }
        public string LastName { get { return this.lastName; } set { this.lastName = value; NotifyOfPropertyChange(() => LastName); NotifyOfPropertyChange(() => CanRegister); } }

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="events"></param>
        public RegisterViewModel(IEventAggregator events)
        {
            _events = events;
        }


        #endregion

        /// <summary>
        /// Switching back to Login View
        /// </summary>
        /// <param name="parameter"></param>
        public void LoadLoginView(string parameter)
        {
            _events.PublishOnUIThreadAsync(new ChangeViewModelMessage("LoginView"));
        }

        /// <summary>
        /// Attempts to register the user
        /// </summary>
        /// <param name="parameter">The SecureString passed in from the view for the users password</param>
        /// <returns></returns>
        public async void Register(PasswordBox parameter, PasswordBox repeat)
        {
            if (this.CanRegister != true || parameter.Password != repeat.Password || parameter.Password.Length <= 7 || !parameter.Password.Any(char.IsDigit))
            { 
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please validate your password correctly.");
                return;
            }

            CommandHelper ch = new CommandHelper();

            await ch.RunCommand(() => IsRegisterRunning, async () =>
            {
                await Task.Delay(2000);

                try
                {
                    using (var db = new ApirsDatabase())
                    {
                        var paramLoginName = new SqlParameter
                        {
                            ParameterName = "pLogin",
                            Value = UserName,
                            Direction = ParameterDirection.Input
                        };

                        var paramPass = new SqlParameter
                        {
                            ParameterName = "pPassword",
                            Value = parameter.Password,
                            Direction = ParameterDirection.Input
                        };


                        var paramMail = new SqlParameter
                        {
                            ParameterName = "pMail",
                            Value = Email,
                            Direction = ParameterDirection.Input
                        };

                        var paramResponse = new SqlParameter
                        {
                            ParameterName = "responseMessage",
                            Size = 250,
                            SqlDbType = SqlDbType.NVarChar,
                            Direction = ParameterDirection.Output
                        };


                        string par = db.Database.SqlQuery<string>("exec dbo.spAddUser @pLogin, @pPassword, @pMail, @responseMessage", paramLoginName, paramPass, paramMail, paramResponse).First();

                        //Forward the user to the home view or denying the login based on the response of the server
                        switch (par)
                        {
                            case "Message":
                                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(par + ". Please try it again.");
                                return;
                            case "Success":

                                var paramLoginName1 = new SqlParameter
                                {
                                    ParameterName = "pLogin",
                                    Value = Email,
                                    Direction = ParameterDirection.Input
                                };

                                var paramFirstName = new SqlParameter
                                {
                                    ParameterName = "pFirstName",
                                    Value = FirstName,
                                    Direction = ParameterDirection.Input
                                };

                                var paramLastName = new SqlParameter
                                {
                                    ParameterName = "pLastName",
                                    Value = LastName,
                                    Direction = ParameterDirection.Input
                                };


                                var paramAffiliation = new SqlParameter
                                {
                                    ParameterName = "pAffiliation",
                                    Value = Affiliation,
                                    Direction = ParameterDirection.Input
                                };

                                var paramStatus = new SqlParameter
                                {
                                    ParameterName = "pStatus",
                                    SqlDbType = SqlDbType.Int,
                                    Value = 3,
                                    Direction = ParameterDirection.Input
                                };

                                var paramResponse1 = new SqlParameter
                                {
                                    ParameterName = "responseMessage",
                                    Size = 250,
                                    SqlDbType = SqlDbType.NVarChar,
                                    Direction = ParameterDirection.Output
                                };

                                string par1 = db.Database.SqlQuery<string>("exec dbo.spAddPerson @pFirstName, @pLastName, @pAffiliation, @pStatus, @pLogin, @responseMessage", paramFirstName, paramLastName, paramAffiliation, paramStatus, paramLoginName1, paramResponse1).First();
                                string param = par1.ToString();

                                switch (par1)
                                {
                                    case "Success":
                                        ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("You successfully created a profile for GeoReVi. You can login now with your user name and password.");
                                        UserName = "";
                                        Affiliation = "";
                                        LastName = "";
                                        FirstName = "";
                                        Email = "";
                                        _events.PublishOnUIThreadAsync(new ChangeViewModelMessage("LoginView"));
                                        break;
                                    case "Message":
                                    default:
                                        var paramLoginName2 = new SqlParameter
                                        {
                                            ParameterName = "pLogin",
                                            Value = Email,
                                            Direction = ParameterDirection.Input
                                        };

                                        var paramResponse2 = new SqlParameter
                                        {
                                            ParameterName = "responseMessage",
                                            Size = 250,
                                            SqlDbType = SqlDbType.NVarChar,
                                            Direction = ParameterDirection.Output
                                        };
                                        //Triggering the delete user sp
                                        string par2 = db.Database.SqlQuery<string>("exec dbo.spDeleteUser @pLogin, @responseMessage", paramLoginName2, paramResponse2).First();
                                        ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please try it again.");
                                        break;
                                }

                                break;
                            default:
                                return;
                        }

                        ////Stored procedures
                        //SqlCommand spAddUser = new SqlCommand("dbo.spAddUser", SqlConn);
                        //SqlCommand spAddPerson = new SqlCommand("dbo.spAddPerson", SqlConn);

                        ////Testing if a connection is established
                        //if (ServerInteractionHelper.IsNetworkAvailable() && ServerInteractionHelper.TryAccessDatabase())
                        //{
                        //    //Preparing the stored procedures
                        //    spAddUser.CommandType = System.Data.CommandType.StoredProcedure;
                        //    spAddPerson.CommandType = System.Data.CommandType.StoredProcedure;

                        //    //Adding the parameters
                        //    spAddUser.Parameters.Add("@pLogin", SqlDbType.NVarChar, 50);
                        //    spAddUser.Parameters.Add("@pPassword", SqlDbType.NVarChar, 50);
                        //    spAddUser.Parameters.Add("@pMail", SqlDbType.NVarChar, 255);
                        //    spAddUser.Parameters.Add("@responseMessage", SqlDbType.NVarChar, 255).Direction = ParameterDirection.Output;

                        //    spAddUser.Parameters["@pLogin"].Value = this.UserName;
                        //    spAddUser.Parameters["@pMail"].Value = this.Email;
                        //    spAddUser.Parameters["@pPassword"].Value = parameter.Password;
                        //}
                        //else
                        //{
                        //    return;
                        //}
                    }
                }
                catch (NullReferenceException ne)
                {
                    Console.WriteLine(ne.Message);
                }
                catch (Exception e)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(e.Message);
                }

            });
        }
    }
}
