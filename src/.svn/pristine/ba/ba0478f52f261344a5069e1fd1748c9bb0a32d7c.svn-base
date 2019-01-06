using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TVProgViewer.TVProgApp.Controls;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp.Dialogs
{
    public partial class MessageDialog : Form
    {
        private string _txt = String.Empty;
        private ctrlOk _ctrlOk = new ctrlOk();
        private ctrlOkCancel _ctrlOkCancel = new ctrlOkCancel();
        private ctrlYesNo _ctrlYesNo = new ctrlYesNo();
        private MessageIcon _messageIcon;
        public enum MessageIcon
        {
            Alert,
            Question,
            Info
        }
        public enum MessageButtons
        {
            OkCancel,
            Ok,
            YesNo
        }
        public MessageDialog(string caption, string text, MessageIcon messageIcon, MessageButtons buttons)
        {
            InitializeComponent();
            this.Text = caption;
            _txt = text;
            _messageIcon = messageIcon;
            switch(_messageIcon)
            {
                case MessageIcon.Alert:
                    System.Media.SystemSounds.Exclamation.Play();
                    break;
                case MessageIcon.Info:
                    System.Media.SystemSounds.Asterisk.Play();
                    break;
                case MessageIcon.Question:
                    System.Media.SystemSounds.Question.Play();
                    break;
            }
            pButtons.Controls.Clear();
            switch (buttons)
            {
                case MessageButtons.OkCancel:
                    pButtons.Controls.Add(_ctrlOkCancel);
                   _ctrlOkCancel.Dock = DockStyle.Fill;
                    break;
                case MessageButtons.YesNo:
                    pButtons.Controls.Add(_ctrlYesNo);
                    _ctrlYesNo.Dock = DockStyle.Fill;
                    break;
                case MessageButtons.Ok:
                    pButtons.Controls.Add(_ctrlOk);
                    _ctrlOk.Dock = DockStyle.Fill;
                    break;
            }
        }

        private void pbFone_Paint(object sender, PaintEventArgs e)
        {
            float x = 140.0f;
            float y = 110.0f;
            if (_txt.Contains("\\n")) _txt = _txt.Replace("\\n", "\n");
            if (_txt.Contains('\n'))
            {
                foreach (string str in _txt.Split('\n'))
                {
                    using (Font font = new Font("MS Sans Serif", 12))
                    {
                        e.Graphics.DrawString(str, font, Brushes.Black,
                                              new PointF(x, y));
                    }
                    y += 17.0f;
                }
            }
            else
            {
                using (Font font = new Font("MS Sans Serif", 12))
                {
                    e.Graphics.DrawString(_txt, font, Brushes.Black,
                                          new PointF(x, y));
                }
            }
            
            x = 12.0f;
            y = 65.0f;
            switch (_messageIcon)
            {
                case MessageIcon.Question:
                    e.Graphics.DrawImage(Resources.QuestionCloud, new PointF(x, y));
                    break;
                case MessageIcon.Alert:
                    e.Graphics.DrawImage(Resources.Alert, new PointF(x, y));
                    break;
                case MessageIcon.Info:
                    e.Graphics.DrawImage(Resources.Information, new PointF(x, y));
                    break;
            }
        }

        private void MessageDialog_Load(object sender, EventArgs e)
        {
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
        }
    }
}
