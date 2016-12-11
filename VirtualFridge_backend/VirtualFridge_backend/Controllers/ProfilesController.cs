using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VirtualFridge_backend.Controllers
{
    [RoutePrefix("Profiles")]
    public class ProfilesController : Controller
    {
        // GET: Profiles
        public ActionResult Index()
        {
            string pathToUsersFile = Server.MapPath("users");
            string profiles = "";
            if (System.IO.File.Exists(pathToUsersFile))
            {
                profiles = System.IO.File.ReadAllText(pathToUsersFile);
            }
            return Content(profiles);
        }
    }
}