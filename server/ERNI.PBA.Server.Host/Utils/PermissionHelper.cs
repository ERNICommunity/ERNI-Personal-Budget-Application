using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Repository;

namespace ERNI.PBA.Server.Host.Utils
{
    public class PermissionHelper
    {
        private readonly IUserRepository _userRepository;
        private readonly IRequestRepository _requestRepository;

        public PermissionHelper(IUserRepository userRepository,
            IRequestRepository requestRepository)
        {
            _userRepository = userRepository;
            _requestRepository = requestRepository;
        }

        public async Task<bool> IsUsersRequestOrAdmin(int userId,
            int requestId, 
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUser(userId, cancellationToken);
            if (user.IsAdmin) return true;
            var request = await _requestRepository.GetRequest(requestId, cancellationToken);
            return request?.UserId == userId;
        }
    }
}
