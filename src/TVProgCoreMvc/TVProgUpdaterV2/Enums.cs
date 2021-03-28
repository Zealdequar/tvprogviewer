using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVProgViewer.TVProgUpdaterV2
{
    public class Enums
    {
        public static readonly Dictionary<int, KeyValuePair<string,string>> ErrorsWorkWithUsers =
            new Dictionary<int, KeyValuePair<string,string>>()
            {
                {1, new KeyValuePair<string,string>("Введенный логин не отличается от предшествующего. Укажите другой логин.", "Логин не отличается от предшествующего.")},
                {2, new KeyValuePair<string,string>("Такое имя пользователя уже существует. Укажите другое имя пользователя.", "Такой же логин найден в системе")},
                {3, new KeyValuePair<string,string>("Пользователь с таким e-mail уже зарегистрирован в системе. Укажите другой e-mail.", "Такой же e-mail найден в системе")},
                {5, new KeyValuePair<string,string>("Ваша учётная запись заблокирована.", "Пользователь заблокирован в системе") },
                {70, new KeyValuePair<string,string>("Произошла ошибка при регистрации в системе. Попробуйте совершить регистрацию позднее.", "Ошибка вставки учетной записи")},
                 // Непредвиденная ошибка, связанная c выполнением команды:
                {71, new KeyValuePair<string,string>("Сервис временно недоступен. Попробуйте воспользоваться сервисом позднее.", "Ошибка при обновлении контактных данных учетной записи.")}, 
                {72, new KeyValuePair<string,string>("Произошла ошибка при инициализации данных регистрации в системе. Попробуйте совершить регистрацию позднее.", "Ошибка при вставке жанров или классов жанров")}
            };
        public static readonly Dictionary<int, KeyValuePair<string, string>> ErrorsWorkWithChannels =
            new Dictionary<int, KeyValuePair<string, string>>()
            {
                {73, new KeyValuePair<string,string>("Сервис временно недоступен. Попробуйте воспользоваться сервисом позднее.", "Ошибка при вставке телевизионного канала.")},
                {74, new KeyValuePair<string,string>("Сервис временно недоступен. Попробуйте воспользоваться сервисом позднее.", "Ошибка обновления скалярных данных телевизионного канала.")},
                {75, new KeyValuePair<string,string>("Сервис временно недоступен. Попробуйте воспользоваться сервисом позднее.", "Ошибка обновления пиктограммы телевизионного канала.")}
            };

        public enum TypeProg
        {
            XMLTV,
            InterTV
        }
    }
}
