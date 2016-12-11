using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VirtualFridge_backend.Controllers
{
    [RoutePrefix("RemoveProduct")]
    public class RemoveProductController : Controller
    {
        private const string MISSING_ATTRIBUTES = "Nie wszystkie pola są uzupełnione";
        private const string WRONG_CREDENTIALS = "Błąd uwierzytelnienia, przeloguj się i ponów akcję";
        // GET: RemoveProduct
        public ActionResult Index(string login = null, string password = null, string productID = null)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(productID))
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
                            int id = 0;
                            if (int.TryParse(productID, out id))
                            {
                                RemoveProductFromList(login, id);
                            }
                            return Content("Produkt usunięty");
                        }
                    }
                }
                return Content(WRONG_CREDENTIALS);

            }
        }

        private void RemoveProductFromList(string login, int productId)
        {
            string pathToUserData = Server.MapPath(login);
            List<string> productsList = new List<string>();
            if (System.IO.File.Exists(pathToUserData))
            {
                productsList = new List<string>(System.IO.File.ReadAllLines(pathToUserData));
                productsList.RemoveAll(prod =>
                {
                    int id;
                    string[] properties = prod.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (properties.Length == 5)
                    {
                        id = int.Parse(properties[0]);
                        return id == productId;
                    }
                    else
                    {
                        return true;
                    }
                });
            }
            System.IO.File.WriteAllLines(pathToUserData, productsList.ToArray());
        }
    }
}