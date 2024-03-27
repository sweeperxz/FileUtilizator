using System.Windows.Forms;

namespace FileUtilizator.Forms;

public partial class DialogBox : Form
{
    public DialogBox(string text, string button1Name, string button2Name)
    {
        InitializeComponent();
        Name = text;
        button1.Text = button1Name;
        button2.Text = button2Name;
    }

    public string TextBox => textBox1.Text;
}