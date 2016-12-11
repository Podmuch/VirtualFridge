using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductsTable : MonoBehaviour
{
    private const string MISSING_ATTRIBUTES = "Nie wszystkie pola są uzupełnione";
    private const string WRONG_CREDENTIALS = "Błąd uwierzytelnienia, przeloguj się i ponów akcję";

    #region SCENE REFERENCES

    public RectTransform Content;
    public LayoutElement Header;
    public List<ProductEntry> TableEntries;
    public GameObject RemoveButton;

    #endregion

    #region PROJECT REFERENCES

    public ProductEntry EntryPrefab;

    #endregion

    public ProductData SelectedProduct { get; private set; }

    public void UpdateData()
    {
        RemoveButton.SetActive(SelectedProduct != null);
        List<ProductData> products = ApplicationManager.Instance.StoredProducts;
        CreateMissingEntries(products.Count);
        DisableAllEntries();
        Content.sizeDelta = new Vector2(Content.sizeDelta.x, Header.preferredHeight * products.Count);
        for (int i = 0; i < products.Count; i++)
        {
            TableEntries[i].AsignData(i % 2 == 0, SelectedProduct == null ? false : products[i].Id == SelectedProduct.Id, 
                                      products[i], SelectNewProduct);
        }
    }

    public void RemoveSelectedItem()
    {
        if (SelectedProduct != null)
        {
            StartCoroutine(TryToRemoveSelectedProduct());
        }
        else
        {
            UpdateData();
        }
    }

    public void SelectNewProduct(ProductEntry entry)
    {
        SelectedProduct = entry.EntryData;
        UpdateData();
    }

    private void CreateMissingEntries(int count)
    {
        int entriesCount = TableEntries.Count;
        for (int i = entriesCount; i < count; i++)
        {
            ProductEntry entry = Instantiate<ProductEntry>(EntryPrefab);
            entry.EntryTransform.SetParent(Content);
            entry.EntryTransform.localPosition = Vector3.zero;
            entry.EntryTransform.localScale = Vector3.one;
            TableEntries.Add(entry);
        }
    }

    private void DisableAllEntries()
    {
        for (int i = 0; i < TableEntries.Count; i++)
        {
            TableEntries[i].SetState(false);
        }
    }

    private IEnumerator TryToRemoveSelectedProduct()
    {
        ApplicationManager.Instance.UiManager.LoadingScreen.Show();
        string login = ApplicationManager.Instance.GetStoredLogin();
        string password = ApplicationManager.Instance.GetStoredPassword();
        WWW dataRequest = new WWW(ApplicationManager.Instance.ServerURL + "RemoveProduct" + "?login=" + login + "&password=" + password + "&productID=" + SelectedProduct.Id);
        yield return dataRequest;
        ApplicationManager.Instance.UiManager.LoadingScreen.Hide();
        if (string.IsNullOrEmpty(dataRequest.error))
        {
            if (!dataRequest.text.Equals(WRONG_CREDENTIALS) && !dataRequest.text.Equals(MISSING_ATTRIBUTES))
            {
                int nextIndex = ApplicationManager.Instance.StoredProducts.IndexOf(SelectedProduct);
                ApplicationManager.Instance.StoredProducts.Remove(SelectedProduct);
                SelectedProduct = ApplicationManager.Instance.StoredProducts.Count > nextIndex ? ApplicationManager.Instance.StoredProducts[nextIndex] : null;
                UpdateData();
            }
        }
    }
}