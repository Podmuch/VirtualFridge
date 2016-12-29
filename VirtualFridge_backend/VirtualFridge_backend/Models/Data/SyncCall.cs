using System;
using System.Collections.Generic;

[Serializable]
public class SyncCall
{
    public string DeviceID;
    public string SynchronizationDate;

    public List<RegisteredChange> UnsyncChanges;
}