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
        public ActionResult Index(string login = null, string password = null, string productID = null, string quantity = null)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(productID) || string.IsNullOrEmpty(quantity))
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
                            int id = 0, newQuantity = 0;
                            if (int.TryParse(productID, out id) && int.TryParse(quantity, out newQuantity))
                            {
                                UpdateProductQuantity(login, id, newQuantity);
                                return Content("Produkt zaktualizowany");
                            }
                            else
                            {
                                return Content(MISSING_ATTRIBUTES);
                            }
                        }
                    }
                }
                return Content(WRONG_CREDENTIALS);
            }
        }

        private void UpdateProductQuantity(string login, int productId, int quantity)
        {
            string pathToUserData = Server.MapPath(login);
            List<string> productsList = new List<string>();
            if (System.IO.File.Exists(pathToUserData))
            {
                productsList = new List<string>(System.IO.File.ReadAllLines(pathToUserData));
                int index = productsList.FindIndex(prod =>
                {
                    int id;
                    string[] properties = prod.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    return properties.Length == 5 && int.Parse(properties[0]) == productId;
                });
                //
                if (index >= 0 && index < productsList.Count)
                {
                    string[] prodProperties = productsList[index].Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    productsList[index] = prodProperties[0] + "\t" + prodProperties[1] + "\t" + prodProperties[2] + "\t" +
                                          prodProperties[3] + "\t" + quantity.ToString();
                }
            }
            System.IO.File.WriteAllLines(pathToUserData, productsList.ToArray());
        }
    }
}