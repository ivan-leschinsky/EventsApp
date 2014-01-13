using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace EventsWebApp.Models
{ 
    public class UserProfileRepository : IUserProfileRepository
    {

        EventsAppDb context;

        public UserProfileRepository(EventsAppDb context)
        {
            this.context = context;
        }

        public IQueryable<UserProfile> All
        {
            get { return context.UserProfiles; }
        }

        public IQueryable<UserProfile> AllIncluding(params Expression<Func<UserProfile, object>>[] includeProperties)
        {
            IQueryable<UserProfile> query = context.UserProfiles;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }
       
        public UserProfile Find(int id)
        {
            return context.UserProfiles.Find(id);
        }

        public UserProfile GetbyName(string Name)
        {
            return context.UserProfiles.FirstOrDefault(user => user.UserName == Name);
        }

        public void InsertOrUpdate(UserProfile userprofile)
        {
            if (userprofile.UserId == default(int)) {
                // New entity
                context.UserProfiles.Add(userprofile);
            } else {
                // Existing entity
                context.Entry(userprofile).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var userprofile = context.UserProfiles.Find(id);
            context.UserProfiles.Remove(userprofile);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Dispose() 
        {
            context.Dispose();
        }
    }

    public interface IUserProfileRepository : IDisposable
    {
        IQueryable<UserProfile> All { get; }
        IQueryable<UserProfile> AllIncluding(params Expression<Func<UserProfile, object>>[] includeProperties);
        UserProfile Find(int id);
        //UserProfile GetByName(string Name);
        //UserProfile GetIncluding(string Name,params Expression<Func<UserProfile, object>>[] includeProperties);
        void InsertOrUpdate(UserProfile userprofile);
        void Delete(int id);
        void Save();
    }
}