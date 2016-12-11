using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ApplicationManager : MonoBehaviour
{
    public static ApplicationManager Instance { get; private set; }

    #region CLASS SETTINGS

    private const string LOGIN_PREF = "login";
    private const string PASSWORD_PREF = "password";

    #endregion

    #region SCENE REFERENCES

    public UiManager UiManager;

    #endregion

    public bool HasStoredCredentials { get { return !string.IsNullOrEmpty(GetStoredLogin()) && !string.IsNullOrEmpty(GetStoredPassword()); } }
    private string storedLogin;
    private string storedPassword;

    public List<ProductData> StoredProducts;
    public string ServerURL = "http://virtualfridgebackend.azurewebsites.net/";

    #region MONO BEHAVIOUR

    private void Awake()
    {
        Instance = this;
        UiManager.Init();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    #endregion

    public string GetStoredLogin()
    {
        if(string.IsNullOrEmpty(storedLogin))
        {
            storedLogin = PlayerPrefs.GetString(LOGIN_PREF, null);
        }
        return storedLogin;
    }

    public string GetStoredPassword()
    {
        if(string.IsNullOrEmpty(storedPassword))
        {
            storedPassword = PlayerPrefs.GetString(PASSWORD_PREF, null);
        }
        return storedPassword;
    }

    public bool LoginToTheApp(string webDataInTsv)
    {
        if (string.IsNullOrEmpty(webDataInTsv))
        {
            StoredProducts = new List<ProductData>();
            SaveCredentials();
            UiManager.ChangeScreen(UiManager.MainScreen);
            return true;
        }
        else
        {
            StoredProducts = new List<ProductData>();
            string[] dataRecords = webDataInTsv.Split(new char[]{ '\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < dataRecords.Length; i++)
            {
                string[] properties = dataRecords[i].Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if(properties.Length != ProductData.PROPERTIES_COUNT)
                {
                    StoredProducts.Clear();
                    return false;
                }
                else
                {
                    StoredProducts.Add(new ProductData(int.Parse(properties[0]), properties[1], properties[2], decimal.Parse(properties[3]), int.Parse(properties[4])));
                }
            }
            SaveCredentials();
            UiManager.ChangeScreen(UiManager.MainScreen);
            return true;
        }
    }
    
    public void LogoutFromTheApp()
    {
        StoredProducts = new List<ProductData>();
        RemoveCredentials();
        UiManager.MainScreen.ProductsTable.UpdateData();
        UiManager.ChangeScreen(UiManager.LoginScreen);
    }

    public void RemoveCredentials()
    {
        storedLogin = null;
        storedPassword = null;
        if (PlayerPrefs.HasKey(LOGIN_PREF)) PlayerPrefs.DeleteKey(LOGIN_PREF);
        if (PlayerPrefs.HasKey(PASSWORD_PREF)) PlayerPrefs.DeleteKey(PASSWORD_PREF);
    }

    private void SaveCredentials()
    {
        storedLogin = UiManager.LoginScreen.LoginField.text;
        PlayerPrefs.SetString(LOGIN_PREF, storedLogin);
        storedPassword = UiManager.LoginScreen.PasswordField.text;
        PlayerPrefs.SetString(PASSWORD_PREF, storedPassword);
    }
}
