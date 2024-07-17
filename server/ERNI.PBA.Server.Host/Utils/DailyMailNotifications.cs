using System;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Host.Services;
using Microsoft.Extensions.Configuration;
using Quartz;

namespace ERNI.PBA.Server.Host.Utils;

[DisallowConcurrentExecution]
public class DailyMailNotifications(IRequestRepository requestRepository, IConfiguration configuration) : IJob
{
    private readonly MailService _mailService = new(configuration);
    private readonly string[] _notificationEmails =
            (configuration["NotificationEmails"] ??
             throw new InvalidOperationException("Missing 'NotificationEmails' configuration")).Split(";");

    public async Task Execute(IJobExecutionContext context) =>
        await SendNotificationsToAdmins(context.CancellationToken);

    private async Task SendNotificationsToAdmins(CancellationToken cancellationToken)
    {
        var pendingRequests = await requestRepository.GetRequests(
            _ => _.Year == DateTime.Now.Year && _.State == RequestState.Pending, cancellationToken);

        if (pendingRequests.Length == 0)
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
                msg.AppendLine(CultureInfo.InvariantCulture, $"   {request}");
            }

            _mailService.SendMail(msg.ToString(), mail);
        }
    }
}