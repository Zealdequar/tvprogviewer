using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TVProgViewer.TVProgApp.Classes
{
    public class Win32API
    {
        public const int WS_EX_LAYERED = 0x80000;
        public const int ULW_ALPHA = 0x2;
        public const byte AC_SRC_OVER = 0x0;
        public const byte AC_SRC_ALPHA = 0x1;


        public const UInt32 SW_HIDE = 0;
        public const UInt32 SW_SHOWNORMAL = 1;
        public const UInt32 SW_NORMAL = 1;
        public const UInt32 SW_SHOWMINIMIZED = 2;
        public const UInt32 SW_SHOWMAXIMIZED = 3;
        public const UInt32 SW_MAXIMIZE = 3;
        public const UInt32 SW_SHOWNOACIVATE = 4;
        public const UInt32 SW_SHOW = 5;
        public const UInt32 SW_MINIMIZE = 6;
        public const UInt32 SW_SHOWMINNOACTIVE = 7;
        public const UInt32 SW_SHOWNA = 8;
        public const UInt32 SW_RESTORE = 9;
        // Точка (координата)
        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int x;
            public int y;
            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
        // Размер
        [StructLayout(LayoutKind.Sequential)]
        public struct Size
        {
            public int cx;
            public int cy;
            public Size(int cx, int cy)
            {
                this.cx = cx;
                this.cy = cy;
            }
        }

        // Определить режим вывода полупрозрачных изображений
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
            public BLENDFUNCTION(byte BlendOp, byte BlendFlags,
                byte SourceContrastAlpha, byte AlphaFormat)
            {
                this.BlendOp = BlendOp;
                this.BlendFlags = BlendFlags;
                this.SourceConstantAlpha = SourceContrastAlpha;
                this.AlphaFormat = AlphaFormat;
            }
        }

        /// Получить дескриптор контекста дисплея для клиентской области указанного окна
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        /// Создать совместмый контекст с заданным устройством
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        /// Освободить контекст
        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        /// Удалить контекст
        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hdc);

        ///Выбрать объект в заданный контекст
        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        /// Удалить объект
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        /// Обновить моногослойное окно
        [DllImport("user32.dll")]
        public static extern bool UpdateLayeredWindow(
            IntPtr hwnd, 
            IntPtr hdcDst,
            ref Win32API.Point pptDst, 
            ref Win32API.Size psize, 
            IntPtr hdcSrc,
            ref Win32API.Point pprSrc,
            int crKey, 
            ref Win32API.BLENDFUNCTION pblend, 
            int dwFlags);

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(
            EnumWindowsProc lpEnumFunc, IntPtr lParam);

        public static bool fullMode;
        public static bool FullScreenForm (IntPtr hwnd,  IntPtr lParam)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(hwnd, ref placement);
            StringBuilder buff = new StringBuilder(256);
            GetWindowText(hwnd, buff, buff.Capacity);
            if ((placement.showCmd == SW_MAXIMIZE || placement.showCmd == SW_NORMAL) && placement.rcNormalPosition.Width >= System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width &&
                placement.rcNormalPosition.Height >= System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height && placement.rcNormalPosition.X <= 0 &&
                placement.rcNormalPosition.Y <= 0 && buff.ToString() != "Program Manager")
            {
                fullMode = true;
                return false;
            }
            fullMode = false;
            return true;
        }
        [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
         internal static extern int GetWindowText(IntPtr hWnd, [Out, MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        [DllImport("kernel32.dll")]
        public static extern bool Beep(int beepFreq, int beepDuration);
    }
}
