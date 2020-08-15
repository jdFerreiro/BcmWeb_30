using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace BcmWeb_30 .Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<UserProfile> UserProfiles { get; set; }
    }
    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
    public class ChangePasswordModel
    {
        [Required(ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [DataType(DataType.Password)]
        [Display(Name = "captionOldPassword", ResourceType = typeof(Resources.LoginResource))]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resources.LoginResource), Name = "captionNewPassword")]
        public string NewPassword { get; set; }

        [Required(ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [DataType(DataType.Password)]
        [Display(Name = "captionPasswordConfirm", ResourceType = typeof(Resources.LoginResource))]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessageResourceType = typeof(Resources.ErrorResource), ErrorMessageResourceName = "CompareErrorFemalevsFemale")]
        public string ConfirmPassword { get; set; }
    }
    public class LoginModel : ModulosUserModel
    {
        [Required(ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [RegularExpression("\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*", ErrorMessageResourceName = "EmailError", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "captionUsername", ResourceType = typeof(Resources.LoginResource))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [DataType(DataType.Password)]
        [Display(Name = "captionPassword", ResourceType = typeof(Resources.LoginResource))]
        public string Password { get; set; }
    }
    public class RegisterModel : ModulosUserModel
    {
        [Required(ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionUsername", ResourceType = typeof(Resources.LoginResource))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources.ErrorResource), ErrorMessageResourceName = "StringLengthErrorFemale", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "captionPassword", ResourceType = typeof(Resources.LoginResource))]
        public string Password { get; set; }

        [Required(ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [Display(Name = "captionPasswordConfirm", ResourceType = typeof(Resources.LoginResource))]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessageResourceName = "CompareErrorFemalevsFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public string ConfirmPassword { get; set; }
    }
    public class ForgotPasswordModel : ModulosUserModel
    {
        [Required(ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [RegularExpression("\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*", ErrorMessageResourceName = "EmailError", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
    public class ResetPasswordModel : ModulosUserModel
    {
        [Required(ErrorMessageResourceName = "RequiredErrorMale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [RegularExpression("\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*", ErrorMessageResourceName = "EmailError", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [DataType(DataType.Password)]
        [Display(Name = "captionPassword", ResourceType = typeof(Resources.LoginResource))]
        public string Password { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        [DataType(DataType.Password)]
        [Display(Name = "captionPasswordConfirm", ResourceType = typeof(Resources.LoginResource))]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessageResourceName = "CompareErrorFemalevsFemale", ErrorMessageResourceType = typeof(Resources.ErrorResource))]
        public string PasswordCompare { get; set; }
    }
    public class PrimeraVezModel
    {
        public int MyProperty { get; set; }
    }
}