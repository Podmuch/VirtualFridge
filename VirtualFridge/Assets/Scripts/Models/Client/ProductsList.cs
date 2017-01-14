using System;
using System.Collections.Generic;
using System.Globalization;

[Serializable]
public class ProductsList : SyncCall
{
    public List<ProductData> StoredProducts;

    public ProductsList(string _deviceID)
    {
        DeviceID = _deviceID;
        SynchronizationDate = DateTime.Now.ToString(new CultureInfo("en-us"));
        StoredProducts = new List<ProductData>();
        UnsyncChanges = new List<RegisteredChange>();
    }

    public void AddNewElement(ProductData productData)
    {
        UnsyncChanges.Add(new RegisteredChange(productData, ChangeType.ADD));
        StoredProducts.Add(productData);
    }

    public void RemoveElement(ProductData productData)
    {
        UnsyncChanges.Add(new RegisteredChange(productData, ChangeType.REMOVE));
        StoredProducts.Remove(productData);
    }

    public ProductData UpdateElement(ProductData productData, int newQuantity)
    {
        int oldQuantity = productData.Quantity;
        productData.Quantity = newQuantity;
        UnsyncChanges.Add(new RegisteredChange(productData, newQuantity - oldQuantity));
        return productData;
    }

    public ProductData ChangeSubowners(ProductData productData, bool isShared, string sharedOwner)
    {
        if (isShared)
        {
            if (!productData.Owners.Contains(sharedOwner))
            {
                productData.Owners.Add(sharedOwner);
            }
        }
        else
        {
            productData.Owners.RemoveAll((o) => o.Equals(sharedOwner));
        }
        UnsyncChanges.Add(new RegisteredChange(productData, sharedOwner, isShared));
        return productData;
    }

    public SyncCall GetSyncCall()
    {
        return new SyncCall(this);
    }
}