using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TVProgViewer.TVProgApp.Classes;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp
{
    public partial class PleaseWaitForm : Form
    {
        /*private IntPtr _screenDC = IntPtr.Zero;
        private IntPtr _memDC = IntPtr.Zero;
        private IntPtr _bitmapHandle = IntPtr.Zero;
        private IntPtr _oldBitmapHandle = IntPtr.Zero;
        private Win32API.Size _size;
        private Win32API.Point _pointSource;
        private Win32API.Point _topPos;
        private Win32API.BLENDFUNCTION _blend;
        private byte _opacity = 255;

        private Bitmap _bmpDest;
        private Bitmap _bmpSrc;
        */
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private Graphics _graphics = null;
        private string _txt = String.Empty;
        private int _value = 0;
        public string TXT
        {
            get { return label1.Text; }
            set
            {
                if (label1.InvokeRequired)
                {
                    label1.Invoke(new MethodInvoker(() => label1.Text = value));
                }
                else label1.Text = value;
            }
        }
        public int Value
        {
            get { return progressBar1.Value; }
            set
            {
                if (progressBar1.InvokeRequired)
                {
                    progressBar1.Invoke(new MethodInvoker(() => progressBar1.Value = value));
                }
                else progressBar1.Value = value;
            }
        }
        public PleaseWaitForm()
        {
            InitializeComponent();
            this.BackgroundImage = Resources.Splash;
            this.TransparencyKey = Color.FromArgb(0, 0, 255, 255);
        }

        
       /* protected override CreateParams CreateParams 
        {
            get 
            { 
                System.Windows.Forms.CreateParams cp = base.CreateParams;
                cp.ExStyle = cp.ExStyle | Win32API.WS_EX_LAYERED;
                return cp;
            }
        }
        public void SetImage(Bitmap bitmap, byte opacity)
        {
            // Получить дескриптор на контекст дисплея
            _screenDC = Win32API.GetDC(IntPtr.Zero);
            // Создать контекст, совместимый с дисплеем
            _memDC = Win32API.CreateCompatibleDC(_screenDC);
            // Хендл изображения
            _bitmapHandle = IntPtr.Zero;
            // Хендл старого изображения:
            _oldBitmapHandle = IntPtr.Zero;
            try
            {
                // Получить хендл изображения
                _bitmapHandle = bitmap.GetHbitmap(Color.FromArgb(0));
                // Сохранить хендл изображения на случай ошибки:
                _oldBitmapHandle = Win32API.SelectObject(_memDC, _bitmapHandle);
                // Указать размеры окна
                _size = new Win32API.Size(bitmap.Width, bitmap.Height);
                _pointSource = new Win32API.Point(0, 0);
                _topPos = new Win32API.Point(this.Left, this.Top);
                // Заполнить структуру BLENDFUNCTION
                _blend = new Win32API.BLENDFUNCTION(Win32API.AC_SRC_OVER, 0, opacity,
                                                    Win32API.AC_SRC_ALPHA);
                // Обновить многослойное окно
                Win32API.UpdateLayeredWindow(
                    this.Handle, _screenDC, ref _topPos,
                                            ref _size, _memDC, ref _pointSource, 0, ref _blend, Win32API.ULW_ALPHA);
            }
            catch (Exception)
            {
                
            }
            finally
            {
                Win32API.ReleaseDC(IntPtr.Zero, _screenDC);
                if (_bitmapHandle != IntPtr.Zero)
                {
                    Win32API.SelectObject(_memDC, _oldBitmapHandle);
                    Win32API.DeleteObject(_bitmapHandle);
                }
                Win32API.DeleteObject(_memDC);
            }
        }
        */
        private void pbPleaseWait_Paint(object sender, PaintEventArgs e)
        {
        /*    label1.Text = _txt;
            progressBar1.Invoke(new MethodInvoker(() => progressBar1.Value = _value));*/
            
            /*_bmpSrc = Resources.Splash;
            _bmpDest = new Bitmap(_bmpSrc.Width, _bmpSrc.Height);
            using (Graphics g = Graphics.FromImage(_bmpDest))
            {
                g.DrawImage(_bmpSrc, 0, 0, _bmpSrc.Width, _bmpSrc.Height);
                using (Font font = new Font("Verdana", 7, FontStyle.Bold))
                {
                    g.DrawString(_txt, font, Brushes.LimeGreen,
                         new PointF(21, 163));
                    ;
                    
                    
                }
            }
           // this.SetImage(_bmpDest, _opacity);
            _bmpDest.Dispose();*/
        }


        


    }
}
