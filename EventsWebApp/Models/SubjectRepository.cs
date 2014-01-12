using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace EventsWebApp.Models
{ 
    public class SubjectRepository : ISubjectRepository
    {
        EventsAppDb context;

        public SubjectRepository(EventsAppDb context)
        {
            this.context = context;
        }

        public IQueryable<Subject> All
        {
            get { return context.Subjects; }
        }

        public IQueryable<Subject> AllIncluding(params Expression<Func<Subject, object>>[] includeProperties)
        {
            IQueryable<Subject> query = context.Subjects;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Subject Find(int id)
        {
            return context.Subjects.Find(id);
        }

        public void InsertOrUpdate(Subject subject)
        {
            if (subject.SubjectId == default(int)) {
                // New entity
                context.Subjects.Add(subject);
            } else {
                // Existing entity
                context.Entry(subject).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var subject = context.Subjects.Find(id);
            context.Subjects.Remove(subject);
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

    public interface ISubjectRepository : IDisposable
    {
        IQueryable<Subject> All { get; }
        IQueryable<Subject> AllIncluding(params Expression<Func<Subject, object>>[] includeProperties);
        Subject Find(int id);
        void InsertOrUpdate(Subject subject);
        void Delete(int id);
        void Save();
    }
}