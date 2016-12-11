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

    public ProductsList(string _deviceID)
    {
        deviceID = _deviceID;
        modificationDate = DateTime.Now.ToString(new CultureInfo("en-us"));
        StoredProducts = new List<ProductData>();
    }
}