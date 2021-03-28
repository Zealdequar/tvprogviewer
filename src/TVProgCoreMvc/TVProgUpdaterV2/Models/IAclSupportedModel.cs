using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TVProgViewer.TVProgUpdaterV2.Models
{
    /// <summary>
    /// Represents a model which supports access control list (ACL)
    /// </summary>
    public partial interface IAclSupportedModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets identifiers of the selected User roles
        /// </summary>
        IList<int> SelectedUserRoleIds { get; set; }

        /// <summary>
        /// Gets or sets items for the all available User roles
        /// </summary>
        IList<SelectListItem> AvailableUserRoles { get; set; }

        #endregion
    }
}