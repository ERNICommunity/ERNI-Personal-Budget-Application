using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Models;

namespace ERNI.PBA.Server.Domain.Interfaces.Repositories
{
    public interface IRequestCategoryRepository
    {
        Task<RequestCategory[]> GetRequestCategories(CancellationToken cancellationToken);

        Task<RequestCategory> GetRequestCategory(int id, CancellationToken cancellationToken);

        void AddRequestCategory(RequestCategory requestCategory);

        void DeleteRequestCategory(RequestCategory requestCategory);
    }
}