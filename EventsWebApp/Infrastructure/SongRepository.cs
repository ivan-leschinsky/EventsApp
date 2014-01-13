using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace EventsWebApp.Models
{ 
    public class SongRepository : ISongRepository
    {
        EventsAppDb context;

        public SongRepository(EventsAppDb context)
        {
            this.context = context;
        }

        public IQueryable<Song> All
        {
            get { return context.Songs; }
        }

        public IQueryable<Song> AllIncluding(params Expression<Func<Song, object>>[] includeProperties)
        {
            IQueryable<Song> query = context.Songs;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Song Find(int id)
        {
            return context.Songs.Find(id);
        }

        public void InsertOrUpdate(Song song)
        {
            if (song.SongId == default(int)) {
                context.Songs.Add(song);
            } else {
                context.Entry(song).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var song = context.Songs.Find(id);
            context.Songs.Remove(song);
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

    public interface ISongRepository : IDisposable
    {
        IQueryable<Song> All { get; }
        IQueryable<Song> AllIncluding(params Expression<Func<Song, object>>[] includeProperties);
        Song Find(int id);
        void InsertOrUpdate(Song song);
        void Delete(int id);
        void Save();
    }
}