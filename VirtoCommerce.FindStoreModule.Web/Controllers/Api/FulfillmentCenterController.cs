using System.Web.Http;
using System.Web.Http.Description;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.FindStoreModule.Core.Model.Search;
using VirtoCommerce.FindStoreModule.Core.Services;

namespace VirtoCommerce.FindStoreModule.Web.Controllers.Api
{
    [RoutePrefix("api")]
    public class FulfillmentCenterController : ApiController
    {
        private readonly IFulfillmentCenterSearchService _fulfillmentCenterSearchService;

        public FulfillmentCenterController(IFulfillmentCenterSearchService fulfillmentCenterSearchService)
        {
            _fulfillmentCenterSearchService = fulfillmentCenterSearchService;
        }

        /// <summary>
        /// Search fulfillment centers registered in the system
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ResponseType(typeof(FulfillmentCenter[]))]
        [Route("fulfillment/search/centers")]
        public IHttpActionResult SearchFulfillmentCenters([FromBody] FulfillmentCenterSearchCriteria searchCriteria)
        {
            var retVal = _fulfillmentCenterSearchService.Search(searchCriteria);
            return Ok(retVal);
        }
    }
}
