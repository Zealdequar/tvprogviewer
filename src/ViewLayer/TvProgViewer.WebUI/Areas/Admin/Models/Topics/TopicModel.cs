﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Topics
{
    /// <summary>
    /// Represents a topic model
    /// </summary>
    public partial record TopicModel : BaseTvProgEntityModel, IAclSupportedModel, ILocalizedModel<TopicLocalizedModel>, IStoreMappingSupportedModel
    {
        #region Ctor

        public TopicModel()
        {
            AvailableTopicTemplates = new List<SelectListItem>();
            Locales = new List<TopicLocalizedModel>();

            SelectedUserRoleIds = new List<int>();
            AvailableUserRoles = new List<SelectListItem>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.SystemName")]
        public string SystemName { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInSitemap")]
        public bool IncludeInSitemap { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInTopMenu")]
        public bool IncludeInTopMenu { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn1")]
        public bool IncludeInFooterColumn1 { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn2")]
        public bool IncludeInFooterColumn2 { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn3")]
        public bool IncludeInFooterColumn3 { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.AccessibleWhenStoreClosed")]
        public bool AccessibleWhenStoreClosed { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.IsPasswordProtected")]
        public bool IsPasswordProtected { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.Password")]
        public string Password { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.URL")]
        public string Url { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.Title")]
        public string Title { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.Body")]
        public string Body { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.Published")]
        public bool Published { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.TopicTemplate")]
        public int TopicTemplateId { get; set; }

        public IList<SelectListItem> AvailableTopicTemplates { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaKeywords")]
        public string MetaKeywords { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaDescription")]
        public string MetaDescription { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaTitle")]
        public string MetaTitle { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.SeName")]
        public string SeName { get; set; }

        public IList<TopicLocalizedModel> Locales { get; set; }

        //store mapping
        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.LimitedToStores")]
        public IList<int> SelectedStoreIds { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        //ACL (user roles)
        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.AclUserRoles")]
        public IList<int> SelectedUserRoleIds { get; set; }

        public IList<SelectListItem> AvailableUserRoles { get; set; }

        public string TopicName { get; set; }

        #endregion
    }

    public partial record TopicLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.Title")]
        public string Title { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.Body")]
        public string Body { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaKeywords")]
        public string MetaKeywords { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaDescription")]
        public string MetaDescription { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaTitle")]
        public string MetaTitle { get; set; }

        [TvProgResourceDisplayName("Admin.ContentManagement.Topics.Fields.SeName")]
        public string SeName { get; set; }
    }
}