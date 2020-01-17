namespace ERNI.PBA.Server.DataAccess.Repository
{
    public class TeamRequestRepository
    {
        private readonly DatabaseContext _context;

        public TeamRequestRepository(DatabaseContext context)
        {
            _context = context;
        }
    }
}
