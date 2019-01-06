using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TVProgViewer.TVProgApp.Dialogs;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp.Classes
{
    public static class ExportBuilder
    {
        public delegate void ExportCompleted(string file);
        public static event ExportCompleted OnExportCompleted;
        /// <summary>
        /// Сохранить и открыть экспорт
        /// </summary>
        /// <param name="ms">поток данных экспорта</param>
        /// <param name="export_dest_name">имя файла</param>
        /// <returns></returns>
        public static bool Save(MemoryStream ms, string export_dest_name)
        {
            if (ms != new MemoryStream())
            {
                if (!Directory.Exists(Application.StartupPath + @"\Export\"))
                {
                    Directory.CreateDirectory(Application.StartupPath + @"\Export\");
                }

                string fname = Application.StartupPath + @"\Export\" + export_dest_name;

                if (File.Exists(fname))
                {
                    try
                    {
                        File.Delete(fname);
                    }
                    catch(Exception ex)
                    {
                        Statics.ShowDialog(Resources.ErrorText,
                                           String.Format(Resources.ExportFileOpenText, fname),
                                           MessageDialog.MessageIcon.Alert, MessageDialog.MessageButtons.Ok);
                        Statics.EL.LogException(ex);
                        return false;
                    }
                }

                var fs = new FileStream(fname, FileMode.CreateNew);
                byte[] msbuff = ms.ToArray();
                fs.Write(msbuff, 0, msbuff.Length);
                fs.Close();
                fs.Dispose();
                OnExportCompleted(export_dest_name);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Класс данных отчета 
        /// </summary>
        public class ReportData
        {
            private readonly string _res_file;
            private readonly string _template;

            public ReportData()
            {
                _template = _res_file = string.Empty;
            }

            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="resFile">файл-результат</param>
            /// <param name="template">шаблон</param>
            public ReportData(string resFile, string template)
            {
                _res_file = resFile;
                _template = template;
            }

            public string Template
            {
                get { return _template; }
            }

            public string ResFile
            {
                get { return _res_file; }
            }
        }
    }
}
