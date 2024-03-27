using System.Collections.Generic;
using System.IO;

namespace FileUtilizator.Sys;

public class Drives
{
    public List<DriveInfo> Disks { get; }

    public Drives()
    {
        Disks = [];
        SetDrives();
    }

    private void SetDrives()
    {
        Disks.Clear();
        foreach (var i in DriveInfo.GetDrives())
            if (i.IsReady)
                Disks.Add(i);
    }
}