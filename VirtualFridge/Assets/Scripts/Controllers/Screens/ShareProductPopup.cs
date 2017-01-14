using UnityEngine;
using UnityEngine.UI;

public class ShareProductPopup : AbstractScreen
{
    private const string MISSING_ATTRIBUTES = "Nie wszystkie pola są uzupełnione";

    #region SCENE REFERENCES

    public InputField NameField;
    public Text Message;
    public Text ErrorMessage;

    #endregion

    private ProductData EntryData;

    #region ABSTRACT SCREEN

    public override void Show()
    {
        base.Show();
        ErrorMessage.text = "";
        NameField.text = "";
        Message.text = "";
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

    public void OnShareButton()
    {
        if (isClickable)
        {
            if (string.IsNullOrEmpty(NameField.text))
            {
                ErrorMessage.text = MISSING_ATTRIBUTES;
            }
            else
            {
                EntryData = ApplicationManager.Instance.Products.ChangeSubowners(EntryData, true, NameField.text);
                ApplicationManager.Instance.SaveLocalData();
                StartCoroutine(WebRequestsUtility.TryGetData((data) => { ApplicationManager.Instance.UpdateData(data, HideWithUpdate); }, HideWithUpdate));
            }
        }
    }

    #endregion

    public void ShareProduct(ProductData data)
    {
        EntryData = data;
        Show();
        Message.text = "Wpisz nick użytkownika z którym chcesz współdzielić produkt <color=#00ff00>" + EntryData.ProductName + "</color> ze sklepu <color=#00ff00>" + EntryData.ShopName + "</color>";
    }

    private void HideWithUpdate()
    {
        Hide();
        ApplicationManager.Instance.UiManager.MainScreen.ProductsTable.UpdateData();
    }
}