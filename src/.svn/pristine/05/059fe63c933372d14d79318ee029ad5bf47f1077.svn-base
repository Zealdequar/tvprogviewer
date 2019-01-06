using System;
using System.Collections.Generic;
using System.Configuration;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp.Classes
{
    internal sealed class gfDataGridViewSettings: ApplicationSettingsBase
    {
        private static gfDataGridViewSettings _defaultInstance =
            (gfDataGridViewSettings) ApplicationSettingsBase.Synchronized(new gfDataGridViewSettings());
        // ---------------------- Поток по умолчанию -------------------------
        public static gfDataGridViewSettings Default
        {
            get { return _defaultInstance; }
        }
        // Для множества контролов:
        [UserScopedSetting]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        [DefaultSettingValue("")]
        public Dictionary<string, List<ColumnOrderItem>> ColumnOrder
        {
            get { return this[Resources.ColumnOrder] as Dictionary<string, List<ColumnOrderItem>>; }
            set { this[Resources.ColumnOrder] = value; }
        }
    }
    
    [Serializable]
    public sealed class ColumnOrderItem
    {
        public int DisplayIndex { get; set; }
        public int Width { get; set; }
        public bool Visible { get; set; }
        public int ColumnIndex { get; set; }
    }
}
