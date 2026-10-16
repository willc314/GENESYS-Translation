using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows.Forms;
using Vitech.Genesys.Client;
using Vitech.Genesys.Client.Schema;
using Vitech.Genesys.Common;
using Vitech.Genesys.Common.FaultContracts;

namespace RequirementCommenter
{
    public partial class RequirementCommenterDialog : Form
    {
        private List<IEntity> m_requirements;
        private int m_requirementIndex;
        private Guid m_selectedNote;


        public RequirementCommenterDialog()
        {
            m_requirements = new List<IEntity>();
            m_requirementIndex = 0;

            InitializeComponent();
            btnViewConcern.Enabled = false;


            //  Present the user with the GENESYS login dialog, return the client
            //  and selected repository
            //  Requires using Vitech.Genesys.Connector.Common;
            LoginDialog loginDlg = new LoginDialog( );
            loginDlg.Text = "GENESYS Login";
            if (loginDlg.ShowDialog() != DialogResult.OK || loginDlg.Client == null)
            {
                /* Fail Message Goes Here */
                this.Close();
            }
            else
            {
                Globals.Client = loginDlg.Client;
                Globals.RepositoryConfig = loginDlg.RepositoryConfig;
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
            // Here's how to get a project with a project name
            // string projectName = cmbProjectList.SelectedItem.ToString();
            // Repository repository = Globals.RepositoryConfig.GetRepository();
            // IProject project = repository.GetProject(projectName); // Get selected project

            IProject project = cmbProjectList.SelectedItem as IProject;
            if (project == null)
                return;
            IFolder requirementsFolder = project.GetFolder("Requirement"); // Get requirements folder in that project

            // Load all requirements in this folder
            m_requirements.Clear();
            m_requirementIndex = 0;
            foreach (IEntity req in requirementsFolder.GetAllEntities())
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
            if (m_selectedNote != Guid.Empty)
            {
                IProject project = cmbProjectList.SelectedItem as IProject;
                if (project == null)
                    return;

                // Get an entity with its Guid, no need to navigate to "concerns" folder
                IEntity concern = project.GetEntity(m_selectedNote);

                NoteDialog dlgConcern = new NoteDialog(false, concern);
                dlgConcern.ShowDialog();
            }
        }

        /// <summary>
        /// Handles the Click event of the buttonConcern control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void buttonNoteAdd_Click(object sender, EventArgs e)
        {
            IProject project = cmbProjectList.SelectedItem as IProject;
            if (project == null)
                return;

            if (m_requirements.Count == 0) //+ No note to add to
                return;

            // Ensure our schema has the proper class and relation in order to create the comment.
            IFolder noteFolder  = project.GetFolder("Note");   // Get Notes folder in that project
            if (noteFolder == null)
            {
                System.Windows.MessageBox.Show("The \"Note\" class could not be found in the project schema. Please select a project with the latest baseline schema.", "Project Schema", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
            IRelationDefinition relation = project.Schema.GetRelationDefinition("has comments");
            if (relation == null)
            {
                System.Windows.MessageBox.Show("The \"has comments\" relation could not be found in the project schema. Please select a project with the latest baseline schema.", "Project Schema", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            String defaultName = project.GetDefaultEntityName(noteFolder.EntityDefinitionId);
            IEntity newNote = null;
            try
            {
                newNote = project.CreateEntity(noteFolder, defaultName);  // Create new note w/ default name  
            }
            catch(FaultException fault)
            {
                System.Windows.MessageBox.Show(fault.Message, "Create Entity", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            // Connect concern to our requirement using the "has comments" Relationship
            IEntity req = m_requirements[m_requirementIndex];    // get selected requirement
            try
            {
                project.CreateRelationship(relation, req, newNote);
            }
            catch (FaultException fault)
            {
                System.Windows.MessageBox.Show(fault.Message, "Create Relationship", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            NoteDialog concernDlg = new NoteDialog(true,newNote);
            concernDlg.ShowDialog();

            DisplaySelectedRequirement();

        }


        /// <summary>
        /// User has selected a new concern from the list, update the selected concern
        /// and update button states
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void lstvNotes_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView lv = sender as ListView;
            if (lv != null && lv.SelectedItems != null && lv.SelectedItems.Count > 0)
            {
                ListViewItem item = lv.SelectedItems[0];
                m_selectedNote = (Guid)item.Tag;
                btnViewConcern.Enabled = true;
            }
            else
            {
                m_selectedNote = Guid.Empty;
                btnViewConcern.Enabled = false;
            }
        }


        #region Helper Functions

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
                if (Globals.RepositoryConfig != null)
                {
                    if (Globals.RepositoryConfig.Status == Vitech.Genesys.Common.AvailabilityStatus.Available)
                    {
                        //+ Globals.ThisAddIn.SelectedRepositoryConfig.GetRepository().SessionTerminated -= new EventHandler(Globals.ThisAddIn.ThisAddIn_SessionTerminated);
                        Globals.RepositoryConfig.Logout();
                    }
                    Globals.RepositoryConfig = null;
                }

                if (Globals.Client != null)
                {
                    Globals.Client.Dispose();
                    Globals.Client = null;
                }

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

            IProject project = cmbProjectList.SelectedItem as IProject;
            if (project == null)
                return;
            IEntity req = m_requirements[m_requirementIndex]; // get selected requirement
            lblRequirementName.Text = req.GetName(project); // gets full name
            // Or just this if you don't care about cross-project extensions
            // lblRequirementName.Text = req.Name; 

            // Add description of the requirement ---------------------------------
            string description = req.GetAttributeValueString("description");
            rtbDescription.Text = description;

            // Add concerns to requirement -----------------------------------------
            // Specifically Requirements related to Concerns via the "has comments" relationship 
            lstvNotes.Items.Clear();
            IEnumerable<IEntity> notes = req.GetRelationshipTargets("has comments");

            foreach (IEntity note in notes)
            {
                ListViewItem item = lstvNotes.Items.Add( note.Name );
                item.Tag = note.Id;  // store GUID of Concern for easy retrieval
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

        #endregion
    }
}
