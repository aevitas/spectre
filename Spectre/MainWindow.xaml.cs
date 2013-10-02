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
        public ObservableCollection<EncryptedCredentials> Credentials = new ObservableCollection<EncryptedCredentials>(); 

        public MainWindow()
        {
            InitializeComponent();
            
            CredentialManager.Reload();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CredentialManager.AddCredentials("Foobar", "Timo", "Ket");
        }

        private void BtnDecrypt_Click(object sender, RoutedEventArgs e)
        {
            var creds = CredentialManager.GetCredentials("Foobar");

            MessageBox.Show("Username: " + creds.Username + " - Password: " + creds.Password);
        }
    }
}
