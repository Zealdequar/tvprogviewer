using System;
using System.ComponentModel.DataAnnotations;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Common
{
    public partial record MaintenanceModel : BaseTvProgModel
    {
        public MaintenanceModel()
        {
            DeleteGuests = new();
            DeleteAbandonedCarts = new();
            DeleteExportedFiles = new();
            BackupFileSearchModel = new();
            DeleteAlreadySentQueuedEmails = new();
            DeleteMinificationFiles = new();
        }

        public DeleteGuestsModel DeleteGuests { get; set; }

        public DeleteAbandonedCartsModel DeleteAbandonedCarts { get; set; }

        public DeleteExportedFilesModel DeleteExportedFiles { get; set; }

        public BackupFileSearchModel BackupFileSearchModel { get; set; }

        public DeleteAlreadySentQueuedEmailsModel DeleteAlreadySentQueuedEmails { get; set; }

        public DeleteMinificationFilesModel DeleteMinificationFiles { get; set; }

        public bool BackupSupported { get; set; }

        #region Nested classes

        public partial record DeleteGuestsModel : BaseTvProgModel
        {
            [TvProgResourceDisplayName("Admin.System.Maintenance.DeleteGuests.StartDate")]
            [UIHint("DateNullable")]
            public DateTime? StartDate { get; set; }

            [TvProgResourceDisplayName("Admin.System.Maintenance.DeleteGuests.EndDate")]
            [UIHint("DateNullable")]
            public DateTime? EndDate { get; set; }

            [TvProgResourceDisplayName("Admin.System.Maintenance.DeleteGuests.OnlyWithoutShoppingCart")]
            public bool OnlyWithoutShoppingCart { get; set; }

            public int? NumberOfDeletedUsers { get; set; }
        }

        public partial record DeleteAbandonedCartsModel : BaseTvProgModel
        {
            [TvProgResourceDisplayName("Admin.System.Maintenance.DeleteAbandonedCarts.OlderThan")]
            [UIHint("Date")]
            public DateTime OlderThan { get; set; }

            public int? NumberOfDeletedItems { get; set; }
        }

        public partial record DeleteExportedFilesModel : BaseTvProgModel
        {
            [TvProgResourceDisplayName("Admin.System.Maintenance.DeleteExportedFiles.StartDate")]
            [UIHint("DateNullable")]
            public DateTime? StartDate { get; set; }

            [TvProgResourceDisplayName("Admin.System.Maintenance.DeleteExportedFiles.EndDate")]
            [UIHint("DateNullable")]
            public DateTime? EndDate { get; set; }

            public int? NumberOfDeletedFiles { get; set; }
        }

        public partial record DeleteAlreadySentQueuedEmailsModel : BaseTvProgModel
        {
            [TvProgResourceDisplayName("Admin.System.Maintenance.DeleteAlreadySentQueuedEmails.StartDate")]
            [UIHint("DateNullable")]
            public DateTime? StartDate { get; set; }

            [TvProgResourceDisplayName("Admin.System.Maintenance.DeleteAlreadySentQueuedEmails.EndDate")]
            [UIHint("DateNullable")]
            public DateTime? EndDate { get; set; }

            public int? NumberOfDeletedEmails { get; set; }
        }

        public partial record DeleteMinificationFilesModel : BaseTvProgModel
        {
            public int? NumberOfDeletedFiles { get; set; }
        }

        #endregion
    }
}
