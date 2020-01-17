using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;

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
    }
}
