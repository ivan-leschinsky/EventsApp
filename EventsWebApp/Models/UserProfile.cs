//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EventsWebApp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserProfile
    {
        public UserProfile()
        {
            this.Event_ = new HashSet<Event_>();
        }
    
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserTrueName { get; set; }
        public string UserInfo { get; set; }
        public string UserAvatarUrl { get; set; }
        public string Subjects { get; set; }
    
        public virtual ICollection<Event_> Event_ { get; set; }
    }
}