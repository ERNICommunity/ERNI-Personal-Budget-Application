using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public interface IRequestCategoryRepository
    {
        Task<RequestCategory[]> GetRequestCategories(CancellationToken cancellationToken);

        void AddRequestCategory(RequestCategory requestCategory);
    }
}