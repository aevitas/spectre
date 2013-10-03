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
        private bool _modifying = false;
        private int _modifyIndex = -1;

        public AddCredentials(bool modifying, int modifyIndex = -1)
        {
            InitializeComponent();

            _modifying = modifying;
            _modifyIndex = modifyIndex;

            if (modifying)
            {
                var c = CredentialManager.GetCredentials(modifyIndex);

                TxtName.Text = c.Name;
                TxtUsername.Text = c.Username;
                TxtPassword.Password = c.Password;
                TxtPasswordAgain.Password = c.Password;
            }
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

            if (!_modifying)
            {
                CredentialManager.AddCredentials(TxtName.Text, TxtUsername.Text, TxtPassword.Password);

                MessageBox.Show(
                    string.Format("Added user {0} under name {1} successfully.", TxtUsername.Text, TxtName.Text),
                    "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                CredentialManager.UpdateCredentials(_modifyIndex, TxtName.Text, TxtUsername.Text, TxtPassword.Password);

                MessageBox.Show(
                    string.Format("Modified user {0} successfully.", TxtUsername.Text),
                    "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
            }

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
