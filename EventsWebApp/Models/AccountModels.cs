using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity.Infrastructure;

namespace EventsWebApp.Models
{

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserTrueName { get; set; }
        public string UserInfo { get; set; }
        public string UserAvatarUrl { get; set; }

        [Display(Name = "")]
        public string Subjects { get; set; }

        public virtual ICollection<Event_> Events { get; set; }

        public UserProfile()
        {
            this.Events = new HashSet<Event_>();
        }

    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "profileName", ResourceType = typeof(Resource.Resource))]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [DataType(DataType.Password)]
        [Required(
                  ErrorMessageResourceName = "passwordReuired")]
        [Display(Name = "currentPassword", ResourceType = typeof(Resource.Resource))]
        public string OldPassword { get; set; }

        [StringLength(100, ErrorMessageResourceName = "minimumLengthPassword", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Required(
          ErrorMessageResourceName = "newPasswordRequired")]
        [Display(Name = "newPassword", ResourceType = typeof(Resource.Resource))]
        public string NewPassword { get; set; }

        [StringLength(100, ErrorMessageResourceName = "minimumLengthPassword", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Required(
          ErrorMessageResourceName = "confirmPasswordRequired")]
        [Display(Name = "confirmPassword", ResourceType = typeof(Resource.Resource))]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "")]
        public string Password { get; set; }

        [Display(Name = "")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "")]
        [Compare("Password", ErrorMessage = "")]
        public string ConfirmPassword { get; set; }

        public string Subjects { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
