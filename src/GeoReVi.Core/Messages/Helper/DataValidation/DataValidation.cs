using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static bool CheckPrerequisites(string operation = "", object selectedObject = null, int uploaderId = 0)
        {
            if (operation == "")
            {
                return false;
            }

            switch (operation)
            {
                case "Update":
                    if ((int)App.Current.Properties["UserId"] == 0)
                    {
                        MessageBox.ShowInformation("Please login first.");
                        return false;
                    }
                    else if (selectedObject == null)
                        return false;
                    else if (uploaderId != (int)App.Current.Properties["UserId"])
                    {
                        MessageBox.ShowInformation("Only the uploader can make changes to the analytical instrument.");
                        return false;
                    }

                    return true;
                case "Delete":

                    return false;
                case "Add":
                    return false;
                case "Refresh":
                    return false;
                case "LoadData":
                    if ((int)App.Current.Properties["UserId"] == 0)
                    {
                        MessageBox.ShowInformation("Please login first.");
                        return false;
                    }
                    return true;
            }

            return false;
        }
    }
}
