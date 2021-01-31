using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Autofac;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.Domain.Interfaces.Export;
using ERNI.PBA.Server.Domain.Interfaces.Services;
using ERNI.PBA.Server.Domain.Security;
using ERNI.PBA.Server.ExcelExport;
using ERNI.PBA.Server.Host.Filters;
using ERNI.PBA.Server.Host.Services;
using ERNI.PBA.Server.Host.Utils;
using ERNI.Rmt.ExcelExport;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace ERNI.PBA.Server.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

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

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.Authority = string.Concat(Configuration["Authentication:AzureAd:AadInstance"], Configuration["Authentication:AzureAd:Tenant"]);
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidIssuer = string.Concat(Configuration["Authentication:AzureAd:AadInstance"], Configuration["Authentication:AzureAd:Tenant"], "/v2.0"),
                        ValidAudience = Configuration["Authentication:AzureAD:ClientId"],

                        // IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };

                    cfg.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            var db = context.HttpContext.RequestServices.GetRequiredService<DatabaseContext>();
                            var sub = context.Principal.Claims.Single(c => c.Type == "sub").Value;
                            var user = await db.Users.SingleOrDefaultAsync(_ => _.UniqueIdentifier == sub);

                            if (user != null)
                            {
                                var claims = new List<Claim> { new Claim(Claims.Id, user.Id.ToString()) };

                                if (user.IsAdmin)
                                {
                                    claims.Add(new Claim(Claims.Role, Roles.Admin));
                                }

                                if (user.IsViewer)
                                {
                                    claims.Add(new Claim(Claims.Role, Roles.Viewer));
                                }

                                context.Principal.AddIdentity(
                                    new ClaimsIdentity(claims, null, null, Claims.Role));
                            }
                        }
                    };
                });

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
            }

            app.UseAuthentication();

            if (!env.IsDevelopment())
            {
                app.UseQuartz();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvc();
        }
    }
}
