using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp.Controls
{
    public partial class AppierenceSettings : UserControl
    {
        public AppierenceSettings()
        {
            InitializeComponent();
            InitSettings();
        }

        private void InitSettings()
        {
            // Инициализация шрифтов:
            fontDialog1.Font = Settings.Default.InterfaceFont;
            tbInterfaceFont.Text = Settings.Default.InterfaceFont.Name + "; " + Settings.Default.InterfaceFont.Size +
                                   Resources.PtText;
            fontDialog2.Font = Settings.Default.TableFont;
            tbTablesFont.Text = Settings.Default.TableFont.Name + "; " + Settings.Default.TableFont.Size + Resources.PtText;
            // Инициализация цветов:
            pbLastCast.BackColor = colDlgLastCast.Color = Settings.Default.LastTelecastColor;
            pbCurCast.BackColor = colDlgCurCast.Color = Settings.Default.CurTelecastColor;
            pbFutCast.BackColor = colDlgFutCast.Color = Settings.Default.FutTelecastColor;
            pbColBack1.BackColor = colDlgBack1.Color = Settings.Default.BackColor1;
            pbColBack2.BackColor = colDlgBack2.Color = Settings.Default.BackColor2;
            pbSelRow.BackColor = colDlgSelRow.Color = Settings.Default.RowSelectColor;
            // Инициализация переключателей:
            rbMonotone.Checked = Settings.Default.FlagBackMonotone;
            rbStriped.Checked = Settings.Default.FlagBackStripy;
            cbGradientMode.Enabled = rbGradient.Checked = Settings.Default.FlagBackGradient;
            rbPicture.Checked = Settings.Default.FlagBackPicture;
            switch (Settings.Default.GradientModeName)
            {
                case "Horizontal":
                    cbGradientMode.Text = Resources.HorizontalText;
                    break;
                case "Vertical":
                    cbGradientMode.Text = Resources.VerticalText;
                    break;
                case "BackwardDiagonal":
                    cbGradientMode.Text = Resources.BackwardDiagonalText;
                    break;
                case "ForwardDiagonal":
                    cbGradientMode.Text = Resources.ForwardDiagonalText;
                    break;
                default:
                    cbGradientMode.Text = Resources.HorizontalText;
                    break;
            }
            rbDefault.Checked = Settings.Default.FlagDefaultColor;
            rbSystem.Checked = Settings.Default.FlagSystemColor;
            rbArbitary.Checked = Settings.Default.FlagArbitraryColor;

            tbBackPicture.Text = Settings.Default.BackPicturePath;
        }

        private void AppierenceSettings_Load(object sender, EventArgs e)
        {
            InitSettings();
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
        }

        /// <summary>
        /// Изменение шрифта интерфейса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChangeInterfaceFont_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                Preferences.SetFonts(fontDialog1.Font, fontDialog2.Font);
                Settings.Default.InterfaceFont = fontDialog1.Font;
                tbInterfaceFont.Text = fontDialog1.Font.Name + "; " + fontDialog1.Font.Size + " пт";
            }
        }

        /// <summary>
        /// Изменение шрифта таблиц
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTableFont_Click(object sender, EventArgs e)
        {
            if (fontDialog2.ShowDialog() == DialogResult.OK)
            {
                Preferences.SetFonts(fontDialog1.Font, fontDialog2.Font);
                Settings.Default.TableFont = fontDialog2.Font;
                tbTablesFont.Text = fontDialog2.Font.Name + "; " + fontDialog2.Font.Size + " пт";
            }
        }

        /// <summary>
        /// Изменение цвета шрифта для прошедших передач
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChangeColorLast_Click(object sender, EventArgs e)
        {
            if (colDlgLastCast.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.LastTelecastColor = pbLastCast.BackColor = colDlgLastCast.Color;
            }
        }

        /// <summary>Изменение цвета шрифта для текущих передач</summary> 
        private void btnChangeColorCur_Click(object sender, EventArgs e)
        {
            if (colDlgCurCast.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.CurTelecastColor = pbCurCast.BackColor = colDlgCurCast.Color;
            }
        }
        /// <summary>
        /// Изменение цвета шрифта для будущих передач
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChangeColorFut_Click(object sender, EventArgs e)
        {
            if (colDlgFutCast.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.FutTelecastColor = pbFutCast.BackColor = colDlgFutCast.Color;
            }
        }
        
        /// <summary>
        /// Изменение флага градиента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbGradient_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.FlagBackGradient = cbGradientMode.Enabled = rbGradient.Checked;
        }

        /// <summary>
        /// Изменение флага монотонности
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbMonotone_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.FlagBackMonotone = rbMonotone.Checked;
        }

        /// <summary>
        /// Изменение флага полосатости
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbStriped_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.FlagBackStripy = rbStriped.Checked;
        }
        
        /// <summary>
        /// Изменение флага картинки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbPicture_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.FlagBackPicture = tbBackPicture.Enabled = btnOpenPic.Enabled = rbPicture.Checked;
        }

        /// <summary>
        /// Изменение режима градиента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbGradientMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbGradientMode.Text)
            {
                case "Horizontal":
                case "Горизонтальный":
                    Settings.Default.GradientModeName = LinearGradientMode.Horizontal.ToString();
                    break;
                case "Vertical":
                case "Вертикальный":
                    Settings.Default.GradientModeName = LinearGradientMode.Vertical.ToString();
                    break;
                case "ForwardDiagonal":
                case "Передне-диагональный":
                    Settings.Default.GradientModeName = LinearGradientMode.ForwardDiagonal.ToString();
                    break;
                case "BackwardDiagonal":
                case "Задне-диагональный":
                    Settings.Default.GradientModeName = LinearGradientMode.BackwardDiagonal.ToString();
                    break;
                default:
                    Settings.Default.GradientModeName = LinearGradientMode.Horizontal.ToString();
                    break;
            }
        }

        /// <summary>
        /// Изменение флага цвета по умолчанию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbDefault_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.FlagDefaultColor = rbDefault.Checked;
        }

        /// <summary>
        /// Изменение флага системности цвета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbSystem_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.FlagSystemColor = rbSystem.Checked;
        }

        /// <summary>
        /// Изменение флага произвольности цвета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbArbitary_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.FlagArbitraryColor = rbArbitary.Checked;
        }

        /// <summary>
        /// Изменение основного цвета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBack1_Click(object sender, EventArgs e)
        {
            if (colDlgBack1.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.BackColor1 = pbColBack1.BackColor = colDlgBack1.Color;
            }
        }

        /// <summary>
        /// Изменение второго цвета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnColBack2_Click(object sender, EventArgs e)
        {
            if (colDlgBack2.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.BackColor2 = pbColBack2.BackColor = colDlgBack2.Color;
            }
        }

        /// <summary>
        /// Изменение цвета строки при выделении
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelRowColor_Click(object sender, EventArgs e)
        {
            if (colDlgSelRow.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.RowSelectColor = pbSelRow.BackColor = colDlgSelRow.Color;
            }
        }
        
        /// <summary>
        /// Открыть картинку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenPic_Click(object sender, EventArgs e)
        {
            if (openPicDlg.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.BackPicturePath = tbBackPicture.Text = openPicDlg.FileName;
            }
        }
        
    }
}