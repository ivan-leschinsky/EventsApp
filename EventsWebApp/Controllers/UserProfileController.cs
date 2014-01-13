using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventsWebApp.Models;
using System.Threading;
using System.IO;
using EventsWebApp.Filters;

namespace EventsWebApp.Controllers
{
    [Authorize]
    [Culture]
    public class UserProfileController : Controller
    {
        private readonly IUserProfileRepository userprofileRepository;
        private readonly ISubjectRepository subjectsRepository;

        EventsAppDb db;

        public UserProfileController()
        {

            db = new EventsAppDb();

            this.userprofileRepository = new UserProfileRepository(db);
            this.subjectsRepository = new SubjectRepository(db);
        }

        //
        // GET: /UserProfiles/

        public ViewResult Index()
        {
            return View(userprofileRepository.AllIncluding(userprofile => userprofile.Events));
        }

        //
        // GET: /UserProfiles/Details/5

        public ViewResult Show(int id)
        {
            var user = userprofileRepository.Find(id);
            
            if (user.UserName == User.Identity.Name)
            {
                ViewBag.CurrentUser = true;
            }
            else
            {
                ViewBag.CurrentUser = false;
            }
            if (string.IsNullOrEmpty(user.Subjects))
            {
                ViewBag.Subjects = "None,None".Split(',');
            }
            else
            {
                ViewBag.Subjects = user.Subjects.Split(',');
            }

            return View(user);
        }
        public ActionResult Edit()
        {
            UserProfile user = userprofileRepository.All.FirstOrDefault(u => u.UserName == User.Identity.Name);

            ViewBag.Subjects = subjectsRepository.All.ToList();

            if (string.IsNullOrEmpty(user.Subjects))
            {
                ViewBag.SubjectsChecked = "None,None".Split(',');
            }
            else
            {
                ViewBag.SubjectsChecked = user.Subjects.Split(',');
            }

            return View(user);
        }


        //
        // POST: /UserProfiles/Edit/5

        [HttpPost]
        public ActionResult Edit(UserProfile userprofile)
        {
            
            if (ModelState.IsValid)
            {
                userprofile.Subjects = Request.Form["SubjectsIds"];
                if (string.IsNullOrEmpty(userprofile.Subjects))
                {
                    userprofile.Subjects = "None,None";
                }

                userprofile.UserId = WebMatrix.WebData.WebSecurity.CurrentUserId;
                userprofileRepository.InsertOrUpdate(userprofile);
                userprofileRepository.Save();
                return RedirectToAction("Index", "Events");
            }
            else
            {
                return View();
            }
        }

        public ActionResult FirstSetup()
        {
            var userid = WebMatrix.WebData.WebSecurity.CurrentUserId;
            var user = userprofileRepository.Find(userid);

            user.UserAvatarUrl = "/Content/User_Images/default-user.png";
            user.UserTrueName = user.UserName;
            user.Subjects = "None,None";
            userprofileRepository.Save();

            ViewBag.Subjects = subjectsRepository.All.ToList();
            
            return View(user);
        }

        [HttpPost]
        public ActionResult UploadPicture(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                string extension = file.FileName.Substring(file.FileName.LastIndexOf('.'));
                var fileName = User.Identity.Name + extension;
                var path = Path.Combine(Server.MapPath("~/Content/User_Images"), fileName);
                string fullPath = "/Content/User_Images/"+fileName;
                file.SaveAs(path);
            
            UserProfile user = userprofileRepository.All.FirstOrDefault(u => u.UserName == User.Identity.Name);
            user.UserAvatarUrl = fullPath;
            userprofileRepository.Save();
            }
            return RedirectToAction("Edit");
        }

        [HttpPost]
        public ActionResult FirstSetup(UserProfile userprofile)
        {
            if (ModelState.IsValid)
            {
                userprofile.Subjects = Request.Form["SubjectsIds"];
                if (string.IsNullOrEmpty(userprofile.Subjects))
                    userprofile.Subjects = "None,None";
                userprofile.UserAvatarUrl = "/Content/User_Images/default-user.png";
                userprofileRepository.InsertOrUpdate(userprofile);
                userprofileRepository.Save();
                return RedirectToAction("Index", "Events");
            }
            else
            {
                return View();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                userprofileRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

