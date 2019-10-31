using Caliburn.Micro;

namespace GeoReVi
{
    public static class DataValidation
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks prerequisites for CRUD operations
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="selectedObject"></param>
        /// <param name="uploaderId"></param>
        /// <returns></returns>
        public static bool CheckPrerequisites(CRUD operation = CRUD.None, 
            object selectedObject = null, 
            int uploaderId = 0, 
            int selectedObjectId = 0,
            bool notify = false)
        {
            if (operation == CRUD.None)
            {
                return false;
            }

            switch (operation)
            {
                case CRUD.Delete:
                case CRUD.Update:
                    if ((int)((ShellViewModel)IoC.Get<IShell>()).UserId == 0 && !(bool)((ShellViewModel)IoC.Get<IShell>()).LocalMode)
                    {
                        if(notify)
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please login first.");

                        return false;
                    }
                    else if (selectedObject == null)
                        return false;
                    else if (uploaderId != (int)((ShellViewModel)IoC.Get<IShell>()).UserId)
                    {
                        if (notify)
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Only the uploader can make changes to the record.");
                        return false;
                    }

                    return true;

                case CRUD.Add:
                    if ((int)((ShellViewModel)IoC.Get<IShell>()).UserId == 0 && !(bool)((ShellViewModel)IoC.Get<IShell>()).LocalMode)
                    {
                        if (notify)
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please login first.");
                        return false;
                    }
                    return true;

                case CRUD.AddToObject:
                    if ((int)((ShellViewModel)IoC.Get<IShell>()).UserId == 0 && !(bool)((ShellViewModel)IoC.Get<IShell>()).LocalMode)
                    {
                        if (notify)
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please login first.");
                        return false;
                    }
                    else if (selectedObject == null || selectedObjectId == 0)
                        return false;
                    else if (uploaderId != (int)((ShellViewModel)IoC.Get<IShell>()).UserId && uploaderId != -1)
                    {
                        if (notify)
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Only the uploader can make changes to the record.");
                        return false;
                    }
                    return true;
                case CRUD.Load:
                    if ((int)((ShellViewModel)IoC.Get<IShell>()).UserId == 0 && !(bool)((ShellViewModel)IoC.Get<IShell>()).LocalMode)
                    {
                        if (notify)
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please login first.");
                        return false;
                    }
                    return true;
                case CRUD.Import:
                    if ((int)((ShellViewModel)IoC.Get<IShell>()).UserId == 0 && !(bool)((ShellViewModel)IoC.Get<IShell>()).LocalMode)
                    {
                        if (notify)
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please login first.");
                        return false;
                    }
                    if ((tblProject)((ShellViewModel)IoC.Get<IShell>()).SelectedProject == null)
                    {
                        if (notify)
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please select a project first.");
                        return false;
                    }
                    if ((int)((ShellViewModel)IoC.Get<IShell>()).SelectedProject.prjIdPk == 0)
                    {
                        if (notify)
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please select a project first.");
                        return false;
                    }
                    return true;
            }

            return false;
        }
    }

    /// <summary>
    /// CRUD operations enum
    /// </summary>
    public enum CRUD
    {
        None=0,
        Add=1,
        Load=2,
        Update=3,
        Delete=4,
        AddToObject=5,
        Import=6

    }
}
