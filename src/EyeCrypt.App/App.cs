using System;
using System.Security.Cryptography;
using System.Windows.Forms;
using EyeCrypt.App.Core;
using EyeCrypt.App.Crypts.Rijndael;

namespace EyeCrypt.App
{
    public partial class App : Form
    {
        private IApplication _application;

        public App()
        {
            InitializeComponent();
        }

        private void App_Load(object sender, EventArgs e)
        {
            _application = new EyeApplication(new RijndaelCrypt(), new RijndaelGen());
        }

        private void unlock_Click(object sender, EventArgs e)
        {
            try
            {
                _application.SetupKey(keyBox.Text);
                foreach (var item in _application.ShowItems()) listBox.Items.Add(item);
            }
            catch (CryptographicException exception)
            {
                MessageBox.Show($"Invalid key or \n{exception.Message}", "Ошибка");
            }
            catch (Exception exception)
            {
                MessageBox.Show($"{exception.Message}", "Ошибка");
            }
        }

        private void gen_Click(object sender, EventArgs e)
        {
            passBox.Text = _application.GeneratePwd();
        }

        private void add_Click(object sender, EventArgs e)
        {
            try
            {
                _application.Add(titleBox.Text, passBox.Text);
                listBox.Items.Add(titleBox.Text);
            }
            catch (CryptographicException exception)
            {
                MessageBox.Show($"Invalid key or \n{exception.Message}", "Ошибка");
            }
            catch (Exception exception)
            {
                MessageBox.Show($"{exception.Message}", "Ошибка");
            }
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var data = _application.Show(listBox.SelectedIndex);
                if (data != string.Empty)
                {
                    var dialogResult = MessageBox.Show($"PWD IS {data}\nCopy to clipboard?",
                        $"Your password of [{listBox.SelectedIndex}] {listBox.SelectedItem}", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes) Clipboard.SetText(data);
                }
            }
            catch (CryptographicException exception)
            {
                MessageBox.Show($"Invalid key or \n{exception.Message}", "Ошибка");
            }
            catch (Exception exception)
            {
                MessageBox.Show($"{exception.Message}", "Ошибка");
            }
        }
    }
}