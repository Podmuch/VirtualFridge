using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VirtualFridge_backend.Controllers
{
    [RoutePrefix("Remove")]
    public class RemoveController : Controller
    {
        // GET: Remove
        public ActionResult Index(string login = null, string password = null)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                return Content("Błąd uwierzytelnienia, przeloguj się i ponów akcję");
            }
            string pathToUsersFile = Server.MapPath("users");
            if (System.IO.File.Exists(pathToUsersFile))
            {
                List<string> accounts = new List<string>(System.IO.File.ReadAllLines(pathToUsersFile));
                if (accounts.Remove(login + "\t" + password))
                {
                    string userFile = Server.MapPath(login);
                    if (System.IO.File.Exists(userFile)) System.IO.File.Delete(userFile);
                    System.IO.File.WriteAllLines(pathToUsersFile, accounts.ToArray());
                    return Content("Konto usunięte");
                }
            }
            return Content("Błąd uwierzytelnienia, przeloguj się i ponów akcję");
        }
    }
}