using Caliburn.Micro;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GeoReVi
{
    public class UserViewModel : Screen
    {
        #region Private members
        /// <summary>
        /// Event aggregator for event subscription to communicate with other viewmodels
        /// </summary>
        private IEventAggregator _events;

        private ApirsDatabase apirsDatabase = new ApirsDatabase();

        private tblPerson selectedPerson;

        public bool isUpdateRunning;
        #endregion

        public bool IsUpdateRunning
        {
            get { return this.isUpdateRunning; }
            private set { this.isUpdateRunning = value; NotifyOfPropertyChange(() => IsUpdateRunning); }
        }

        /// <summary>
        /// The selected person
        /// </summary>
        public tblPerson SelectedPerson
        {
            get { return this.selectedPerson; }
            set
            {
                this.selectedPerson = value;
                NotifyOfPropertyChange(() => SelectedPerson);
                NotifyOfPropertyChange(() => CanUpdate);
                NotifyOfPropertyChange(() => CanDelete);
            }
        }

        public bool CanUpdate
        {
            get
            {
                if (SelectedPerson != null)
                {
                    if (SelectedPerson.persVorname.Length > 0 &&
                        SelectedPerson.persName.Length > 0 &&
                        SelectedPerson.persAffiliation.Length > 0)
                        return true;
                    return false;
                }
                else
                    return false;
            }
            set
            {
                NotifyOfPropertyChange(() => CanUpdate); ;
            }
        }

        /// <summary>
        /// Checks if the user can be deleted
        /// </summary>
        public bool CanDelete
        {
            get
            {
                if (SelectedPerson.persIdPk != 0)
                    return true;

                return false;
            }
            set
            {
                NotifyOfPropertyChange(() => CanDelete);
            }
        }

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="events"></param>
        public UserViewModel(IEventAggregator events)
        {
            _events = events;
            LoadData();
        }


        #endregion

        private void LoadData()
        {
            try
            {
                using (var db = new ApirsDatabase())
                {
                    var user = db.tblPersons.SqlQuery("SELECT * FROM tblPersons WHERE persIdPk=" + (int)((ShellViewModel)IoC.Get<IShell>()).UserId + ";").ToList();
                    SelectedPerson = user.First();
                }
            }
            catch (Exception e)
            {
                selectedPerson = new tblPerson();
            }
        }

        /// <summary>
        /// Attempts to register the user
        /// </summary>
        /// <param name="parameter">The SecureString passed in from the view for the users password</param>
        /// <returns></returns>
        public async void Delete()
        {
            if (SelectedPerson.persIdPk == 0)
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("You have to be logged in.");

            // If existing window is visible, delete the customer and all their orders.  
            // In a real application, you should add warnings and allow the user to cancel the operation.  
            if (((ShellViewModel)IoC.Get<IShell>()).ShowQuestion("Are you REALLY sure to delete your user profile?" + Environment.NewLine
                + "You won't be able to reconstruct your created projects, rock samples and measurements." + Environment.NewLine
                + "Please be sure, that you have exported all relevant data.") == MessageBoxViewResult.No)
            {
                return;
            }

            CommandHelper ch = new CommandHelper();

            await ch.RunCommand(() => IsUpdateRunning, async () =>
            {
                await Task.Delay(2000);

                try
                {
                    //Establishing a sql connection
                    using (SqlConnection SqlConn = new SqlConnection(this.apirsDatabase.Database.Connection.ConnectionString.ToString()))
                    {
                        //Testing if a connection is established
                        if (ServerInteractionHelper.IsNetworkAvailable() && ServerInteractionHelper.TryAccessDatabase())
                        {
                            //Triggering the delete user sp
                            SqlCommand spDeleteUser = new SqlCommand("dbo.spDeleteUser", SqlConn);
                            spDeleteUser.CommandType = CommandType.StoredProcedure;
                            //Adding the parameters
                            spDeleteUser.Parameters.Add("@pLogin", SqlDbType.NVarChar, 50);
                            spDeleteUser.Parameters["@pLogin"].Value = SelectedPerson.persUserName;
                            spDeleteUser.Parameters.Add("@responseMessage", SqlDbType.NVarChar, 250).Direction = ParameterDirection.Output;
                            //Executing the stored procedure
                            SqlConn.Open();
                            spDeleteUser.ExecuteNonQuery();
                            var par = Convert.ToString(spDeleteUser.Parameters["@responseMessage"].Value);
                            SqlConn.Close();

                            switch (par)
                            {
                                case "1":
                                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("You successfully deleted your profile.");
                                    _events.PublishOnUIThreadAsync(new ChangeUserMessage(0, "Logged out"));
                                    _events.PublishOnUIThreadAsync(new ChangeViewModelMessage("LoginView"));
                                    break;
                                default:
                                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("An unexpected error occured.");
                                    break;
                            }

                        }
                        else
                        {
                            return;
                        }
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

        /// <summary>
        /// Attempts to update the user information
        /// </summary>
        /// <param name="parameter">The SecureString passed in from the view for the users password</param>
        /// <returns></returns>
        public async void Update()
        {
            if (!CanDelete)
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("You have to be logged in.");

            if (CanUpdate != true)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please validate your information correctly.");
                return;
            }

            CommandHelper ch = new CommandHelper();

            await ch.RunCommand(() => IsUpdateRunning, async () =>
             {
                 await Task.Delay(2000);

                 try
                 {
                     using (var db = new ApirsRepository<tblPerson>())
                     {
                         db.UpdateModel(SelectedPerson, SelectedPerson.persIdPk);
                         ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Your account information was updated.");
                         _events.PublishOnUIThreadAsync(new ChangeUserMessage((int)((ShellViewModel)IoC.Get<IShell>()).UserId, SelectedPerson.persVorname + " " + SelectedPerson.persName));
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

        /// <summary>
        /// Switching back to Login View
        /// </summary>
        /// <param name="parameter"></param>
        public void LoadLoginView(string parameter)
        {
            _events.PublishOnUIThreadAsync(new ChangeViewModelMessage("LoginView"));
        }
    }
}
