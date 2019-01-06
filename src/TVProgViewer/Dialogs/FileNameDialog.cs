using System;
using System.Text;
using System.Windows.Forms;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp.Dialogs
{
    public partial class FileNameDialog : Form
    {
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public FileNameDialog()
        {
            InitializeComponent();
            chbDate.Checked = FileNameSettings.Default.Date;
            chbTime.Checked = FileNameSettings.Default.Time;
            chbProgram.Checked = FileNameSettings.Default.Programm;
            chbTelecast.Checked = FileNameSettings.Default.Telecast;
            chbUserText.Checked = FileNameSettings.Default.UserBool;
            tbUserOptName.Text = FileNameSettings.Default.UserText;
            cbDivider.Text = FileNameSettings.Default.Divider;
        }

        /// <summary>
        /// Шаблон для имени файла
        /// </summary>
        public string CaptureFilename
        {
            get
            {
                string strRes = String.Empty;
                StringBuilder strFilename = new StringBuilder(String.Empty);
                char divider = GetDivider();
                if (chbDate.Checked)
                {
                    strFilename.Append(Resources.DateText);
                    if (divider != 'n') strFilename.Append(divider);
                }
                if (chbTime.Checked)
                {
                    strFilename.Append(Resources.TimeText);
                    if (divider != 'n') strFilename.Append(divider);
                }
                if (chbProgram.Checked)
                {
                    strFilename.Append(Resources.ProgramText);
                    if (divider != 'n') strFilename.Append(divider);
                }
                if (chbTelecast.Checked)
                {
                    strFilename.Append(Resources.TelecastText);
                    if (divider != 'n') strFilename.Append(divider);
                }
                if (chbUserText.Checked) strFilename.AppendLine(tbUserOptName.Text);
                if (divider != 'n')
                {
                    strRes = strFilename.ToString().TrimEnd(divider);
                }
                else
                {
                    strRes = strFilename.ToString();
                }
                lblTextPreview.Text = strRes;
                return strRes;
            }
        }

        public bool Date
        {
            get { return chbDate.Checked; }
        }

        public bool Time
        {
            get { return chbTime.Checked; }
        }

        public bool Program
        {
            get { return chbProgram.Checked; }
        }

        public bool Telecast
        {
            get { return chbTelecast.Checked; }
        }

        public bool UserBool
        {
            get { return chbUserText.Checked; }
        }

        public string UserText
        {
            get { return tbUserOptName.Text; }
        }

        public string DividerText
        {
            get { return cbDivider.Text; }
        }

        /// <summary>
        /// Получить разделитель
        /// </summary>
        /// <returns></returns>
        private char GetDivider()
        {
            if (cbDivider.Text == Resources.EmptyText) return 'n';
            if (cbDivider.Text == Resources.SpaceText + " ( )") return ' ';
            if (cbDivider.Text == Resources.SharpText + " (#)") return '#';
            if (cbDivider.Text == Resources.AccetuationText + " (_)") return '_';
            if (cbDivider.Text == Resources.CommaText + " (,)") return ',';
            if (cbDivider.Text == Resources.DotText + " (.)") return '.';
            return 'n';
        }

        /// <summary>
        /// Вкл/откл чекбокса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckedBoxCheckedChanged(object sender, EventArgs e)
        {
            lblTextPreview.Text = CaptureFilename;
            if (sender is CheckBox)
            {
                if ((sender as CheckBox).Name == "chbUserText")
                {
                    tbUserOptName.Enabled = (sender as CheckBox).Checked;
                }
            }
        }

        private void FileNameDialog_Load(object sender, EventArgs e)
        {
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
        }
    }
}
