using System;
using System.Collections;
using UnityEngine;

public static class WebRequestsUtility
{
    #region CLASS SETTINGS

    private const string LOGIN_PREF = "login";
    private const string PASSWORD_PREF = "password";

    private const string ACCOUNT_WITH_THIS_LOGIN_EXISTS = "Podany login jest zajęty, wybierz inny";
    private const string CREDENTIALS_WRONG = "Błędny login lub hasło";
    private const string UNREACHABLE_HOST = "Usługa sieciowa niedostępna, spróbuj jeszcze raz";
    private const string UNKNOWN_ERROR = "Nieznany bład, spróbuj jeszcze raz";
    private const string WRONG_CREDENTIALS = "Błąd uwierzytelnienia, przeloguj się i ponów akcję";

    #endregion

    public static string storedLogin;
    public static string storedPassword;
    public static bool HasStoredCredentials { get { return !string.IsNullOrEmpty(GetStoredLogin()) && !string.IsNullOrEmpty(GetStoredPassword()); } }

    public static string requestError;

    #region CREDENTIALS

    public static string GetStoredLogin()
    {
        if (string.IsNullOrEmpty(storedLogin))
        {
            storedLogin = PlayerPrefs.GetString(LOGIN_PREF, null);
        }
        return storedLogin;
    }

    public static string GetStoredPassword()
    {
        if (string.IsNullOrEmpty(storedPassword))
        {
            storedPassword = PlayerPrefs.GetString(PASSWORD_PREF, null);
        }
        return storedPassword;
    }

    public static void RemoveCredentials()
    {
        storedLogin = null;
        storedPassword = null;
        if (PlayerPrefs.HasKey(LOGIN_PREF)) PlayerPrefs.DeleteKey(LOGIN_PREF);
        if (PlayerPrefs.HasKey(PASSWORD_PREF)) PlayerPrefs.DeleteKey(PASSWORD_PREF);
        PlayerPrefs.Save();
    }
    public static void SaveCredentials()
    {
        PlayerPrefs.SetString(LOGIN_PREF, storedLogin);
        PlayerPrefs.SetString(PASSWORD_PREF, storedPassword);
        PlayerPrefs.Save();
    }

    #endregion

    #region WEB REQUESTS

    public static IEnumerator TryCreateNewAccount(Action successCallback, Action failCallback)
    {
        ApplicationManager.Instance.UiManager.LoadingScreen.Show();
        WWW dataRequest = new WWW(ApplicationManager.Instance.ServerURL + "Create" + "?login=" + storedLogin + "&password=" + storedPassword);
        yield return dataRequest;
        ApplicationManager.Instance.UiManager.LoadingScreen.Hide();
        if (!CheckErrors(dataRequest)) successCallback();
        else failCallback();
    }

    public static IEnumerator TryRemoveAccount(Action successCallback)
    {
        ApplicationManager.Instance.UiManager.LoadingScreen.Show();
        WWW dataRequest = new WWW(ApplicationManager.Instance.ServerURL + "Remove" + "?login=" + storedLogin + "&password=" + storedPassword);
        yield return dataRequest;
        ApplicationManager.Instance.UiManager.LoadingScreen.Hide();
        if (!CheckErrors(dataRequest)) successCallback();
    }

    public static IEnumerator TryGetData(Action<string> successCallback, Action failedCallback)
    {
        ApplicationManager.Instance.UiManager.LoadingScreen.Show();
        WWW dataRequest = new WWW(ApplicationManager.Instance.ServerURL + "Data" + "?login=" + storedLogin + "&password=" + storedPassword);
        yield return dataRequest;
        ApplicationManager.Instance.UiManager.LoadingScreen.Hide();
        if (!CheckErrors(dataRequest)) successCallback(dataRequest.text);
        else failedCallback();
    }

    public static IEnumerator UpdateData()
    {
        ApplicationManager.Instance.UiManager.LoadingScreen.Show();
        string jsonToSave = JsonUtility.ToJson(ApplicationManager.Instance.Products);
        WWW dataRequest = new WWW(ApplicationManager.Instance.ServerURL + "Update" + "?login=" + storedLogin + "&password=" + storedPassword,
                                  System.Text.Encoding.UTF8.GetBytes(jsonToSave));
        yield return dataRequest;
        ApplicationManager.Instance.UiManager.LoadingScreen.Hide();
    }

    private static bool CheckErrors(WWW dataRequest)
    {
        if (string.IsNullOrEmpty(dataRequest.error))
        {
            if (dataRequest.text.Equals(ACCOUNT_WITH_THIS_LOGIN_EXISTS) ||
                dataRequest.text.Equals(CREDENTIALS_WRONG))
            {
                requestError = dataRequest.text;
                return true;
            }
            else
            {
                requestError = "";
                return false;
            }
        }
        else
        {
            if(dataRequest.error.Contains("Could not resolve host"))
            {
                requestError = UNREACHABLE_HOST;
            }
            else
            {
                requestError = dataRequest.error;
            }
            return true;
        }
    }

    #endregion
}