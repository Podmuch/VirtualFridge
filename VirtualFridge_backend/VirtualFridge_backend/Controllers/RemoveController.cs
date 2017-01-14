using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace VirtualFridge_backend.Controllers
{
    [RoutePrefix("Remove")]
    public class RemoveController : Controller
    {
        private const string DATA_FILE_NAME = "usersData";

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
                    string pathToUserData = Server.MapPath(DATA_FILE_NAME);
                    if (System.IO.File.Exists(pathToUserData))
                    {
                        string details = System.IO.File.ReadAllText(pathToUserData);
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        ServerState serverState = serializer.Deserialize<ServerState>(details);
                        serverState.RemoveAllOwnedBy(login);
                        details = serializer.Serialize(serverState);
                        System.IO.File.WriteAllText(Server.MapPath(DATA_FILE_NAME), details);
                    }
                    System.IO.File.WriteAllLines(pathToUsersFile, accounts.ToArray());
                    return Content("Konto usunięte");
                }
            }
            return Content("Błąd uwierzytelnienia, przeloguj się i ponów akcję");
        }
    }
}