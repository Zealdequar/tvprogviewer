using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TVProgViewer.TVProgApp.Classes;
using TVProgViewer.TVProgApp.Dialogs;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp
{
    public partial class SearchDialog : Form
    {

        private CtrlDays ctrlDays;
        private CtrlChannel ctrlChannel;
        private CtrlGenre ctrlGenre;
        private CtrlRating ctrlRating;

        private DataSet _dsConfig = new DataSet("Config");

        private TimeSpan _tsFrom, _tsTo, _tsFinalFrom, _tsFinalTo;
       
        /// <summary>
        /// Содержит
        /// </summary>
        public string Match
        {
            get { return tbMatch.Text; }
            set { tbMatch.Text = value; }
        }
        /// <summary>
        /// Поиск не содержит
        /// </summary>
        public string DontMatch
        {
            get { return tbDontMatch.Text; }
            set { tbDontMatch.Text = value; }
        }
        /// <summary>
        /// В анонсах
        /// </summary>
        public bool InAnons
        {
            get { return chkSearchInAnons.Checked; }
            set { chkSearchInAnons.Checked = value; }
        }
        /// <summary>
        /// По времени
        /// </summary>
        public bool InTime
        {
            get { return chkTimeFromTo.Checked; }
            set { chkTimeFromTo.Checked = value; }
        }
        /// <summary>
        /// Начиная с 
        /// </summary>
        public TimeSpan TsFinalFrom
        {
            get { return _tsFinalFrom; }
            set { _tsFinalFrom = value; }
        }
        /// <summary>
        /// Заканчивая временем
        /// </summary>
        public TimeSpan TsFinalTo
        {
            get { return _tsFinalTo; }
            set{_tsFinalFrom = value;}
        }
        /// <summary>
        /// Положение ползунка от:
        /// </summary>
        public int TrackBarValueFrom
        {
            get { return trackBarFrom.Value; }
            set { trackBarFrom.Value = value; }
        }
        /// <summary>
        /// Положение ползунка до
        /// </summary>
        public int TrackBarValueTo
        {
            get { return trackBarTo.Value; }
            set { trackBarTo.Value = value; }
        }

        private DataTable _dtSearch;
        private TVProgClass _tvProgClass;
        private Control _listViewDays = new Control();
        private Control _listViewChannel = new Control();
        private Control _listViewGenre = new Control();
        private Control _listViewRating = new Control();


        public SearchDialog(TVProgClass tvProgClass)
        {
            InitializeComponent();
            _tvProgClass = tvProgClass;
            toolTip1.AutoPopDelay = 300;
            toolTip1.InitialDelay = 500;
            toolTip1.ReshowDelay = 100;
            GetXmlParams();
           
        }

        public SearchDialog(TVProgClass tvProgClass, string match)
        {
            InitializeComponent();
            _tvProgClass = tvProgClass;
            toolTip1.AutoPopDelay = 300;
            toolTip1.InitialDelay = 500;
            toolTip1.ReshowDelay = 100;
            GetXmlParams();
            tbMatch.Text = match;
           
        }


        private void GetXmlParams()
        {
            if (!Directory.Exists(Application.StartupPath + "\\Data"))
                Directory.CreateDirectory(Application.StartupPath + "\\Data");
            string xmlOptionsFileName = Path.Combine(Application.StartupPath, Preferences.xmlSearchOptionsFile);
            if (File.Exists(xmlOptionsFileName))
            {
                _dsConfig.ReadXml(xmlOptionsFileName, XmlReadMode.Auto);
                if (_dsConfig.Tables["SearchSettings"] != null)
                {
                    DataRow dr = _dsConfig.Tables["SearchSettings"].Rows[0];
                    if (dr != null)
                    {
                        if (dr["LoadSettings"].ToString().ToLower() == "true")
                        {
                            chkSaveParamSearch.Checked = true;
                            Match = dr["Match"].ToString();
                            DontMatch = dr["DontMatch"].ToString();
                            InAnons =  dr["InAnons"].ToString().ToLower() == "true";
                            InTime = dr["InTime"].ToString().ToLower() == "true";
                            TsFinalFrom = new TimeSpan(long.Parse( dr["TsFinalFrom"].ToString()));
                            TsFinalTo = new TimeSpan(long.Parse( dr["TsFinalTo"].ToString()));
                            TrackBarValueFrom = int.Parse(dr["TrackBarValueFrom"].ToString());
                            TrackBarValueTo = int.Parse( dr["TrackBarValueTo"].ToString());
                        }
                        else chkSaveParamSearch.Checked = false;
                    }
                }
            }
            else  // Установка значений по умолчанию
            {
                DataTable searchSettings = new DataTable("SearchSettings");
                searchSettings.Columns.Add("LoadSettings", typeof(bool));
                searchSettings.Columns.Add("Match", typeof(string));
                searchSettings.Columns.Add("DontMatch", typeof(string));
                searchSettings.Columns.Add("InAnons", typeof(bool));
                searchSettings.Columns.Add("InTime", typeof(bool));
                searchSettings.Columns.Add("TsFinalFrom", typeof (long));
                searchSettings.Columns.Add("TsFinalTo", typeof (long));
                searchSettings.Columns.Add("TrackBarValueFrom", typeof (int));
                searchSettings.Columns.Add("TrackBarValueTo", typeof(int));
                searchSettings.Rows.Add(chkSaveParamSearch.Checked = true, Match, DontMatch, 
                                        InAnons, InTime, TsFinalFrom.Ticks, TsFinalTo.Ticks, TrackBarValueFrom, TrackBarValueTo);
                _dsConfig.Tables.Add(searchSettings);
                _dsConfig.WriteXml(xmlOptionsFileName);
            }
        }

        private void trackBarFrom_ValueChanged(object sender, EventArgs e)
        {
            TimeSpan tsMinute = new TimeSpan(0, 10 * trackBarFrom.Value, 0);
            _tsFinalFrom = _tsFrom.Add(tsMinute);
            chkTimeFromTo.Text = Resources.TimeFromText + " " +  String.Format("{0:D2}:{1:D2}",  _tsFinalFrom.Hours, 
                _tsFinalFrom.Minutes)+
                Resources.ToText + String.Format("{0:D2}:{1:D2}", _tsFinalTo.Hours, _tsFinalTo.Minutes);
            if (trackBarTo.Value < trackBarFrom.Value )
            {
                trackBarTo.Value = trackBarFrom.Value;
            }
            toolTip1.SetToolTip(trackBarFrom, Resources.FromText + " " + String.Format("{0:D2}:{1:D2}", _tsFinalFrom.Hours, _tsFinalFrom.Minutes));
            chkTimeFromTo.Checked = trackBarTo.Value != trackBarFrom.Value;
            
        }

        private void trackBarTo_ValueChanged(object sender, EventArgs e)
        {
            TimeSpan tsMinute = new TimeSpan(0, 10 * trackBarTo.Value, 0);
            _tsFinalTo = _tsTo.Add(tsMinute);
            chkTimeFromTo.Text = Resources.TimeFromText + " " + String.Format("{0:D2}:{1:D2}", _tsFinalFrom.Hours, _tsFinalFrom.Minutes) +
                Resources.ToText + String.Format("{0:D2}:{1:D2}", _tsFinalTo.Hours, _tsFinalTo.Minutes);
            if (trackBarTo.Value < trackBarFrom.Value) trackBarFrom.Value = trackBarTo.Value;
            toolTip1.SetToolTip(trackBarTo, Resources.AndToText + String.Format("{0:D2}:{1:D2}", _tsFinalTo.Hours, _tsFinalTo.Minutes));
            chkTimeFromTo.Checked = trackBarTo.Value != trackBarFrom.Value;
        }

        private void SearchForm_Load(object sender, EventArgs e)
        {
            if (Owner != null && Owner is MainForm)
            {
                ctrlDays = new CtrlDays((Owner as MainForm).Start, (Owner as MainForm).Stop);
                ctrlChannel = new CtrlChannel((Owner as MainForm).Channels);
                ctrlGenre = new CtrlGenre((Owner as MainForm).GenresTable);
                ctrlRating = new CtrlRating((Owner as MainForm).Ratings, (Owner as MainForm).PropCapture);
            }
            _tsTo = _tsFrom = _tsFinalTo = _tsFinalFrom = Settings.Default.BeginEndTime;
            pControls.Controls.Clear();
            pControls.Controls.Add(ctrlDays);
            ctrlDays.Dock = DockStyle.Fill;
            _listViewDays = ctrlDays.Controls.Find("listViewDays", true)[0];
            _listViewChannel = ctrlChannel.Controls.Find("listViewChannels", true)[0];
            _listViewGenre = ctrlGenre.Controls.Find("listViewGenre", true)[0];
            _listViewRating = ctrlRating.Controls.Find("listViewRating", true)[0];
            if (!chkDaysOfWeek.Checked) chkDaysOfWeek.Checked = true;
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
            chkTimeFromTo.Text = Resources.TimeFromText + String.Format("{0:D2}:{1:D2}", _tsFinalFrom.Hours, _tsFinalFrom.Minutes) +
             Resources.ToText + String.Format("{0:D2}:{1:D2}", _tsFinalTo.Hours, _tsFinalTo.Minutes);
        }
        /// <summary>
        /// Дни недели
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkDaysOfWeek_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDaysOfWeek.Checked)
            {
                pControls.Controls.Clear();
                pControls.Controls.Add(ctrlDays);
                ctrlDays.Dock = DockStyle.Fill;
                chkChannels.Checked = false;
                chkGenres.Checked = false;
                chkRating.Checked = false;
            }
        }
        
        /// <summary>
        /// Каналы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkChannels_CheckedChanged(object sender, EventArgs e)
        {
            if (chkChannels.Checked)
            {
                pControls.Controls.Clear();
                pControls.Controls.Add(ctrlChannel);
                ctrlChannel.Dock = DockStyle.Fill;
                chkDaysOfWeek.Checked = false;
                chkGenres.Checked = false;
                chkRating.Checked = false;
            }
        }


        /// <summary>
        /// Жанры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkGenres_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGenres.Checked)
            {
                pControls.Controls.Clear();
                pControls.Controls.Add(ctrlGenre);
                ctrlGenre.Dock = DockStyle.Fill;
                chkDaysOfWeek.Checked = chkChannels.Checked = chkRating.Checked = false;
            }
        }

        /// <summary>
        /// Рейтинги
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkRating_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRating.Checked)
            {
                pControls.Controls.Clear();
                pControls.Controls.Add(ctrlRating);
                ctrlRating.Dock = DockStyle.Fill;
                chkDaysOfWeek.Checked = chkChannels.Checked = chkGenres.Checked = false;
            }
        }

        /// <summary>
        /// Отметить все
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            foreach (Control ctrl in pControls.Controls)
            {
                foreach (Control ctrlView in ctrl.Controls)
                {
                    if (ctrlView is ListView)
                    {
                        foreach (ListViewItem li in (ctrlView as ListView).Items)
                        {
                            li.Checked = true;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Снять отметку у всех
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUnchecked_Click(object sender, EventArgs e)
        {
            foreach (Control ctrl in pControls.Controls)
            {
                foreach (Control ctrlView in ctrl.Controls)
                {
                    if (ctrlView is ListView)
                    {
                        foreach (ListViewItem li in (ctrlView as ListView).Items)
                        {
                            li.Checked = false;
                        }
                    }
                }
            }
        }

        private void btnInvert_Click(object sender, EventArgs e)
        {
            foreach (Control ctrl in pControls.Controls)
            {
                foreach (Control ctrlView in ctrl.Controls)
                {
                    if (ctrlView is ListView)
                    {
                        foreach (ListViewItem li in (ctrlView as ListView).Items)
                        {
                            li.Checked = !li.Checked;
                        }
                    }
                }
            }
        }

        private DataTable AcceptFilter(string titleMatch)
        {
            this.Cursor = Cursors.WaitCursor;
            DataTable dtSearch = new DataTable("Searching");
            dtSearch.Columns.Add("id", typeof (int));
            dtSearch.Columns.Add("cid", typeof(int));
            dtSearch.Columns.Add("start", typeof (DateTime));
            dtSearch.Columns.Add("stop", typeof (DateTime));
            dtSearch.Columns.Add("start_mo", typeof (DateTime));
            dtSearch.Columns.Add("stop_mo", typeof (DateTime));
            dtSearch.Columns.Add("title", typeof (string));
            dtSearch.Columns.Add("category", typeof (string));
            dtSearch.Columns.Add("desc", typeof (string));
            dtSearch.Columns.Add("record", typeof (bool));
            dtSearch.Columns.Add("remind", typeof(bool));
            dtSearch.Columns.Add("bell", typeof(Image));
            dtSearch.Columns.Add("capture", typeof (Image));
            string filterStr = String.Empty;
            if (!InAnons)
            {
                filterStr = "title LIKE '*" + titleMatch + "*' ";
                if (DontMatch != String.Empty) filterStr += " and NOT title LIKE '*" + DontMatch + "*' ";
            }
            else
            {
                filterStr = "desc LIKE '*" + titleMatch + "*' ";
                if (DontMatch != String.Empty) filterStr += " and NOT desc LIKE '*" + DontMatch + "*' ";
            }
            string filtStrChannel = "(";
            filtStrChannel += (_listViewChannel as ListView).Items.Cast<ListViewItem>().Where(liChan =>
                liChan.Checked).Aggregate(String.Empty, (current, liChan) => current +
                    " cid = " + liChan.Name + " or");
            if (filtStrChannel.Length > 3)
            {
                filtStrChannel = filtStrChannel.Substring(0, filtStrChannel.Length - 3) + ")";
            }
            else
            {
                filtStrChannel = String.Empty;
            }
            if (InTime)
            {
                filterStr += "and";
                string filterStrInTime = (_listViewDays as ListView).Items.Cast<ListViewItem>().Where(liDay =>
                    liDay.Checked).Aggregate(String.Empty, (current, liDay) => current + 
                        (" (" + filterStr + " start >= '" + 
                        Convert.ToDateTime(liDay.Text).AddTicks(TsFinalFrom.Ticks) +
                        "' and start <= '" + Convert.ToDateTime(liDay.Text).AddTicks(TsFinalTo.Ticks) + 
                        (filtStrChannel.Length > 0 ? "' and " + filtStrChannel : "'") + ") or"));
                if (filterStrInTime.Length > 3)
                {
                    filterStr += " " + filterStrInTime.Substring(0, filterStrInTime.Length - 3);
                }
                else
                {
                    filterStr = filterStr.Substring(0, filterStr.Length - 3);
                }
            }
            else
            {
                filterStr += "and";
                string filterStrInDays = (_listViewDays as ListView).Items.Cast<ListViewItem>().Where(li =>
                    li.Checked).Aggregate(String.Empty, (current, li) => current +
                        (" (" + filterStr + " start >= '" + Convert.ToDateTime(li.Text).AddHours(5) +
                        "' and start <= '" + Convert.ToDateTime(li.Text).AddDays(1).AddHours(5).AddMilliseconds(-1) + 
                        (filtStrChannel.Length > 0 ? "' and " + filtStrChannel : "'") + ") or"));
                if (filterStrInDays.Length > 3)
                {
                    filterStr += " " + filterStrInDays.Substring(0, filterStrInDays.Length - 3);
                }
                else
                {
                    filterStr = filterStr.Substring(0, filterStr.Length - 3);
                }
            }
            if (Owner != null && Owner is MainForm)
            {
                DataRow[] drsSearch = (Owner as MainForm).Programms.Select(filterStr);
                foreach (DataRow drSearch in drsSearch)
                {
                    dtSearch.Rows.Add(drSearch.ItemArray);
                }
                dtSearch.Columns.Add("display-name", typeof (string));
                dtSearch.Columns.Add("image", typeof (Image));
                dtSearch.Columns.Add("day", typeof (string));
                dtSearch.Columns.Add("anons", typeof (Image));
                
                dtSearch.Columns.Add("genre", typeof(Image));
                dtSearch.Columns.Add("rating", typeof (Image));
                dtSearch.Columns.Add("favname", typeof (string));
                dtSearch.Columns.Add("number", typeof(int));
                foreach (DataRow drSearch in dtSearch.Rows)
                {
                    drSearch.BeginEdit();
                    foreach (DataRow drChan in (Owner as MainForm).Channels)
                    {
                        if ((int) drSearch["cid"] == (int) drChan["id"])
                        {
                            drSearch["display-name"] = drChan["user-name"];
                            drSearch["image"] = drChan["icon"];
                            DateTime tsStart = Convert.ToDateTime(drSearch["start"]);
                            drSearch["day"] = tsStart.ToString("ddd", new CultureInfo("ru-Ru")) +
                                              String.Format("({0:D2}.{1:D2})", tsStart.Day, tsStart.Month);
                            drSearch["number"] = drChan["number"];
                        }
                    }
                    if (!String.IsNullOrEmpty(drSearch["desc"].ToString()))
                    {
                        drSearch["anons"] = Resources.GreenAnons;
                    }
                    if (string.IsNullOrEmpty(drSearch["category"].ToString()))
                    {
                        foreach (DataRow drClassifGenre in (Owner as MainForm).Keywords.Rows)
                        {
                            foreach (string strContain in drClassifGenre["contain"].ToString().ToLower().Split(';'))
                            {
                                foreach (
                                    string strNonContain in drClassifGenre["noncontain"].ToString().ToLower().Split(';'))
                                {
                                    if (
                                        (
                                            !string.IsNullOrEmpty(strNonContain) &&
                                            ((drSearch["title"].ToString().ToLower()).Contains(strContain) &&
                                             (!(drSearch["title"].ToString().ToLower()).Contains(strNonContain)))
                                        ) ||
                                        (string.IsNullOrEmpty(strNonContain) &&
                                         (drSearch["title"].ToString().ToLower()).Contains(strContain))
                                        )
                                    {
                                        DataRow[] drsGenre =
                                            (Owner as MainForm).GenresTable.Select("id = " + drClassifGenre["gid"]);
                                        foreach (DataRow drGenre in drsGenre)
                                        {
                                            drSearch["category"] = drGenre["genrename"];
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(drSearch["category"].ToString()))
                    {
                        drSearch["category"] = "Без типа";
                    }
                    DataRow[] drsGenres = (Owner as MainForm).GenresTable.Select("genrename = '" + drSearch["category"].ToString() + "'");
                    foreach (DataRow dataRow in drsGenres)
                    {
                        if ((bool)dataRow["visible"]) drSearch["genre"] = (Image)dataRow["image"];
                    }

                    foreach (DataRow drClassifRating in (Owner as MainForm).Favwords.Rows)
                    {
                        foreach (string strContain in drClassifRating["contain"].ToString().ToLower().Split(';'))
                        {
                            foreach (string strNonContain in
                                drClassifRating["noncontain"].ToString().ToLower().Split(';'))
                            {
                                if (
                                    (
                                        !string.IsNullOrEmpty(strNonContain) &&
                                        ((drSearch["title"].ToString().ToLower()).Contains(strContain) &&
                                         (!(drSearch["title"].ToString().ToLower()).Contains(strNonContain)))
                                    ) ||
                                    (string.IsNullOrEmpty(strNonContain) &&
                                     (drSearch["title"].ToString().ToLower()).Contains(strContain))
                                    )
                                {
                                    DataRow[] drsRating =
                                        (Owner as MainForm).Ratings.Select("id = " + drClassifRating["fid"]);
                                    if (drsRating.Length > 0)
                                    {
                                        foreach (DataRow drFavorite in drsRating)
                                        {
                                            if ((bool)drFavorite["visible"])
                                            {
                                                if ((bool) drClassifRating["remind"])
                                                {
                                                    _tvProgClass.SetRemind(drSearch["cid"].ToString(),
                                                                           drSearch["title"].ToString(),
                                                                           (DateTime) drSearch["start"],
                                                                           (DateTime) drSearch["stop"], true);
                                                    drSearch["remind"] = true;
                                                }
                                                else
                                                {
                                                    _tvProgClass.SetRemind(drSearch["cid"].ToString(),
                                                                           drSearch["title"].ToString(),
                                                                           (DateTime)drSearch["start"],
                                                                           (DateTime)drSearch["stop"], false);
                                                    drSearch["remind"] = false;
                                                }
                                                drSearch["favname"] = drFavorite["favname"];
                                                drSearch["rating"] = drFavorite["image"];
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (String.IsNullOrEmpty(drSearch["rating"].ToString()))
                    {
                        DataRow[] drsWithoutRating = (Owner as MainForm).Ratings.Select("favname = 'Без рейтинга'");
                        foreach (DataRow drFavorite in drsWithoutRating)
                        {
                            if ((bool)drFavorite["visible"])
                            {
                                drSearch["favname"] = "Без рейтинга";
                                drSearch["rating"] = drFavorite["image"];
                            }
                        }
                    }
                    if (String.IsNullOrEmpty(drSearch["remind"].ToString()))
                    {
                        drSearch["remind"] = false;
                    }
                    if (String.IsNullOrEmpty(drSearch["record"].ToString()))
                    {
                        drSearch["record"] = false;
                    }
                    drSearch.EndEdit();
                }
            }
            
           string strFiltGR = "(";
           foreach (ListViewItem li in (_listViewGenre as ListView).Items)
           {
               if (li.Checked)
               {
                   strFiltGR += "category LIKE '" + li.Text + "' or ";
               }
                
           }
           if (strFiltGR.Length > 1)
           {
               strFiltGR = strFiltGR.Substring(0, strFiltGR.Length - 4) + ") and (";
           }
           // Поиск по рейтингам: 
           foreach (ListViewItem li in (_listViewRating as ListView).Items)
           {
               if (li.Checked)
               {
                   strFiltGR += "favname LIKE '" + li.Text + "' or ";
               }
           }
           
           // Поиск по напоминаниям:
           CheckBox chkWithRemind = (ctrlRating.Controls.Find("chkWithRemind", true)[0] as CheckBox);
           CheckBox chkWithoutRemind = (ctrlRating.Controls.Find("chkWithoutRemind", true)[0] as CheckBox);
           if (strFiltGR.Length > 8)
           {
               strFiltGR = strFiltGR.Substring(0, strFiltGR.Length - 4) + 
                   String.Format(") and (remind = {0} or remind = {1})", 
                    chkWithRemind.Checked, !chkWithoutRemind.Checked);
           }
           else
           {
               strFiltGR = String.Format("(remind = {0} or remind = {1})", 
                   chkWithRemind.Checked, !chkWithoutRemind.Checked); 
           }
           if ((Owner as MainForm).Capture != null)
           {
               // Поиск по записям:
               CheckBox chkWithRecord = (ctrlRating.Controls.Find("chkWithRecord", true)[0] as CheckBox);
               CheckBox chkWithoutRecord = (ctrlRating.Controls.Find("chkWithoutRecord", true)[0] as CheckBox);

               if (strFiltGR.Length > 8)
               {
                   strFiltGR = strFiltGR + String.Format(" and (record = {0} or record = {1})",
                                                         chkWithRecord.Checked, !chkWithoutRecord.Checked);
               }
           }
            DataRow[] drsResult = dtSearch.Select(strFiltGR);
            foreach (var drSearch in drsResult)
            {
                drSearch.BeginEdit();
                bool rem = false, rec = false;
                bool.TryParse(drSearch["remind"].ToString(), out rem);
                bool.TryParse(drSearch["record"].ToString(), out rec);
                drSearch["bell"] =  rem ? Resources.bell : Resources.bellempty;
                drSearch["capture"] = rec ? Resources.capture : Resources.capturempty;
                drSearch.EndEdit();
            }
            if (drsResult.Length > 0)
            {
                dtSearch = drsResult.CopyToDataTable();
            }
            else dtSearch.Rows.Clear();
            this.Cursor = Cursors.Default;
            return dtSearch;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string xmlOptionsFileName = Path.Combine(Application.StartupPath, Preferences.xmlSearchOptionsFile);
            DataTable confSearchTable = _dsConfig.Tables["SearchSettings"];
            DataRow row = confSearchTable.Rows[0];
            if (chkSaveParamSearch.Checked)
            {
                row["LoadSettings"] = true;
                row["Match"] = Match;
                row["DontMatch"] = DontMatch;
                row["InAnons"] = InAnons;
                row["InTime"] = InTime;
                row["TsFinalFrom"] = TsFinalFrom.Ticks;
                row["TsFinalTo"] = TsFinalTo.Ticks;
                row["TrackBarValueFrom"] = TrackBarValueFrom;
                row["TrackBarValueTo"] = TrackBarValueTo;
            }
            else row["LoadSettings"] = false;
            _dsConfig.WriteXml(xmlOptionsFileName);
            btnOK.DialogResult = DialogResult.OK;
            _dtSearch = AcceptFilter(Match);
            if (_dtSearch.Rows.Count  == 0)
            {
               if (Statics.ShowDialog(Resources.ConfirmChoiceText, Resources.NoFoundText, 
                    MessageDialog.MessageIcon.Question, MessageDialog.MessageButtons.YesNo) == DialogResult.Yes)
               {
                   
                   this.DialogResult = DialogResult.None;
               }
            }
            else
            {
                SearchResultsForm searchResultsForm = new SearchResultsForm(_tvProgClass, _dtSearch);
                searchResultsForm.ShowDialog(Owner as MainForm);   
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

       
    }
}
