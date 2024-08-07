using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Web.Framework.Mvc.ModelBinding.Binders
{
    /// <summary>
    /// Представляет модель провайдера связей для специфичных свойств  
    /// </summary>
    public class CustomPropertiesModelBinderProvider : IModelBinderProvider
    {
        IModelBinder IModelBinderProvider.GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.PropertyName == nameof(BaseTvProgModel.CustomProperties) && context.Metadata.ModelType == typeof(Dictionary<string, string>))
                return new CustomPropertiesModelBinder();

            if (!context.Metadata.IsComplexType && context.Metadata.ModelType == typeof(string))
                return new StringModelBinder();

            return null;
        }
    }
}
