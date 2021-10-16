using System.Collections.Generic;
using System.Reflection;
using Autofac;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Domain.Interfaces.Export;
using ERNI.PBA.Server.Domain.Interfaces.Services;
using ERNI.PBA.Server.Host.Auth;
using ERNI.PBA.Server.Host.Filters;
using ERNI.PBA.Server.Host.Services;
using ERNI.PBA.Server.Host.Utils;
using ERNI.Rmt.ExcelExport;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Constants = ERNI.PBA.Server.Domain.Constants;

namespace ERNI.PBA.Server.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options
                            =>
                                options.AddPolicy(
                                    "CorsPolicy",
                                    b => b.AllowAnyOrigin()
                                        .AllowAnyMethod()
                                        .AllowAnyHeader()));

            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ConnectionString")));

            services.AddAzureClients(o => o.AddBlobServiceClient(Configuration["Storage:ConnectionString"]));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAd"));

            services.AddAuthorization(options =>
                options.AddPolicy(Constants.ClientPolicy, builder => builder.RequireScope("pba_client")));

            services.AddAuthorization();

            services.AddQuartz(typeof(DailyMailNotifications), Configuration["Crons:EmailCron"]);

            services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                c.ExampleFilters();

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter into field the word 'Bearer' following by space and JWT",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
            });

            services.AddMvc(configuration =>
            {
                configuration.EnableEndpointRouting = false;
                configuration.Filters.AddService<ApiExceptionFilter>();
            });

            services.Configure<BlobStorageSettings>(Configuration.GetSection(BlobStorageSettings.SectionName));

            services.AddOptions();
        }

        public void ConfigureProductionContainer(ContainerBuilder builder)
        {
            builder.RegisterType<MailService>().As<IMailService>().InstancePerDependency();

            ConfigureModules(builder);
        }

        public void ConfigureDevelopmentContainer(ContainerBuilder builder)
        {
            builder.RegisterType<MailServiceMock>().As<IMailService>().InstancePerDependency();

            ConfigureModules(builder);
        }

        public void ConfigureModules(ContainerBuilder builder)
        {
            builder.RegisterModule(new ApplicationModule());

            builder.RegisterType<DownloadTokenManager>().As<IDownloadTokenManager>().SingleInstance();
            builder.RegisterType<ExcelExportService>().As<IExcelExportService>().InstancePerLifetimeScope();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) // , ILogger<Startup> logger)
        {
            app.UseCors("CorsPolicy");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
                    c.OAuthAppName("ERNI Resource Management Tool Client");
                });
            }

            app.UseAuthentication();

            if (!env.IsDevelopment())
            {
                app.UseQuartz();
            }

            app.UseMvc();
        }
    }
}
