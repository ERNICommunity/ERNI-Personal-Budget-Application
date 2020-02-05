using System.Linq;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace ERNI.PBA.Server.DataAccess.Repository
{
    public class TeamRequestRepository : ITeamRequestRepository
    {
        private readonly DatabaseContext _context;

        public TeamRequestRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TeamRequest teamRequest)
        {
            await _context.TeamRequests.AddAsync(teamRequest);
        }

        public async Task<TeamRequest> GetAsync(int requestId)
        {
            return await _context.TeamRequests
                .Include(_ => _.Requests)
                .SingleOrDefaultAsync(_ => _.Id == requestId);
        }

        public async Task<TeamRequest[]> GetAllAsync(int userId)
        {
            return await _context.TeamRequests
                .Include(_ => _.Requests)
                .Where(_ => _.UserId == userId)
                .ToArrayAsync();
        }

        public void Remove(TeamRequest teamRequest)
        {
            _context.TeamRequests.Remove(teamRequest);
        }
    }
}
