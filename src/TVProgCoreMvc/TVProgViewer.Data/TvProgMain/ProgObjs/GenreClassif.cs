using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TVProgViewer.Data.TvProgMain.ProgObjs
{
    [DataContract]
    public class GenreClassif
    {
        /// <summary>
        /// Идентификатор элемента классификатора жанров
        /// </summary>
        [DataMember]
        public long GenreClassificatorId { get; set; }

        /// <summary>
        /// Идентификатор жанра
        /// </summary>
        [DataMember]
        public long Gid { get; set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [DataMember]
        public long? Uid { get; set; }

        /// <summary>
        /// Содержит
        /// </summary>
        [DataMember]
        public string ContainPhrases { get; set; }

        /// <summary>
        /// Не содержит
        /// </summary>
        [DataMember]
        public string NonContainPhrases { get; set; }

        /// <summary>
        /// Путь к пиктограмме жанра
        /// </summary>
        [DataMember]
        public string GenrePath { get; set; }

        /// <summary>
        /// Название жанра
        /// </summary>
        [DataMember]
        public string GenreName { get; set; }

        /// <summary>
        /// Порядок принятия в силу
        /// </summary>
        [DataMember]
        public int? OrderCol { get; set; }

        /// <summary>
        /// Удалить после
        /// </summary>
        [DataMember]
        public DateTime? DeleteAfterDate { get; set; }
    }
}
