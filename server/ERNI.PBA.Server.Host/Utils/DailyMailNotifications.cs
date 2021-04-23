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
        private readonly MailService _mailService;
        private readonly string[] _notificationEmails;

        public DailyMailNotifications(IRequestRepository requestRepository, IConfiguration configuration)
        {
            _requestRepository = requestRepository;
            _mailService = new MailService(configuration);
            _notificationEmails = configuration["NotificationEmails"].Split(";");
        }

        public async Task Execute(IJobExecutionContext context) =>
            await SendNotificationsToAdmins(context.CancellationToken);

        private async Task SendNotificationsToAdmins(CancellationToken cancellationToken)
        {
            var pendingRequests = await _requestRepository.GetRequests(
                _ => _.Year == DateTime.Now.Year && (_.State == RequestState.Approved
                || _.State == RequestState.Pending), cancellationToken);

            if (!pendingRequests.Any())
            {
                return;
            }

            foreach (var mail in _notificationEmails)
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