using System;

[Serializable]
public class ProductData
{
    public const int PROPERTIES_COUNT = 5;

    public int Id;
    public string ProductName;
    public string ShopName;
    public double Price;
    public int Quantity;

    public ProductData()
    {

    }

    public ProductData(int _id, string _prodName, string _shopName, double _price, int _quantity)
    {
        Id = _id;
        ProductName = _prodName;
        ShopName = _shopName;
        Price = _price;
        Quantity = _quantity;
    }

    public override string ToString()
    {
        return "nazwa=" + ProductName + " sklep=" + ShopName + " cena=" + Price + " ilość=" + Quantity;
    }

    public bool TheSameMainValues(ProductData second)
    {
        return ProductName.Equals(second.ProductName) && 
               ShopName.Equals(second.ShopName) &&
               Price.Equals(second.Price);
    }

    public bool TheSameIdAndMainValues(ProductData second)
    {
        return Id.Equals(second.Id) && TheSameMainValues(second);
    }
}
