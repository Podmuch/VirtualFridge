using UnityEngine;
using System.Collections;

public class MainScreen : AbstractScreen
{
    #region SCENE REFERENCES

    public AddProductPopup AddProductPopup;
    public RemoveAccountPopup RemoveAccountPopup;
    public ProductsTable ProductsTable;

    #endregion

    #region ABSTRACT SCREEN

    public override void Show()
    {
        base.Show();
        AddProductPopup.SetStateImmediately(false);
        RemoveAccountPopup.SetStateImmediately(false);
        ProductsTable.UpdateData();
    }

    #endregion

    #region INPUT HANDLING

    public void OnLogoutPressed()
    {
        if(isClickable)
        {
            ApplicationManager.Instance.LogoutFromTheApp();
        }
    }

    public void OnRefreshPressed()
    {
        if (isClickable)
        {
            StartCoroutine(WebRequestsUtility.TryGetData((data) =>
            {
                ApplicationManager.Instance.UpdateData(data, ProductsTable.UpdateData);
            }, ()=> { }));
        }
    }

    public void OnRemoveAccountPressed()
    {
        if(isClickable)
        {
            RemoveAccountPopup.Show();
        }
    }

    public void OnRemoveProductPressed()
    {
        if (isClickable)
        {
            ProductsTable.RemoveSelectedItem();
        }
    }

    public void OnAddProductPressed()
    {
        if (isClickable)
        {
            AddProductPopup.Show();
        }
    }

    #endregion
}
