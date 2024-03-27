using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FileUtilizator.Forms;

public partial class FileInfoDialogBox : Form
{
    public FileInfoDialogBox(FileInfo fileInfo)
    {
        InitializeComponent();

        pictureBox1.BackgroundImage = Icon.ExtractAssociatedIcon(fileInfo.FullName)?.ToBitmap();
        textBox1.Text = fileInfo.Name;
        locationLabel.Text = Path.GetDirectoryName(fileInfo.FullName);
        createdLabel.Text = fileInfo.CreationTime.ToLongDateString();
        editedLabel.Text = fileInfo.LastWriteTime.ToLongDateString();
        openedLabel.Text = fileInfo.LastAccessTime.ToLongDateString();
        typeLabel.Text = Path.GetExtension(fileInfo.FullName);
        sizeLabel.Text = $"{fileInfo.Length / Math.Pow(1024, 2): 0.00} МБ.";

        if (fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
            checkBox1.CheckState = CheckState.Checked;

        if (fileInfo.Attributes.HasFlag(FileAttributes.ReadOnly))
            checkBox2.CheckState = CheckState.Checked;
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