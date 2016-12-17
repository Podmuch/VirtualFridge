using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class ProductsList
{
    public string deviceID;
    public string modificationDate;
    public List<ProductData> StoredProducts;

    [NonSerialized] public string oldDeviceId;
    [NonSerialized] public string oldModificationDate;

    public ProductsList(string _deviceID)
    {
        deviceID = _deviceID;
        modificationDate = DateTime.Now.ToString(new CultureInfo("en-us"));
        oldDeviceId = deviceID;
        oldModificationDate = modificationDate;
        StoredProducts = new List<ProductData>();
    }
}