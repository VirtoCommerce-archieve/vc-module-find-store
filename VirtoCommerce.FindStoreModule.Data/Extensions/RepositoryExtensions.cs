using System.Data.Entity;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.FindStoreModule.Data.Extensions
{
    public static class RepositoryExtensions
    {
        public static void DisableChangesTracking(this IRepository repository)
        {
            //http://stackoverflow.com/questions/29106477/nullreferenceexception-in-entity-framework-from-trygetcachedrelatedend
            if (repository is DbContext context)
            {
                var dbConfiguration = context.Configuration;
                dbConfiguration.ProxyCreationEnabled = false;
                dbConfiguration.AutoDetectChangesEnabled = false;
            }
        }
    }
}