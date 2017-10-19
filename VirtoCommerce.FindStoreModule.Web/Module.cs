using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using VirtoCommerce.Domain.Search;
using VirtoCommerce.FindStoreModule.Core.Services;
using VirtoCommerce.FindStoreModule.Data.Search.Indexing;
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

        public override void PostInitialize()
        {
            base.PostInitialize();

            #region Search

            var productIndexingConfigurations = _container.Resolve<IndexDocumentConfiguration[]>();
            if (productIndexingConfigurations != null)
            {
                var productCompletenessDocumentSource = new IndexDocumentSource
                {
                    ChangesProvider = _container.Resolve<ProductAvailabilityChangesProvider>(),
                    DocumentBuilder = _container.Resolve<ProductAvailabilityDocumentBuilder>(),
                };

                foreach (var configuration in productIndexingConfigurations.Where(c => c.DocumentType == KnownDocumentTypes.Product))
                {
                    if (configuration.RelatedSources == null)
                    {
                        configuration.RelatedSources = new List<IndexDocumentSource>();
                    }

                    configuration.RelatedSources.Add(productCompletenessDocumentSource);
                }
            }

            #endregion
        }
    }
}