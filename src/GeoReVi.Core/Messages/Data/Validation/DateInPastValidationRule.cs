using System;
using System.Globalization;
using System.Windows.Controls;

namespace GeoReVi
{
    /// <summary>
    /// A validation rule class for empty strings
    /// FROM https://stackoverflow.com/questions/36071418/validation-in-wpf-mvvm-with-entity-framework
    /// </summary>
    public class DateInPastValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            DateTime s = (DateTime)value;
            if (s > DateTime.Now)
            {
                return new ValidationResult(false, "Date has to be in the past.");
            }

            return ValidationResult.ValidResult;
        }
    }
}
