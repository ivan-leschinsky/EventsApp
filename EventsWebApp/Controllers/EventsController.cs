using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventsWebApp.Models;
using EventsWebApp.Workers;
using System.Data;
using SimpleLucene.Impl;
using EventsWebApp.Search;
using SimpleLucene.IndexManagement;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Directory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.Version;
using Lucene.Net.Documents;
using Lucene.Net.Store;
using Lucene.Net.Index;
using System.IO;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using MultilingualSite.Filters;

namespace EventsWebApp.Controllers
{
    [Authorize]
    [Culture]
    public class EventsController : Controller
    {
        private readonly IUserProfileRepository userprofileRepository;
        private readonly IEvent_Repository event_Repository;
        private readonly ISubjectRepository subjectRepository;
        private readonly ISongRepository songRepository;

        EventsAppDb db;
        UserProfile currentuser;
        IQueryable<UserProfile> allUsersIncluding;
        public EventsController()
        {
            db = new EventsAppDb();
            this.userprofileRepository = new UserProfileRepository(db);
            this.event_Repository = new Event_Repository(db);
            this.subjectRepository = new SubjectRepository(db);
            this.songRepository = new SongRepository(db);
            allUsersIncluding = userprofileRepository.AllIncluding(user => user.Events);
        }

        //
        // GET: /Events/

        public ActionResult Index()
        {
            currentuser = userprofileRepository.AllIncluding(user => user.Events).FirstOrDefault(user => user.UserName == User.Identity.Name);
            ViewBag.CurrentUser = currentuser;
            ViewBag.CurrentUserId = currentuser.UserId;
            ViewBag.ActiveSubscribers = allUsersIncluding.Where(i => i.Events.Count > 0).OrderByDescending(i => i.Events.Count).Take(5);

            return View(event_Repository.GetIndex(currentuser.UserId, event_ => event_.UserProfiles, event_ => event_.Songs).ToList());
        }

        public ViewResult Archived()
        {
            ViewBag.ActiveSubscribers = allUsersIncluding.Where(i => i.Events.Count > 0).OrderByDescending(i => i.Events.Count).Take(5);
            currentuser = userprofileRepository.AllIncluding(user => user.Events).FirstOrDefault(user => user.UserName == User.Identity.Name);
            return View(event_Repository.GetArchived(currentuser.UserId, event_ => event_.UserProfiles, event_ => event_.Songs).ToList());
        }

        

        public ViewResult Show(int id)
        {
            currentuser = userprofileRepository.AllIncluding(user => user.Events).FirstOrDefault(user => user.UserName == User.Identity.Name);
            ViewBag.CurrentUser = currentuser;
            ViewBag.CurrentUserId = currentuser.UserId;

            var Songs = db.Events.Include("Songs").FirstOrDefault(e => e.Event_Id == id).Songs;
            int j = Songs.Count;
            int i = 0;
            string playlist = "";
            foreach (var song in Songs)
            {
                playlist += "{\"file\":\"" + song.SongUrl + "\",\"comment\":\"" + song.SongName + "\"}";
                i++;
                if (i != j)
                {
                    playlist += ",";
                }
            }

            var event_ = event_Repository.Find(id);

            ViewBag.Playlist = playlist;
            ViewBag.Songs = Songs;

            ViewBag.Interests = eWorker.IsEventIcludeUserInterests(currentuser, event_);

            return View(event_);
        }

        //
        // GET: /Event_/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Event_/Create

        [HttpPost]
        public ActionResult Create(Event_ event_)
        {
            event_.Subjects = Request.Form["SubjectsIds"];
            if (ModelState.IsValid)
            {
                var userprofile = userprofileRepository.AllIncluding(user => user.Events).FirstOrDefault(user => user.UserName == User.Identity.Name);
                event_.UserID = userprofile.UserId;
                userprofile.Events.Add(event_);
                userprofileRepository.InsertOrUpdate(userprofile);
                userprofileRepository.Save();

                CreateIndex(event_);

                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }


        private void CreateIndex(Event_ entity)
        {
            var document = new Document();
            document.Add(new Field("Event_Id", entity.Event_Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("EventName", entity.EventName, Field.Store.YES, Field.Index.ANALYZED));
            if (!string.IsNullOrEmpty(entity.EventDescription))
            {
                document.Add(new Field("EventDescription", entity.EventDescription, Field.Store.YES, Field.Index.ANALYZED));
            }

            Directory directory = FSDirectory.Open(new DirectoryInfo(Server.MapPath("~/Index")));
            Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);

            var writer = new IndexWriter(directory, analyzer, false, IndexWriter.MaxFieldLength.LIMITED);
            writer.AddDocument(document);

            writer.Optimize();
            writer.Dispose();

        }


        public ActionResult Search(string searchText)
        {
            currentuser = userprofileRepository.AllIncluding(user => user.Events).FirstOrDefault(user => user.UserName == User.Identity.Name);

            string IndexPath = Server.MapPath("~/Index");
            var indexSearcher = new DirectoryIndexSearcher(new DirectoryInfo(IndexPath), true);

            Directory directory = FSDirectory.Open(new DirectoryInfo(Server.MapPath("~/Index")));
            Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);

            IndexReader indexReader = IndexReader.Open(directory, true);
            Searcher indexSearch = new IndexSearcher(indexReader);

            string[] fields = { "EventName", "EventDescription" };
            var queryParser = new Lucene.Net.QueryParsers.MultiFieldQueryParser(Version.LUCENE_30, fields, analyzer);

            var query = queryParser.Parse(searchText.ToLower() + "*");

            TopDocs resultDocs = indexSearch.Search(query, indexReader.MaxDoc);

            var hits = resultDocs.ScoreDocs;
            List<Event_> ev = new List<Event_>();
            foreach (var hit in hits)
            {
                var documentFromSearcher = indexSearch.Doc(hit.Doc);
                ev.Add(event_Repository.Find(int.Parse(documentFromSearcher.Get("Event_Id"))));
            }

            indexSearch.Dispose();
            directory.Dispose();
            
            List<Event_> trueEvents = new List<Event_>();

            foreach (var event_ in ev)
            {
                foreach (string userInterest in currentuser.Subjects.Split(','))
                {
                    if (event_.Subjects.Contains(userInterest))
                    {
                        trueEvents.Add(event_);
                    }

                }
            }

            return View(new SearchModel(searchText, resultDocs.TotalHits.ToString(), trueEvents));
        }

        

        [HttpPost]
        public ActionResult AddTrack(Song model)
        {
            var event_ = db.Events.Include("Songs").FirstOrDefault(e => e.Event_Id == model.Event_Id);
            if (eWorker.CheckForArchive(event_))
            {
                event_.Songs.Add(model);
                db.SaveChanges();
            }
            return RedirectToAction("Show", new { id = model.Event_Id });
        }

        public ActionResult RemoveTrack(int songId)
        {
            var eventId = songRepository.Find(songId).Event_Id;
            songRepository.Delete(songId);
            songRepository.Save();
            return RedirectToAction("Show", new { id = eventId });
        }

        public ActionResult Remove(int id)
        {
            var event_ = event_Repository.Find(id);
            if (eWorker.CheckForArchive(event_))
            {
                event_Repository.Delete(id);
                event_Repository.Save();
            }
            return RedirectToAction("Index");
        }


        public ActionResult Subscribe(int id, string ReturnPage)
        {
            currentuser = userprofileRepository.AllIncluding(user => user.Events).FirstOrDefault(user => user.UserName == User.Identity.Name);
            var event_ = event_Repository.Find(id);
            if (eWorker.CheckForArchive(event_) && eWorker.IsEventIcludeUserInterests(currentuser, event_))
            {
                if (currentuser.Events.Contains(event_))
                {
                    currentuser.Events.Remove(event_);
                }
                else
                {
                    currentuser.Events.Add(event_);
                }

                userprofileRepository.Save();
                if (ReturnPage.Equals("Show"))
                {
                    return RedirectToAction("Show", new { id = id });
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult ChangeCulture(string lang)
        {
            string returnUrl = Request.UrlReferrer.AbsolutePath;
            List<string> cultures = new List<string>() { "ru", "en"};
            if (!cultures.Contains(lang))
            {
                lang = "ru";
            }
            HttpCookie cookie = Request.Cookies["lang"];
            if (cookie != null)
                cookie.Value = lang;
            else
            {

                cookie = new HttpCookie("lang");
                cookie.HttpOnly = false;
                cookie.Value = lang;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            Response.Cookies.Add(cookie);
            return Redirect(returnUrl);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                userprofileRepository.Dispose();
                event_Repository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

