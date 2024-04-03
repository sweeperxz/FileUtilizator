using System;
using System.IO;
using System.Security.Cryptography;

namespace FileUtilizator.Sys;

public static class FileExtenction
{
    public static void DeleteFile(this FileInfo filePath)
    {
        if (!File.Exists(filePath.FullName)) return;
        using var fsInput = new FileStream(filePath.FullName, FileMode.Open, FileAccess.Read);
        using var ms = new MemoryStream();
        using var aes = new AesManaged();
        aes.GenerateKey();
        aes.GenerateIV();
        var encryptor = aes.CreateEncryptor();

        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            fsInput.CopyTo(cs);
        }

        fsInput.Close();

        File.WriteAllBytes(filePath.FullName, Array.Empty<byte>());

        File.Delete(filePath.FullName);
        Shell32.SHEmptyRecycleBin(IntPtr.Zero, null, RecycleFlags.SHERB_NOCONFIRMATION);
    }
    
    public static void RecursiveDeleteFolders(this DirectoryInfo baseDir, bool isRootDir)
    {
        if (!baseDir.Exists)
            return;
        foreach (var dir in baseDir.EnumerateDirectories()) RecursiveDeleteFolders(dir, false);
        foreach (var file in baseDir.GetFiles())
        {
            file.IsReadOnly = false;
            file.DeleteFile();
        }

        if (!isRootDir) baseDir.Delete();
    }
}