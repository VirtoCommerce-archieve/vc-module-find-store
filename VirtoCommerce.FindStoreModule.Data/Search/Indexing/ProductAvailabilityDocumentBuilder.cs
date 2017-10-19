using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Inventory.Services;
using VirtoCommerce.Domain.Search;

namespace VirtoCommerce.FindStoreModule.Data.Search.Indexing
{
    /// <summary>
    /// Extend product indexation process and provides available_in field for indexed products
    /// </summary>
    public class ProductAvailabilityDocumentBuilder : IIndexDocumentBuilder
    {
        private readonly IItemService _itemService;
        private readonly IInventoryService _inventoryService;

        public ProductAvailabilityDocumentBuilder(IItemService itemService, IInventoryService inventoryService)
        {
            _itemService = itemService;
            _inventoryService = inventoryService;
        }

        public async Task<IList<IndexDocument>> GetDocumentsAsync(IList<string> documentIds)
        {
            var now = DateTime.UtcNow;
            var products = GetProducts(documentIds);
            var productIds = products.Select(p => p.Id).ToArray();
            var documentsByProductId = productIds.ToDictionary(id => id, id => new IndexDocument(id));
            var inventoryInfosByProductId = _inventoryService.GetProductsInventoryInfos(productIds).GroupBy(i => i.ProductId).ToDictionary(g => g.Key, g => g.AsEnumerable());
            products = products.Where(p => inventoryInfosByProductId.ContainsKey(p.Id)).ToList();
            foreach (var product in products)
            {
                var document = documentsByProductId[product.Id];
                foreach (var inventory in inventoryInfosByProductId[product.Id])
                {
                    var isAvailable = inventory.AllowPreorder && inventory.PreorderAvailabilityDate <= now && inventory.PreorderQuantity > 0 ||
                                      inventory.AllowBackorder && inventory.BackorderAvailabilityDate >= now && inventory.BackorderQuantity > 0 ||
                                      Math.Max(0, inventory.InStockQuantity - inventory.ReservedQuantity) > 0;
                    if (isAvailable)
                    {
                        document.Add(new IndexDocumentField("available_in", inventory.FulfillmentCenterId.ToLowerInvariant()) { IsRetrievable = true, IsFilterable = true, IsCollection = true });
                    }
                }
            }
            IList<IndexDocument> result = documentsByProductId.Values.Where(d => d.Fields.Any()).ToArray();
            return await Task.FromResult(result);
        }

        protected virtual IList<CatalogProduct> GetProducts(IList<string> productIds)
        {
            return _itemService.GetByIds(productIds.ToArray(), ItemResponseGroup.Inventory).Where(p => p.TrackInventory.HasValue && p.TrackInventory.Value).ToList();
        }
    }
}