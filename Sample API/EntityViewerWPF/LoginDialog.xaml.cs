using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Vitech.Genesys.Client;
using Vitech.Genesys.Common;
using System.ServiceModel;
using Vitech.Genesys.Common.FaultContracts;

namespace EntityViewerWPF
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog : Window
    {
        private const string m_invalidLoginMessage = "The username or password specified was not recognized by the system.";
        private const string m_servicesNotFoundMessage = "The GENESYS services could not be found. Please ensure GENESYS is running on the local machine.";

        public LoginDialog()
        {
            InitializeComponent();

            txtPassword.Focus();
        }

        /// <summary>
        /// Handles the Click event of the btnLogin control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string userName = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                MessageBox.Show("A username and password must be specified.", "Login", MessageBoxButton.OK, MessageBoxImage.Error);

            // Create a client root object to work with. The client will internally cache data as it is pulled from the service for efficiency.
            ClientModel client = new ClientModel();
            Globals.Client = client;

            // Get the configuration for the local repository.
            RepositoryConfiguration repositoryConfiguration = client.GetKnownRepositories().LocalRepository;

            // Set the credentials.
            // NOTE: Currently this is hard coded to use GENESYS AuthenticationType.

            GenesysClientCredentials credentials = new GenesysClientCredentials(userName, password, AuthenticationType.GENESYS);
            string errorMessage = null;

            try
            {
                // This operation will call the service to authenticate the user.
                repositoryConfiguration.Login(credentials);
            }
            catch (EndpointNotFoundException) { errorMessage = m_servicesNotFoundMessage; }
            catch (Exception) { errorMessage = m_invalidLoginMessage; }

            if (errorMessage != null)
            {
                MessageBox.Show(errorMessage, "Login", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                new EntityViewer().Show();
                Close();
            }
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
