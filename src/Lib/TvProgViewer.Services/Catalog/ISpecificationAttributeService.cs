﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Specification attribute service interface
    /// </summary>
    public partial interface ISpecificationAttributeService
    {
        #region Specification attribute group

        /// <summary>
        /// Gets a specification attribute group
        /// </summary>
        /// <param name="specificationAttributeGroupId">The specification attribute group identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attribute group
        /// </returns>
        Task<SpecificationAttributeGroup> GetSpecificationAttributeGroupByIdAsync(int specificationAttributeGroupId);

        /// <summary>
        /// Gets specification attribute groups
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attribute groups
        /// </returns>
        Task<IPagedList<SpecificationAttributeGroup>> GetSpecificationAttributeGroupsAsync(int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets tvChannel specification attribute groups
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attribute groups
        /// </returns>
        Task<IList<SpecificationAttributeGroup>> GetTvChannelSpecificationAttributeGroupsAsync(int tvChannelId);

        /// <summary>
        /// Deletes a specification attribute group
        /// </summary>
        /// <param name="specificationAttributeGroup">The specification attribute group</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteSpecificationAttributeGroupAsync(SpecificationAttributeGroup specificationAttributeGroup);

        /// <summary>
        /// Inserts a specification attribute group
        /// </summary>
        /// <param name="specificationAttributeGroup">The specification attribute group</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertSpecificationAttributeGroupAsync(SpecificationAttributeGroup specificationAttributeGroup);

        /// <summary>
        /// Updates the specification attribute group
        /// </summary>
        /// <param name="specificationAttributeGroup">The specification attribute group</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateSpecificationAttributeGroupAsync(SpecificationAttributeGroup specificationAttributeGroup);

        #endregion

        #region Specification attribute

        /// <summary>
        /// Gets a specification attribute
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attribute
        /// </returns>
        Task<SpecificationAttribute> GetSpecificationAttributeByIdAsync(int specificationAttributeId);

        /// <summary>
        /// Gets specification attributes
        /// </summary>
        /// <param name="specificationAttributeIds">The specification attribute identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attributes
        /// </returns>
        Task<IList<SpecificationAttribute>> GetSpecificationAttributeByIdsAsync(int[] specificationAttributeIds);

        /// <summary>
        /// Gets specification attributes
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attributes
        /// </returns>
        Task<IPagedList<SpecificationAttribute>> GetSpecificationAttributesAsync(int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets specification attributes that have options
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attributes that have available options
        /// </returns>
        Task<IList<SpecificationAttribute>> GetSpecificationAttributesWithOptionsAsync();

        /// <summary>
        /// Gets specification attributes by group identifier
        /// </summary>
        /// <param name="specificationAttributeGroupId">The specification attribute group identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attributes
        /// </returns>
        Task<IList<SpecificationAttribute>> GetSpecificationAttributesByGroupIdAsync(int? specificationAttributeGroupId = null);

        /// <summary>
        /// Deletes a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteSpecificationAttributeAsync(SpecificationAttribute specificationAttribute);

        /// <summary>
        /// Deletes specifications attributes
        /// </summary>
        /// <param name="specificationAttributes">Specification attributes</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteSpecificationAttributesAsync(IList<SpecificationAttribute> specificationAttributes);

        /// <summary>
        /// Inserts a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertSpecificationAttributeAsync(SpecificationAttribute specificationAttribute);

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateSpecificationAttributeAsync(SpecificationAttribute specificationAttribute);

        #endregion

        #region Specification attribute option

        /// <summary>
        /// Gets a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attribute option
        /// </returns>
        Task<SpecificationAttributeOption> GetSpecificationAttributeOptionByIdAsync(int specificationAttributeOption);

        /// <summary>
        /// Get specification attribute options by identifiers
        /// </summary>
        /// <param name="specificationAttributeOptionIds">Identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attribute options
        /// </returns>
        Task<IList<SpecificationAttributeOption>> GetSpecificationAttributeOptionsByIdsAsync(int[] specificationAttributeOptionIds);

        /// <summary>
        /// Gets a specification attribute option by specification attribute id
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attribute option
        /// </returns>
        Task<IList<SpecificationAttributeOption>> GetSpecificationAttributeOptionsBySpecificationAttributeAsync(int specificationAttributeId);

        /// <summary>
        /// Deletes a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteSpecificationAttributeOptionAsync(SpecificationAttributeOption specificationAttributeOption);

        /// <summary>
        /// Inserts a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertSpecificationAttributeOptionAsync(SpecificationAttributeOption specificationAttributeOption);

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateSpecificationAttributeOptionAsync(SpecificationAttributeOption specificationAttributeOption);

        /// <summary>
        /// Returns a list of IDs of not existing specification attribute options
        /// </summary>
        /// <param name="attributeOptionIds">The IDs of the attribute options to check</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of IDs not existing specification attribute options
        /// </returns>
        Task<int[]> GetNotExistingSpecificationAttributeOptionsAsync(int[] attributeOptionIds);

        /// <summary>
        /// Gets the filtrable specification attribute options by category id
        /// </summary>
        /// <param name="categoryId">The category id</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attribute options
        /// </returns>
        Task<IList<SpecificationAttributeOption>> GetFiltrableSpecificationAttributeOptionsByCategoryIdAsync(int categoryId);

        /// <summary>
        /// Gets the filtrable specification attribute options by manufacturer id
        /// </summary>
        /// <param name="manufacturerId">The manufacturer id</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the specification attribute options
        /// </returns>
        Task<IList<SpecificationAttributeOption>> GetFiltrableSpecificationAttributeOptionsByManufacturerIdAsync(int manufacturerId);

        #endregion

        #region TvChannel specification attribute

        /// <summary>
        /// Deletes a tvChannel specification attribute mapping
        /// </summary>
        /// <param name="tvChannelSpecificationAttribute">TvChannel specification attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelSpecificationAttributeAsync(TvChannelSpecificationAttribute tvChannelSpecificationAttribute);

        /// <summary>
        /// Gets a tvChannel specification attribute mapping collection
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier; 0 to load all records</param>
        /// <param name="specificationAttributeOptionId">Specification attribute option identifier; 0 to load all records</param>
        /// <param name="allowFiltering">0 to load attributes with AllowFiltering set to false, 1 to load attributes with AllowFiltering set to true, null to load all attributes</param>
        /// <param name="showOnTvChannelPage">0 to load attributes with ShowOnTvChannelPage set to false, 1 to load attributes with ShowOnTvChannelPage set to true, null to load all attributes</param>
        /// <param name="specificationAttributeGroupId">Specification attribute group identifier; 0 to load all records; null to load attributes without group</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel specification attribute mapping collection
        /// </returns>
        Task<IList<TvChannelSpecificationAttribute>> GetTvChannelSpecificationAttributesAsync(int tvChannelId = 0,
            int specificationAttributeOptionId = 0, bool? allowFiltering = null, bool? showOnTvChannelPage = null, int? specificationAttributeGroupId = 0);

        /// <summary>
        /// Gets a tvChannel specification attribute mapping 
        /// </summary>
        /// <param name="tvChannelSpecificationAttributeId">TvChannel specification attribute mapping identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel specification attribute mapping
        /// </returns>
        Task<TvChannelSpecificationAttribute> GetTvChannelSpecificationAttributeByIdAsync(int tvChannelSpecificationAttributeId);

        /// <summary>
        /// Inserts a tvChannel specification attribute mapping
        /// </summary>
        /// <param name="tvChannelSpecificationAttribute">TvChannel specification attribute mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertTvChannelSpecificationAttributeAsync(TvChannelSpecificationAttribute tvChannelSpecificationAttribute);

        /// <summary>
        /// Updates the tvChannel specification attribute mapping
        /// </summary>
        /// <param name="tvChannelSpecificationAttribute">TvChannel specification attribute mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelSpecificationAttributeAsync(TvChannelSpecificationAttribute tvChannelSpecificationAttribute);

        /// <summary>
        /// Gets a count of tvChannel specification attribute mapping records
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier; 0 to load all records</param>
        /// <param name="specificationAttributeOptionId">The specification attribute option identifier; 0 to load all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the count
        /// </returns>
        Task<int> GetTvChannelSpecificationAttributeCountAsync(int tvChannelId = 0, int specificationAttributeOptionId = 0);

        /// <summary>
        /// Get mapped tvChannels for specification attribute
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannels
        /// </returns>
        Task<IPagedList<TvChannel>> GetTvChannelsBySpecificationAttributeIdAsync(int specificationAttributeId, int pageIndex, int pageSize);

        #endregion
    }
}
