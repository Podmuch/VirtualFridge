using System;
using System.Globalization;

[Serializable]
public class RegisteredChange
{
    public string ModificationDate;
    public ProductData Product;
    public ChangeType Type;

    public RegisteredChange(ProductData _product, ChangeType _type)
    {
        Product = new ProductData(_product.Id, _product.ProductName, _product.ShopName, _product.Price, _product.Quantity);
        Type = _type;
        ModificationDate = DateTime.Now.ToString(new CultureInfo("en-us"));
    }

    public RegisteredChange(ProductData _product, int quantityChange)
    {
        Product = new ProductData(_product.Id, _product.ProductName, _product.ShopName, _product.Price, quantityChange);
        Type = ChangeType.UPDATE;
    }
}