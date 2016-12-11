using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ProductEntry : MonoBehaviour
{
    #region CLASS SETTINGS

    private static readonly Color EVEN_COLOR = new Color(1, 1, 1, 0.5f);
    private static readonly Color ODD_COLOR = new Color(1, 1, 1, 0.25f);
    private static readonly Color SELECTED_COLOR = new Color(1, 1, 0, 0.5f);

    private const string MISSING_ATTRIBUTES = "Nie wszystkie pola są uzupełnione";
    private const string WRONG_CREDENTIALS = "Błąd uwierzytelnienia, przeloguj się i ponów akcję";

    #endregion

    #region SCENE REFERENCES

    public GameObject EntryGameObject;
    public Transform EntryTransform;
    public LayoutElement LayoutElement;
    public Image BackgroundImage;
    //
    public Text ProductNameLabel;
    public Text ShopNameLabel;
    public Text PriceLabel;
    public InputField QuantityField;

    #endregion

    public ProductData EntryData { get; private set; }
    private Action<ProductEntry> pressedCallback;

    public void AsignData(bool isEven, bool isSelected, ProductData _entryData, Action<ProductEntry> _pressedCallback)
    {
        SetState(true);
        BackgroundImage.color = isSelected ? SELECTED_COLOR :
                                isEven ? EVEN_COLOR : ODD_COLOR;
        EntryData = _entryData;
        ProductNameLabel.text = EntryData.ProductName;
        ShopNameLabel.text = EntryData.ShopName;
        PriceLabel.text = EntryData.Price.ToString("F2");
        QuantityField.text = EntryData.Quantity.ToString();
        pressedCallback = _pressedCallback;
    }

    public void SetState(bool state)
    {
        EntryGameObject.SetActive(state);
    }

    public void OnEntryPressed()
    {
        if (BackgroundImage.color != SELECTED_COLOR)
        {
            pressedCallback(this);
        }
    }

    public void OnQuantityEditingFinished()
    {
        StartCoroutine(TryToEditQuantity());
    }

    private IEnumerator TryToEditQuantity()
    {
        ApplicationManager.Instance.UiManager.LoadingScreen.Show();
        string login = ApplicationManager.Instance.GetStoredLogin();
        string password = ApplicationManager.Instance.GetStoredPassword();
        int newQuantity = int.Parse(QuantityField.text);
        WWW dataRequest = new WWW(ApplicationManager.Instance.ServerURL + "Update" + "?login=" + login + "&password=" + password + "&productID=" + EntryData.Id + "&quantity=" + newQuantity);
        yield return dataRequest;
        ApplicationManager.Instance.UiManager.LoadingScreen.Hide();
        if (string.IsNullOrEmpty(dataRequest.error))
        {
            if (!dataRequest.text.Equals(WRONG_CREDENTIALS) && !dataRequest.text.Equals(MISSING_ATTRIBUTES))
            {
                int itemIndex = ApplicationManager.Instance.StoredProducts.IndexOf(EntryData);
                ApplicationManager.Instance.StoredProducts[itemIndex].Quantity = newQuantity;
                EntryData.Quantity = newQuantity;
            }
            else
            {
                QuantityField.text = EntryData.Quantity.ToString();
            }
        }
        else
        {
            QuantityField.text = EntryData.Quantity.ToString();
        }
    }
}
