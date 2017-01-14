using System;
using System.Collections.Generic;
using System.Globalization;

[Serializable]
public class ProductsList : SyncCall
{
    public List<ProductData> StoredProducts;

    public ProductsList()
    {

    }

    public ProductsList(string _deviceID)
    {
        DeviceID = _deviceID;
        SynchronizationDate = DateTime.Now.ToString(new CultureInfo("en-us"));
        StoredProducts = new List<ProductData>();
        UnsyncChanges = new List<RegisteredChange>();
    }
}