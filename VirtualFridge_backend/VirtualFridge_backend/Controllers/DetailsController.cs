using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VirtualFridge_backend.Controllers
{
    [RoutePrefix("Details")]
    public class DetailsController : Controller
    {
        // GET: Details
        public ActionResult Index(string login = null)
        {
            string details = "";
            if (!string.IsNullOrEmpty(login))
            {
                string pathToUserData = Server.MapPath(login);
                if (System.IO.File.Exists(pathToUserData))
                {
                    return Content(System.IO.File.ReadAllText(pathToUserData));
                }
            }
            return Content(details);
        }
    }
}