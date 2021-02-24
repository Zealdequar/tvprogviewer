using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TVProgViewer.Core.Events;
using TVProgViewer.Core.Infrastructure;
using TVProgViewer.Services.Events;
using TVProgViewer.Web.Framework.Events;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.Web.Framework.Components
{
    /// <summary>
    /// Base class for ViewComponent in TvProg
    /// </summary>
    public abstract class TvProgViewComponent : ViewComponent
    {
        private void PublishModelPrepared<TModel>(TModel model)
        {
            //Components are not part of the controller life cycle.
            //Hence, we could no longer use Action Filters to intercept the Models being returned
            //as we do in the /TvProg.Web.Framework/Mvc/Filters/PublishModelEventsAttribute.cs for controllers

            //model prepared event
            if (model is BaseTvProgModel)
            {
                var eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();

                //we publish the ModelPrepared event for all models as the BaseTvProgModel, 
                //so you need to implement IConsumer<ModelPrepared<BaseTvProgModel>> interface to handle this event
                eventPublisher.ModelPreparedAsync(model as BaseTvProgModel).Wait();
            }

            if (model is IEnumerable<BaseTvProgModel> modelCollection)
            {
                var eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();

                //we publish the ModelPrepared event for collection as the IEnumerable<BaseTvProgModel>, 
                //so you need to implement IConsumer<ModelPrepared<IEnumerable<BaseTvProgModel>>> interface to handle this event
                eventPublisher.ModelPreparedAsync(modelCollection).Wait();
            }
        }
        /// <summary>
        /// Returns a result which will render the partial view with name <paramref name="viewName"/>.
        /// </summary>
        /// <param name="viewName">The name of the partial view to render.</param>
        /// <param name="model">The model object for the view.</param>
        /// <returns>A <see cref="ViewViewComponentResult"/>.</returns>
        public new ViewViewComponentResult View<TModel>(string viewName, TModel model)
        {
            PublishModelPrepared(model);

            //invoke the base method
            return base.View<TModel>(viewName, model);
        }

        /// <summary>
        /// Returns a result which will render the partial view
        /// </summary>
        /// <param name="model">The model object for the view.</param>
        /// <returns>A <see cref="ViewViewComponentResult"/>.</returns>
        public new ViewViewComponentResult View<TModel>(TModel model)
        {
            PublishModelPrepared(model);

            //invoke the base method
            return base.View<TModel>(model);
        }

        /// <summary>
        ///  Returns a result which will render the partial view with name viewName
        /// </summary>
        /// <param name="viewName">The name of the partial view to render.</param>
        /// <returns>A <see cref="ViewViewComponentResult"/>.</returns>
        public new ViewViewComponentResult View(string viewName)
        {
            //invoke the base method
            return base.View(viewName);
        }
    }
}