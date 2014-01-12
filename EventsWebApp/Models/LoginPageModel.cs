using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using EventsWebApp.Models;


namespace EventsWebApp.Models
{
    public class LoginPageModel
    {
         public LoginModel loginModel { get; set; }
         public RegisterModel registerModel { get; set; }
    }

   

}