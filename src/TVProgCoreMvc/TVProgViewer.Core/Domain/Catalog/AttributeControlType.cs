namespace TVProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Представляет тип контрола атрибута
    /// </summary>
    public enum AttributeControlType
    {
        /// <summary>
        /// Раскрывающийся список
        /// </summary>
        DropdownList = 1,

        /// <summary>
        /// Список радиокнопок
        /// </summary>
        RadioList = 2,

        /// <summary>
        /// Флажки
        /// </summary>
        Checkboxes = 3,

        /// <summary>
        /// Текстовое поле
        /// </summary>
        TextBox = 4,

        /// <summary>
        /// Многострочное текстовое поле
        /// </summary>
        MultilineTextbox = 10,

        /// <summary>
        /// Элемент управления Datepicker
        /// </summary>
        Datepicker = 20,

        /// <summary>
        /// Элемент управления Загрузчик файлов
        /// </summary>
        FileUpload = 30,

        /// <summary>
        /// Цветные квадраты
        /// </summary>
        ColorSquares = 40,

        /// <summary>
        /// Изображения
        /// </summary>
        ImageSquares = 45,

        /// <summary>
        /// Флаги только для чтения
        /// </summary>
        ReadonlyCheckboxes = 50
    }
}