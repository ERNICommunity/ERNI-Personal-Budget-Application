﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using server.Controllers;
using System;

namespace ERNI.PBA.Server.Host.Utils
{
    public static class QuartzExtensions
    {
        public static void AddQuartz(this IServiceCollection services, Type jobType, string cron)
        {
            services.Add(new ServiceDescriptor(typeof(IJob), jobType, ServiceLifetime.Transient));
            services.AddSingleton<IJobFactory, ScheduledJobFactory>();
            services.AddSingleton(provider =>
            {
                return JobBuilder.Create<DailyMailNotifications>()
                  .Build();
            });

            services.AddSingleton(provider =>
            {
                return TriggerBuilder.Create()
                .StartNow()
                .WithCronSchedule(cron)
                .Build();
            });

            services.AddSingleton(provider =>
            {
                var schedulerFactory = new StdSchedulerFactory();
                var scheduler = schedulerFactory.GetScheduler().Result;
                scheduler.JobFactory = provider.GetService<IJobFactory>();
                scheduler.Start();
                return scheduler;
            });
        }

        public static void UseQuartz(this IApplicationBuilder app)
        {
            app.ApplicationServices.GetService<IScheduler>()
                .ScheduleJob(app.ApplicationServices.GetService<IJobDetail>(),
                app.ApplicationServices.GetService<ITrigger>()
                );
        }
    }
}