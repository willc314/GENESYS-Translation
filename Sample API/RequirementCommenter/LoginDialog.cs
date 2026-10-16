using System;
using System.Windows;
using System.Windows.Forms;
using Vitech.Genesys.Client;
using Vitech.Genesys.Common;
using System.ServiceModel;
using Vitech.Genesys.Common.FaultContracts;


namespace RequirementCommenter
{
    public partial class LoginDialog : Form
    {
        public ClientModel Client;
        public RepositoryConfiguration RepositoryConfig;

        public LoginDialog()
        {
            Client = new ClientModel();
            RepositoryConfig = null;
            InitializeComponent();
        }


        /// <summary>
        /// Handles the login button click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string host = txtHost.Text.Trim();
            string userName = txtUser.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(host))
                System.Windows.MessageBox.Show("A username and password must be specified.", "Login", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
         
            if (host.Equals("localhost"))
            {   // Get the configuration for the local repository.
                RepositoryConfig = Client.GetKnownRepositories().LocalRepository;
            }
            else
            {   // Get the configuration for a remote repository.
                ConnectionInfo connInfo = new ConnectionInfo(host, 39131, ConnectionInfo.ProtocolType.NetTcp);
                RepositoryConfig = Client.GetKnownRepositories().Add(connInfo, false, "remote", userName);
            }

            // Set the credentials.
            // NOTE: Currently this is hard coded to use GENESYS AuthenticationType.
            GenesysClientCredentials credentials = new GenesysClientCredentials(userName, password, AuthenticationType.GENESYS);
            string errorMessage = null;

            try
            {
                // This operation will call the service to authenticate the user.
                RepositoryConfig.Login(credentials);
            }
            catch (FaultException<UserNotFoundFault> ) { errorMessage = "user not found"; }
            catch (FaultException<PasswordIncorrectFault> ) { errorMessage = "bad password"; }
            catch (EndpointNotFoundException) { errorMessage = "Endpoint not found"; }
            catch (Exception ex)              { errorMessage = "Invalid Login" + ex.Message; }

            if (errorMessage != null)
            {
                System.Windows.MessageBox.Show(errorMessage, "Login", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            else
            {
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        /// <summary>
        /// Handles the cancel button click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

            //+ Release the licenses, etc.
            if (this.Client != null)
                this.Client.Dispose();

            Close();
        }
    }
}
