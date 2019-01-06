using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace TVProgViewer.TVProgApp
{
    public class Genres
    {
        private DataTable _genresTable = new DataTable("Genres");
        private DataTable _classifTable = new DataTable("KeywordsTable");
        public DataTable GenresTable
        {
            get { return _genresTable; }
        }
        public DataTable ClassifTable
        {
            get { return _classifTable; }
            set { _classifTable = value; }
        }

        public Genres(DataTable genresTable)
        {
            _genresTable = genresTable;
            string xmlPath = Path.Combine(Application.StartupPath, Preferences.xmlClassifGenres);
            if (!File.Exists(xmlPath))
            {
                _classifTable.Columns.Clear();
                _classifTable.Columns.Add("id", typeof (int));
                _classifTable.Columns["id"].AutoIncrement = true;
                _classifTable.Columns.Add("gid", typeof(int));
                _classifTable.Columns.Add("contain", typeof (string));
                _classifTable.Columns.Add("noncontain", typeof (string));
                _classifTable.Columns.Add("deleteafter", typeof (DateTime));
                _classifTable.Columns.Add("prior", typeof(int));
                foreach (DataRow drGenre in _genresTable.Rows)
                {
                    switch ((int) drGenre["id"])
                    {
                        case 1:
                            _classifTable.Rows.Add(null, drGenre["id"], "фильм", "мульт;док;Д/ф", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "х.ф", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "х. ф", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "т. ф", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "драм", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "х/ф", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "х\\ф", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Режиссер", "", DBNull.Value);
                            break;
                        case 2:
                            _classifTable.Rows.Add(null, drGenre["id"], "т/с", "м/с", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Теленовелла", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Сери", "М/;мульт;турнир", DBNull.Value);
                            break;
                        case 3:
                            _classifTable.Rows.Add(null, drGenre["id"], "Детям", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "детск", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "дете", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "сказка", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Спокойной ночи, малыши", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "До 16", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Ералаш", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "АБВГДейка", "", DBNull.Value);
                            break;
                        case 4:
                            _classifTable.Rows.Add(null, drGenre["id"], "чемпион","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "гран-при","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "турнир","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "первенство","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Футбол","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Кубок","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "атлет","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "стадион","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "хоккей","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "спартак","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Физра","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Бои без правил","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Пенальти","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Трюкачи","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "НХЛ","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "матч","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "волейбол","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "бокс","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "НБА","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "ринг","оринг", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "рекорд","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "физкульт","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "баскетбол","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "корт","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "ралли","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "гонки","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "олимп","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "финал", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Боевые искус", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Спорт", "", DBNull.Value);
                            break;
                        case 5:
                            _classifTable.Rows.Add(null, drGenre["id"], "Док.", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "д. ф", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "документ", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "репор", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "т. ж.", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Д/ф", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Д/с", "", DBNull.Value);
                            break;                  
                        case 6:
                            _classifTable.Rows.Add(null, drGenre["id"], "информац", "тайна", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Новост", "в перерыве", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Время", "муз;личное", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Вести", "палаты", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Итоговая программа", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Сегодня", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "хроник", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "События", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Афиша", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "News", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Настроен", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Доброе утро", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "День за днем", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Доска", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "24", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Неделя", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Личный вклад", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Постскриптум", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Страна и мир", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "ньюс", "", DBNull.Value);
                            break;
                        case 7:
                            _classifTable.Rows.Add(null, drGenre["id"], "комеди", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "юмор", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "КВН", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Клуб Веселых и Находчивых", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Городок;городке", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Жванецкий", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Задорнов", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Смех", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Аншлаг", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "О. С. П.", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Большая разница", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Прожекторперисхилтон", "", new DateTime(2014, 1, 1));
                            _classifTable.Rows.Add(null, drGenre["id"], "Каламбур", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "фитиль", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "шутка", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "розыгрыш", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Смешн", "", DBNull.Value);
                            break;
                        case 8:
                            _classifTable.Rows.Add(null, drGenre["id"], "Шоу", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "конкурс", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Сам себе режиссер", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Модный приговор", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Давай поженимся!", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Пусть говорят", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Дом 2", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Час суда", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "НТВшники", "", DBNull.Value);
                            break; 

                        case 9:
                            _classifTable.Rows.Add(null, drGenre["id"], "игра", "гармонь;тигра;концерт", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Поле чудес", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "лото", "олото", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Умницы и умники", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Десять миллионов", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Кто хочет стать миллионером", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Что? Где? Когда?", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Своя игра", "", DBNull.Value);
                            break;
                        case 10:
                            _classifTable.Rows.Add(null, drGenre["id"], "артист", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "театр", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Опера", "операб;операц", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "спектакл", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "балет", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "актер", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "кино", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Stop! Снято!", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Третий звонок", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Большой экран", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "сп.", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "кумиры", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Синемания", "", DBNull.Value);
                            break;
                        case 11:
                            _classifTable.Rows.Add(null, drGenre["id"], "М/", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "М. ф", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "М/ сериал", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Дисней", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "мульт", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Футурама", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Симпсоны", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Fox Kids", "", DBNull.Value);
                            break;
                        case 12: 
                            _classifTable.Rows.Add(null, drGenre["id"], "детектив", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "комиссар", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Боевик", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "ужас", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "триллер", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "криминал", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Дорожный патруль", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Дорожная часть", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Чрезвычайное происшествие", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Петровка", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Телеспецназ", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "следствие", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "расследование", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "убийств", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Чистосердечное признание", "", DBNull.Value);
                            break;                 
                        case 13:
                            _classifTable.Rows.Add(null, drGenre["id"], "Муз", "музе", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Концерт", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Диск", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "фестивал", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "песн", "чудес", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "поют", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "джаз", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "БКЗ", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "гармонь", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Утренняя почта", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Два рояля", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "оркестр", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "хит", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "романс", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "клип", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "шансон", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Танцпол", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Русская 10-ка", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Мобильная 10-ка", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Танцкласс", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Зажигай!", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Extra", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Impulse", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "чарт", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Горячая десятка", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Утренняя звезда", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "10 наших", "", DBNull.Value);
                        break;                      
                        case 14:                   
                            _classifTable.Rows.Add(null, drGenre["id"], "Интернет", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], ".Ru", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Глобаль", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Всемирная паутина", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "сеть", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "www", "", DBNull.Value);
                            break;                 
                        case 15:                  
                            _classifTable.Rows.Add(null, drGenre["id"], "животн", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "зоо", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "звер", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "природ", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "я и моя собака", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "диалоги о рыбалке", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Рыболов", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "сами с усами", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "динозавр", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "пород", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "ветерин", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "хищник", "", DBNull.Value);
                            break;             
                        case 16:                   
                            _classifTable.Rows.Add(null, drGenre["id"], "Погод", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Солнечный прогноз", "", DBNull.Value);
                            break;                 
                        case 17:                  
                            _classifTable.Rows.Add(null, drGenre["id"], "искусств", "боев", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "музе", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "культур", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "интерьер", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Артэкспресс", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Архтектур", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Дворцовые тайны", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Царская ложа", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "эрмитаж", "", DBNull.Value);
                            break;                  
                        case 18:                   
                            _classifTable.Rows.Add(null, drGenre["id"], "полити", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Международ", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Съезд", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Федерация", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Парламентский", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "зеркало", "Петросян", DBNull.Value);
                            break;                  
                        case 19:                   
                            _classifTable.Rows.Add(null, drGenre["id"], "Xфактор", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Цивилизац", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Наук", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Экологи", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Космос", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "тайна", "", DBNull.Value);
                            break;                  
                        case 20:                   
                            _classifTable.Rows.Add(null, drGenre["id"], "город","", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Путе", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Непутевые заметки", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Хочу всё знать", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Турбюро", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Знаменитые замки", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Планета Земля", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Вокруг света", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Вояж без саквояжа", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Чудес", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Приключени", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Гербы России", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Окно в мир", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "История", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Поля сражений", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Их нравы", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "экспедиц", "", DBNull.Value);
                            break;                 
                        case 21:                   
                            _classifTable.Rows.Add(null, drGenre["id"], "в гостях", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Антропология", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Пока все дома", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Голливуд", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Женский взгляд", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Встреча с", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Жизнь замечательных людей", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "мужчина и женщина", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "завтрак", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Без протокола", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Момент истины", "", DBNull.Value);
                            break;                  
                        case 22:                   
                            _classifTable.Rows.Add(null, drGenre["id"], "компьютер", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Авиац", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "изобрет", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "монитор", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "PRO-новости", "", DBNull.Value);
                            break;                
                        case 23:
                            _classifTable.Rows.Add(null, drGenre["id"], "мода", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "моды", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "fashion", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Мир красоты", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Шпилька", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Видеомода", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Стильные штучки", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Стилиссимо", "", DBNull.Value);
                            break;                  
                        case 24:                  
                            _classifTable.Rows.Add(null, drGenre["id"], "Авто;мото", "Автор;стаи", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Карданный вал", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Фаркоп", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Машин", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Top Gear", "", DBNull.Value);
                            break;
                        case 25:
                            _classifTable.Rows.Add(null, drGenre["id"], "здоров", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Без рецепта", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "доктор", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "мед.", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "медиц", "", DBNull.Value);
                            break;                  
                        case 26:                   
                            _classifTable.Rows.Add(null, drGenre["id"], "пастыря", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "религ", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Православ", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Победоносный голос верующего", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Библ", "мания", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Из библейсиких пророчеств", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Благая весть", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Коран", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Воскресная школа", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "христ", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Патриарх", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Муфтий", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Час силы духа", "", DBNull.Value);
                            break;                  
                        case 27:                    
                            _classifTable.Rows.Add(null, drGenre["id"], "сад", "кольцо" , DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Растительная жизнь", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "урожай", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "даче", "передаче", DBNull.Value);
                            break;                  
                        case 28:                  
                            _classifTable.Rows.Add(null, drGenre["id"], "магазин", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "реклам", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "телепокупка", "", DBNull.Value);
                            break;                  
                        case 29:                   
                            _classifTable.Rows.Add(null, drGenre["id"], "Из жизни женщины", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Дамский клуб", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Женские истории", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Для женщин", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "мама;мать", "", DBNull.Value);
                            break;                 
                        case 30:                  
                            _classifTable.Rows.Add(null, drGenre["id"], "Кухня", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Вкусные истории", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Сладкие истории", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Вкус", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "деликатес", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "кулинар", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "смак", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Лакомый кусочек", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "блюд", "", DBNull.Value);
                            break;                
                        case 31:                   
                            _classifTable.Rows.Add(null, drGenre["id"], "экстр", "машин", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Служба спасения", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Катастрофы недели", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Дорожные войны", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "каскаде", "", DBNull.Value);
                            break;                  
                        case 32:                   
                            _classifTable.Rows.Add(null, drGenre["id"], "Деньги", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Бизнес", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Инвестиции", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Финанс", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Делов", "", DBNull.Value);
                            break;
                        case 33:
                            _classifTable.Rows.Add(null, drGenre["id"], "Секс", "", DBNull.Value);
                            _classifTable.Rows.Add(null, drGenre["id"], "Эрот", "", DBNull.Value);
                            break;
                    }
                }
                _classifTable.Rows.Add(null, 32, "Рынки", "", DBNull.Value);
                foreach (DataRow drClassifGenre in _classifTable.Rows)
                {
                    drClassifGenre["prior"] = _classifTable.Rows.IndexOf(drClassifGenre);
                }
                DataTable xmlWriteTable = _classifTable.Copy();
                xmlWriteTable.Columns.Remove("id");
                xmlWriteTable.WriteXml(xmlPath);
            }
            else
            {
                _classifTable.Columns.Clear();
                _classifTable.Columns.Add("id", typeof (int));
                _classifTable.Columns["id"].AutoIncrement = true;
                _classifTable.Columns.Add("gid", typeof (int));
                _classifTable.Columns.Add("contain", typeof (string));
                _classifTable.Columns.Add("noncontain", typeof (string));
                _classifTable.Columns.Add("deleteafter", typeof (DateTime));
                _classifTable.Columns.Add("prior", typeof (int));
                // Set the reader settings.
                DataSet dsClassif = new DataSet();
                dsClassif.ReadXml(xmlPath);
                if (dsClassif.Tables.Count > 0)
                {
                    if (dsClassif.Tables[0] != null)
                    {
                        foreach (DataRow dataRow in dsClassif.Tables[0].Rows)
                        {

                            _classifTable.Rows.Add(null, dataRow["gid"] ?? 0,
                                                   dataRow["contain"] ?? "",
                                                   dataRow["noncontain"] ?? "",
                                                   dataRow.Table.Columns.Contains("deleteafter")
                                                       ? dataRow["deleteafter"]
                                                       : null,
                                                   dataRow.Table.Columns.Contains("prior")
                                                       ? dataRow["prior"]
                                                       : _classifTable.Rows.IndexOf(dataRow))
                                ;
                        }
                    }
                }
            }

            bool pr = false;
            for (int i = 0; i <= _classifTable.Rows.Count - 1; i++)
            {
                if (!String.IsNullOrEmpty(_classifTable.Rows[i]["deleteafter"].ToString()))
                {
                    DateTime tsDeleteAfter = (DateTime) _classifTable.Rows[i]["deleteafter"];
                    if (tsDeleteAfter.Year >= 2000 && tsDeleteAfter < DateTime.Now)
                    {
                        _classifTable.Rows[i].Delete();
                        pr = true;
                    }
                }
            }
            if (pr)
            {
                DataTable xmlTableToWrite = _classifTable.Copy();
                xmlTableToWrite.Columns.Remove("id");
                xmlTableToWrite.WriteXml(xmlPath);
            }
            if (!_classifTable.Columns.Contains("image"))
            {
                _classifTable.Columns.Add("image", typeof(Image));
            }
            foreach (DataRow drGenre in _genresTable.Rows)
            {
                foreach(DataRow drClassif in _classifTable.Rows)
                {
                    if (drGenre["id"].ToString() == drClassif["gid"].ToString())
                    {
                        drClassif["image"] = drGenre["image"];
                    }
                }
            }
        }
    }
}
