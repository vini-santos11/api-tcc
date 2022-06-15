using Application.Converters;
using Application.Identity;
using Application.Middlewares;
using Cross.Cutting.Identity.Describers;
using Cross.Cutting.IoC;
using CrossCutting.Configurations;
using DbUp;
using Domain.Enumerables;
using Domain.Interfaces;
using Domain.Models;
using Infra.Data.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application
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
            #region [ CONFIGURATIONS ]
            services.Configure<TokenConfiguration>(options => Configuration.GetSection(nameof(TokenConfiguration)).Bind(options));
            services.Configure<TokenConfiguration>(options => Configuration.GetSection(nameof(PasswordConfiguration)).Bind(options));
            #endregion

            #region [ AUTHENTICATION AND AUTORIZATION ]
            services.AddIdentity<AppUser, ERoles>(options =>
            {
                var passwordConfiguration = new PasswordConfiguration();
                Configuration.GetSection(nameof(PasswordConfiguration)).Bind(passwordConfiguration);

                options.Tokens.PasswordResetTokenProvider = ResetPasswordTokenProvider.ProviderKey;
                options.Lockout.AllowedForNewUsers = passwordConfiguration.UserLockoutEnabled;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(passwordConfiguration.LockoutTimeSpan);
                options.Lockout.MaxFailedAccessAttempts = passwordConfiguration.LockoutMaxFailedAccess;

                options.SignIn.RequireConfirmedEmail = passwordConfiguration.RequireConfirmedEmail;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                options.Password.RequireDigit = passwordConfiguration.RequireDigit;
                options.Password.RequiredLength = passwordConfiguration.RequiredLength;
                options.Password.RequireNonAlphanumeric = passwordConfiguration.RequireNonAlphanumeric;
                options.Password.RequireUppercase = passwordConfiguration.RequireUppercase;
                options.Password.RequireLowercase = passwordConfiguration.RequireLowercase;

                options.User.RequireUniqueEmail = true;
            })
            .AddDefaultTokenProviders()
            .AddTokenProvider<ResetPasswordTokenProvider>(ResetPasswordTokenProvider.ProviderKey)
            .AddErrorDescriber<AuthorizationIdentityErrorDescriber>();

            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearerOptions =>
            {
                var tokenConfiguration = new TokenConfiguration();
                Configuration.GetSection(nameof(TokenConfiguration)).Bind(tokenConfiguration);

                var paramsValidation = bearerOptions.TokenValidationParameters;
                paramsValidation.ValidateIssuer = true;
                paramsValidation.ValidateAudience = true;
                paramsValidation.ValidateIssuerSigningKey = true;
                paramsValidation.ValidateLifetime = true;

                paramsValidation.IssuerSigningKey = tokenConfiguration.AccessKey;
                paramsValidation.ValidAudience = tokenConfiguration.Audience;
                paramsValidation.ValidIssuer = tokenConfiguration.Issuer;
                paramsValidation.ClockSkew = TimeSpan.Zero;

                bearerOptions.SaveToken = true;
            });

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy(JwtBearerDefaults.AuthenticationScheme, new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                    .RequireAuthenticatedUser().Build());
            });


            #endregion

             services.AddMvc(options =>
            {
                options.RespectBrowserAcceptHeader = false;
                options.ReturnHttpNotAcceptable = true;
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
                options.JsonSerializerOptions.Converters.Add(new StringConverter());
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            services.AddHttpContextAccessor();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserContext, UserContext>();
            services.AddScoped<IDBContext, DBContext>();

            services.AddServicesAndRepositories();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "Application", 
                    Version = "v1" ,
                    Description = "API TCC Documentation"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string>()
                    }
                });
            });

            #region [ CORS ]
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.SetIsOriginAllowed(origin => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                });
            });
            #endregion

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.EnableForHttps = true;
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Application v1"));
            }

            var connectionString = Configuration.GetConnectionString("Connection");
            EnsureDatabase.For.MySqlDatabase(connectionString);
            var upgrader = DeployChanges.To.MySqlDatabase(connectionString)
                .WithScriptsFromFileSystem(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Scripts"))
                .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
                throw new Exception("Failed to update database.");

            app.UseMiddleware(typeof(ExceptionMiddleware));

            app.UseRouting();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders(HeaderNames.ContentDisposition));
            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseResponseCaching();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
