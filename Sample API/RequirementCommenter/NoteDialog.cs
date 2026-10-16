using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vitech.Genesys.Client;
using Vitech.Genesys.Common;
using Vitech.Genesys.Common.FaultContracts;

namespace RequirementCommenter
{
    public partial class NoteDialog : Form
    {
        private bool m_CanEdit;
        private IEntity m_Note; 


        public NoteDialog(bool bEdit, IEntity note)
        {
            m_CanEdit = bEdit;
            m_Note = note;

            InitializeComponent();
            DisplayConcern();
        }

        /// <summary>
        /// Handles the Click event of the OkBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OkBtn_Click(object sender, EventArgs e)
        {
            try
            {
                m_Note.Update(); // force write to db
            }
            catch (FaultException fault)
            {
                System.Windows.MessageBox.Show(fault.Message, "Set Entity Attribute", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            this.Close();
        }

        /// <summary>
        /// User selected Cancel button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // DO NOT UPDATE - RELOAD to REMOVE CHANGES
            this.Close();
        }


        /// <summary>
        /// Handles the TextChanged event of the tbNumber control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void tbNumber_TextChanged(object sender, EventArgs e)
        {
            if (m_CanEdit)
            {
                SetEntityAttributeStr(m_Note, "number", tbNumber.Text.Trim());
            }
        }

        /// <summary>
        /// User has changed the Concern name
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void tbNoteName_LostFocus(object sender, EventArgs e)
        {
            if (m_CanEdit && (m_Note.Name != tbConcernName.Name.Trim()))
            {
                SetEntityAttributeStr(m_Note, "name", tbConcernName.Text.Trim());
            }
        }

        /// <summary>
        /// Handles the LostFocus event of the tbConcernDescription control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void tbNoteDescription_LostFocus(object sender, EventArgs e)
        {
            if (m_CanEdit)
            {
                SetEntityAttributeStr(m_Note, "description", tbConcernDescription.Text.Trim());
            }
        }

        // HELPER FUNCTIONS ------------------------------------

        /// <summary>
        /// Displays the concern information.
        /// </summary>
        private void DisplayConcern()
        {
            tbConcernName.Text = m_Note.Name; 
            tbConcernName.Enabled = m_CanEdit;

            // Get Concern number and display ----------------------
            tbNumber.Text = GetEntityAttributeStr(m_Note, "number");
            tbNumber.ReadOnly = !m_CanEdit;

            // Get description and display ----------------------
            tbConcernDescription.Text = GetEntityAttributeStr(m_Note, "description");
            tbConcernDescription.ReadOnly = !m_CanEdit;

        }

        /// <summary>
        /// Gets the string value of a specified entity attribute
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="attrVal">The attribute value.</param>
        /// <returns></returns>
        private String GetEntityAttributeStr(IEntity entity, String attrVal )
        {
            String ret = "";

            foreach (IEntityAttributeValue attr in entity.Attributes)
            {
                if(attr.DisplayName == attrVal)
                    { ret = attr.GetValueString(); }
            }

            return ret;
        }

        /// <summary>
        /// Sets the entity attribute's value
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="Attr">The attribute.</param>
        /// <param name="attrVal">The attribute value.</param>
        private void SetEntityAttributeStr( IEntity entity, String Attr, String attrVal )
        {
            IEntityAttributeValue av = entity.Attributes[Attr];
            DataTypeDefinition typeDef = av.AttributeDefinition.DataType;
            object outVal;
            if (typeDef.TryConvertValue(attrVal,out outVal))
               av.SetValue(outVal);
            else
               MessageBox.Show("Attribute conversion failed.");
         }
     
    }
}
