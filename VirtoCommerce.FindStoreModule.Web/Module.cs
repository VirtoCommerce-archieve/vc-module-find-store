using Microsoft.Practices.Unity;
using VirtoCommerce.FindStoreModule.Core.Services;
using VirtoCommerce.FindStoreModule.Data.Services;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.FindStoreModule.Web
{
    public class Module: ModuleBase
    {
        private readonly IUnityContainer _container;

        public Module(IUnityContainer container)
        {
            _container = container;
        }

        public override void Initialize()
        {
            _container.RegisterType<IFulfillmentCenterService, FulfillmentCenterServiceImpl>();
            _container.RegisterType<IFulfillmentCenterSearchService, FulfillmentCenterServiceImpl>();
        }
    }
}