using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Filters;
using ERNI.PBA.Server.Host.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;

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

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IBudgetRepository, BudgetRepository>();
            services.AddTransient<IRequestRepository, RequestRepository>();
            services.AddTransient<IRequestCategoryRepository, RequestCategoryRepository>();
            services.AddTransient<IInvoiceImageRepository, InvoiceImageRepository>();
            services.AddTransient<ApiExceptionFilter>();

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
                                    new System.Security.Claims.ClaimsIdentity(claims, null, null, Claims.Role));
                            }
                        }
                    };
                });

            services.AddAuthorization();

            services.AddQuartz(typeof(DailyMailNotifications), Configuration["Crons:EmailCron"]);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                c.OperationFilter<ExamplesOperationFilter>();

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    In = "header",
                    Description = "Please enter into field the word 'Bearer' following by space and JWT",
                    Name = "Authorization",
                    Type = "apiKey"
                });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> { { "Bearer", Enumerable.Empty<string>() } });
            });

            services.AddMvc(c => { c.Filters.AddService<ApiExceptionFilter>(); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) // , ILogger<Startup> logger)
        {
            app.UseCors("CorsPolicy");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseQuartz();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvc();
        }
    }
}
