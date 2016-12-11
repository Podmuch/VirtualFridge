using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VirtualFridge_backend.Controllers
{
    [RoutePrefix("Update")]
    public class UpdateController : Controller
    {
        private const string MISSING_ATTRIBUTES = "Nie wszystkie pola są uzupełnione";
        private const string WRONG_CREDENTIALS = "Błąd uwierzytelnienia, przeloguj się i ponów akcję";
        // GET: Update
        public ActionResult Index(string login = null, string password = null)
        {
            string data = new System.IO.StreamReader(Request.InputStream,System.Text.Encoding.UTF8).ReadToEnd();
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(data))
            {
                return Content(MISSING_ATTRIBUTES);
            }
            else
            {
                string pathToUsersFile = Server.MapPath("users");
                if (System.IO.File.Exists(pathToUsersFile))
                {
                    string[] userFile = System.IO.File.ReadAllLines(pathToUsersFile);
                    for (int i = 0; i < userFile.Length; i++)
                    {
                        string[] properties = userFile[i].Split('\t');
                        if (properties[0].Equals(login) && properties[1].Equals(password))
                        {
                            UpdateProduct(login, data);
                            return Content("Produkt zaktualizowany");
                        }
                    }
                }
                return Content(WRONG_CREDENTIALS);
            }
        }

        private void UpdateProduct(string login, string data)
        {
            string pathToUserData = Server.MapPath(login);
            System.IO.File.WriteAllText(pathToUserData, data);
        }
    }
}