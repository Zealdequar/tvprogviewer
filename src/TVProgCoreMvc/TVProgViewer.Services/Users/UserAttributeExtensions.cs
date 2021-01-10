using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Users;

namespace TVProgViewer.Services.Users
{
    /// <summary>
    /// Расширения
    /// </summary>
    public static class UserAttributeExtensions
    {
        /// <summary>
        /// Значение показывает, какой пользовательских атрибут должен иметь значения
        /// </summary>
        /// <param name="UserAttribute">Атрибут пользователь</param>
        /// <returns>Результат</returns>
        public static bool ShouldHaveValues(this UserAttribute UserAttribute)
        {
            if (UserAttribute == null)
                return false;

            if (UserAttribute.AttributeControlType == AttributeControlType.TextBox ||
                UserAttribute.AttributeControlType == AttributeControlType.MultilineTextbox ||
                UserAttribute.AttributeControlType == AttributeControlType.Datepicker ||
                UserAttribute.AttributeControlType == AttributeControlType.FileUpload)
                return false;

            //другой атрибут тип контрола поддерживает значения
            return true;
        }
    }
}
