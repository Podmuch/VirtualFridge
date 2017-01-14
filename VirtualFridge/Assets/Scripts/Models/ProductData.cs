using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

[Serializable]
public class ProductData
{
    public const int PROPERTIES_COUNT = 5;

    public int Id;
    public string ProductName;
    public string ShopName;
    public double Price;
    public int Quantity;
    public List<string> Owners;

    public ProductData()
    {
        Owners = new List<string>();
    }

    public ProductData(int _id, string _prodName, string _shopName, double _price, int _quantity, List<string> _owners)
    {
        Id = _id;
        ProductName = _prodName;
        ShopName = _shopName;
        Price = _price;
        Quantity = _quantity;
        Owners = new List<string>(_owners);
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < Owners.Count; i++)
        {
            builder.Append(Owners[i] + ",");
        }
        return "nazwa=" + ProductName + " sklep=" + ShopName + " cena=" + Price + " ilość=" + Quantity + " właściciele=" + builder.ToString();
    }
}
