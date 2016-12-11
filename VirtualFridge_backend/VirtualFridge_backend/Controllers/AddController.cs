using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VirtualFridge_backend.Controllers
{
    [RoutePrefix("Add")]
    public class AddController : Controller
    {
        private const string MISSING_ATTRIBUTES = "Nie wszystkie pola są uzupełnione";
        private const string WRONG_CREDENTIALS = "Błąd uwierzytelnienia, przeloguj się i ponów akcję";
        // GET: Add
        public ActionResult Index(string login = null, string password = null, string product = null, string shop = null, string price = null)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(product) ||
                string.IsNullOrEmpty(shop) || string.IsNullOrEmpty(price))
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
                            decimal priceDec;
                            decimal.TryParse(price, out priceDec);
                            AddProductToTheList(login, product, shop, priceDec);
                            return Content("Produkt dodany");
                        }
                    }
                }
                return Content(WRONG_CREDENTIALS);
            }
        }

        private void AddProductToTheList(string login, string product, string shop, decimal price)
        {
            string pathToUserData = Server.MapPath(login);
            List<string> productsList = new List<string>();
            if (System.IO.File.Exists(pathToUserData))
            {
                productsList = new List<string>(System.IO.File.ReadAllLines(pathToUserData));
            }
            int uniqueId = GetUniqueID(productsList);
            productsList.Add(uniqueId + "\t" + product + "\t" + shop + "\t" + price + "\t0");
            System.IO.File.WriteAllLines(pathToUserData, productsList.ToArray());
        }

        private int GetUniqueID(List<string> productsList)
        {
            int uniqueId = productsList.Count;
            List<int> existingIds = new List<int>();
            for(int i = 0; i < productsList.Count; i++)
            {
                string[] properties = productsList[i].Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if(properties.Length == 5)
                {
                    existingIds.Add(int.Parse(properties[0]));
                }
            }
            while(existingIds.Exists(id => id == uniqueId))
            {
                uniqueId++;
            }
            return uniqueId;
        }
    }
}