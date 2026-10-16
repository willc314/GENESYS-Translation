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

namespace EntityViewerWPF
{
    /// <summary>
    /// Interaction logic for OpenProjectDialog.xaml
    /// </summary>
    public partial class OpenProjectDialog : Window
    {
        public OpenProjectDialog()
        {
            InitializeComponent();

            // Bind the list box to the list of projects in the repository.
            lstProjects.ItemsSource = Globals.Repository.GetProjects();
        }

        /// <summary>
        /// Handles the Click event of the btnOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Project = lstProjects.SelectedItem as IProject;
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Project = null;
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Handles the SelectionChanged event of the lstProjects control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void lstProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IProject project = lstProjects.SelectedItem as IProject;
            btnOK.IsEnabled = project != null;
        }

        /// <summary>
        /// Gets the selected project.
        /// </summary>
        /// <value>The project.</value>
        public IProject Project { get; private set; }
    }
}
