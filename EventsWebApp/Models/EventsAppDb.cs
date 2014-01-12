using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity.Infrastructure;

namespace EventsWebApp.Models
{
    public class EventsAppDb : DbContext
    {
        public EventsAppDb()
            : base("DefaultConnection")
        {
        }

        /*
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
        */

        public DbSet<Event_> Events { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Song> Songs { get; set; }


    }

}