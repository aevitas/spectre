using System;
using System.Collections.ObjectModel;
using System.Windows;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CredentialManager.AddCredentials("Some website", "aevitas", "somepassword");
        }

        private void BtnDecrypt_Click(object sender, RoutedEventArgs e)
        {
            var creds = CredentialManager.GetCredentials("Foobar");

            MessageBox.Show("Username: " + creds.Username + " - Password: " + creds.Password);
        }
    }
}
