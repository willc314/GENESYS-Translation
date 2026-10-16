using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;
using Vitech.Genesys.Client;

namespace EntityViewerWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Handles the Exit event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.ExitEventArgs"/> instance containing the event data.</param>
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // Dispose of the client model.  This ensures the session is logged out appropriately and other resources are discarded.
            ClientModel client = Globals.Client;
            if (client != null)
                client.Dispose();
        }
    }
}
