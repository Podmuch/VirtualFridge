﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class AddProductPopup : AbstractScreen
{
    private const string MISSING_ATTRIBUTES = "Nie wszystkie pola są uzupełnione";

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
            if (string.IsNullOrEmpty(NameField.text) || string.IsNullOrEmpty(ShopField.text) || string.IsNullOrEmpty(PriceField.text))
            {
                ErrorMessage.text = MISSING_ATTRIBUTES;
            }
            else
            {
                int uniqueId = GetUniqueID(ApplicationManager.Instance.Products.StoredProducts);
                ApplicationManager.Instance.Products.StoredProducts.Add(new ProductData(uniqueId, NameField.text, ShopField.text, double.Parse(PriceField.text), 0));
                ApplicationManager.Instance.SaveLocalData();
                StartCoroutine(WebRequestsUtility.TryGetData((data) =>
                {
                    ApplicationManager.Instance.UpdateData(data, () =>
                    {
                        Hide();
                        ApplicationManager.Instance.UiManager.MainScreen.ProductsTable.UpdateData();
                    });
                }, () => { }));
            }
        }
    }

    #endregion

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
