using System;
using System.Collections.Generic;
using System.Globalization;

[Serializable]
public class RegisteredChange
{
    public string ModificationDate;
    public ProductData Product;
    public ChangeType Type;

    public RegisteredChange(ProductData _product, ChangeType _type)
    {
        Product = new ProductData(_product.Id, _product.ProductName, _product.ShopName, _product.Price, _product.Quantity, _product.Owners);
        Type = _type;
        ModificationDate = DateTime.Now.ToString(new CultureInfo("en-us"));
    }

    public RegisteredChange(ProductData _product, int quantityChange)
    {
        Product = new ProductData(_product.Id, _product.ProductName, _product.ShopName, _product.Price, quantityChange, _product.Owners);
        Type = ChangeType.UPDATE;
        ModificationDate = DateTime.Now.ToString(new CultureInfo("en-us"));
    }

    public RegisteredChange(ProductData _product, string sharedOwner, bool isShared)
    {
        Product = new ProductData(_product.Id, _product.ProductName, _product.ShopName, _product.Price, _product.Quantity, new List<string>(new string[] { sharedOwner }));
        Type = isShared ? ChangeType.SHARE : ChangeType.UNSHARE;
        ModificationDate = DateTime.Now.ToString(new CultureInfo("en-us"));
    }
}