using System.Collections.Generic;

namespace FileUtilizator.Sys;

public class Buffer
{
    private readonly List<string> _files = [];
    private readonly List<string> _folders = [];
    public TransferType Type { get; set; }

    public void AddFile(string name)
    {
        _files.Add(name);
    }

    public void AddFolder(string name)
    {
        _folders.Add(name);
    }

    public List<string> GetFiles()
    {
        return _files;
    }

    public List<string> GetFolders()
    {
        return _folders;
    }

    public void Clear()
    {
        _files.Clear();
        _folders.Clear();
    }
}