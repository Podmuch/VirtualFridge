using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RemoveAccountPopup : AbstractScreen
{
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
            isClickable = false;
            ErrorMessage.text = "";
            StartCoroutine(WebRequestsUtility.TryRemoveAccount(OnAccountRemoved, OnRemoveFailed));
        }
    }

    #endregion
    
    private void OnRemoveFailed()
    {
        ErrorMessage.text = WebRequestsUtility.requestError;
        isClickable = true;
    }

    private void OnAccountRemoved()
    {
        ApplicationManager.Instance.RemoveLocalData();
        ApplicationManager.Instance.LogoutFromTheApp();
    }
}
