using System;
using System.Linq
using VirtoCommerce.CoreModule.Data.Converters;
using VirtoCommerce.CoreModule.Data.Repositories;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.FindStoreModule.Core.Model.Search;
using VirtoCommerce.FindStoreModule.Core.Services;
using VirtoCommerce.FindStoreModule.Data.Extensions;
using VirtoCommerce.Platform.Core.Common;
using domainModel = VirtoCommerce.Domain.Commerce.Model;
using dataModel = VirtoCommerce.CoreModule.Data.Model;

namespace VirtoCommerce.FindStoreModule.Data.Services
{
    public class FulfillmentCenterServiceImpl : IFulfillmentCenterService, IFulfillmentCenterSearchService
    {
        private readonly Func<ICommerceRepository> _repositoryFactory;

        public FulfillmentCenterServiceImpl(Func<ICommerceRepository> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public domainModel.FulfillmentCenter[] GetById(string[] ids)
        {
            domainModel.FulfillmentCenter[] result = null;
            if (ids != null)
            {
                using (var repository = _repositoryFactory())
                {
                    result = repository.FulfillmentCenters.Where(x => ids.Contains(x.Id)).Select(x => x.ToCoreModel()).ToArray();
                }
            }
            return result;
        }

        public virtual GenericSearchResult<domainModel.FulfillmentCenter> Search(FulfillmentCenterSearchCriteria criteria)
        {
            using (var repository = _repositoryFactory())
            {
                repository.DisableChangesTracking();

                var query = repository.FulfillmentCenters;

                if (!string.IsNullOrEmpty(criteria.SearchPhrase))
                {
                    query = query.Where(x => new[] { x.Name, x.Description, x.PostalCode, x.CountryName, x.CountryCode, x.StateProvince, x.City, x.Line1, x.Line2, x.DaytimePhoneNumber }
                                 .Any(y => y.Contains(criteria.SearchPhrase)));
                }

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<dataModel.FulfillmentCenter>(x => x.Name), SortDirection = SortDirection.Descending } };
                }
                query = query.OrderBySortInfos(sortInfos);

                var result = new GenericSearchResult<domainModel.FulfillmentCenter> { TotalCount = query.Count() };
                var fulfillmentIds = query.Skip(criteria.Skip)
                                          .Take(criteria.Take)
                                          .Select(x => x.Id)
                                          .ToArray();
                result.Results = GetById(fulfillmentIds);
                return result;
            }
        }
    }
}