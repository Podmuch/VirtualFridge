using UnityEngine;
using System;

public class ProductData
{
    public const int PROPERTIES_COUNT = 5;

    public int Id;
    public string ProductName;
    public string ShopName;
    public decimal Price;
    public int Quantity;

    public ProductData(int _id, string _prodName, string _shopName, decimal _price, int _quantity)
    {
        Id = _id;
        ProductName = _prodName;
        ShopName = _shopName;
        Price = _price;
        Quantity = _quantity;
    }
}
