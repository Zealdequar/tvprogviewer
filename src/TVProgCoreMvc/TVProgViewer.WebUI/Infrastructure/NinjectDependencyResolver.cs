using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ninject;
using TVProgViewer.WebUI.Abstract;
using Moq;
using TVProgViewer.WebUI.MainServiceReferences;
using System.Threading.Tasks;
using TVProgViewer.WebUI.Concrete;

namespace TVProgViewer.WebUI.Infrastructure
{
    public class NinjectDependencyResolver: IDependencyResolver
    {
        private IKernel kernel;
        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        /// <summary>
        /// Привязки
        /// </summary>
        private void AddBindings()
        {
            /*Mock<IProgrammesRepository> mock = new Mock<IProgrammesRepository>();
            mock.Setup<Task<SystemProgramme[]>>(m => 
            m.GetSystemProgrammesAtNowAsycList(It.IsAny<int>(), It.IsAny<DateTimeOffset>()))
            .Returns(Task.FromResult<SystemProgramme[]> (
                new SystemProgramme[]{
                new SystemProgramme { TelecastTitle = "Время", ChannelName = "Первый канал", CID = 1, Remain=20},
                new SystemProgramme { TelecastTitle = "Вести", ChannelName = "Россия 1", CID = 2, Remain = 1 },
                new SystemProgramme { TelecastTitle = "События", ChannelName = "ТВ Центр", CID = 3, Remain = 99},
                new SystemProgramme { TelecastTitle = "Сегодня", ChannelName = "НТВ", CID = 4, Remain = 50}
            }));
            kernel.Bind<IProgrammesRepository>().ToConstant(mock.Object);*/
            kernel.Bind<IProgrammesRepository>().To<EFProgrammeRepository>();
            kernel.Bind<IUsersRepository>().To<EFUserRepository>();
            kernel.Bind<ITreeRepository>().To<EFTreeRepository>();
            kernel.Bind<IChannelsRepository>().To<EFChannelRepository>();
            kernel.Bind<IGenresRepository>().To<EFGenresRepository>();
            kernel.Bind<IRatingsRepository>().To<EFRatingsRepository>();
        }
    }
}