using System;
using System.Windows;
using System.Windows.Controls;

namespace GeoReVi
{
    /// <summary>
    /// The IsBusy attached property for a anything that wants to flag if the control is busy
    /// </summary>
    public class IsBusyProperty : BaseAttachedProperty<HasTextProperty, bool>
    {
    }

    /// <summary>
    /// The IsBusy attached property for a anything that wants to flag if the control is busy
    /// </summary>
    public class ImageSourceProperty : BaseAttachedProperty<ImageSourceProperty, Uri>
    {

    }

    /// <summary>
    /// The HasText attached property for a <see cref="PasswordBox"/>
    /// </summary>
    public class HasTextProperty : BaseAttachedProperty<HasTextProperty, bool>
    {
        /// <summary>
        /// Sets the has text property based on if the callser <see cref="PasswordBox"/> has any text
        /// </summary>
        /// <param name="sender"></param>
        public static void SetValue(DependencyObject sender)
        {
            SetValue(sender, ((PasswordBox)sender).SecurePassword.Length > 0);
        }
    }
}