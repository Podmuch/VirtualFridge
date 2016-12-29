using System;
using System.Collections.Generic;

[Serializable]
public class SyncCall
{
    public string DeviceID;
    public string SynchronizationDate;

    public List<RegisteredChange> UnsyncChanges;

    public SyncCall()
    {

    }

    public SyncCall(SyncCall _call)
    {
        DeviceID = _call.DeviceID;
        SynchronizationDate = _call.SynchronizationDate;
        UnsyncChanges = _call.UnsyncChanges;
    }
}