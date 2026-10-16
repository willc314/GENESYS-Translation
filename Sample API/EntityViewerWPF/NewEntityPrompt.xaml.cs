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

namespace EntityViewerWPF
{
    /// <summary>
    /// Interaction logic for NewEntityPrompt.xaml
    /// </summary>
    public partial class NewEntityPrompt : Window
    {
        public NewEntityPrompt()
        {
            InitializeComponent();
        }

        public string EntityName { get; private set; }
        public string EntityDescription { get; private set; }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            EntityName = txtName.Text;
            EntityDescription = txtDescription.Text;
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
