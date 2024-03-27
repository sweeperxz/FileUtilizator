using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using FileUtilizator.Sys;

namespace FileUtilizator.Forms;

public partial class DirectoryInfoDialogBox : Form
{
    private readonly DirectoryInfo _dirInfo;
    private readonly Counter _counter;

    public DirectoryInfoDialogBox(DirectoryInfo dirInfo)
    {
        InitializeComponent();
        _dirInfo = dirInfo;
        _counter = new Counter();

        CountSize();
        textBox1.Text = _dirInfo.Name;
        locationLabel.Text = Path.GetDirectoryName(_dirInfo.FullName);
        createdLabel.Text = _dirInfo.CreationTime.ToLongDateString();

        if (_dirInfo.Attributes.HasFlag(FileAttributes.Hidden))
            checkBox1.CheckState = CheckState.Checked;

        if (_dirInfo.Attributes.HasFlag(FileAttributes.ReadOnly))
            checkBox2.CheckState = CheckState.Checked;
    }

    private async void CountSize()
    {
        await Task.Factory.StartNew(() => { _counter.CountSize(_dirInfo); });
        sizeLabel.Text = $"{_counter.FolderSize:0.00}" + " МБ.";

        await Task.Factory.StartNew(() => { _counter.Count(_dirInfo); });
        filesCountLabel.Text = $"Папок: {_counter.DirectoriesCount}; файлов: {_counter.FilesCount}.";
    }

    private void button2_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        Close();
    }
}