using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Spectre.Vault;

namespace Spectre
{
    /// <summary>
    /// Interaction logic for AddCredentials.xaml
    /// </summary>
    public partial class AddCredentials : Window
    {
        public AddCredentials()
        {
            InitializeComponent();
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtName.Text) || string.IsNullOrEmpty(TxtUsername.Text) ||
                string.IsNullOrEmpty(TxtPassword.Password) || string.IsNullOrEmpty(TxtPasswordAgain.Password))
            {
                MessageBox.Show("Make sure you've filled out all the fields!", "Oops", MessageBoxButton.OK, MessageBoxImage.Hand);
                return;
            }

            if (TxtPassword.Password != TxtPasswordAgain.Password)
            {
                MessageBox.Show("Passwords don't match, make sure you enter the same password twice.",
                    "Password mismatch", MessageBoxButton.OK, MessageBoxImage.Hand);
                return;
            }

            CredentialManager.AddCredentials(TxtName.Text, TxtUsername.Text, TxtPassword.Password);

            MessageBox.Show(
                string.Format("Added user {0} under name {1} successfully.", TxtUsername.Text, TxtName.Text),
                "Success!", MessageBoxButton.OK, MessageBoxImage.Information);

            CredentialManager.Reload();

            Close();
        }

        private void OnPasswordAgainChanged(object sender, TextCompositionEventArgs e)
        {
            if (TxtPasswordAgain.Password != TxtPassword.Password)
                TxtPasswordAgain.BorderBrush = new SolidColorBrush(Colors.Red);
            else
            {
                TxtPasswordAgain.BorderBrush = new SolidColorBrush(Colors.Green);
            }
        }
    }
}
