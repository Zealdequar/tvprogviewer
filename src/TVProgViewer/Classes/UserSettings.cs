using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using DirectX.Capture;

namespace TVProgViewer.TVProgApp.Classes
{
    // Класс для настроек Capture
    internal sealed class UserSettings : ApplicationSettingsBase
    {
        private readonly static UserSettings DefaultInstance =
            (UserSettings)ApplicationSettingsBase.Synchronized(new UserSettings());
        // Поток по умолчанию
        public static UserSettings Default
        {
            get { return DefaultInstance; }
        }

        [UserScopedSetting]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        [DefaultSettingValue("")]
        public Dictionary<string, List<ColumnOrderItem>> ColumnOrder
        {
            get { return this["ColumnOrder"] as Dictionary<string, List<ColumnOrderItem>>; }
            set { this["ColumnOrder"] = value; }
        }
    }
}