using System;
using System.Globalization;
using System.Windows.Controls;

namespace GeoReVi
{
    /// <summary>
    /// A validation rule class for empty strings
    /// FROM https://stackoverflow.com/questions/36071418/validation-in-wpf-mvvm-with-entity-framework
    /// </summary>
    public class NotEmptyStringValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string s = value as string;
            if (String.IsNullOrEmpty(s))
            {
                return new ValidationResult(false, "Field cannot be empty.");
            }

            return ValidationResult.ValidResult;
        }
    }
}
