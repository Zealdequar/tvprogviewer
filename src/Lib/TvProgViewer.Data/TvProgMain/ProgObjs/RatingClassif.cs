using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TvProgViewer.Data.TvProgMain.ProgObjs
{
    [DataContract]
    public class RatingClassif
    {
        /// <summary>
        /// Идентификатор элемента классификатора рейтингов
        /// </summary>
        [DataMember]
        public long RatingClassificatorId { get; set; }

        /// <summary>
        /// Идентификатор рейтинга
        /// </summary>
        [DataMember]
        public long Rid { get; set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [DataMember]
        public long Uid { get; set; }

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
        /// Путь к пиктограмме рейтинга 
        /// </summary>
        [DataMember]
        public string RatingPath { get; set; }

        /// <summary>
        /// Название рейтинга
        /// </summary>
        [DataMember]
        public string RatingName { get; set; }

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
