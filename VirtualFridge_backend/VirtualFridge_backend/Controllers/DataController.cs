using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace VirtualFridge_backend.Controllers
{
    [RoutePrefix("Data")]
    public class DataController : Controller
    {
        private const string MISSING_ATTRIBUTES = "Nie wszystkie pola są uzupełnione";
        private const string CREDENTIALS_WRONG = "Błędny login lub hasło";
        private const string USER_LIST_FILE_NAME = "users";
        private const string DATA_FILE_NAME = "usersData";

        // GET: Data
        public ActionResult Index(string login = null, string password = null)
        {
            string data = new System.IO.StreamReader(Request.InputStream, System.Text.Encoding.UTF8).ReadToEnd();
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(data))
            {
                return Content(MISSING_ATTRIBUTES);
            }
            else
            {
                string pathToUsersFile = Server.MapPath(USER_LIST_FILE_NAME);
                if (System.IO.File.Exists(pathToUsersFile))
                {
                    string[] userFile = System.IO.File.ReadAllLines(pathToUsersFile);
                    for (int i = 0; i < userFile.Length; i++)
                    {
                        string[] properties = userFile[i].Split('\t');
                        if (properties[0].Equals(login) && properties[1].Equals(password))
                        {
                            return Content(UpdateProduct(login, data));
                        }
                    }
                }
                return Content(CREDENTIALS_WRONG);
            }
        }

        private string UpdateProduct(string login, string clientData)
        {
            string serverData = GetServerData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ServerState serverState = serializer.Deserialize<ServerState>(serverData);
            if (serverState == null) serverState = new ServerState();
            SyncCall clientChanges = serializer.Deserialize<SyncCall>(clientData);
            for (int i = 0; i < clientChanges.UnsyncChanges.Count; i++)
            {
                ProductData prodWithTheSameId = serverState.StoredProducts.Find((p) => p.Id.Equals(clientChanges.UnsyncChanges[i].Product.Id));
                //
                switch (clientChanges.UnsyncChanges[i].Type)
                {
                    case ChangeType.ADD:
                        if(prodWithTheSameId != null)
                        {
                            clientChanges.UnsyncChanges[i].Product.Id = GetUniqueID(serverState.StoredProducts);
                        }
                        if(clientChanges.UnsyncChanges[i].Product.Owners.Count == 0)
                        {
                            clientChanges.UnsyncChanges[i].Product.Owners.Add(login);
                        }
                        serverState.StoredProducts.Add(clientChanges.UnsyncChanges[i].Product);
                        break;
                    case ChangeType.REMOVE:
                        bool removed = false;
                        if (prodWithTheSameId != null)
                        {
                            int idOfRemovedItem = serverState.StoredProducts.FindIndex((p) => p.TheSameIdAndMainValues(clientChanges.UnsyncChanges[i].Product, login));
                            if (idOfRemovedItem >= 0 && idOfRemovedItem < serverState.StoredProducts.Count)
                            {
                                serverState.StoredProducts.RemoveAt(idOfRemovedItem);
                                removed = true;
                            }
                        }
                        //Remove first occured with the same data
                        if (!removed)
                        {
                            int idOfRemovedItem = serverState.StoredProducts.FindIndex((p) => p.TheSameMainValues(clientChanges.UnsyncChanges[i].Product, login));
                            if (idOfRemovedItem >= 0 && idOfRemovedItem < serverState.StoredProducts.Count)
                            {
                                serverState.StoredProducts.RemoveAt(idOfRemovedItem);
                            }
                        }
                        break;
                    case ChangeType.UPDATE:
                        bool updated = false;
                        if (prodWithTheSameId != null)
                        {
                            int idOfUpdatedItem = serverState.StoredProducts.FindIndex((p) => p.TheSameIdAndMainValues(clientChanges.UnsyncChanges[i].Product, login));
                            if (idOfUpdatedItem >= 0 && idOfUpdatedItem < serverState.StoredProducts.Count)
                            {
                                serverState.StoredProducts[idOfUpdatedItem].Quantity += clientChanges.UnsyncChanges[i].Product.Quantity;
                                updated = true;
                            }
                        }
                        //Update first occured with the same data
                        if (!updated)
                        {
                            int idOfUpdatedItem = serverState.StoredProducts.FindIndex((p) => p.TheSameMainValues(clientChanges.UnsyncChanges[i].Product, login));
                            if (idOfUpdatedItem >= 0 && idOfUpdatedItem < serverState.StoredProducts.Count)
                            {
                                serverState.StoredProducts[idOfUpdatedItem].Quantity += clientChanges.UnsyncChanges[i].Product.Quantity;
                                updated = true;
                            }
                        }
                        break;
                    case ChangeType.SHARE:
                    case ChangeType.UNSHARE:
                        bool shared = false;
                        if (prodWithTheSameId != null)
                        {
                            int idOfSharedItem = serverState.StoredProducts.FindIndex((p) => p.TheSameIdAndMainValues(clientChanges.UnsyncChanges[i].Product, login));
                            if (idOfSharedItem >= 0 && idOfSharedItem < serverState.StoredProducts.Count)
                            {
                                if (clientChanges.UnsyncChanges[i].Type == ChangeType.SHARE)
                                {
                                    if (!serverState.StoredProducts[idOfSharedItem].Owners.Contains(clientChanges.UnsyncChanges[i].Product.Owners[0]))
                                    {
                                        serverState.StoredProducts[idOfSharedItem].Owners.Add(clientChanges.UnsyncChanges[i].Product.Owners[0]);
                                    }
                                }
                                else
                                {
                                    serverState.StoredProducts[idOfSharedItem].Owners.RemoveAll((o) => o.Equals(clientChanges.UnsyncChanges[i].Product.Owners[0]));
                                }
                                shared = true;
                            }
                            //share first occured with the same data
                            if (!shared)
                            {
                                idOfSharedItem = serverState.StoredProducts.FindIndex((p) => p.TheSameMainValues(clientChanges.UnsyncChanges[i].Product, login));
                                if (idOfSharedItem >= 0 && idOfSharedItem < serverState.StoredProducts.Count)
                                {
                                    if (clientChanges.UnsyncChanges[i].Type == ChangeType.SHARE)
                                    {
                                        if (!serverState.StoredProducts[idOfSharedItem].Owners.Contains(clientChanges.UnsyncChanges[i].Product.Owners[0]))
                                        {
                                            serverState.StoredProducts[idOfSharedItem].Owners.Add(clientChanges.UnsyncChanges[i].Product.Owners[0]);
                                        }
                                    }
                                    else
                                    {
                                        serverState.StoredProducts[idOfSharedItem].Owners.RemoveAll((o) => o.Equals(clientChanges.UnsyncChanges[i].Product.Owners[0]));
                                    }
                                    shared = true;
                                }
                            }
                        }
                        break;
                }
            }
            //
            serverData = serializer.Serialize(serverState);
            System.IO.File.WriteAllText(Server.MapPath(DATA_FILE_NAME), serverData);
            //
            serverState.KeepOnlyOwnedProducts(login);
            serverData = serializer.Serialize(serverState);
            return serverData;
        }

        private string GetServerData()
        {
            string pathToUserData = Server.MapPath(DATA_FILE_NAME);
            if (System.IO.File.Exists(pathToUserData))
            {
                return System.IO.File.ReadAllText(pathToUserData);
            }
            else
            {
                return "";
            }
        }

        private int GetUniqueID(List<ProductData> productsList)
        {
            int uniqueId = productsList.Count;
            while (productsList.Exists(p => p.Id == uniqueId))
            {
                uniqueId++;
            }
            return uniqueId;
        }
    }
}