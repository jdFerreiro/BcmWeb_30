using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BcmWeb_30 
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FechaMayorIgualAHoy : ValidationAttribute
    {
        
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            if (value != null)
            {
                DateTime fechaAComparar = DateTime.Parse(((DateTime)value).ToString("yyyy/MM/dd"));
                DateTime hoy = DateTime.Parse(DateTime.UtcNow.ToString("yyyy/MM/dd"));

                if (fechaAComparar <= hoy)
                {
                    return ValidationResult.Success;
                }

            }
            string Error = string.Format(Resources.ErrorResource.FechaMayorIgualQueHoy, validationContext.DisplayName);
            return new ValidationResult(ErrorMessage ?? Error);
        }
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class FechaMenorIgualAHoy : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                DateTime fechaAComparar = DateTime.Parse(((DateTime)value).ToString("yyyy/MM/dd"));
                DateTime hoy = DateTime.Parse(DateTime.UtcNow.ToString("yyyy/MM/dd"));

                if (fechaAComparar >= hoy)
                {
                    return ValidationResult.Success;
                }
            }

            string Error = string.Format(Resources.ErrorResource.FechaMenorIgualQueHoy, validationContext.DisplayName);
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
    }
}