using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Vitech.Genesys.Client;

namespace EntityViewerWPF
{
    /// <summary>
    /// This class is used to support WPF data binding. It pulls attribute values from an entity.
    /// </summary>
    public class AttributeValueConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            IEntity entity = value as IEntity;
            string attributeName = parameter as string;

            if (entity != null && attributeName != null)
            {
                IAttributeValue attributeValue = entity.Attributes[attributeName];
                return GetAttributeValue(attributeValue);
            }

            return null;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion


        /// <summary>
        /// Gets the string representation of the attribute value.
        /// </summary>
        /// <param name="attributeValue">The attribute value.</param>
        /// <returns></returns>
        private string GetAttributeValue(IAttributeValue attributeValue)
        {
            if (attributeValue.HasValue)
                return attributeValue.GetValue().ToString();

            return string.Empty;
        }
    }
}
