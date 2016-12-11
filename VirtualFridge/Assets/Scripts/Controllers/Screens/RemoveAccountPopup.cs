using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RemoveAccountPopup : AbstractScreen
{
    private const string WRONG_CREDENTIALS = "Błąd uwierzytelnienia, przeloguj się i ponów akcję";
    
    #region SCENE REFERENCE

    public Text ErrorMessage;

    #endregion

    #region ABSTRACT SCREEN

    public override void Show()
    {
        base.Show();
        ErrorMessage.text = "";
    }

    #endregion

    #region INPUT HANDLING

    public void OnCloseButton()
    {
        if (isClickable)
        {
            Hide();
        }
    }

    public void OnConfirmButton()
    {
        if (isClickable)
        {
            StartCoroutine(TryToRemoveAccount());
        }
    }

    #endregion

    private IEnumerator TryToRemoveAccount()
    {
        isClickable = false;
        ErrorMessage.text = "";
        ApplicationManager.Instance.UiManager.LoadingScreen.Show();
        string login = ApplicationManager.Instance.GetStoredLogin();
        string password = ApplicationManager.Instance.GetStoredPassword();
        WWW dataRequest = new WWW(ApplicationManager.Instance.ServerURL + "Remove" + "?login=" + login + "&password=" + password);
        yield return dataRequest;
        ApplicationManager.Instance.UiManager.LoadingScreen.Hide();
        if (string.IsNullOrEmpty(dataRequest.error))
        {
            if (dataRequest.text.Equals(WRONG_CREDENTIALS))
            {
                isClickable = true;
                ErrorMessage.text = dataRequest.text;
            }
            else
            {
                ApplicationManager.Instance.LogoutFromTheApp();
            }
        }
        else
        {
            isClickable = true;
            ErrorMessage.text = Utilities.GetErrorMessageFromString(dataRequest.error);
        }
    }
}
