using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vitech.Genesys.Client;

namespace EntityViewerWPF
{
    public static class Globals
    {
        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>The client.</value>
        public static ClientModel Client { get; set; }

        /// <summary>
        /// Gets the root repository object for data in the local repository.
        /// </summary>
        /// <value>The repository.</value>
        public static Repository Repository
        {
            get { return Client.GetKnownRepositories().LocalRepository.GetRepository(); }
        }
    }
}
