using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VirtualFridge_backend.Controllers
{
    [RoutePrefix("Create")]
    public class CreateController : Controller
    {
        // GET: Create
        public ActionResult Index(string login = null, string password = null)
        {
            if(string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                return Content("Brak loginu lub hasła");
            }
            string pathToUsersFile = Server.MapPath("users");
            if (System.IO.File.Exists(pathToUsersFile))
            {
                string[] userFile = System.IO.File.ReadAllLines(pathToUsersFile);
                for(int i = 0; i < userFile.Length; i++)
                {
                    string[] properties = userFile[i].Split('\t');
                    if(properties[0].Equals(login))
                    {
                        return Content("Podany login jest zajęty, wybierz inny");
                    }
                }
            }
            System.IO.File.AppendAllLines(pathToUsersFile, new string[]{ login + "\t" + password });
            return Content("Profil utworzony");
        }
    }
}