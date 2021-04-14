using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Users;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Interfaces.Services;
using ERNI.PBA.Server.Domain.Models.Entities;

namespace ERNI.PBA.Server.Business.Commands.Users
{
    public class SynchronizeUsersCommand : Command, ISynchronizeUsersCommand
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserResourceService _userResourceService;
        private readonly IUnitOfWork _unitOfWork;

        public SynchronizeUsersCommand(
            IUserRepository userRepository,
            IUserResourceService userResourceService,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _userResourceService = userResourceService;
            _unitOfWork = unitOfWork;
        }

        protected override async Task Execute(ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var resources = (await _userResourceService.GetAsync()).ToList();

            var existingUsers = (await _userRepository.GetAllUsers(cancellationToken)).ToList();
            var userIds = existingUsers.Select(u => u.ObjectId).ToHashSet();

            var newResources = resources.Where(r => !userIds.Contains(r.Id));

            // 1. Update information about existing users
            foreach (var user in existingUsers)
            {
                var resource = resources.FirstOrDefault(r => r.Id == user.ObjectId);

                if (resource is not null)
                {
                    var names = resource.Name.Split(new[] { ' ' }, 2);
                    user.FirstName = names[0];
                    user.LastName = names[1];
                    user.Username = resource.Email!;
                    user.Utilization = resource.Fte;
                    user.State = UserState.Active;
                }
                else
                {
                    // User not found in Resource Tool Management
                    user.State = UserState.Inactive;
                }
            }

            // 2. Add new users and save changes to get their db generated Ids
            var newUsers = newResources.Select(r => new User
            {
                FirstName = "",
                LastName = r.Name,
                Username = r.Email!,
                ObjectId = r.Id,
                State = UserState.Active,
                Utilization = r.Fte
            }).ToList();

            await _userRepository.AddUsersAsync(newUsers);

            await _unitOfWork.SaveChanges(cancellationToken);

            // 3. Update superiors to active users (existing + new ones)
            var activeUsers = existingUsers.Where(u => u.State != UserState.Inactive).Union(newUsers).ToList();
            foreach (var user in activeUsers)
            {
                var managerId = resources.FirstOrDefault(r => r.Id == user.ObjectId)?.ManagerId;
                user.SuperiorId = activeUsers.FirstOrDefault(u => u.ObjectId == managerId)?.Id;
            }

            await _unitOfWork.SaveChanges(cancellationToken);
        }
    }
}