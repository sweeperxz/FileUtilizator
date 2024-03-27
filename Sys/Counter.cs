using System;
using System.Collections;
using System.IO;

namespace FileUtilizator.Sys;

public class Counter
{
    public int FilesCount { get; private set; }
    public int DirectoriesCount { get; private set; }
    public double FolderSize { get; private set; }

    public Counter()
    {
        FilesCount = DirectoriesCount = 0;
    }

    public void Count(DirectoryInfo dir)
    {
        try
        {
            var files = new ArrayList();
            files.AddRange(dir.GetDirectories());
            files.AddRange(dir.GetFiles());

            foreach (var file in files)
                if (file is DirectoryInfo directoryInfo)
                {
                    DirectoriesCount++;
                    Count(directoryInfo);
                }
                else
                {
                    FilesCount++;
                }
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public void CountSize(DirectoryInfo dir)
    {
        try
        {
            var files = new ArrayList();
            files.AddRange(dir.GetDirectories());
            files.AddRange(dir.GetFiles());

            foreach (var file in files)
                if (file is DirectoryInfo)
                    CountSize(file as DirectoryInfo);
                else
                    FolderSize += ((FileInfo)file).Length / Math.Pow(1024, 2);
        }
        catch (Exception)
        {
            // ignored
        }
    }
}