using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.FindStoreModule.Core.Model.Search;

namespace VirtoCommerce.FindStoreModule.Core.Services
{
    public interface IFulfillmentCenterSearchService
    {
        GenericSearchResult<FulfillmentCenter> Search(FulfillmentCenterSearchCriteria criteria);
    }
}