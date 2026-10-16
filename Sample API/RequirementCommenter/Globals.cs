using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vitech.Genesys.Client;

namespace RequirementCommenter
{
    public static class Globals
    {
        static Globals()
        {
            Client           = null;
            RepositoryConfig = null;

        }

        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>The client.</value>
        public static ClientModel Client { get; set; }

        /// <summary>
        /// Gets the root repository object for data in the local repository.
        /// </summary>
        /// <value>The repository.</value>
        public static RepositoryConfiguration RepositoryConfig { get; set; }

    }
}
