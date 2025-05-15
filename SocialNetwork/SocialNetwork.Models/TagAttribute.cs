using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SocialNetwork.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class TagAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Tag cannot be null");
            }

            string tag = value.ToString();

            // A valid tag starts with #, has no spaces, and is at most 20 characters long
            if (!tag.StartsWith("#") || tag.Contains(" ") || tag.Length > 20)
            {
                return new ValidationResult("Tag must start with #, contain no spaces, and be at most 20 characters long");
            }

            return ValidationResult.Success;
        }
    }
}