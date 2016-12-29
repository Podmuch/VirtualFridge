using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class LoginScreen : AbstractScreen
{
    #region CLASS SETTINGS

    private const string MISSING_CREDENTIALS = "Uzupełnij hasło lub login";
    
    #endregion

    #region SCENE REFERENCES

    public InputField LoginField;
    public InputField PasswordField;
    public Text ErrorMessage;

    #endregion

    #region MONO BEHAVIOUR

    private void Start()
    {
        if(WebRequestsUtility.HasStoredCredentials)
        {
            LoginField.text = WebRequestsUtility.GetStoredLogin();
            PasswordField.text = WebRequestsUtility.GetStoredPassword();
            OnLoginPressed();
        }
    }

    #endregion

    #region ABSTRACT SCREEN

    public override void Show()
    {
        base.Show();
        ErrorMessage.text = "";
    }

    public override void SetStateImmediately(bool isShow)
    {
        base.SetStateImmediately(isShow);
        if (isShow) ErrorMessage.text = "";
    }

    #endregion

    #region INPUT HANDLING

    public void OnCreateNewAccountPressed()
    {
        if(isClickable)
        {
            if (string.IsNullOrEmpty(LoginField.text) || string.IsNullOrEmpty(PasswordField.text))
            {
                ErrorMessage.text = MISSING_CREDENTIALS;
            }
            else
            {
                isClickable = false;
                ErrorMessage.text = "";
                WebRequestsUtility.storedLogin = LoginField.text;
                WebRequestsUtility.storedPassword = PasswordField.text;
                StartCoroutine(WebRequestsUtility.TryCreateNewAccount(()=> 
                {
                    isClickable = true;
                    OnLoginPressed();
                }, () =>
                {
                    ErrorMessage.text = WebRequestsUtility.requestError;
                    isClickable = true;
                }));
            }
        }
    }

    public void OnLoginPressed()
    {
        if(isClickable)
        {
            if (string.IsNullOrEmpty(LoginField.text) || string.IsNullOrEmpty(PasswordField.text))
            {
                ErrorMessage.text = MISSING_CREDENTIALS;
            }
            else
            {
                isClickable = false;
                ErrorMessage.text = "";
                WebRequestsUtility.storedLogin = LoginField.text;
                WebRequestsUtility.storedPassword = PasswordField.text;
                bool accountExitsts = ApplicationManager.Instance.LoadLocalData();
                StartCoroutine(WebRequestsUtility.TryGetData(ApplicationManager.Instance.LoginToTheApp, ()=> { HandleLoginErrors(accountExitsts); }));
            }
        }
    }

    private void HandleLoginErrors(bool accountExitsts)
    {
        if (accountExitsts)
        {
            if (WebRequestsUtility.requestError == WebRequestsUtility.CREDENTIALS_WRONG)
            {
                ApplicationManager.Instance.RemoveLocalData();
                WebRequestsUtility.RemoveCredentials();
            }
            else
            {
                WebRequestsUtility.SaveCredentials();
                ApplicationManager.Instance.UiManager.ChangeScreen(ApplicationManager.Instance.UiManager.MainScreen);
            }
        }
        ErrorMessage.text = WebRequestsUtility.requestError;
        isClickable = true;
    }

    #endregion
}
