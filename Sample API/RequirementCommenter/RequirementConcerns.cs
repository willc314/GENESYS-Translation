using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows.Forms;
using Vitech.Genesys.Client;
using Vitech.Genesys.Common;
using Vitech.Genesys.Common.FaultContracts;

namespace RequirementCommenter
{
    public partial class RequirementConcernsDialog : Form
    {

        private List<IEntity> m_requirements;
        private int m_requirementIndex;
        private Guid m_selectedConcern;


        public RequirementConcernsDialog()
        {
            m_requirements = new List<IEntity>();
            m_requirementIndex = 0;

            InitializeComponent();
            btnViewConcern.Enabled = false;


            //  Present the user with the GENESYS login dialog, return the client
            //  and selected repository
            //  Requires using Vitech.Genesys.Connector.Common;
            LoginDialog loginDlg = new LoginDialog( );
            loginDlg.Text = "Requirement Concerns";
            if (loginDlg.ShowDialog() != DialogResult.OK || loginDlg.client == null)
            {
                /* Fail Message Goes Here */
                this.Close();
            }
            else
            {
                Globals.Client = loginDlg.client;
                Globals.RepositoryConfig = loginDlg.repositoryConfig;
                LoadProjectList();
                UpdateNavButtons();
            }


            
        }



        /// <summary>
        /// Handles the Click event of the buttonClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void buttonClose_Click(object sender, EventArgs e)
        {
            GenesysLogout();
            this.Close();
        }


        /// <summary>
        /// Called when user selects a new project in the project combo box.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Project_Changed(object sender, EventArgs e)
        {
            // Load the requirement list
            string projectName = cmbProjectList.SelectedItem.ToString();

            //Get repository from our configuration object
            Repository repository = Globals.RepositoryConfig.GetRepository();
            IProject project = repository.GetProject(projectName);              // Get selected project
            IFolder requirementsFolder = project.GetFolder("Requirement");     // Get requirements folder in that project

            // Load all requirements in this folder
            m_requirements.Clear();
            m_requirementIndex = 0;
            foreach (IEntity req in requirementsFolder.GetEntities())
            {
                m_requirements.Add(req);
            }

            DisplaySelectedRequirement();
        }


        /// <summary>
        /// User clicked the Next button
        /// Display the next requiremrnt
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnNext_Click(object sender, EventArgs e)
        {
            m_requirementIndex ++; // Enable/disable protects index overwrite
            DisplaySelectedRequirement();
        }


        /// <summary>
        /// USer Selected the Previous button
        /// display the previous requirement
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnPrev_Click(object sender, EventArgs e)
        {
            m_requirementIndex --; // Enable/disable protects index underwrite
            DisplaySelectedRequirement();
        }


        /// <summary>
        /// User has select the View button
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnViewConcern_Click(object sender, EventArgs e)
        {
            if( m_selectedConcern != null &&  m_selectedConcern != Guid.Empty)
            {
                //Get repository from our configuration object
                string projectName = cmbProjectList.SelectedItem.ToString();
                Repository repository = Globals.RepositoryConfig.GetRepository();
                IProject project = repository.GetProject(projectName);   // Get selected project

                // Get an entity with its Guid, no need to navigate to "concerns" folder
                IEntity concern = project.GetEntity(m_selectedConcern);

                ConcernDialog dlgConcern = new ConcernDialog(false, concern);
                dlgConcern.ShowDialog();
            }
        }


        /// <summary>
        /// Handles the Click event of the buttonConcern control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void buttonConcernAdd_Click(object sender, EventArgs e)
        {
            // Load the requirement list
            string projectName = cmbProjectList.SelectedItem.ToString();

            //Get repository from our configuration object
            Repository repository = Globals.RepositoryConfig.GetRepository();
            IProject project = repository.GetProject(projectName);  // Get selected project
            IFolder concernsFolder  = project.GetFolder("Concern"); // Get requirements folder in that project
            String defaultName = project.GetDefaultEntityName( concernsFolder.EntityDefinitionId );
            IEntity newConcern = null;
            try
            {
                newConcern = project.CreateEntity(concernsFolder, defaultName);  // Create new concern w/ default name  
            }
            catch(FaultException<DuplicateEntityNameFault> dupFault)
            {
                System.Windows.MessageBox.Show( dupFault.Message, "Create Entity", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
            catch(FaultException<InsufficientPrivilegesFault> privFault)
            {
                System.Windows.MessageBox.Show(privFault.Message, "Create Entity", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show( ex.Message, "Create Entity", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            // Connect concern to our requirement using the generates Relationship
            IEntity req = m_requirements[m_requirementIndex];                // get selected requirement
            try
            {
                project.CreateRelationship(
                    project.Schema.GetRelationDefinition(Vitech.Genesys.Common.RelationDefinitionConstants.generates),
                    req,
                    newConcern);
            }
            catch(FaultException<RelationshipCreationFault> relFault)
            {
                System.Windows.MessageBox.Show(relFault.Message, "Create Relation", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Create Relation", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            ConcernDialog concernDlg = new ConcernDialog(true,newConcern);
            concernDlg.ShowDialog();

            DisplaySelectedRequirement();

        }


        /// <summary>
        /// User has selected a new concern from the list, update the selected concern
        /// and update button states
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void lstvConcerns_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView lv = sender as ListView;
            if(lv != null && lv.SelectedItems != null && lv.SelectedItems.Count > 0)
            {
                ListViewItem item = lv.SelectedItems[0];
                m_selectedConcern = (Guid)item.Tag;
                btnViewConcern.Enabled = true;
            }
            else
            {
                m_selectedConcern = Guid.Empty;
                btnViewConcern.Enabled = false;
            }

        }


        // HELPER FUNCTIONS ------------------------------------

        /// <summary>
        /// Loads the list of projects in a repository.
        /// </summary>
        private void LoadProjectList()
        {
            //Get repository from our configuration object
            Repository repository = Globals.RepositoryConfig.GetRepository();
            cmbProjectList.DataSource = repository.GetProjects();  // works w/ data binding

        }
              
        /// <summary>
        /// Logout of the GENESYS model.
        /// </summary>
        private void GenesysLogout()
        {

            try
            {
                if(Globals.RepositoryConfig != null )
                {
                    if( Globals.RepositoryConfig.Status == Vitech.Genesys.Common.AvailabilityStatus.Available)
                    {
                        //+ Globals.ThisAddIn.SelectedRepositoryConfig.GetRepository().SessionTerminated -= new EventHandler(Globals.ThisAddIn.ThisAddIn_SessionTerminated);
                        Globals.RepositoryConfig.Logout();
                    }
                    Globals.RepositoryConfig = null;
                }
                Globals.Client = null;

                // Update state of all controls               
            }
            catch(Exception)
            {
                // Handle Error here
            }

        }

        /// <summary>
        /// Displays the selected requirement.
        /// </summary>
        private void DisplaySelectedRequirement()
        {
            if (m_requirements.Count == 0) return;

            IEntity req = m_requirements[m_requirementIndex]; // get selected requirement

            string projectName = cmbProjectList.SelectedItem.ToString();
            Repository repository = Globals.RepositoryConfig.GetRepository();
            IProject project = repository.GetProject(projectName);  // Get selected project
            lblRequirementName.Text = req.GetName(project); // gets full name
            // Or just this if you don't care about cross-project extensions
            // lblRequirementName.Text = req.Name; 

            // Add description of the requirement ---------------------------------
            string description = req.GetAttributeValueString("description");
            rtbDescription.Text = description;

            // Add concerns to requirement -----------------------------------------
            // Specifically Requirements related to Concerns via the generates relationship 
            lstvConcerns.Items.Clear();
            IEnumerable<IEntity> concerns = req.GetRelationshipTargets("generates");

            foreach (IEntity con in concerns)
            {
                ListViewItem item = lstvConcerns.Items.Add( con.Name );
                item.Tag = con.Id;  // store GUID of Concern for easy retrieval
            }

            UpdateNavButtons();
        }

        /// <summary>
        /// Updates the next and previous navigation button for requirements in 
        /// a project
        /// </summary>
        private void UpdateNavButtons()
        {
            if (m_requirements.Count > 0)
            {
                btnNext.Enabled = m_requirementIndex < m_requirements.Count - 1;
                btnPrev.Enabled = m_requirementIndex > 0;
            }
            else
            {
                btnNext.Enabled = false;
                btnPrev.Enabled = false;
            }
        }



    }
}
