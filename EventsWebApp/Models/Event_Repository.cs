using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace EventsWebApp.Models
{
    public class Event_Repository : IEvent_Repository
    {
        EventsAppDb context;

        public Event_Repository(EventsAppDb context)
        {
            this.context = context;
        }

        public IQueryable<Event_> All
        {
            get { return context.Events; }
        }


        public ICollection<Event_> GetIndex(int id, params Expression<Func<Event_, object>>[] includeProperties)
        {

            IQueryable<Event_> query = context.Events.Where(e => e.EventTime > DateTime.Now);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            query = query.OrderBy(e => e.EventTime);

            List<Event_> events = new List<Event_>();

            foreach (string userInterest in context.UserProfiles.Find(id).Subjects.Split(','))
            {
                foreach (Event_ event_ in query.ToList())
                {

                    if (event_.Subjects.Contains(userInterest))
                    {
                       events.Add(event_);
                    }

                }

            }

            return events;
        }


        public ICollection<Event_> GetArchived(int id, params Expression<Func<Event_, object>>[] includeProperties)
        {

            IQueryable<Event_> query = context.Events.Where(e => e.EventTime <= DateTime.Now);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            query = query.OrderByDescending(e => e.EventTime);

            List<Event_> events = new List<Event_>();

            foreach (string userInterest in context.UserProfiles.Find(id).Subjects.Split(','))
            {
                foreach (Event_ event_ in query.ToList())
                {

                    if (event_.Subjects.Contains(userInterest))
                    {
                        events.Add(event_);
                    }

                }

            }

            return events;
        }

        public IQueryable<Event_> AllIncluding(params Expression<Func<Event_, object>>[] includeProperties)
        {
            IQueryable<Event_> query = context.Events;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Event_ Find(int id)
        {
            return context.Events.Find(id);
        }

        public void InsertOrUpdate(Event_ event_)
        {
            if (event_.Event_Id == default(int))
            {
                // New entity
                context.Events.Add(event_);
            }
            else
            {
                // Existing entity
                context.Entry(event_).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var event_ = context.Events.Find(id);
            context.Events.Remove(event_);
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

    public interface IEvent_Repository : IDisposable
    {
        IQueryable<Event_> All { get; }
        ICollection<Event_> GetIndex(int id, params Expression<Func<Event_, object>>[] includeProperties);
        ICollection<Event_> GetArchived(int id, params Expression<Func<Event_, object>>[] includeProperties);
        IQueryable<Event_> AllIncluding(params Expression<Func<Event_, object>>[] includeProperties);
        Event_ Find(int id);
        void InsertOrUpdate(Event_ event_);
        void Delete(int id);
        void Save();
    }
}