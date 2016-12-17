using System;
using System.Collections;
using System.Globalization;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    public static ApplicationManager Instance { get; private set; }

    #region SCENE REFERENCES

    public UiManager UiManager;

    #endregion

    public ProductsList Products { get; set; }
    public string ServerURL = "http://virtualfridgebackend.azurewebsites.net/";

    #region MONO BEHAVIOUR

    private void Awake()
    {
        Instance = this;
        UiManager.Init();
        Products = null;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    #endregion

    public void LoginToTheApp(string jsonData)
    {
        LoadLocalData();
        UpdateData(jsonData, () =>
        {
            WebRequestsUtility.SaveCredentials();
            UiManager.ChangeScreen(UiManager.MainScreen);
        });
    }
    
    public bool EnterOfflineToTheApp()
    {
        bool accountExitsts = LoadLocalData();
        if (accountExitsts)
        {
            WebRequestsUtility.SaveCredentials();
            UiManager.ChangeScreen(UiManager.MainScreen);
        }
        else
        {
            WebRequestsUtility.RemoveCredentials();
        }
        return accountExitsts;
    }

    public void LogoutFromTheApp()
    {
        Products = null;
        WebRequestsUtility.RemoveCredentials();
        UiManager.MainScreen.ProductsTable.UpdateData();
        UiManager.ChangeScreen(UiManager.LoginScreen);
    }

    public void UpdateData(string jsonData, Action callback)
    {
        StartCoroutine(ResolveConflicts(JsonUtility.FromJson<ProductsList>(jsonData), callback));
    }

    public void RemoveLocalData()
    {
        string fileName = WebRequestsUtility.storedLogin + "_" + WebRequestsUtility.storedPassword;
        if (PlayerPrefs.HasKey(fileName)) PlayerPrefs.DeleteKey(fileName);
        PlayerPrefs.Save();
    }

    public void SaveLocalData()
    {
        Products.oldDeviceId = Products.deviceID;
        Products.oldModificationDate = Products.modificationDate;
        Products.deviceID = SystemInfo.deviceUniqueIdentifier;
        Products.modificationDate = DateTime.Now.ToString(new CultureInfo("en-us"));
        string fileName = WebRequestsUtility.storedLogin + "_" + WebRequestsUtility.storedPassword;
        PlayerPrefs.SetString(fileName, JsonUtility.ToJson(Products));
        PlayerPrefs.Save();
    }

    private bool LoadLocalData()
    {
        string fileName = WebRequestsUtility.storedLogin + "_" + WebRequestsUtility.storedPassword;
        if (PlayerPrefs.HasKey(fileName))
        {
            Products = JsonUtility.FromJson<ProductsList>(PlayerPrefs.GetString(fileName));
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator ResolveConflicts(ProductsList serverList, Action callback)
    {
        if(Products == null)
        {
            if (serverList != null)
            {
                Products = serverList;
            }
            else
            {
                Products = new ProductsList(SystemInfo.deviceUniqueIdentifier);
                yield return StartCoroutine(WebRequestsUtility.UpdateData());
            }
        }
        else if(serverList == null || (Products.oldDeviceId != null && Products.oldDeviceId.Equals(serverList.deviceID) &&
                Products.oldModificationDate != null && Products.oldModificationDate.Equals(serverList.modificationDate)))
        {
            yield return StartCoroutine(WebRequestsUtility.UpdateData());
        }
        else if(Products.deviceID.Equals(serverList.deviceID))
        {
            DateTime localDateTime = DateTime.Parse(Products.modificationDate, new CultureInfo("en-us"));
            DateTime serverDateTime = DateTime.Parse(serverList.modificationDate, new CultureInfo("en-us"));
            if (localDateTime < serverDateTime)
            {
                Products = serverList;
            }
            else
            {
                yield return StartCoroutine(WebRequestsUtility.UpdateData());
            }
        }
        else
        {
            ProductsList newList = new ProductsList(SystemInfo.deviceUniqueIdentifier);
            //serverList
            for (int i = 0; i < serverList.StoredProducts.Count;i++)
            {
                int index = Products.StoredProducts.FindIndex((p) => p.Id == serverList.StoredProducts[i].Id);
                if (index >= 0 && index < Products.StoredProducts.Count)
                {
                    if (Products.StoredProducts[index].Price != serverList.StoredProducts[i].Price ||
                        Products.StoredProducts[index].ProductName != serverList.StoredProducts[i].ProductName ||
                        Products.StoredProducts[index].Quantity != serverList.StoredProducts[i].Quantity ||
                        Products.StoredProducts[index].ShopName != serverList.StoredProducts[i].ShopName)
                    {
                        UiManager.ResolveConflictPopup.Show();
                        UiManager.ResolveConflictPopup.Init(Products.StoredProducts[index], serverList.StoredProducts[i]);
                        Products.StoredProducts.RemoveAt(index);
                        yield return StartCoroutine(UiManager.ResolveConflictPopup.WaitForAnswer());
                        if (UiManager.ResolveConflictPopup.SelectedProduct != null)
                        {
                            newList.StoredProducts.Add(UiManager.ResolveConflictPopup.SelectedProduct);
                        }
                    }
                    else
                    {
                        Products.StoredProducts.RemoveAt(index);
                        newList.StoredProducts.Add(serverList.StoredProducts[i]);
                    }
                }
                else
                {
                    UiManager.ResolveConflictPopup.Show();
                    UiManager.ResolveConflictPopup.Init(serverList.StoredProducts[i], true);
                    yield return StartCoroutine(UiManager.ResolveConflictPopup.WaitForAnswer());
                    if (UiManager.ResolveConflictPopup.SelectedProduct != null)
                    {
                        newList.StoredProducts.Add(UiManager.ResolveConflictPopup.SelectedProduct);
                    }
                }
            }
            //localList
            for(int i = 0; i < Products.StoredProducts.Count; i++)
            {
                UiManager.ResolveConflictPopup.Show();
                UiManager.ResolveConflictPopup.Init(Products.StoredProducts[i], false);
                yield return StartCoroutine(UiManager.ResolveConflictPopup.WaitForAnswer());
                if (UiManager.ResolveConflictPopup.SelectedProduct != null)
                {
                    newList.StoredProducts.Add(UiManager.ResolveConflictPopup.SelectedProduct);
                }
            }
            Products = newList;
            yield return StartCoroutine(WebRequestsUtility.UpdateData());
        }
        callback();
    }
}
