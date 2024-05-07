using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public class RequestCategoryRepository(DatabaseContext context) : IRequestCategoryRepository
    {
        public void AddRequestCategory(RequestCategory requestCategory) =>
            context.RequestCategories.Add(requestCategory);

        public Task<RequestCategory[]> GetRequestCategories(CancellationToken cancellationToken) =>
            context.RequestCategories.ToArrayAsync(cancellationToken);

        public Task<RequestCategory?> GetRequestCategory(int id, CancellationToken cancellationToken) => context
            .RequestCategories.Where(rc => rc.Id == id).FirstOrDefaultAsync(cancellationToken: cancellationToken);

        public void DeleteRequestCategory(RequestCategory requestCategory) =>
            context.RequestCategories.Remove(requestCategory);
    }
}