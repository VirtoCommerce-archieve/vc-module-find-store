using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Domain.Inventory.Services;
using VirtoCommerce.Domain.Search;
using VirtoCommerce.InventoryModule.Data.Model;
using VirtoCommerce.InventoryModule.Data.Repositories;
using VirtoCommerce.Platform.Core.ChangeLog;

namespace VirtoCommerce.FindStoreModule.Data.Search.Indexing
{
    /// <summary>
    /// Extend product indexation process. Invalidate products as changed when products availability in fulfillment centers updated.
    /// </summary>
    public class ProductAvailabilityChangesProvider : IIndexDocumentChangesProvider
    {
        public const string ChangeLogObjectType = nameof(Inventory);

        private readonly IChangeLogService _changeLogService;
        private readonly Func<IInventoryRepository> _inventoryRepositoryFactory;
        private readonly IInventoryService _inventoryService;

        public ProductAvailabilityChangesProvider(IChangeLogService changeLogService, Func<IInventoryRepository> inventoryRepositoryFactory, IInventoryService inventoryService)
        {
            _changeLogService = changeLogService;
            _inventoryRepositoryFactory = inventoryRepositoryFactory;
            _inventoryService = inventoryService;
        }

        public async Task<long> GetTotalChangesCountAsync(DateTime? startDate, DateTime? endDate)
        {
            long result;

            if (startDate == null && endDate == null)
            {
                // We don't know the total products count
                result = 0L;
            }
            else
            {
                // Get changes count from operation log
                result = _changeLogService.FindChangeHistory(ChangeLogObjectType, startDate, endDate).Count();
            }

            return await Task.FromResult(result);
        }

        public async Task<IList<IndexDocumentChange>> GetChangesAsync(DateTime? startDate, DateTime? endDate, long skip, long take)
        {
            IList<IndexDocumentChange> result;

            if (startDate == null && endDate == null)
            {
                result = null;
            }
            else
            {
                // Get changes from operation log
                var operations = _changeLogService.FindChangeHistory(ChangeLogObjectType, startDate, endDate)
                    .Skip((int)skip)
                    .Take((int)take)
                    .ToArray();

                var inventoryIds = operations.Select(o => o.ObjectId).ToArray();
                var productIdsByInventoryId = GetProductIds(inventoryIds);

                result = operations
                    .Where(o => productIdsByInventoryId.ContainsKey(o.ObjectId))
                    .Select(o =>
                    new IndexDocumentChange
                    {
                        DocumentId = productIdsByInventoryId[o.ObjectId],
                        ChangeType = IndexDocumentChangeType.Modified,
                        ChangeDate = o.ModifiedDate ?? o.CreatedDate,
                    }
                ).ToArray();
            }

            return await Task.FromResult(result);
        }

        protected virtual IDictionary<string, string> GetProductIds(string[] inventoryIds)
        {
            // TODO: How to get product for deleted completeness entry?
            using (var repository = _inventoryRepositoryFactory())
            {
                // TODO: Replace with service after GetById will be implemented
                var inventories = repository.Inventories.Where(i => inventoryIds.Contains(i.Id)).ToArray();
                // Inventory.Sku is Product ID actually
                var result = inventories.ToDictionary(e => e.Id, e => e.Sku);
                return result;
            }
        }
    }
}