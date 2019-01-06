using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Web.Mvc;

namespace TVProgViewer.BusinessLogic.Users
{
    /// <summary>
    /// Пользователь
    /// </summary>
    [DataContract]
    public class User 
    {
        #region Конструктор

        public User()
        {
            this.AvailableGenders = new List<SelectListItem>();
            this.AvailableGenders.Clear();
            this.AvailableGenders.Add(new SelectListItem() { Text = "", Value = null });
            this.AvailableGenders.Add(new SelectListItem() { Text = "Женский", Value = "false" });
            this.AvailableGenders.Add(new SelectListItem() { Text = "Мужской", Value = "true" });
        }

        #endregion

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        [DataMember]
        public long UserID { get; set; }

        [DisplayName("Имя пользователя (логин)")]
        [Required(ErrorMessage = "Укажите логин")]
        [DataMember]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Укажите пароль")]
        [DisplayName("Пароль")]
        public string Pass { get; set; }

        [Required(ErrorMessage = "Укажите подтверждение пароля")]
        [DisplayName("Подтверждение пароля")]
        public string PassRepeate { get; set; }

        [Required(ErrorMessage = "Укажите фамилию")]
        [DisplayName("Фамилия")]
        [DataMember]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Укажите имя")]
        [DisplayName("Имя")]
        [DataMember]
        public string FirstName { get; set; }
        
        [DisplayName("Отчество (если есть)")]
        [DataMember]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Укажите дату рождения")]
        [DisplayName("Дата рождения")]
        [DataMember]
        public DateTime BirthDate { get; set; }

        [DisplayName("Пол")]
        [DataMember]
        public bool? Gender { get; set; }

        public IList<SelectListItem> AvailableGenders { get; set; }

        [Required(ErrorMessage ="Укажите адрес электронной почты")]
        [DisplayName("Адрес электронной почты")]
        [DataMember]
        public string Email { get; set; }

        [Required(ErrorMessage ="Укажите номер своего мобильного телефона")]
        [DisplayName("Номер мобильного телефона")]
        [DataMember]
        public string MobilePhone { get; set; }

        [DisplayName("Доп. номер телефона 1")]
        [DataMember]
        public string OtherPhone1 { get; set; }

        [DisplayName("Доп. номер телефона 2")]
        [DataMember]
        public string OtherPhone2 { get; set; }

        [DisplayName("Адрес")]
        [DataMember]
        public string Address { get; set; }

        [DisplayName("GMT Зона")]
        [DataMember]
        public string GmtZone { get; set; }

        [DataMember]
        public short Status { get; set; }

        [DataMember]
        public string Catalog { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", LastName, FirstName, MiddleName);
        }
    }
}
