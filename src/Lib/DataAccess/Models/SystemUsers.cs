using System;
using System.Collections.Generic;

namespace TvProgViewer.DataAccess.Models
{
    /// <summary>
    /// Перенесено на TvProgViewer.Core.Domain
    /// </summary>
    public partial class SystemUsers
    {
        public SystemUsers()
        {
            ExtUserSettings = new HashSet<ExtUserSettings>();
            GenreClassificator = new HashSet<GenreClassificator>();
            Genres = new HashSet<Genres>();
            RatingClassificator = new HashSet<RatingClassificator>();
            Ratings = new HashSet<Ratings>();
            SearchSettings = new HashSet<SearchSettings>();
            UserChannels = new HashSet<UserChannels>();
            UsersPrograms = new HashSet<UsersPrograms>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string PassHash { get; set; }
        public string PassExtend { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string MobPhoneNumber { get; set; }
        public string OtherPhoneNumber1 { get; set; }
        public string OtherPhoneNumber2 { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }
        public string Country { get; set; }
        public string StreetAddress { get; set; }

        public string City { get; set; }
        public string GmtZone { get; set; }
        public short Status { get; set; }
        public string Catalog { get; set; }

        public virtual ICollection<ExtUserSettings> ExtUserSettings { get; set; }
        public virtual ICollection<GenreClassificator> GenreClassificator { get; set; }
        public virtual ICollection<Genres> Genres { get; set; }
        public virtual ICollection<RatingClassificator> RatingClassificator { get; set; }
        public virtual ICollection<Ratings> Ratings { get; set; }
        public virtual ICollection<SearchSettings> SearchSettings { get; set; }
        public virtual ICollection<UserChannels> UserChannels { get; set; }
        public virtual ICollection<UsersPrograms> UsersPrograms { get; set; }
    }
}
