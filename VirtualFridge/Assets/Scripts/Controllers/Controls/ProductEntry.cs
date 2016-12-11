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
        int newQuantity = int.Parse(QuantityField.text);
        int itemIndex = ApplicationManager.Instance.Products.StoredProducts.IndexOf(EntryData);
        ApplicationManager.Instance.Products.StoredProducts[itemIndex].Quantity = newQuantity;
        EntryData.Quantity = newQuantity;
        ApplicationManager.Instance.SaveLocalData();
        StartCoroutine(WebRequestsUtility.TryGetData((data) =>
        {
            ApplicationManager.Instance.UpdateData(data, ApplicationManager.Instance.UiManager.MainScreen.ProductsTable.UpdateData);
        }, () => { }));
    }
}
