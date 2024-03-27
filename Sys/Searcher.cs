using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FileUtilizator.Sys;

public class Searcher
{
    public List<string> Results { get; } = [];

    public void Clear()
    {
        Results.Clear();
    }

    public void Search(DirectoryInfo dir, string fileName)
    {
        try
        {
            var tmpFiles = new ArrayList();
            tmpFiles.AddRange(dir.GetDirectories());
            tmpFiles.AddRange(dir.GetFiles());

            foreach (var file in tmpFiles)
                if (file is DirectoryInfo directoryInfo)
                {
                    if (Regex.IsMatch(directoryInfo.Name, fileName))
                        Results.Add(directoryInfo.FullName);

                    Search(directoryInfo, fileName);
                }
                else
                {
                    if (Regex.IsMatch((file as FileInfo)!.Name, fileName))
                        Results.Add((file as FileInfo)!.FullName);
                }
        }
        catch (Exception)
        {
            // ignored
        }
    }
}