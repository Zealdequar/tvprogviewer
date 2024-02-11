using System;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Tax;

namespace TvProgViewer.Core.Domain.Users
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public partial class User : BaseEntity, ISoftDeletedEntity
    {
        public User()
        {
            UserGuid = Guid.NewGuid();
        }

        /// <summary>
        /// Gets or sets the user GUID
        /// </summary>
        public Guid UserGuid { get; set; }

        /// <summary>
        /// Имя пользователя (логин)
        /// </summary>
        public string Username { get; set; }
		
		/// <summary>
        /// Согласие на обработку персональных данных
        /// </summary>
        //[MustBeTrue(ErrorMessage = "Для регистрации необходимо согласиться с условиями обработки персональных данных")]
        //[DisplayName("Я согласен(а) на обработку моих персональных данных. В соответствии со статьей 9 ФЗ-152 от 27 июля 2006 года «О персональных данных», даю согласие на обработку в документальной и/или электронной форме моих персональных данных сайту TvProgViewer.Ru")]
        public bool PersonalDataAggree { get; set; }
        
        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; set; }
		
		/// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
		
		/// <summary>
        /// Отчество (если есть)
        /// </summary>
        public string MiddleName { get; set; }
		
		/// Дата рождения
        /// </summary>
        public DateTime? BirthDate { get; set; }
		
        /// <summary>
        /// Пол
        /// </summary>
        public string Gender { get; set; }
		
		/// <summary>
        /// Адрес электронной почты
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the company
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Улица
        /// </summary>
        public string StreetAddress { get; set; }

        /// <summary>
        /// Улица 2
        /// </summary>
        public string StreetAddress2 { get; set; }

        /// <summary>
        /// Почтовый индекс
        /// </summary>
        public string ZipPostalCode { get; set; }

        /// <summary>
        /// Город
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Страна
        /// </summary>
        public string County { get; set; }

        /// <summary>
        /// Gets or sets the country id
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// Gets or sets the state province id
        /// </summary>
        public int StateProvinceId { get; set; }

        /// <summary>
        /// Gets or sets the phone number
        /// </summary>
        public string SmartPhone { get; set; }

        /// <summary>
        /// Статус пользователя
        /// </summary>
        public short Status { get; set; }

        /// <summary>
        /// Индивидуальный каталог для хранения изображений
        /// </summary>
        public string Catalog { get; set; }

		/// <summary>
        /// Полные Фамилия Имя Отчество
        /// </summary>
        public override string ToString() => $"{LastName} {FirstName} {MiddleName}";
        
        /// <summary>
        /// Gets or sets the fax
        /// </summary>
        public string Fax { get; set; }

        /// <summary>
        /// Gets or sets the vat number
        /// </summary>
        public string VatNumber { get; set; }

        /// <summary>
        /// Gets or sets the vat number status id
        /// </summary>
        public int VatNumberStatusId { get; set; }

        /// <summary>
        /// GMT Зона
        /// </summary>
        public string GmtZone { get; set; }

        /// <summary>
        /// Gets or sets the custom attributes
        /// </summary>
        public string CustomUserAttributesXML { get; set; }

        /// <summary>
        /// Gets or sets the currency id
        /// </summary>
        public int? CurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the language id
        /// </summary>
        public int? LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the tax display type id
        /// </summary>
        public int? TaxDisplayTypeId { get; set; }

        /// <summary>
        /// Эл. почтовый адрес для подтверждения. Используется в сценариях, когда пользователь уже зарегистрирован и хочет изменить эл. почтовый адрес
        /// </summary>
        public string EmailToRevalidate { get; set; }

        /// <summary>
        /// Коммент администратора
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is tax exempt
        /// </summary>
        public bool IsTaxExempt { get; set; }

        /// <summary>
        /// Gets or sets the affiliate identifier
        /// </summary>
        public int AffiliateId { get; set; }

        /// <summary>
        /// Gets or sets the vendor identifier with which this user is associated (maganer)
        /// </summary>
        public int VendorId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this user has some tvchannels in the shopping cart
        /// <remarks>The same as if we run ShoppingCartItems.Count > 0
        /// We use this property for performance optimization:
        /// if this property is set to false, then we do not need to load "ShoppingCartItems" navigation property for each page load
        /// It's used only in a couple of places in the presenation layer
        /// </remarks>
        /// </summary>
        public bool HasShoppingCartItems { get; set; }

        /// <summary>
        /// Индицирует, требуется ли пользователю перелогиниться
        /// </summary>
        public bool RequireReLogin { get; set; }

        /// <summary>
        /// Индицирует число неуспешных попыток залогиниться (неверный пароль)
        /// </summary>
        public int FailedLoginAttempts { get; set; }

        /// <summary>
        /// Дата и время до которой пользователь не может логиниться (заблокирован)
        /// </summary>
        public DateTime? CannotLoginUntilDateUtc { get; set; }

        /// <summary>
        /// Индицирует, активен ли пользователь
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Дата и время удаления пользователя
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Индицирует, системный ли аккаунт у пользователя
        /// </summary>
        public bool IsSystemAccount { get; set; }

        /// <summary>
        /// Системне имя пользователя
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Последний IP-адрес
        /// </summary>
        public string LastIpAddress { get; set; }

        /// <summary>
        /// Дата и время создания сущности
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Дата и время последнего входа
        /// </summary>
        public DateTime? LastLoginDateUtc { get; set; }

        /// <summary>
        /// Дата и время последней активности
        /// </summary>
        public DateTime LastActivityDateUtc { get; set; }

        /// <summary>
        /// Идентификатор хранилища, в который был зарегистрирован пользователь 
        /// </summary>
        public int RegisteredInStoreId { get; set; }

        /// <summary>
        /// Биллинговый идентификатор адреса
        /// </summary>
        public int? BillingAddressId { get; set; }

        /// <summary>
        /// Отгрузочный идентификатор адреса
        /// </summary>
        public int? ShippingAddressId { get; set; }

        #region Персонализированные свойства

        /// <summary>
        /// Статус НДС
        /// </summary>
        public VatNumberStatus VatNumberStatus
        {
            get => (VatNumberStatus)VatNumberStatusId;
            set => VatNumberStatusId = (int)value;
        }

        /// <summary>
        /// Для отображения типа налога
        /// </summary>
        public TaxDisplayType? TaxDisplayType
        {
            get => TaxDisplayTypeId.HasValue ?(TaxDisplayType)TaxDisplayTypeId : null;
            set => TaxDisplayTypeId = value.HasValue ? (int)value : null;
        }

        #endregion
    }
}