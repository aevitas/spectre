using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Spectre.Common;
using Spectre.Vault;
using Spectre.Vault.Storage;

namespace Spectre
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<EncryptedCredentials> Credentials { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Logging.OnLog += OnLogging;

            CredentialManager.OnReloadCompleted += () =>
            {
                Credentials = new ObservableCollection<EncryptedCredentials>(CredentialManager.GetDisplayEntries());
            };
            
            CredentialManager.Reload();

            Logging.Write("Spectre startup has finished.");
        }

        private void OnLogging(Logging.LogMessage message)
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                // We want those messages displayed asap, because they're already delayed. Thanks!
                Dispatcher.BeginInvoke(DispatcherPriority.Send, new Logging.LoggingDelegate(OnLogging), message);
                return;
            }
            else
            {
                if (RichTextBox.LineCount >= 10000)
                {
                    RichTextBox.Clear();
                    Logging.Write("We've exceeded 10,000 lines - clearing the log window. Log files will be preserved!");
                }

                RichTextBox.AppendText(message.PlainMessage + Environment.NewLine);
                RichTextBox.ScrollToEnd();
            }
        }

        private void BtnCopyPassword_Click(object sender, RoutedEventArgs e)
        {
            var creds = CredentialManager.GetCredentials(LstEntries.SelectedIndex);

            try
            {
                Clipboard.SetText(creds.Password);
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);

                MessageBox.Show("An error occured while attempting to set the clipboard text to your password. :(",
                    "Something went wrong..", MessageBoxButton.OK);
            }
        }
    }
}
