using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoginScreen : AbstractScreen
{
    #region CLASS SETTINGS

    private const string MISSING_CREDENTIALS = "Uzupełnij hasło lub login";
    private const string ACCOUNT_WITH_THIS_LOGIN_EXISTS = "Podany login jest zajęty, wybierz inny";

    #endregion

    #region SCENE REFERENCES

    public InputField LoginField;
    public InputField PasswordField;
    public Text ErrorMessage;

    #endregion

    #region MONO BEHAVIOUR

    private void Start()
    {
        if(ApplicationManager.Instance.HasStoredCredentials)
        {
            LoginField.text = ApplicationManager.Instance.GetStoredLogin();
            PasswordField.text = ApplicationManager.Instance.GetStoredPassword();
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
            StartCoroutine(TryCreateNewAccount());
        }
    }

    public void OnLoginPressed()
    {
        if(isClickable)
        {
            StartCoroutine(TryLoginToTheApp());
        }
    }

    #endregion

    private IEnumerator TryCreateNewAccount()
    {
        if (string.IsNullOrEmpty(LoginField.text) || string.IsNullOrEmpty(PasswordField.text))
        {
            ErrorMessage.text = MISSING_CREDENTIALS;
        }
        else
        {
            isClickable = false;
            ErrorMessage.text = "";
            ApplicationManager.Instance.UiManager.LoadingScreen.Show();
            WWW dataRequest = new WWW(ApplicationManager.Instance.ServerURL + "Create" + "?login="+ LoginField.text + "&password=" + PasswordField.text);
            yield return dataRequest;
            ApplicationManager.Instance.UiManager.LoadingScreen.Hide();
            isClickable = true;
            if (string.IsNullOrEmpty(dataRequest.error))
            {
                if (dataRequest.text.Equals(ACCOUNT_WITH_THIS_LOGIN_EXISTS))
                {
                    ErrorMessage.text = dataRequest.text;
                }
                else
                {
                    OnLoginPressed();
                }
            }
            else
            {
                ErrorMessage.text = Utilities.GetErrorMessageFromString(dataRequest.error);
            }
        }
    }

    private IEnumerator TryLoginToTheApp()
    {
        if(string.IsNullOrEmpty(LoginField.text) || string.IsNullOrEmpty(PasswordField.text))
        {
            ErrorMessage.text = MISSING_CREDENTIALS;
        }
        else
        {
            isClickable = false;
            ErrorMessage.text = "";
            ApplicationManager.Instance.UiManager.LoadingScreen.Show();
            WWW dataRequest = new WWW(ApplicationManager.Instance.ServerURL + "Data" + "?login=" + LoginField.text + "&password=" + PasswordField.text);
            yield return dataRequest;
            ApplicationManager.Instance.UiManager.LoadingScreen.Hide();
            isClickable = true;
            if (string.IsNullOrEmpty(dataRequest.error))
            {
                if(!ApplicationManager.Instance.LoginToTheApp(dataRequest.text))
                {
                    ErrorMessage.text = dataRequest.text;
                    ApplicationManager.Instance.RemoveCredentials();
                }
            }
            else
            {
                ErrorMessage.text = Utilities.GetErrorMessageFromString(dataRequest.error);
            }
        }
    }
}
