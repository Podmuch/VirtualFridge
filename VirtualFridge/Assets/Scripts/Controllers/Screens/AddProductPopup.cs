using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class AddProductPopup : AbstractScreen
{
    private const string MISSING_ATTRIBUTES = "Nie wszystkie pola są uzupełnione";
    private const string WRONG_CREDENTIALS = "Błąd uwierzytelnienia, przeloguj się i ponów akcję";

    #region SCENE REFERENCES

    public InputField NameField;
    public InputField ShopField;
    public InputField PriceField;
    public Text ErrorMessage;

    #endregion

    #region ABSTRACT SCREEN

    public override void Show()
    {
        base.Show();
        ErrorMessage.text = "";
        NameField.text = "";
        ShopField.text = "";
        PriceField.text = "";
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

    public void OnAddButton()
    {
        if (isClickable)
        {
            StartCoroutine(TryToAddProduct());
        }
    }

    #endregion

    private IEnumerator TryToAddProduct()
    {
        if (string.IsNullOrEmpty(NameField.text) || string.IsNullOrEmpty(ShopField.text) || string.IsNullOrEmpty(PriceField.text))
        {
            ErrorMessage.text = MISSING_ATTRIBUTES;
        }
        else
        {
            isClickable = false;
            ErrorMessage.text = "";
            ApplicationManager.Instance.UiManager.LoadingScreen.Show();
            string login = ApplicationManager.Instance.GetStoredLogin();
            string password = ApplicationManager.Instance.GetStoredPassword();
            WWW dataRequest = new WWW(ApplicationManager.Instance.ServerURL + "Add" + "?login=" + login + "&password=" + password +
                                      "&product=" + NameField.text + "&shop=" + ShopField.text + "&price=" + decimal.Parse(PriceField.text).ToString("F2"));
            yield return dataRequest;
            ApplicationManager.Instance.UiManager.LoadingScreen.Hide();
            if (string.IsNullOrEmpty(dataRequest.error))
            {
                if (dataRequest.text.Equals(WRONG_CREDENTIALS) || dataRequest.text.Equals(MISSING_ATTRIBUTES))
                {
                    isClickable = true;
                    ErrorMessage.text = dataRequest.text;
                }
                else
                {
                    Hide();
                    int uniqueId = GetUniqueID(ApplicationManager.Instance.StoredProducts);
                    ApplicationManager.Instance.StoredProducts.Add(new ProductData(uniqueId, NameField.text, ShopField.text, decimal.Parse(PriceField.text), 0));
                    ApplicationManager.Instance.UiManager.MainScreen.ProductsTable.UpdateData();
                }
            }
            else
            {
                isClickable = true;
                ErrorMessage.text = Utilities.GetErrorMessageFromString(dataRequest.error);
            }
        }
    }

    private int GetUniqueID(List<ProductData> productsList)
    {
        int uniqueId = productsList.Count;
        while (productsList.Exists(p => p.Id == uniqueId))
        {
            uniqueId++;
        }
        return uniqueId;
    }
}
