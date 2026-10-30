using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vitech.Genesys.Client;
using Vitech.Genesys.Common;

namespace Pentateuch.Genesys
{
    class Program
    {
        /// <summary>
        /// This application demonstrates the traversal of an entity's structure model which is comprised of collection of branches and constructs
        /// that make up the function flow model used by the FFBD, EFFBD, and Activity diagrams.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            // TODO: Update credentials to match your environment.
            string userName = "Administrator";
            string password = "admin";

            Console.WriteLine("Loading model...");

            // Create a client root object to work with. The client will internally cache data as it is pulled from the service for efficiency.
            using (ClientModel client = new ClientModel())
            {
                // Get the configuration for the local repository.
                RepositoryConfiguration repositoryConfiguration = client.GetKnownRepositories().LocalRepository;
                GenesysClientCredentials credentials = new GenesysClientCredentials(userName, password, AuthenticationType.GENESYS);

                try
                {
                    // This operation will call the service to authenticate the user.
                    repositoryConfiguration.Login(credentials);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred during the login process: " + ex.Message);
                    Console.WriteLine("Press ENTER to exit.");
                    Console.ReadLine();
                    return;
                }

                // TODO: Replace the string below with the name of the target project (or pass it as the first argument).
                var projectName = args.Length > 0 ? args[0] : "SAMPLE: Fast Food";

                // TODO: Replace the string below with the name of the target entity (or pass it as the second argument).
                var entityName = args.Length > 1 ? args[1] : "Operate FF Restaurant";

                // TODO: Replace the ID below with the ID of the class of entity referenced above.
                // Note that only subclasses of ProcessingUnit have a structure.
                var entityDefinitionId = EntityDefinitionConstants.Function;

                var repository = repositoryConfiguration.GetRepository();
                var project = repository.GetProject(projectName);
                var entity = project.GetEntity(entityName, EntityDefinitionConstants.Function);
                var structure = entity.GetStructure();
                if (structure == null)
                {
                    Console.WriteLine(string.Format("Entity \"{0}\" does not contain a structure model. Only ProcessingUnits have structures.", entity.Name));
                    Console.WriteLine("Press ENTER to exit.");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine(string.Format("Structure output for entity \"{0}\"", entity.Name));
                Console.WriteLine("Structure items:");

                // Structure items implement the visitor software pattern and therefore respond to the Accept(IStructureItemVisitor visitor) method
                // which has been implemented in the StructureItemExporter class referenced below.
                var exporter = new StructureItemExporter();

                // Iterate through each item in the structure and output some information about each branch/construct.
                foreach (IStructureItem item in structure.GetItems<IStructureItem>())
                {
                    item.Accept(exporter);
                }

                Console.WriteLine();
                Console.WriteLine("Press ENTER to exit.");
                Console.ReadLine();
            }
        }
    }
}
