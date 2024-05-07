using System;
using Quartz;
using Quartz.Spi;

namespace ERNI.PBA.Server.Host.Utils
{
    public class ScheduledJobFactory(IServiceProvider serviceProvider) : IJobFactory
    {
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler) =>
            (serviceProvider.GetService(typeof(IJob)) as IJob)!;

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}