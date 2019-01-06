using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TVProgViewer.BusinessLogic.Users;
using TVProgViewer.WebUI.Abstract;

namespace TVProgViewer.WebUI.Concrete
{
    public class EFUserRepository : BaseEFRepository, IUsersRepository
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
        public Task<int> UserRegister(string userName, string passHash, string passExtend, string lastName, string firstName, string middleName, DateTime birthDate, bool? gender, string email, string mobPhone, string otherPhone1, string otherPhone2, string address, string gmtZone)
        {
            return Task<int>.Factory.StartNew(() => { return TvProgService.AddUser(userName, passHash, passExtend, lastName, firstName, middleName, birthDate, gender, email, mobPhone, otherPhone1, otherPhone2, address, gmtZone); });
        }

        /// <summary>
        /// Получение хешей
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        public Task<SecureData> GetHashes(string userName)
        {
            return Task<SecureData>.Factory.StartNew(() => { return TvProgService.GetHashes(userName); });
        }

        /// <summary>
        /// Получение пользователя
        /// </summary>
        /// <param name="uid">Код пользователя</param>
        /// <param name="errCode">Код ошибки</param>
        public Task<User> GetUser(long uid, out int errCode)
        {
            int tempErr = 0;
            Task<User> task = Task<User>.Factory.StartNew(() => 
            {
                int err;
                User user = TvProgService.GetUser(uid, out err);
                tempErr = err;
                return user;
            });
            errCode = tempErr;
            return task;
        }

        /// <summary>
        /// Установка каталога
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        public void UpdateCatalog(long uid)
        {
            TvProgService.UpdateCatalog(uid);   
        }
    }
}