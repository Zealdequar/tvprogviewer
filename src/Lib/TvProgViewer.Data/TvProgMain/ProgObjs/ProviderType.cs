using System;
using System.Runtime.Serialization;

namespace TvProgViewer.Data.TvProgMain.ProgObjs
{
    /// <summary>
    /// Контракт (DTO) для провайдера телепрограммы
    /// </summary>
    [DataContract]
    public class ProviderType
    {
        /// <summary>
        /// Идентификатор провайдера
        /// </summary>
        [DataMember]
        public int TvProgProviderId { get; set; }

        /// <summary>
        /// Название провайдера
        /// </summary>
        [DataMember]
        public string ProviderName { get; set; }

        /// <summary>
        /// Описание провайдера
        /// </summary>
        public string ProviderText { get; set; }

        /// <summary>
        /// URI провайдера
        /// </summary>
        [DataMember]
        public Uri ProviderUri
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ProviderText))
                    return new Uri(ProviderText);
                return null;
            }
            set
            {
            }
        }

        /// <summary>
        /// Идентификатор типа телепрограммы
        /// </summary>
        [DataMember]
        public int TypeProgId { get; set; }

        /// <summary>
        /// Формат телепрограммы
        /// </summary>
        [DataMember]
        public Enums.TypeProg TypeEnum { get; set; }

        /// <summary>
        /// Название типа телепрограммы
        /// </summary>
        [DataMember]
        public string TypeName { get; set; }

        /// <summary>
        /// Формат файла сохранения телепрограммы
        /// </summary>
        [DataMember]
        public string FileFormat { get; set; }
    }
}
