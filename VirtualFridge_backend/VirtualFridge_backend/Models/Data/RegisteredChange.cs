using System;
using System.Globalization;

[Serializable]
public class RegisteredChange
{
    public string ModificationDate;
    public ProductData Product;
    public ChangeType Type;

    public RegisteredChange()
    {

    }
}