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
    }
}
