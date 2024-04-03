using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace FileUtilizator.Sys;

public class FileManager
{
    private readonly Buffer _buffer;

    // icons
    private readonly List<ImageList> _leftImagelist;

    private readonly ListView _leftListView;
    private readonly List<ImageList> _rightImagelist;
    private readonly ListView _rightListView;

    public FileManager(ListView left, ListView right,
        ImageList leftImg, ImageList rightImg,
        ImageList leftImgLarge, ImageList rightImgLarge)
    {
        _buffer = new Buffer();
        Drives = new Drives();
        LeftPath = RightPath = Drives.Disks[0].Name;
        Section = Section.Left;
        Directory.SetCurrentDirectory(LeftPath);

        _leftListView = left;
        _rightListView = right;

        //---------------------------------------------
        _leftImagelist = new List<ImageList>(2);
        _rightImagelist = new List<ImageList>(2);

        _leftImagelist.Add(leftImg);
        _leftImagelist.Add(leftImgLarge);

        _rightImagelist.Add(rightImg);
        _rightImagelist.Add(rightImgLarge);
        //---------------------------------------------

        LeftDirectory = new DirectoryInfo(LeftPath);
        RightDirectory = new DirectoryInfo(RightPath);

        SetUpListView(Section.Left);
        SetUpListView(Section.Right);
    }

    public Drives Drives { get; }
    public Section Section { get; set; }

    private string LeftPath { get; }
    private string RightPath { get; }

    public DirectoryInfo LeftDirectory { get; private set; }
    public DirectoryInfo RightDirectory { get; private set; }

    public void SetUpListView(Section s)
    {
        var imgList = s == Section.Left ? _leftImagelist : _rightImagelist;
        var listView = s == Section.Left ? _leftListView : _rightListView;
        var directory = s == Section.Left ? LeftDirectory : RightDirectory;

        imgList[0].Images.Clear();
        imgList[1].Images.Clear();
        listView.Items.Clear();

        // Arrow up
        var i = 0;
        if (directory.Parent != null)
        {
            listView.Items.Add(new ListViewItem("..", i++) { Tag = "Folder" });
            imgList[0].Images.Add(Images.arrowUp);
            imgList[1].Images.Add(Images.arrowUp);
        }

        var index = i;
        // Directories
        if (directory.GetDirectories().Length > 0)
        {
            imgList[0].Images.Add(Images.folder);
            imgList[1].Images.Add(Images.folder);

            foreach (var dir in directory.GetDirectories())
            {
                listView.Items.Add(dir.Name, index);
                listView.Items[i].Tag = "Folder";
                listView.Items[i].SubItems.Add("");
                listView.Items[i].SubItems.Add("<Папка>");
                listView.Items[i++].SubItems.Add(dir.CreationTime.ToShortDateString());
            }

            index++;
        }

        // Files
        foreach (var file in directory.GetFiles())
        {
            imgList[0].Images.Add(Icon.ExtractAssociatedIcon(file.FullName)!.ToBitmap());
            imgList[1].Images.Add(Icon.ExtractAssociatedIcon(file.FullName)!.ToBitmap());

            listView.Items.Add(file.Name, index++);
            listView.Items[i].Tag = "File";
            listView.Items[i].SubItems.Add(file.Extension);
            listView.Items[i].SubItems.Add(file.Length.ToString());
            listView.Items[i++].SubItems.Add(file.CreationTime.ToShortDateString());
        }
    }

    public void ChangeViewMode(string mode)
    {
        var tmp = Section == Section.Left ? _leftListView : _rightListView;
        tmp.View = (View)Enum.Parse(typeof(View), mode);
    }

    public void ItemDoubleClick(object sender)
    {
        var tmp = sender as ListView;
        try
        {
            switch ((string)tmp!.FocusedItem.Tag)
            {
                case "File":
                    Process.Start(tmp.FocusedItem.Text);
                    break;
                case "Folder":
                    ChangeDirectory(tmp.FocusedItem.Text, Section);
                    break;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void ChangeDirectory(string newPath, Section s)
    {
        var tmp = new DirectoryInfo(newPath);
        tmp.GetFiles();

        if (s == Section.Left)
            LeftDirectory = tmp;
        else
            RightDirectory = tmp;

        Directory.SetCurrentDirectory(newPath);
        SetUpListView(s);
    }

    public static void CreateFolder(string name)
    {
        if (name.Length < 1)
            throw new ArgumentException("Слишком короткое название!");
        if (Directory.Exists(name))
            throw new ArgumentException("Папка с таким именем уже существует!");

        Directory.CreateDirectory(name);
    }

    public static void CreateFile(string name)
    {
        if (name.Length < 1)
            throw new ArgumentException("Слишком короткое название!");
        if (File.Exists(name))
            throw new ArgumentException("Файл с таким именем уже существует!");

        var fs = new FileStream(name, FileMode.CreateNew, FileAccess.Write, FileShare.Inheritable);
        fs.Close();
    }


    public void DeleteFiles()
    {
        DeleteFilesFromListView(_leftListView);
        DeleteFilesFromListView(_rightListView);
    }

    private static void DeleteFilesFromListView(ListView listView)
    {
        foreach (ListViewItem item in listView.SelectedItems)
        {
            if (item.Text == "..")
                continue;

            var filePath = item.Text;

            switch (item.Tag as string)
            {
                case "Folder":
                    new DirectoryInfo(filePath).RecursiveDeleteFolders(true);
                    break;
                case "File":
                    if (File.Exists(filePath))
                    {
                        new FileInfo(filePath).DeleteFile();
                    }

                    break;
            }

            var index = listView.Items.IndexOf(item);
            listView.Items.RemoveAt(index);
        }
    }

    public object GetSelectedItem()
    {
        var listView = Section == Section.Left ? _leftListView : _rightListView;
        var sourcePath = Section == Section.Left ? LeftDirectory.FullName : RightDirectory.FullName;

        if (listView.SelectedItems.Count == 0)
            return null;
        var item = listView.SelectedItems[0];

        return ((item.Tag as string) switch
        {
            "Folder" => new DirectoryInfo(sourcePath + "\\" + item.Text),
            "File" => new FileInfo(sourcePath + "\\" + item.Text),
            _ => null
        })!;
    }

    public string[] GetSelectedItemsPath()
    {
        var listView = Section == Section.Left ? _leftListView : _rightListView;
        var sourcePath = Section == Section.Left ? LeftDirectory.FullName : RightDirectory.FullName;

        return (from ListViewItem item in listView.SelectedItems
            where item.Text != ".."
            select sourcePath + "\\" + item.Text).ToArray();
    }

    public void CopyFiles()
    {
        _buffer.Type = TransferType.Copy;
        SetFilesToBuffer();
    }

    public void PasteFiles(Section s)
    {
        var sourcePath = s == Section.Left ? LeftDirectory.FullName : RightDirectory.FullName;

        var dir = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(sourcePath);

        if (_buffer.Type == TransferType.Copy)
        {
            foreach (var item in _buffer.GetFiles())
                try
                {
                    File.Copy(item, Path.GetFileName(item));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            foreach (var item in _buffer.GetFolders())
                try
                {
                    CopyFolder(new DirectoryInfo(item), Directory.GetCurrentDirectory());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    Directory.SetCurrentDirectory(sourcePath);
                }
        }
        else
        {
            foreach (var item in _buffer.GetFiles())
                try
                {
                    File.Move(item, Path.GetFileName(item));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            foreach (var item in _buffer.GetFolders())
                try
                {
                    Directory.Move(item, Path.GetFileName(item));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
        }

        Directory.SetCurrentDirectory(dir);
    }

    public void CutFiles()
    {
        _buffer.Type = TransferType.Cut;
        SetFilesToBuffer();
    }

    public void SetFilesToBuffer(IEnumerable<string> path)
    {
        _buffer.Clear();
        _buffer.Type = TransferType.Copy;

        foreach (var item in path)
            if (File.Exists(item))
                _buffer.AddFile(item);
            else if (Directory.Exists(item))
                _buffer.AddFolder(item);
    }

    private void SetFilesToBuffer()
    {
        _buffer.Clear();
        var listView = Section == Section.Left ? _leftListView : _rightListView;
        var sourcePath = Section == Section.Left ? LeftDirectory.FullName : RightDirectory.FullName;

        foreach (ListViewItem item in listView.SelectedItems)
            switch (item.Tag as string)
            {
                case "Folder":
                    _buffer.AddFolder(sourcePath + "\\" + item.Text);
                    break;
                case "File":
                    _buffer.AddFile(sourcePath + "\\" + item.Text);
                    break;
            }
    }

    private static void CopyFolder(DirectoryInfo from, string to)
    {
        try
        {
            Directory.SetCurrentDirectory(to);
            Directory.CreateDirectory(from.Name);
            Directory.SetCurrentDirectory(from.Name);

            var current = new ArrayList();
            current.AddRange(from.GetFiles());
            current.AddRange(from.GetDirectories());

            foreach (var i in current)
                if (i is FileInfo fileInfo)
                {
                    fileInfo.CopyTo($"{Directory.GetCurrentDirectory()}\\{fileInfo.Name}");
                }
                else
                {
                    CopyFolder(i as DirectoryInfo, Directory.GetCurrentDirectory());
                    Directory.SetCurrentDirectory("..");
                }
        }
        catch (Exception)
        {
            // ignored
        }
    }
}