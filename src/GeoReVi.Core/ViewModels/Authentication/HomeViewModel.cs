using Caliburn.Micro;
using System;
using System.Linq;

namespace GeoReVi
{
    /// <summary>
    /// Viewmodel for the home screen
    /// </summary>
    public class HomeViewModel : Screen
    {
        #region Private members

        public string WelcomeText { get; set; } = "Welcome to GeoReVi";

        #endregion

        #region Constructor

        public HomeViewModel()
        {
            LoadMessages();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loading user-related messages
        /// </summary>
        public void LoadMessages()
        {
            if (!DataValidation.CheckPrerequisites(CRUD.Load))
                return;

            try
            {
                int userid = (int)((ShellViewModel)IoC.Get<IShell>()).UserId;

                var query = new ApirsRepository<tblMessage>().GetModelByExpression(x => x.messToPersonIdFk == userid).OrderByDescending(x => x.messDate).ToList();

                foreach (tblMessage mess in query)
                {
                    mess.messPlainText = StringCipher.Decrypt(mess.messPlainText, "helloPhrase").ToString();
                }

                ((ShellViewModel)IoC.Get<IShell>()).SideMenuViewModel.Messages = new BindableCollection<tblMessage>(query);
            }
            catch (Exception e)
            {
                ((ShellViewModel)IoC.Get<IShell>()).SideMenuViewModel.Messages = new BindableCollection<tblMessage>();
            }
        }

        /// <summary>
        /// Opening the documentation
        /// </summary>
        public void OpenDocumentation()
        {
            try
            {
                System.Diagnostics.Process.Start(@"Media\Documents\User manual.pdf");
            }
            catch (Exception e)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpected error occurred.");
            }
        }
        #endregion
    }
}
