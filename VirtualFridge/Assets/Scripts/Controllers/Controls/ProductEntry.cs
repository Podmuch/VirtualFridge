using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

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
    public Dropdown SubownersDropdown;
    public GameObject RemoveButton;

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
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        for (int i = 0; i < EntryData.Owners.Count; i++)
        {
            if (!EntryData.Owners[i].Equals(WebRequestsUtility.storedLogin))
            {
                options.Add(new Dropdown.OptionData(EntryData.Owners[i]));
            }
        }
        SubownersDropdown.value = 0;
        SubownersDropdown.options = options;
        SubownersDropdown.interactable = options.Count > 0;
        RemoveButton.SetActive(SubownersDropdown.interactable);
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

    public void OnShareWithPressed()
    {
        ApplicationManager.Instance.UiManager.MainScreen.ShareProductPopup.ShareProduct(EntryData);
    }

    public void OnUnshareWithSelectedPressed()
    {
        if (SubownersDropdown.interactable)
        {
            EntryData = ApplicationManager.Instance.Products.ChangeSubowners(EntryData, false, SubownersDropdown.options[SubownersDropdown.value].text);
            ApplicationManager.Instance.SaveLocalData();
            StartCoroutine(WebRequestsUtility.TryGetData((data) => { ApplicationManager.Instance.UpdateData(data, ApplicationManager.Instance.UiManager.MainScreen.ProductsTable.UpdateData); }, () => { }));
        }
    }

    public void OnQuantityEditingFinished()
    {
        EntryData = ApplicationManager.Instance.Products.UpdateElement(EntryData, int.Parse(QuantityField.text));
        ApplicationManager.Instance.SaveLocalData();
        StartCoroutine(WebRequestsUtility.TryGetData((data) => { ApplicationManager.Instance.UpdateData(data, ApplicationManager.Instance.UiManager.MainScreen.ProductsTable.UpdateData); }, () => { }));
    }
}
