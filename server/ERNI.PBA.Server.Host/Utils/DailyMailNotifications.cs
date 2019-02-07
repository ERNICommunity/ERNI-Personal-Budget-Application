using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Services;
using Microsoft.Extensions.Configuration;
using Quartz;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERNI.PBA.Server.Host.Utils
{
    [DisallowConcurrentExecution]
    public class DailyMailNotifications : IJob
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IUserRepository _userRepository;
        private readonly MailService _mailService;
        // private readonly ILogger _logger;

        public DailyMailNotifications(IRequestRepository requestRepository, IUserRepository userRepository,IConfiguration configuration)//, ILogger<DailyMailNotifications> logger)
        {
            _requestRepository = requestRepository;
            _userRepository = userRepository;
            _mailService = new MailService(configuration);
            //_logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await SendNotificationsForPendingRequests(context.CancellationToken);
            await SendNotificationsForApprovedBySuperiorRequests(context.CancellationToken);
            // _logger.LogInformation("Scheduled Job for notifications executed");
        }

        private async Task SendNotificationsForPendingRequests(CancellationToken cancellationToken)
        {
            var pendingRequests = await _requestRepository.GetRequests(
            _ => _.Year == DateTime.Now.Year && _.State == RequestState.Pending, cancellationToken);

            if (pendingRequests.Any())
            {
                var superiorsMails = pendingRequests.Where(_ => _.User.Superior != null).Select(_ => new
                {
                    _.User.Superior.Username,
                }).Distinct();

                foreach (var mail in superiorsMails)
                {
                    _mailService.SendMail("You have new requests to handle", mail.Username);
                }
            }
        }

        private async Task SendNotificationsForApprovedBySuperiorRequests(CancellationToken cancellationToken)
        {
            var approvedBySuperiorRequests = await _requestRepository.GetRequests(
            _ => _.Year == DateTime.Now.Year && _.State == RequestState.ApprovedBySuperior, cancellationToken);

            if (approvedBySuperiorRequests.Any())
            {
                var admins = await _userRepository.GetAdminUsers(cancellationToken);
                var adminsMails = admins.Select(u => u.Username).ToArray();

                foreach (var mail in adminsMails)
                {
                    _mailService.SendMail("You have new requests to handle", mail);
                }
            }
        }
    }
}
