using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TVProgViewer.Core.Domain.Users;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using TVProgViewer.DataAccess.Models;

namespace TVProgViewer.DataAccess.Adapters
{
    /// <summary>
    /// Адаптер для работы с пользователями
    /// </summary>
    public class UsersAdapter: AdapterBase
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Заведение пользователя
        /// </summary>
        /// <param name="username">Логин</param>
        /// <param name="hash">Хеш пароля</param>
        /// <param name="extendHash">Хеш соли</param>
        /// <param name="lastName">Фамилия</param>
        /// <param name="firstName">Имя</param>
        /// <param name="middleName">Отчество</param>
        /// <param name="birthDate">Дата рождения</param>
        /// <param name="gender">Пол</param>
        /// <param name="email">Адрес электронной почты</param>
        /// <param name="mobPhone">Моб. телефон</param>
        /// <param name="otherPhone1">Доп. телефон 1</param>
        /// <param name="otherPhone2">Доп. телефон 2</param>
        /// <param name="address">Адрес</param>
        /// <param name="gmtZone">Часовой пояс</param>
        public int UserStart(
            string username,
            string hash,
            string extendHash,
            string lastName,
            string firstName,
            string middleName,
            DateTime birthDate,
            bool? gender,
            string email,
            string mobPhone,
            string otherPhone1,
            string otherPhone2,
            string address,
            string gmtZone
            )
        {
            
            int result = 0;
            object genderObj ;
            if (gender == null)
                genderObj = DBNull.Value;
            else genderObj = gender;
            object gmtZoneObj;
            if (gmtZone == null)
                gmtZoneObj = DBNull.Value;
            else gmtZoneObj = gmtZone;
            List<SqlParameter> pars = new List<SqlParameter>()
            {
                new SqlParameter("@UserName", SqlDbType.NVarChar, 70){Value = username},
                new SqlParameter("@PassHash", SqlDbType.NVarChar, 100) {Value = hash},
                new SqlParameter("@PassExtend", SqlDbType.NVarChar, 100) {Value = extendHash},
                new SqlParameter("@LastName", SqlDbType.NVarChar,   150) {Value = lastName},
                new SqlParameter("@FirstName", SqlDbType.NVarChar,  150) {Value = firstName},
                new SqlParameter("@MiddleName", SqlDbType.NVarChar, 150) {Value = middleName},
                new SqlParameter("@BirthDate", SqlDbType.DateTime) {Value = birthDate},
                new SqlParameter("@Gender", SqlDbType.Bit) {Value = genderObj, IsNullable=true},
                new SqlParameter("@Email", SqlDbType.NVarChar, 300) {Value = email},
                new SqlParameter("@MobPhoneNumber", SqlDbType.NVarChar, 25) {Value = mobPhone},
                new SqlParameter("@OtherPhoneNumber1", SqlDbType.NVarChar, 25) {Value = otherPhone1},
                new SqlParameter("@OtherPhoneNumber2", SqlDbType.NVarChar, 25) {Value = otherPhone2},
                new SqlParameter("@Address", SqlDbType.NVarChar, 1000) {Value = address},
                new SqlParameter("@GmtZone", SqlDbType.NVarChar, 10) {Value = gmtZoneObj, IsNullable=true},
                new SqlParameter("@Catalog", SqlDbType.NChar, 36) {Value = Guid.NewGuid().ToString().ToUpper() }
            };
            SqlParameter outRes = new SqlParameter("@ErrCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars.Add(outRes);
            Logger.Debug("Старт заведения пользователя");
          //  da.ExecCommand(GetTvProgSecureConnection(), "spUserStart", pars.Cast<DbParameter>().ToList<DbParameter>());
            result = (outRes.Value != DBNull.Value) ? int.Parse(outRes.Value.ToString()) : -1;
            Logger.Debug("result = " + result);
            return result;
        }
        
        /// <summary>
        /// Получение хеша
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        public SecureData GetHashesByUserName(string username)
        {

            SystemUsers su = dataContext.SystemUsers.SingleOrDefault(x => x.UserName == username);
            if (su != null)
            {
                SecureData sd = new SecureData(su.UserId, su.PassHash, su.PassExtend);
                return sd;
            }
            return null;
        }

        /// <summary>
        /// Изменение логина (имени пользователя)
        /// </summary>
        /// <param name="oldUserName">Старое имя пользователя</param>
        /// <param name="newUserName">Новое имя пользователя</param>
        public int ChangeUserName(string oldUserName, string newUserName)
        {
            int result = 0;
            if (oldUserName == newUserName)
                return 1;
            if (dataContext.SystemUsers.AsNoTracking(). SingleOrDefault(x => x.UserName == newUserName) != null)
                return 2;

            if (dataContext.SystemUsers.AsNoTracking().SingleOrDefault(x => x.UserName == oldUserName && !(x.Status == 6 || (x.DateBegin < DateTime.Now && DateTime.Now < x.DateEnd))) != null)
                return 5;
            try
            {
                SystemUsers su = dataContext.SystemUsers.SingleOrDefault(x => x.UserName == oldUserName);
                if (su != null)
                {
                    su.UserName = newUserName;
                    dataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        /// <summary>
        /// Изменить пароль
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="newHashExtend">Новый хеш соли</param>
        /// <param name="newHash">Новый хеш пароля</param>
        public int ChangeHashes(string userName, string newHashExtend, string newHash)
        {
            int result = 0;

            if (dataContext.SystemUsers.SingleOrDefault(x=> x.UserName == userName) == null)
            {
                return 2;
            }

            if (dataContext.SystemUsers.SingleOrDefault(x => x.UserName == userName && (x.Status == 6 || !(x.DateBegin < DateTime.Now && DateTime.Now < x.DateEnd))) != null)
            {
                return 5;
            }
            try
            {
                SystemUsers su = dataContext.SystemUsers.SingleOrDefault(x => x.UserName == userName);
                if (su != null)
                {
                    su.PassExtend = newHashExtend;
                    su.PassHash = newHash;
                    dataContext.SaveChanges();
                }
            }
            catch(Exception ex)
            {
            }

            return result;
        }

        /// <summary>
        /// Смена контактных данных
        /// </summary>
        /// <param name="userName">Логин (Имя пользователя)</param>
        /// <param name="lastName">Фамилия</param>
        /// <param name="firstName">Имя</param>
        /// <param name="middleName">Отчество</param>
        /// <param name="birthDate">День рождения</param>
        /// <param name="gender">Пол</param>
        /// <param name="email">Адрес электронной почты</param>
        /// <param name="mobPhone">Моб. телефон</param>
        /// <param name="otherPhone1">Доп. телефон 1</param>
        /// <param name="otherPhone2">Доп. телефон 2</param>
        /// <param name="address">Адрес</param>
        /// <param name="gmtZone">Часовой пояс</param>
        public int ChangeContacts(
            long uid,
            string lastName,
            string firstName,
            string middleName,
            DateTime birthDate,
            bool? gender,
            string email,
            string mobPhone,
            string otherPhone1,
            string otherPhone2,
            string address,
            string gmtZone)
        {
            int result = 0;
            if (dataContext.SystemUsers.SingleOrDefault(x => x.UserId != uid && x.Email.ToUpper() == email.ToUpper()) != null)
                return 3;

            if (dataContext.SystemUsers.SingleOrDefault(x => x.UserId == uid && (x.Status == 6 || !(x.DateBegin < DateTime.Now && DateTime.Now < x.DateEnd))) != null)
                return 5;

            try
            {
                SystemUsers su = dataContext.SystemUsers.SingleOrDefault(x => x.UserId == uid);
                if (su != null)
                {
                    su.LastName = lastName;
                    su.FirstName = firstName;
                    su.MiddleName = middleName;
                    su.BirthDate = birthDate;
                    su.Gender = gender;
                    su.Email = email;
                    su.MobPhoneNumber = mobPhone;
                    su.OtherPhoneNumber1 = otherPhone1;
                    su.OtherPhoneNumber2 = otherPhone2;
                    su.Address = address;
                    su.GmtZone = gmtZone;
                    dataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        /// <summary>
        /// Блокирование пользователя по статусу
        /// </summary>
        /// <param name="uid">Уникальный идентификатор пользователя</param>
        public void BlockUser(long uid)
        {
            try
            {
                SystemUsers su = dataContext.SystemUsers.SingleOrDefault(x => x.UserId == uid);
                if (su != null)
                {
                    su.Status = 6;
                    dataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Блокирование пользователем навечно
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        public void BlockUserOwn(long uid)
        {
            try
            {
                SystemUsers su = dataContext.SystemUsers.SingleOrDefault(x => x.UserId == uid);
                if (su != null)
                {
                    su.DateBegin = new DateTime(2900, 1, 1);
                    su.DateEnd = new DateTime(1, 1, 1);
                    dataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
            }
        }
        
        /// <summary>
        /// Блокирование пользователем до определенной даты
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="endBlockDate">Дата, после которой пользователь разблокируется</param>
        public void BlockUserToDate(long uid, DateTime endBlockDate)
        {
            try
            {
                SystemUsers su = dataContext.SystemUsers.SingleOrDefault(x => x.UserId == uid);
                if (su != null)
                {
                    su.DateEnd = endBlockDate;
                    dataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Получение пользователя
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="errCode">Код ошибки</param>
        public User GetUserByUserName(string userName)
        {
            
            try
            {
                SystemUsers su = dataContext.SystemUsers.AsNoTracking().SingleOrDefault(x => x.UserName == userName);
                if (su != null)
                {
                    User user = new User()
                    {
                        Id = su.UserId,
                        Username = su.UserName,
                        LastName = su.LastName,
                        FirstName = su.FirstName,
                        MiddleName = su.MiddleName,
                        BirthDate = su.BirthDate,
                        Gender = su.Gender,
                        Email = su.Email,
                        MobilePhone = su.MobPhoneNumber,
                        Address = su.Address,
                        GmtZone = su.GmtZone,
                        Status = su.Status,
                        Catalog = su.Catalog
                    };
                    return user;
                }
            }
            catch (Exception ex)
            {
            }
            
            return null;
        }

        /// <summary>
        /// Разблокировка пользователя
        /// </summary>
        /// <param name="userName">Логин (имя пользователя)</param>
        public void UnblockUser(string userName)
        {
            try
            {
                SystemUsers su = dataContext.SystemUsers.SingleOrDefault(x => x.UserName == userName);
                if (su != null)
                {
                    su.DateBegin = new DateTime(1, 1, 1);
                    su.DateEnd = new DateTime(2900, 1, 1);
                    su.Status = 3;
                    dataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Установка названия каталога
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        public void UpdateCatalog(long uid)
        {
            foreach (SystemUsers user in dataContext.SystemUsers)
            {
                user.Catalog = Guid.NewGuid().ToString().ToUpper();
            }
            dataContext.SaveChanges();
        }
    }
}