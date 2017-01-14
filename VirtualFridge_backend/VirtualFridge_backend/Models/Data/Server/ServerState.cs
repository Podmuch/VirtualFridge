using System;
using System.Collections.Generic;

[Serializable]
public class ServerState
{
    public List<ProductData> StoredProducts;

    public ServerState()
    {
        StoredProducts = new List<ProductData>();
    }

    public void KeepOnlyOwnedProducts(string login)
    {
        StoredProducts = StoredProducts.FindAll((p) => p.Owners.Contains(login));
    }

    public void RemoveAllOwnedBy(string login)
    {
        StoredProducts.RemoveAll((p) => p.Owners.Count == 1 && p.Owners[0].Equals(login));
        for(int i = 0; i < StoredProducts.Count; i++)
        {
            StoredProducts[i].Owners.Remove(login);
        }
    }
}