using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Host.Services;
using Microsoft.Extensions.Configuration;
using Quartz;

namespace ERNI.PBA.Server.Host.Utils
{
    [DisallowConcurrentExecution]
    public class DailyMailNotifications : IJob
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IUserRepository _userRepository;
        private readonly MailService _mailService;
        /* private readonly ILogger _logger; */

        public DailyMailNotifications(IRequestRepository requestRepository, IUserRepository userRepository, IConfiguration configuration) // , ILogger<DailyMailNotifications> logger)
        {
            _requestRepository = requestRepository;
            _userRepository = userRepository;
            _mailService = new MailService(configuration);

            // _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            // await SendNotificationsForPendingRequests(context.CancellationToken);
            await SendNotificationsToAdmins(context.CancellationToken);

            // _logger.LogInformation("Scheduled Job for notifications executed");
        }

        private async Task SendNotificationsForPendingRequests(CancellationToken cancellationToken)
        {
            var pendingRequests = await _requestRepository.GetRequests(
            _ => _.Year == DateTime.Now.Year && _.State == RequestState.Pending, cancellationToken);

            if (pendingRequests.Any())
            {
                var requestBySuperior = pendingRequests
                    .Where(_ => _.User.Superior != null)
                    .GroupBy(_ => _.User.Superior);

                foreach (var group in requestBySuperior)
                {
                    var msg = new StringBuilder("You have new requests to handle");
                    msg.AppendLine();
                    msg.AppendLine();

                    foreach (var request in group)
                    {
                        msg.AppendLine($"   {request}");
                    }

                    _mailService.SendMail(msg.ToString(), group.Key.Username);
                }
            }
        }

        private async Task SendNotificationsToAdmins(CancellationToken cancellationToken)
        {
            var pendingRequests = await _requestRepository.GetRequests(
                _ => _.Year == DateTime.Now.Year && (_.State == RequestState.ApprovedBySuperior
                || _.State == RequestState.Pending), cancellationToken);

            if (!pendingRequests.Any())
            {
                return;
            }

            var admins = await _userRepository.GetAdminUsers(cancellationToken);
            var adminsMails = admins.Select(u => u.Username).ToArray();

            foreach (var mail in adminsMails)
            {
                var msg = new StringBuilder("You have new requests to handle");
                msg.AppendLine();
                msg.AppendLine();

                foreach (var request in pendingRequests)
                {
                    msg.AppendLine($"   {request}");
                }

                _mailService.SendMail(msg.ToString(), mail);
            }
        }
    }
}