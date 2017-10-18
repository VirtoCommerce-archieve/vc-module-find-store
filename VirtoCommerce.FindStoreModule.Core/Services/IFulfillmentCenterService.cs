using VirtoCommerce.Domain.Commerce.Model;

namespace VirtoCommerce.FindStoreModule.Core.Services
{
    public interface IFulfillmentCenterService
    {
        FulfillmentCenter[] GetById(string[] ids);
    }
}