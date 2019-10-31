using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace GeoReVi
{
    /// <summary>
    /// A base value converter that allows direct XAML usage
    /// </summary>
    /// <typeparam name="T">The type of this value converter</typeparam>
    public abstract class BaseValueConverter<T> : MarkupExtension, IValueConverter
        where T : class, new()
    {
        /// <summary>
        /// A single static instance of this value converter
        /// </summary>
        private static T mConverter = null;

        /// <summary>
        /// Overrides the method of MarkupExtension to provide a value
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            //Test if the converter already is declared.
            //If not, a new instance of the converter will be instantiated
            //The "??"-operator returns the left side of the formular if it is not null, else it returns the right side
            return mConverter ?? (mConverter = new T());
        }

        #region Value converter methods

        /// <summary>
        /// The method to convert one type to another
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        /// <summary>
        /// The method to voncert a value back to it's source
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);


        #endregion
    }
}
