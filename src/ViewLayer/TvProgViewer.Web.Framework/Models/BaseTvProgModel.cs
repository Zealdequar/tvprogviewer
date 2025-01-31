﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace TvProgViewer.Web.Framework.Models
{
    /// <summary>
    /// Represents base tvProgViewer model
    /// </summary>
    public partial record BaseTvProgModel
    {
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public BaseTvProgModel()
        {
            CustomProperties = new Dictionary<string, string>();
            PostInitialize();
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Perform additional actions for the model initialization
        /// </summary>
        /// <remarks>Developers can override this method in custom partial classes in order to add some custom initialization code to constructors</remarks>
        protected virtual void PostInitialize()
        {
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets or sets property to store any custom values for models 
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, string> CustomProperties { get; set; }

        #endregion

    }
}