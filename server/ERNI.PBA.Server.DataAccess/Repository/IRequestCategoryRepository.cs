using ERNI.PBA.Server.DataAccess.Model;
using System.Threading;
using System.Threading.Tasks;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public interface IRequestCategoryRepository
    {
        Task<RequestCategory[]> GetRequestCategories(CancellationToken cancellationToken);

        Task<RequestCategory> GetRequestCategory(int id, CancellationToken cancellationToken);

        void AddRequestCategory(RequestCategory requestCategory);

        void DeleteRequestCategory(RequestCategory requestCategory);
    }
}