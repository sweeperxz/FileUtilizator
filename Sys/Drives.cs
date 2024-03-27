using System.Collections.Generic;
using System.IO;

namespace FileUtilizator.Sys;

public class Drives
{
    public Drives()
    {
        Disks = [];
        SetDrives();
    }

    public List<DriveInfo> Disks { get; }

    private void SetDrives()
    {
        Disks.Clear();
        foreach (var i in DriveInfo.GetDrives())
            if (i.IsReady)
                Disks.Add(i);
    }
}