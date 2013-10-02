using System;
using System.Collections.ObjectModel;
using System.Windows;
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

            CredentialManager.OnReloadCompleted += () =>
            {
                Credentials = new ObservableCollection<EncryptedCredentials>(CredentialManager.GetDisplayEntries());
            };
            
            CredentialManager.Reload();
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
