using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
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

        public async Task<TeamRequest[]> GetAllAsync(Expression<Func<TeamRequest, bool>> filter, CancellationToken cancellationToken)
        {
            return await _context.TeamRequests
                .Where(filter)
                .Include(_ => _.Requests)
                .Include(_ => _.User)
                .ToArrayAsync(cancellationToken);
        }

        public async Task<TeamRequest[]> GetAllByUserAsync(int userId)
        {
            return await _context.TeamRequests
                .Include(_ => _.Requests)
                .Where(_ => _.UserId == userId)
                .ToArrayAsync();
        }

        public void Delete(TeamRequest teamRequest)
        {
            _context.TeamRequests.Remove(teamRequest);
        }
    }
}
