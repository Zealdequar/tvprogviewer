using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TVProgViewer.TVProgApp.Dialogs;
using TVProgViewer.TVProgApp.Logger;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp.Classes
{
        

    static class Statics
    {
        private static Thread splashThread;
        private static PleaseWaitForm splash;

        
        public static ExceptionLogger EL;
        public static DialogResult ShowDialog(string caption, string text, 
            MessageDialog.MessageIcon messageIcon, MessageDialog.MessageButtons messageButtons)
        {
            MessageDialog msgDlg = new MessageDialog(caption, text, messageIcon, messageButtons);
            return msgDlg.ShowDialog();
        }
        public static void ShowTODO ()
        {
            MessageBox.Show("TODO");
        }


        private static void ShowThread()
        {
            splash = new PleaseWaitForm();
            splash.Show();
            Application.Run(splash);
        }

        /// <summary>
        /// Отображение
        /// </summary>
        public static void ShowLogo(string txt, int value)
        {
            if (Settings.Default.FlagDontShowLogo) return;
            Status = txt;
            Progress = value;
            if (splashThread != null)
                return;
            splashThread = new Thread(ShowThread) { IsBackground = true };
            //splashThread.ApartmentState = ApartmentState.MTA;
            splashThread.Start();
            Status = txt;
            Progress = value;
        }

        /// <summary>
        /// Скрытие
        /// </summary>
        public static void HideLogo()
        {
            if (Settings.Default.FlagDontShowLogo) return;
            if (splashThread == null) return;
            if (splash == null) return;
            try
            {
                if (splash.InvokeRequired)
                {
                    splash.Invoke(new MethodInvoker(splash.Hide));
                }
                else {splash.Hide();}
            }
            catch (Exception ex)
            {
                Statics.EL.LogException(ex);
            }
            splashThread = null;
            splash = null;
        }

        /// <summary>
        /// Задание Статуса(лейбла)
        /// </summary>
        public static string Status
        {
            set
            {
                if (splash == null)
                {
                    Thread.CurrentThread.Join(600);
                }
                try
                {
                    splash.TXT = value;
                }
                catch (NullReferenceException)
                {
                }
            }
            get
            {
                if (splash == null)
                {
                    return "";
                }
                return splash.TXT;
            }
        }
        /// <summary>
        /// Задание значения прогресса
        /// </summary>
        public static int Progress
        {
            set
            {
                if (splash == null)
                {
                    Thread.CurrentThread.Join(600);
                }
                try
                {
                    splash.Value = value;
                }
                catch (NullReferenceException)
                {
                }
            }
            get
            {
                if (splash == null)
                {
                    return 0;
                }
                return splash.Value;
            }
        }
       
    }
}
