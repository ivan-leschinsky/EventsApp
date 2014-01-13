using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventsWebApp.Models;
using EventsWebApp.Workers;
using System.Data;
using System.IO;
using EventsWebApp.Filters;

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
            if (Request.IsAjaxRequest())
                return PartialView("_SubscribePartial");

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

                string IndexPath = Server.MapPath("~/Index");
                iWorker.CreateIndex(event_, IndexPath);

                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }


        public ActionResult Search(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                searchText = "empty";
            }
            currentuser = userprofileRepository.AllIncluding(user => user.Events).FirstOrDefault(user => user.UserName == User.Identity.Name);

            string IndexPath = Server.MapPath("~/Index");

            List<Event_> events = iWorker.SearchEvents(IndexPath, searchText, event_Repository);

            List<Event_> sortedEvents = eWorker.GetEventsByInterests(events, currentuser);

            return View(new SearchModel(searchText, sortedEvents.Count.ToString(), sortedEvents));
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
                ViewBag.CurrentUserId = currentuser.UserId;
                if (currentuser.Events.Contains(event_))
                {
                    currentuser.Events.Remove(event_);
                    userprofileRepository.Save();
                    return PartialView("_SubscribePartial", event_);
                }
                else
                {
                    currentuser.Events.Add(event_);
                    userprofileRepository.Save();
                    return PartialView("_UnsubscribePartial", event_);
                }
                

                if (ReturnPage.Equals("Show"))
                {
                    return RedirectToAction("Show", new { id = id });
                }
            }
            return RedirectToAction("Index");
        }

        #region Partial Views Actions
        public ActionResult SubscribeAjax(int id, string ReturnPage)
        {

            currentuser = userprofileRepository.AllIncluding(user => user.Events).FirstOrDefault(user => user.UserName == User.Identity.Name);
            var event_ = event_Repository.Find(id);
            if (eWorker.CheckForArchive(event_) && eWorker.IsEventIcludeUserInterests(currentuser, event_))
            {
                ViewBag.CurrentUserId = currentuser.UserId;
                if (currentuser.Events.Contains(event_))
                {
                    currentuser.Events.Remove(event_);
                    userprofileRepository.Save();
                    return PartialView("_SubscribePartial", event_);
                }
                else
                {
                    currentuser.Events.Add(event_);
                    userprofileRepository.Save();
                    return PartialView("_UnsubscribePartial", event_);
                }

            }
            return RedirectToAction("Index");
        }
        #endregion


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

