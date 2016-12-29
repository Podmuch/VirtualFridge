using System;
using System.Collections;
using System.Collections.Generic;
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

    #region LOCAL DATA

    public void RemoveLocalData()
    {
        string fileName = WebRequestsUtility.storedLogin + "_" + WebRequestsUtility.storedPassword;
        if (PlayerPrefs.HasKey(fileName)) PlayerPrefs.DeleteKey(fileName);
        PlayerPrefs.Save();
    }

    public void SaveLocalData()
    {
        Products.DeviceID = SystemInfo.deviceUniqueIdentifier;
        string fileName = WebRequestsUtility.storedLogin + "_" + WebRequestsUtility.storedPassword;
        PlayerPrefs.SetString(fileName, JsonUtility.ToJson(Products));
        PlayerPrefs.Save();
    }

    public bool LoadLocalData()
    {
        string fileName = WebRequestsUtility.storedLogin + "_" + WebRequestsUtility.storedPassword;
        if (PlayerPrefs.HasKey(fileName))
        {
            Products = JsonUtility.FromJson<ProductsList>(PlayerPrefs.GetString(fileName));
            return true;
        }
        else
        {
            Products = new ProductsList(SystemInfo.deviceUniqueIdentifier);
            return false;
        }
    }

    #endregion

    public void LoginToTheApp(string jsonData)
    {
        UpdateData(jsonData, () =>
        {
            WebRequestsUtility.SaveCredentials();
            UiManager.ChangeScreen(UiManager.MainScreen);
        });
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
        ServerState serverList = JsonUtility.FromJson<ServerState>(jsonData);
        if (Products == null)
        {
            Products = new ProductsList(SystemInfo.deviceUniqueIdentifier);
            Products.StoredProducts = serverList.StoredProducts;
        }
        else
        {
            Products.SynchronizationDate = DateTime.Now.ToString(new CultureInfo("en-us"));
            Products.UnsyncChanges.Clear();
            Products.StoredProducts = serverList.StoredProducts;
        }
        SaveLocalData();
        callback();
    }
}
