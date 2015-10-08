using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Connection.SSH;

namespace ClientWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SSHClient _sshClient;

        int _promptLength;

        string ShellPrompt
        {
            get
            {
                var prompt = _sshClient.ClientPrompt;
                _promptLength = prompt.Length;
                return prompt;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectClient()
        {
            try
            {
                _sshClient = new SSHClient(textBoxHost.Text, textBoxPort.Text, textBoxUsername.Text, passwordBox.Password);
                _sshClient.Connect();
                if (_sshClient.IsConnected)
                {
                    labelStatus.Content = "Connected";
                    labelStatus.Visibility = Visibility.Visible;
                    LoadShell();
                }
                else
                {
                    labelStatus.Content = "Not Connected";
                    labelStatus.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void LoadShell()
        {
            tabControl.SelectedIndex = 1;
            textBoxShell.Text = ShellPrompt;
            textBoxShell.Focus();
            textBoxShell.Select(textBoxShell.Text.Length - 1, 0);
        }

        private void RunCommand(string commandText)
        {
            try
            {
                var result = String.Empty;
                if (!string.IsNullOrEmpty(commandText))
                {
                    result = _sshClient.ProcessCommand(commandText);
                }
                textBoxShell.Text += $"\n{result}";

                textBoxShell.Text += $"{ShellPrompt}";
                textBoxShell.Select(textBoxShell.Text.Length - 1, 0);
                textBoxShell.CaretIndex = textBoxShell.Text.Length;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void buttonConnect_Click(object sender, RoutedEventArgs e)
        {
            ConnectClient();
        }

        private void textBoxShell_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var lines = textBoxShell.Text.Split('\n');
                var cmd = lines[textBoxShell.LineCount - 1];
                cmd = cmd.Replace(ShellPrompt, string.Empty);
                RunCommand(cmd);
            }
            if (e.Key == Key.Back)
            {
                if (textBoxShell.CaretIndex >= _promptLength)
                {
                    
                }
            }
        }

        private void passwordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ConnectClient();
            }
        }
    }
}
