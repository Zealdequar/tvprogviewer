using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TVProgViewer.WebUI.MainServiceReferences;
using TVProgViewer.BusinessLogic.Users;

namespace TVProgViewer.WebUI.Abstract
{
    public interface IUsersRepository
    {
        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        /// <param name="userName">Логин (имя пользователя)</param>
        /// <param name="passHash">Хеш пароля</param>
        /// <param name="passExtend">Хеш соли</param>
        /// <param name="lastName">Фамилия</param>
        /// <param name="firstName">Имя</param>
        /// <param name="middleName">Отчество</param>
        /// <param name="birthDate">Дата рождения</param>
        /// <param name="gender">Пол</param>
        /// <param name="email">Адрес электронной почты</param>
        /// <param name="mobPhone">Номер моб. телефона</param>
        /// <param name="otherPhone1">Доп. номер 1</param>
        /// <param name="otherPhone2">Доп. номер 2</param>
        /// <param name="address">Адрес</param>
        /// <param name="gmtZone">Часовой пояс</param>
        Task<int> UserRegister(string userName, string passHash, string passExtend, string lastName,
            string firstName, string middleName, DateTime birthDate, bool? gender, string email, string mobPhone,
            string otherPhone1, string otherPhone2, string address, string gmtZone);

        /// <summary>
        /// Получение хешей
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        Task<SecureData> GetHashes(string userName);

        /// <summary>
        /// Получение пользователя
        /// </summary>
        /// <param name="uid">Код пользователя</param>
        /// <param name="errCode">Код ошибки</param>
        Task<User> GetUser(long uid, out int errCode);

        /// <summary>
        /// Установка каталога
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        void UpdateCatalog(long uid);
    }
}
