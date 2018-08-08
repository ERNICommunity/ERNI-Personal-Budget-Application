using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public class RequestCategoryRepository : IRequestCategoryRepository
    {
        private readonly DatabaseContext _context;

        public RequestCategoryRepository(DatabaseContext context)
        {
            _context = context;
        }

        public void AddRequestCategory(RequestCategory requestCategory)
        {
            _context.RequestCategories.Add(requestCategory);
        }

        public Task<RequestCategory[]> GetRequestCategories(CancellationToken cancellationToken)
        {
            return _context.RequestCategories.ToArrayAsync(cancellationToken);
        }

        public Task<RequestCategory> GetRequestCategory(int id, CancellationToken cancellationToken)
        {
            return _context.RequestCategories.Where(rc => rc.Id == id).FirstOrDefaultAsync();
        }
       
        public void UpdateRequestCategory(RequestCategory requestCategory)
        {
            _context.RequestCategories.Update(requestCategory);
        }

        public void DeleteRequestCategory(RequestCategory requestCategory)
        {
            _context.RequestCategories.Remove(requestCategory);
        }
    }
}