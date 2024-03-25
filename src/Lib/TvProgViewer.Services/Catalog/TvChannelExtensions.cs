using System.Linq;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Data;

namespace TvProgViewer.Services.Catalog
{
    public static class TvChannelExtensions
    {
        /// <summary>
        /// Sorts the elements of a sequence in order according to a tvchannel sorting rule
        /// </summary>
        /// <param name="tvchannelsQuery">A sequence of tvchannels to order</param>
        /// <param name="currentLanguage">Current language</param>
        /// <param name="orderBy">TvChannel sorting rule</param>
        /// <param name="localizedPropertyRepository">Localized property repository</param>
        /// <returns>An System.Linq.IOrderedQueryable`1 whose elements are sorted according to a rule.</returns>
        /// <remarks>
        /// If <paramref name="orderBy"/> is set to <c>Position</c> and passed <paramref name="tvchannelsQuery"/> is
        /// ordered sorting rule will be skipped
        /// </remarks>
        public static IQueryable<TvChannel> OrderBy(this IQueryable<TvChannel> tvchannelsQuery, IRepository<LocalizedProperty> localizedPropertyRepository, Language currentLanguage, TvChannelSortingEnum orderBy)
        {
            if (orderBy == TvChannelSortingEnum.NameAsc || orderBy == TvChannelSortingEnum.NameDesc)
            {
                var currentLanguageId = currentLanguage.Id;
                
                var query =
                    from tvchannel in tvchannelsQuery
                    join localizedProperty in localizedPropertyRepository.Table on new
                        {
                            tvchannel.Id,
                            languageId = currentLanguageId,
                            keyGroup = nameof(TvChannel),
                            key = nameof(TvChannel.Name)
                        }
                        equals new
                        {
                            Id = localizedProperty.EntityId,
                            languageId = localizedProperty.LanguageId,
                            keyGroup = localizedProperty.LocaleKeyGroup,
                            key = localizedProperty.LocaleKey
                        } into localizedProperties
                    from localizedProperty in localizedProperties.DefaultIfEmpty(new LocalizedProperty { LocaleValue = tvchannel.Name })
                    select new { localizedProperty, tvchannel };

                if (orderBy == TvChannelSortingEnum.NameAsc)
                    tvchannelsQuery = from item in query
                        orderby item.localizedProperty.LocaleValue, item.tvchannel.Name
                        select item.tvchannel;
                else
                    tvchannelsQuery = from item in query
                        orderby item.localizedProperty.LocaleValue descending, item.tvchannel.Name descending
                        select item.tvchannel;

                return tvchannelsQuery;
            }
            
            return orderBy switch
            {
                /*TvChannelSortingEnum.PriceAsc => tvchannelsQuery.OrderBy(p => p.Price),
                TvChannelSortingEnum.PriceDesc => tvchannelsQuery.OrderByDescending(p => p.Price),*/
                TvChannelSortingEnum.CreatedOn => tvchannelsQuery.OrderByDescending(p => p.CreatedOnUtc),
                TvChannelSortingEnum.Position when tvchannelsQuery is IOrderedQueryable => tvchannelsQuery,
                _ => tvchannelsQuery.OrderBy(p => p.DisplayOrder).ThenBy(p => p.Id)
            };
        }
    }
}