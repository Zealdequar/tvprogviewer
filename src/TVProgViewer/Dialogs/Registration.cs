using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace TVProgViewer.TVProgApp.Dialogs
{
    public partial class Registration : Form
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void Registry_Load(object sender, EventArgs e)
        {
            tbOS.Text = Environment.OSVersion.VersionString;
            tbVersion.Text = Application.ProductVersion;
            tbFirstName.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        [Obfuscation(Feature = "all", ApplyToMembers = true)]
        private void btnDoLetter_Click(object sender, EventArgs e)
        {
            string message = tbFirstName.Text + "|" + tbMiddleName.Text + "|" + tbLastName.Text + "|" + tbEmail.Text +
                             "|" + tbCountry.Text + "|" + tbRegion.Text + "|" + tbCity.Text + "|" + tbOS.Text + "|" +
                             tbVersion.Text;
            // Получить из строки набор байтов, которые будем шифровать:
            byte[] sourcedata = Encoding.UTF8.GetBytes(message);
            // Алгоритм:
            SymmetricAlgorithm sa_in = Rijndael.Create();
            // Объект преобразования данных:
            ICryptoTransform ct_in = sa_in.CreateEncryptor(
                (new PasswordDeriveBytes("Jessica", null)).GetBytes(16), new byte[16]);
            // Поток:
            MemoryStream ms_in = new MemoryStream();
            // Шифровальщик потока:
            CryptoStream cs_in = new CryptoStream(ms_in, ct_in, CryptoStreamMode.Write);
            // Записать шифрованные данные в поток:
            cs_in.Write(sourcedata, 0, sourcedata.Length);
            cs_in.FlushFinalBlock();
            // Создать сроку:
            string crypt_str = Convert.ToBase64String(ms_in.ToArray());
            var proc = new System.Diagnostics.Process();
            Clipboard.SetText(crypt_str);
            proc.StartInfo.FileName = String.Format( "mailto:TVProgViewer.TVProgApp@gmail.com?subject=\"Registration\"&body=\"{0}\"", crypt_str);
            proc.StartInfo.UseShellExecute = true;
            proc.Start();
        }
    }
}
