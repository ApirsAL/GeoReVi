using System;
using System.Runtime.InteropServices;
using System.Security;

namespace GeoReVi
{
    /// <summary>
    /// Helpers for the <see cref="SecureString"/> class
    /// </summary>
    public static class SecurityStringHelpers
    {
        /// <summary>
        /// Unsecures a secure string to plain text
        /// </summary>
        /// <param name="securePassword">the secure string</param>
        /// <returns></returns>
        public static string Unsecure(this SecureString secureString)
        {
            //Make sure we have a string
            if (secureString == null)
                return string.Empty;

            //Get a pointer for an unsecure string in memory
            var unmanagedString = IntPtr.Zero;

            try
            {
                //Unsecures the password
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);

                //returns the string from the pointer previously defined
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}