using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace VirtualFridge_backend.Controllers
{
    [RoutePrefix("Details")]
    public class DetailsController : Controller
    {
        private const string DATA_FILE_NAME = "usersData";
        // GET: Details
        public ActionResult Index(string login = null)
        {
            string details = "";
            string pathToUserData = Server.MapPath(DATA_FILE_NAME);
            if (System.IO.File.Exists(pathToUserData))
            {
                details = System.IO.File.ReadAllText(pathToUserData);
            }
            //
            if(login != null && details != "")
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                ServerState serverState = serializer.Deserialize<ServerState>(details);
                serverState.KeepOnlyOwnedProducts(login);
                details = serializer.Serialize(serverState);
            }
            return Content(details);
        }
    }
}