using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for EntityViewer.xaml
    /// </summary>
    public partial class EntityViewer : Window
    {
        private IProject m_project;

        public EntityViewer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the btnOpenProject control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnOpenProject_Click(object sender, RoutedEventArgs e)
        {
            OpenProjectDialog dialog = new OpenProjectDialog();
            if (dialog.ShowDialog() == true)
            {
                if (dialog.Project == m_project)
                    return;

                // Store the currently open existing project.
                IProject projectToClose = m_project;

                m_project = dialog.Project;

                txtProject.Text = m_project.Name;
                Title = "Entity Viewer - " + m_project.Name;

                // Subscribe to service update events so that our model stays in sync with changes on the service!
                // Given that we are using WPF data binding, our GUI will automatically stay in sync (e.g. when entities are created/deleted).
                m_project.ReceiveUpdates(true);

                // Bind the folder combo box to the list of root folders in the project.
                cmbFolders.ItemsSource = m_project.GetFolders();

                // Sort the list by name.
                cmbFolders.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));

                // If we had a project open, flush its cache and turn off updates to lessen load.
                if (projectToClose != null)
                {
                    projectToClose.ReceiveUpdates(false);
                    projectToClose.Flush();
                    projectToClose.Schema.Flush();
                }
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the cmbFolders control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void cmbFolders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Bind the list of entities in the folder to the list box.
            IFolder folder = cmbFolders.SelectedItem as IFolder;

            if (folder != null)
            {
                // Bind the entity list box to the list of entities in the folder.
                // Note as entities are added and removed on the server, our list updates.
                lstEntities.ItemsSource = folder.GetEntities();

                // Sort the list by name.
                lstEntities.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
            }
            else
            {
                lstEntities.ItemsSource = null;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnNewEntity control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnNewEntity_Click(object sender, RoutedEventArgs e)
        {
            IFolder folder = cmbFolders.SelectedItem as IFolder;
            if (folder != null)
            {
                NewEntityPrompt dialog = new NewEntityPrompt();
                if (dialog.ShowDialog() == true)
                {
                    if (!string.IsNullOrEmpty(dialog.EntityName))
                    {
                        try
                        {
                            // Create the entity.
                            IEntity entity = m_project.CreateEntity(folder, dialog.EntityName);

                            // Set the description attribute.
                            entity.Attributes["description"].SetValue(new Text(dialog.EntityDescription));
                            entity.Update();
                        }
                        catch (FaultException ex)
                        {
                            MessageBox.Show(string.Format("An error occurred while attempting to create or update the entity. {0)", ex.Message), "Error");
                        }
                    }
                }
            }
        }
    }
}
