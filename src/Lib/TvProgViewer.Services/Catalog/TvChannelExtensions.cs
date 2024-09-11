using System.Linq;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Data;

namespace TvProgViewer.Services.Catalog
{
    public static class TvChannelExtensions
    {
        /// <summary>
        /// Sorts the elements of a sequence in order according to a tvChannel sorting rule
        /// </summary>
        /// <param name="tvChannelsQuery">A sequence of tvChannels to order</param>
        /// <param name="currentLanguage">Current language</param>
        /// <param name="orderBy">TvChannel sorting rule</param>
        /// <param name="localizedPropertyRepository">Localized property repository</param>
        /// <returns>An System.Linq.IOrderedQueryable`1 whose elements are sorted according to a rule.</returns>
        /// <remarks>
        /// If <paramref name="orderBy"/> is set to <c>Position</c> and passed <paramref name="tvChannelsQuery"/> is
        /// ordered sorting rule will be skipped
        /// </remarks>
        public static IQueryable<TvChannel> OrderBy(this IQueryable<TvChannel> tvChannelsQuery, IRepository<LocalizedProperty> localizedPropertyRepository, Language currentLanguage, TvChannelSortingEnum orderBy)
        {
            if (orderBy == TvChannelSortingEnum.NameAsc || orderBy == TvChannelSortingEnum.NameDesc)
            {
                var currentLanguageId = currentLanguage.Id;
                
                var query =
                    from tvChannel in tvChannelsQuery
                    join localizedProperty in localizedPropertyRepository.Table on new
                        {
                            tvChannel.Id,
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
                    from localizedProperty in localizedProperties.DefaultIfEmpty(new LocalizedProperty { LocaleValue = tvChannel.Name })
                    select new { localizedProperty, tvChannel };

                if (orderBy == TvChannelSortingEnum.NameAsc)
                    tvChannelsQuery = from item in query
                        orderby item.localizedProperty.LocaleValue, item.tvChannel.Name
                        select item.tvChannel;
                else
                    tvChannelsQuery = from item in query
                        orderby item.localizedProperty.LocaleValue descending, item.tvChannel.Name descending
                        select item.tvChannel;

                return tvChannelsQuery;
            }
            
            return orderBy switch
            {
                /*TvChannelSortingEnum.PriceAsc => tvChannelsQuery.OrderBy(p => p.Price),
                TvChannelSortingEnum.PriceDesc => tvChannelsQuery.OrderByDescending(p => p.Price),*/
                TvChannelSortingEnum.CreatedOn => tvChannelsQuery.OrderByDescending(p => p.CreatedOnUtc),
                TvChannelSortingEnum.Position when tvChannelsQuery is IOrderedQueryable => tvChannelsQuery,
                _ => tvChannelsQuery.OrderBy(p => p.DisplayOrder).ThenBy(p => p.Id)
            };
        }
    }
}