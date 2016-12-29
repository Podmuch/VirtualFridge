﻿using System;
using System.Collections.Generic;

[Serializable]
public class ServerState
{
    public List<ProductData> StoredProducts;

    public ServerState()
    {
        StoredProducts = new List<ProductData>();
    }
}