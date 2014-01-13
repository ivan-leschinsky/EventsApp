using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity.Infrastructure;
using EventsWebApp.Filters;

namespace EventsWebApp.Models
{
    [Table("UserProfile")]
    [Culture]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserTrueName { get; set; }
        public string UserInfo { get; set; }
        public string UserAvatarUrl { get; set; }

        [Display(Name = "Интересы")]
        public string Subjects { get; set; }

        public virtual ICollection<Event_> Events { get; set; }

        public UserProfile()
        {
            this.Events = new HashSet<Event_>();
        }

    }

    [Culture]
    public class RegisterExternalLoginModel
    {
        [Required(ErrorMessageResourceName = "NameRequired")]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    [Culture]
    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessageResourceType = typeof(Resource.Resource), ErrorMessageResourceName = "tooShortPassword", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(Resource.Resource), ErrorMessageResourceName = "newPasswordAndConfirmationDoNotMatch")]
        public string ConfirmPassword { get; set; }
    }

    [Culture]
    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    [Culture]
    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessageResourceType = typeof(Resource.Resource), ErrorMessageResourceName = "tooShortPassword", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessageResourceType = typeof(Resource.Resource), ErrorMessageResourceName = "passwordsDoNotMatch")]
        public string ConfirmPassword { get; set; }

        public string Subjects { get; set; }
    }

    [Culture]
    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
