using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Graph;
using Microsoft.Extensions.Configuration;

namespace ERNI.PBA.Server.Business.Queries.Employees
{
    public class SyncUserObjectIdCommand
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SyncUserObjectIdCommand(IConfiguration configuration, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Execute()
        {
            var config = new GraphConfiguration(
                _configuration["Graph:ClientId"],
                _configuration["Graph:TenantId"],
                _configuration["Graph:ClientSecret"]);


            var dbUsers = await _userRepository.GetAllUsers(CancellationToken.None);



            var f = new GraphFacade(config);

            var adUsers = await f.GetUsers();

            var userDict = dbUsers.ToDictionary(_ => _.Username.ToUpperInvariant());

            foreach (var u in adUsers)
            {
                if (userDict.TryGetValue(u.UserPrincipalName.ToUpperInvariant(), out var dbUser) && dbUser.ObjectId != Guid.Parse(u.Id))
                {
                    dbUser.ObjectId = Guid.Parse(u.Id);
                }
            }

            await _unitOfWork.SaveChanges(CancellationToken.None);
        }
    }
}
