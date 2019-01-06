using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//---------------------------------------------------------------------
//THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
//KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//PARTICULAR PURPOSE.
//---------------------------------------------------------------------
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TVProgViewer.TVProgApp.Classes;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp
{
    [Description("DataGridView который сохраняет Column Order, Width and Visibility to user.config")]
    [ToolboxBitmap(typeof(System.Windows.Forms.DataGridView))]
    public class DataGridViewExt: DataGridView
    {
        // ------------ Установка параметров грида ----------------
        private void SetColumnOrder()
        {
            if (!gfDataGridViewSettings.Default.ColumnOrder.ContainsKey(this.Name))
            {
                return;
            }
            try
            {
                List<ColumnOrderItem> columnOrder = gfDataGridViewSettings.Default.ColumnOrder[this.Name];
                if (columnOrder != null)
                {
                    var sorted = columnOrder.OrderBy(i => i.DisplayIndex);
                    foreach (var item in sorted)
                    {
                        this.Columns[item.ColumnIndex].DisplayIndex = item.DisplayIndex;
                        this.Columns[item.ColumnIndex].Visible = item.Visible;
                        this.Columns[item.ColumnIndex].Width = item.Width;
                    }
                }
            }catch(Exception)
            {
                
            }
        }


        // ---------  Сохранение параметров грида --------------------
        private void SaveColumnOrder()
        {
            if (this.AllowUserToOrderColumns)
            {
                List<ColumnOrderItem> columnOrder = new List<ColumnOrderItem>();
                DataGridViewColumnCollection columns = this.Columns;
                for (int i = 0; i < columns.Count; i++)
                {
                    columnOrder.Add(new ColumnOrderItem()
                                        {
                                            ColumnIndex = i,
                                            DisplayIndex = columns[i].DisplayIndex,
                                            Visible = columns[i].Visible,
                                            Width = columns[i].Width
                                        });
                }
                
                gfDataGridViewSettings.Default.ColumnOrder[this.Name] = columnOrder;
                gfDataGridViewSettings.Default.Save();
            }
        }
        // ------------------------ Перегрузка создания контрола ---------------------
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            SetColumnOrder();
        }
        // -------------------- Перегрузка уничтожения контрола -------------------------
        protected override void Dispose(bool disposing)
        {
            SaveColumnOrder();
            base.Dispose(disposing);
        }
        private bool _isGradient;
        
        public bool IsGradient
        {
            get { return _isGradient; }
            set { _isGradient = value; }
        }
        private Image backgroundImage;

        public Image BackgroundImage
        {
            get { return backgroundImage; }
            set { backgroundImage = value; }
        }

        private Color backcolor2;

        public Color BackgroundColor2
        {
            get { return backcolor2; }
            set { backcolor2 = value; }
        }

        private System.Drawing.Drawing2D.LinearGradientMode gradientMode;

        public System.Drawing.Drawing2D.LinearGradientMode GradientMode
        {
            get { return gradientMode; }
            set { gradientMode = value; }
        }
        protected override void PaintBackground(Graphics graphics, Rectangle clipBounds, Rectangle gridBounds)
        {
            base.PaintBackground(graphics, clipBounds, gridBounds);
            if (_isGradient)
            {
                graphics.FillRectangle(new
                                           System.Drawing.Drawing2D.LinearGradientBrush(gridBounds,
                                                                                        this.BackgroundColor,
                                                                                        this.BackgroundColor2,
                                                                                        gradientMode), gridBounds);
               
            }
            if (backgroundImage != null)
                graphics.DrawImageUnscaled(backgroundImage, gridBounds);
        }
    }

    static class DataGridViewExtentions
    {
        /// <summary>
        /// Стили таблиц
        /// </summary>
        public enum Styles
        {
            TVGridView,
            TVGridWithEdit,
            View,
            None,
            ViewWithRow
        }


        /// <summary>
        /// привести таблицу к установленному виду
        /// </summary>
        /// <param name="dgr">таблица</param>
        /// <param name="style">стиль</param>
        public static void Style(this DataGridViewExt dgr, Styles style)
        {
            switch (style)
            {
                case Styles.TVGridView:
                    dgr.BackgroundColor = Settings.Default.BackColor1;
                    dgr.GridColor = SystemColors.ControlLight;
                    SetDgvColors(dgr);
                    SetGridLines(dgr);
                    dgr.RowHeadersVisible = false;
                    dgr.AllowUserToAddRows = dgr.AllowUserToDeleteRows = dgr.AllowUserToResizeRows = false;
                    dgr.ReadOnly = true;
                    dgr.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    break;
                case Styles.TVGridWithEdit:
                    dgr.BackgroundColor = Settings.Default.BackColor1;
                    
                    dgr.GridColor = SystemColors.ControlLight;
                    SetDgvColors(dgr);
                    SetGridLines(dgr); 
                    
                    dgr.RowHeadersVisible = true;
                    dgr.AllowUserToAddRows = dgr.AllowUserToDeleteRows = dgr.AllowUserToResizeRows = false;
                    break;
                case Styles.ViewWithRow:

                    dgr.GridColor =
                        dgr.BackColor = SystemColors.ControlLight;

                    dgr.AllowUserToAddRows =
                        dgr.AllowUserToDeleteRows =
                        dgr.AllowUserToOrderColumns =
                        dgr.AllowUserToResizeRows = false;

                    dgr.AllowUserToResizeColumns =
                        dgr.RowHeadersVisible =
                        dgr.ReadOnly = true;
                    SetGridLines(dgr);
                    dgr.AutoSize = false;
                    dgr.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                    dgr.AlternatingRowsDefaultCellStyle.BackColor = SystemColors.ControlLightLight;

                    break;

                case Styles.View:

                    dgr.GridColor =
                        dgr.BackColor = SystemColors.ControlLight;

                    dgr.AllowUserToAddRows =
                        dgr.AllowUserToDeleteRows =
                        dgr.AllowUserToOrderColumns =
                        dgr.AllowUserToResizeRows =
                        dgr.RowHeadersVisible = false;

                    dgr.ReadOnly = true;
                    dgr.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    dgr.AutoSize = false;
                    dgr.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    break;
                case Styles.None:
                    break;
            }
        }


        /// <summary>
        /// Установка линий таблице
        /// </summary>
        /// <param name="dgr"></param>
        private static void SetGridLines(DataGridViewExt dgr)
        {
            if (Settings.Default.FlagHorizGrid && Settings.Default.FlagVertGrid)
            {
                dgr.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            }
            else
            {
                if (Settings.Default.FlagHorizGrid)
                {
                    dgr.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                }
                else
                {
                    dgr.CellBorderStyle = DataGridViewCellBorderStyle.SingleVertical;
                }
            }
        }

        /// <summary>
        /// Установка цветов таблице
        /// </summary>
        /// <param name="dgr"></param>
        private static void SetDgvColors(DataGridViewExt dgr)
        {
            dgr.BackgroundImage = null;
            dgr.AlternatingRowsDefaultCellStyle.BackColor = Color.Empty;
            dgr.BackgroundColor = Color.FromArgb(239,255,239);
            dgr.BackgroundColor2 = Color.FromArgb(239, 255, 239);
            dgr.IsGradient = false;
            
            if (Settings.Default.FlagBackMonotone)
            {
                SetBackColor(dgr);
            }
            else
            {
                if (Settings.Default.FlagBackStripy)
                {
                     SetBackColor(dgr);
                    dgr.AlternatingRowsDefaultCellStyle.BackColor = Settings.Default.BackColor2;
                }
                else
                {
                    if (dgr.IsGradient = Settings.Default.FlagBackGradient)
                    {
                        dgr.DefaultCellStyle.BackColor = Color.Transparent;
                        dgr.GradientMode =
                            (LinearGradientMode)
                            Enum.Parse(typeof (LinearGradientMode), Settings.Default.GradientModeName);
                        dgr.BackgroundColor = Settings.Default.BackColor1;
                        dgr.BackgroundColor2 = Settings.Default.BackColor2;
                    }
                    else
                    {
                        if (Settings.Default.FlagBackPicture)
                        {
                            try
                            {
                                dgr.DefaultCellStyle.BackColor = Color.Transparent;
                                dgr.BackgroundImage =
                                    Image.FromFile(Settings.Default.BackPicturePath).GetThumbnailImage(dgr.Width, dgr.Height,
                                                                                                       myCallback,
                                                                                                       IntPtr.Zero);
                            }
                            catch (Exception)
                            {

                            }
                        }
                        dgr.IsGradient = false;
                    }
                }
            }
            dgr.RowsDefaultCellStyle.SelectionBackColor = dgr.DefaultCellStyle.SelectionBackColor = Settings.Default.RowSelectColor;
        }

        static Image.GetThumbnailImageAbort myCallback =
                       new Image.GetThumbnailImageAbort(ThumbnailCallback);
        private static bool ThumbnailCallback ()
        {
            return false;
        }
        private static void SetBackColor(DataGridViewExt dgr)
        {
            if (Settings.Default.FlagDefaultColor)
            {
                dgr.BackgroundColor = dgr.RowsDefaultCellStyle.BackColor = Color.FromArgb(239, 255, 239);
            }
            else
            {
                if (Settings.Default.FlagSystemColor)
                {
                    dgr.BackgroundColor = dgr.RowsDefaultCellStyle.BackColor =
                        SystemColors.Window;
                }
                else
                {
                    if (Settings.Default.FlagArbitraryColor)
                    {
                        dgr.BackgroundColor = dgr.RowsDefaultCellStyle.BackColor = Settings.Default.BackColor1;
                    }
                }
            }
        }

        /// <summary>
        /// Переименовать колонки таблицы
        /// </summary>
        /// <param name="dgv">таблица</param>
        /// <param name="capts">заголовки</param>
        public static void RenameColumns(this DataGridViewExt dgv, Dictionary<string, string> capts)
        {
            foreach (KeyValuePair<string, string> pr in capts)
            {
                if (dgv.Columns.Contains(pr.Key))
                {
                    if (pr.Value == string.Empty)
                    {
                        //dgv.Columns[pr.Key].Visible = false;
                    }
                    else
                    {
                        dgv.Columns[pr.Key].HeaderText = pr.Value;
                    }

                }
            }
        }


        /// <summary>
        /// Процедура подгонки данных грида 
        /// </summary>
        /// <param name="dgr"></param>
        public static void FitColumns(this DataGridViewExt dgr, bool allfill, bool main, bool arr)
        {
            int cnt = dgr.Columns.Count;
            int i = 0;

            foreach (DataGridViewColumn c in dgr.Columns)
            {
                i++;
                if (main)
                {
                    if (i > 5)
                    {
                        if (allfill)
                        {
                            c.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }
                        else
                        {
                            c.AutoSizeMode = i == cnt
                                                 ? DataGridViewAutoSizeColumnMode.Fill
                                                 : DataGridViewAutoSizeColumnMode.DisplayedCells;
                        }
                    }
                    else
                    {
                        c.Width = 45;
                    }
                }
                else
                {
                    if (allfill)
                    {
                        c.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                    else
                    {
                        c.AutoSizeMode = i == cnt
                                             ? DataGridViewAutoSizeColumnMode.Fill
                                             : DataGridViewAutoSizeColumnMode.DisplayedCells;
                    }
                }
                c.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }

            dgr.DefaultCellStyle.ForeColor = Color.Black;
            dgr.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            dgr.AllowUserToResizeColumns = true;
            if (arr)
            {
                dgr.AutoResizeRows(DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders);
            }
            dgr.Refresh();
        }
        /// <summary>
        /// A DataGridViewColumn implementation that provides a column that
        /// will display a progress bar.
        /// </summary>
        public class DataGridViewProgressBarColumn : DataGridViewTextBoxColumn
        {
            public DataGridViewProgressBarColumn()
            {
                // Set the cell template
                CellTemplate = new DataGridViewProgressBarCell();

                // Set the default style padding
                Padding pad = new Padding(
                  DataGridViewProgressBarCell.STANDARD_HORIZONTAL_MARGIN,
                  DataGridViewProgressBarCell.STANDARD_VERTICAL_MARGIN,
                  DataGridViewProgressBarCell.STANDARD_HORIZONTAL_MARGIN,
                  DataGridViewProgressBarCell.STANDARD_VERTICAL_MARGIN);
                DefaultCellStyle.Padding = pad;

                // Set the default format
                DefaultCellStyle.Format = "## \\%";
            }


        }

        /// <summary>
        /// A DataGridViewCell class that is used to display a progress bar
        /// within a grid cell.
        /// </summary>
        public class DataGridViewProgressBarCell : DataGridViewTextBoxCell
        {
            /// <summary>
            /// Standard value to use for horizontal margins
            /// </summary>
            internal const int STANDARD_HORIZONTAL_MARGIN = 4;

            /// <summary>
            /// Standard value to use for vertical margins
            /// </summary>
            internal const int STANDARD_VERTICAL_MARGIN = 4;

            /// <summary>
            /// Constructor sets the expected type to int
            /// </summary>
            public DataGridViewProgressBarCell()
            {
                this.ValueType = typeof(int);
            }

            /// <summary>
            /// Paints the content of the cell
            /// </summary>
            protected override void Paint(Graphics g, Rectangle clipBounds, Rectangle cellBounds,
              int rowIndex, DataGridViewElementStates cellState,
              object value, object formattedValue,
              string errorText,
              DataGridViewCellStyle cellStyle,
              DataGridViewAdvancedBorderStyle advancedBorderStyle,
              DataGridViewPaintParts paintParts)
            {
                int leftMargin = STANDARD_HORIZONTAL_MARGIN;
                int rightMargin = STANDARD_HORIZONTAL_MARGIN;
                int topMargin = STANDARD_VERTICAL_MARGIN;
                int bottomMargin = STANDARD_VERTICAL_MARGIN;
                int imgHeight = 1;
                int imgWidth = 1;
                int progressWidth = 1;
                PointF fontPlacement = new PointF(0, 0);

                int progressVal;
                if (value != null)
                    progressVal = (int)value;
                else
                    progressVal = 0;

                // Draws the cell grid
                base.Paint(g, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, (paintParts & ~DataGridViewPaintParts.ContentForeground));

                // Get margins from the style
                if (null != cellStyle)
                {
                    leftMargin = cellStyle.Padding.Left;
                    rightMargin = cellStyle.Padding.Right;
                    topMargin = cellStyle.Padding.Top;
                    bottomMargin = cellStyle.Padding.Bottom;
                }

                // Calculate the sizes
                imgHeight = cellBounds.Bottom - cellBounds.Top - (topMargin + bottomMargin);
                imgWidth = cellBounds.Right - cellBounds.Left - (leftMargin + rightMargin);
                if (imgWidth <= 0)
                {
                    imgWidth = 1;
                }
                if (imgHeight <= 0)
                {
                    imgHeight = 1;
                }

                // Calculate the progress
                progressWidth = (imgWidth * (progressVal) / 100);
                if (progressWidth <= 0)
                {
                    if (progressVal > 0)
                    {
                        progressWidth = 1;
                    }
                    else
                    {
                        progressWidth = 0;
                    }
                }

                // Calculate the font
                if (null != formattedValue)
                {
                    SizeF availArea = new SizeF(imgWidth, imgHeight);
                    SizeF fontSize = g.MeasureString(formattedValue.ToString(), cellStyle.Font, availArea);

                    #region [Font Placement Calc]

                    if (null == cellStyle)
                    {
                        fontPlacement.Y = cellBounds.Y + topMargin + (((float)imgHeight - fontSize.Height) / 2);
                        fontPlacement.X = cellBounds.X + leftMargin + (((float)imgWidth - fontSize.Width) / 2);
                    }
                    else
                    {
                        // Set the Y vertical position
                        switch (cellStyle.Alignment)
                        {
                            case DataGridViewContentAlignment.BottomCenter:
                            case DataGridViewContentAlignment.BottomLeft:
                            case DataGridViewContentAlignment.BottomRight:
                                {
                                    fontPlacement.Y = cellBounds.Y + topMargin + imgHeight - fontSize.Height;
                                    break;
                                }
                            case DataGridViewContentAlignment.TopCenter:
                            case DataGridViewContentAlignment.TopLeft:
                            case DataGridViewContentAlignment.TopRight:
                                {
                                    fontPlacement.Y = cellBounds.Y + topMargin - fontSize.Height;
                                    break;
                                }
                            case DataGridViewContentAlignment.MiddleCenter:
                            case DataGridViewContentAlignment.MiddleLeft:
                            case DataGridViewContentAlignment.MiddleRight:
                                {
                                    fontPlacement.Y = cellBounds.Y + topMargin + fontSize.Height - imgHeight - 2;
                                    break;
                                }
                            case DataGridViewContentAlignment.NotSet:
                            default:
                                {
                                    fontPlacement.Y = cellBounds.Y + topMargin + (((float)imgHeight - fontSize.Height) / 2);
                                    break;
                                }
                        }
                        // Set the X horizontal position
                        switch (cellStyle.Alignment)
                        {

                            case DataGridViewContentAlignment.BottomLeft:
                            case DataGridViewContentAlignment.MiddleLeft:
                            case DataGridViewContentAlignment.TopLeft:
                                {
                                    fontPlacement.X = cellBounds.X + leftMargin;
                                    break;
                                }
                            case DataGridViewContentAlignment.BottomRight:
                            case DataGridViewContentAlignment.MiddleRight:
                            case DataGridViewContentAlignment.TopRight:
                                {
                                    fontPlacement.X = cellBounds.X + leftMargin + imgWidth - fontSize.Width;
                                    break;
                                }
                            case DataGridViewContentAlignment.BottomCenter:
                            case DataGridViewContentAlignment.MiddleCenter:
                            case DataGridViewContentAlignment.TopCenter:
                            case DataGridViewContentAlignment.NotSet:
                            default:
                                {
                                    fontPlacement.X = cellBounds.X + leftMargin + (((float)imgWidth - fontSize.Width) / 2);
                                    break;
                                }
                        }
                    }
                    #endregion [Font Placement Calc]
                }

                // Draw the background
                Rectangle backRectangle = new Rectangle(cellBounds.X + leftMargin, cellBounds.Y + topMargin, imgWidth, imgHeight);
                using (SolidBrush backgroundBrush = new SolidBrush(Color.FromKnownColor(KnownColor.PaleGreen)))
                {
                    g.FillRectangle(backgroundBrush, backRectangle);
                }

                // Draw the progress bar
                if (progressWidth > 0)
                {
                    Rectangle progressRectangle = new Rectangle(cellBounds.Width + cellBounds.X - rightMargin - progressWidth, cellBounds.Y + topMargin, progressWidth, imgHeight);
                    using (LinearGradientBrush progressGradientBrush = new LinearGradientBrush(progressRectangle, Color.LightGreen, Color.MediumSeaGreen, LinearGradientMode.Vertical))
                    {
                        progressGradientBrush.SetBlendTriangularShape((float).5);
                        g.FillRectangle(progressGradientBrush, progressRectangle);
                    }
                }

                // Draw the text
                if (null != formattedValue && null != cellStyle)
                {
                    using (SolidBrush fontBrush = new SolidBrush(cellStyle.ForeColor))
                    {
                        g.DrawString(formattedValue.ToString(), cellStyle.Font, fontBrush, fontPlacement);
                    }
                }
            }

        }
    }
}

  


