using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace ERNI.PBA.Server
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
                                options.AddPolicy("CorsPolicy",
                                    b => b.AllowAnyOrigin()
                                        .AllowAnyMethod()
                                        .AllowAnyHeader()
                                        .AllowCredentials()));

            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ConnectionString")));

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IBudgetRepository, BudgetRepository>();
            services.AddTransient<IRequestRepository, RequestRepository>();
            services.AddTransient<IRequestCategoryRepository, RequestCategoryRepository>();

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
                    cfg.Authority = String.Concat(Configuration["Authentication:AzureAd:AadInstance"], Configuration["Authentication:AzureAd:Tenant"]);
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {

                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidIssuer = String.Concat(Configuration["Authentication:AzureAd:AadInstance"], Configuration["Authentication:AzureAd:Tenant"], "/v2.0"),
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

                            if (user == null)
                            {
                                user = new User
                                {
                                    UniqueIdentifier = sub,
                                    FirstName = context.Principal.Claims.Single(c => c.Type == "given_name").Value,
                                    LastName = context.Principal.Claims.Single(c => c.Type == "family_name").Value,
                                    Username = context.Principal.Claims.Single(c => c.Type == "upn").Value
                                };
                                db.Users.Add(user);
                                db.SaveChanges();
                            }

                            var claims = new List<System.Security.Claims.Claim>();
                            claims.Add(new System.Security.Claims.Claim(Claims.Id, user.Id.ToString()));
                            claims.Add(new System.Security.Claims.Claim(Claims.FirstName, user.FirstName));
                            claims.Add(new System.Security.Claims.Claim(Claims.LastName, user.LastName));

                            if (user.IsAdmin)
                            {
                                claims.Add(new System.Security.Claims.Claim(Claims.Role, "admin"));
                            }

                            context.Principal.AddIdentity(new System.Security.Claims.ClaimsIdentity(claims, null, null, Claims.Role));
                        }
                    };

                });


            services.AddAuthorization();

            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                c.OperationFilter<ExamplesOperationFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger<Startup> logger)
        {
            app.UseCors("CorsPolicy");

            //if (env.IsDevelopment())
            //{
                app.UseDeveloperExceptionPage();
            //}
            app.UseAuthentication();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.ConfigureExceptionHandler(logger);

            app.UseMvc();
        }
    }
}
