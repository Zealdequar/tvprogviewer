using System.Runtime.Serialization;

namespace TVProgViewer.Core.Domain.Users
{
    /// <summary>
    /// Данные безопасности
    /// </summary>
    [DataContract]
    public class SecureData
    {
        #region Конструктор

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="passHash">Хеш пароля</param>
        /// <param name="passExtend">Соль пароля</param>
        public SecureData(long uid, string passHash, string passExtend)
        {
            UID = uid;
            PassHash = passHash;
            PassExtend = passExtend;
        }

        #endregion

        #region Автосвойства

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [DataMember]
        public long UID { get; private set; }
        
        /// <summary>
        /// Хеш пароля
        /// </summary>
        [DataMember]
        public string PassHash {get; private set;}
        
        /// <summary>
        /// Соль пароля
        /// </summary>
        [DataMember]
        public string PassExtend { get; private set; }

        #endregion
    }
}
