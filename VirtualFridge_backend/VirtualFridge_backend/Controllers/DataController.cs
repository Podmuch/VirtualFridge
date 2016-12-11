using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VirtualFridge_backend.Controllers
{
    [RoutePrefix("Data")]
    public class DataController : Controller
    {
        // GET: Data
        public ActionResult Index(string login = null, string password = null)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                return Content("Brak loginu lub hasła");
            }
            string pathToUsersFile = Server.MapPath("users");
            if (System.IO.File.Exists(pathToUsersFile))
            {
                string[] userFile = System.IO.File.ReadAllLines(pathToUsersFile);
                for (int i = 0; i < userFile.Length; i++)
                {
                    string[] properties = userFile[i].Split('\t');
                    if (properties[0].Equals(login) && properties[1].Equals(password))
                    {
                        return Content(GetUserData(login));
                    }
                }
            }
            return Content("Błędny login lub hasło");
        }

        private string GetUserData(string login)
        {
            string pathToUserData = Server.MapPath(login);
            if (System.IO.File.Exists(pathToUserData))
            {
                return System.IO.File.ReadAllText(pathToUserData);
            }
            else
            {
                return "";
            }
        }
    }
}