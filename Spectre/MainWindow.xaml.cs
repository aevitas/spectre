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
        public ObservableCollection<CredentialPair> Credentials = new ObservableCollection<CredentialPair>(); 

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CredentialManager.AddCredentials("Foobar", "Aevitas", "SomeShit");
        }

        private void BtnDecrypt_Click(object sender, RoutedEventArgs e)
        {
            var creds = CredentialManager.GetCredentials("Foobar");

            MessageBox.Show("Username: " + creds.Item1 + " - Password: " + creds.Item2);
        }
    }
}
