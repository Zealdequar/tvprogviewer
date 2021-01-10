using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using TVProgViewer.Core.Infrastructure;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.Web.Framework.Mvc.ModelBinding
{
    /// <summary>
    /// Represents model binder provider for the creating TvProgModelBinder
    /// </summary>
    public class TvProgModelBinderProvider : IModelBinderProvider
    {
        /// <summary>
        /// Creates a TVProgViewer model binder based on passed context
        /// </summary>
        /// <param name="context">Model binder provider context</param>
        /// <returns>Model binder</returns>
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));


            var modelType = context.Metadata.ModelType;
            if (!typeof(BaseTvProgModel).IsAssignableFrom(modelType))
                return null;

            //use TvProgModelBinder as a ComplexTypeModelBinder for BaseTvProgModel
            if (context.Metadata.IsComplexType && !context.Metadata.IsCollectionType)
            {
                //create binders for all model properties
                var propertyBinders = context.Metadata.Properties
                    .ToDictionary(modelProperty => modelProperty, modelProperty => context.CreateBinder(modelProperty));
                
                return new TvProgModelBinder(propertyBinders, EngineContext.Current.Resolve<ILoggerFactory>());
            }

            //or return null to further search for a suitable binder
            return null;
        }
    }
}
