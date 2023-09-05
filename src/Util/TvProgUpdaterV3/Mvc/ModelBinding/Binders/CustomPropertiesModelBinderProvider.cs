using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TvProgViewer.TvProgUpdaterV3.Models;

namespace TvProgViewer.TvProgUpdaterV3.Mvc.ModelBinding.Binders
{
    /// <summary>
    /// Represents a model binder provider for CustomProperties
    /// </summary>
    public class CustomPropertiesModelBinderProvider : IModelBinderProvider
    {
        IModelBinder IModelBinderProvider.GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.PropertyName == nameof(BaseTvProgModel.CustomProperties) && context.Metadata.ModelType == typeof(Dictionary<string, string>))
                return new CustomPropertiesModelBinder();
            
            return null;
        }
    }
}
