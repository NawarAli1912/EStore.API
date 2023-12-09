using Application.Common.Authentication.Jwt;
using Application.Common.Data;
using Application.Common.Repository;
using Domain.Authentication;
using Domain.ModelsSnapshots;
using Infrastructure.Authentication;
using Infrastructure.Authentication.Models;
using Infrastructure.BackgroundJobs;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Persistence.Repostiory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Nest;
using Quartz;
using Serilog;
using System.Text;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuth(configuration);

        services.AddElasticSearch(configuration);

        services.AddScoped<ConvertDomainEventsToOutboxMessagesInterceptor>();

        services.AddDbContext<ApplicationDbContext>(
            (sp, options) =>
            {
                var interceptor = sp.GetService<ConvertDomainEventsToOutboxMessagesInterceptor>();

                options.UseSqlServer(configuration.GetConnectionString("Default"))
                    .AddInterceptors(interceptor);
            });

        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

        services.AddScoped<IProductsRepository, ProductsRepository>();
        services.AddScoped<ICategoriesRepository, CategoriesRepository>();

        services.AddQuartz(configure =>
        {
            var jobKey = new JobKey(nameof(ProcessOutboxMessagesJob));
            configure.AddJob<ProcessOutboxMessagesJob>(jobKey)
            .AddTrigger(
                trigger =>
                trigger.ForJob(jobKey)
                    .WithSimpleSchedule(
                        schedule =>
                        schedule.WithIntervalInSeconds(10)
                        .RepeatForever()));
        });

        services.AddQuartzHostedService();

        return services;
    }

    private static IServiceCollection AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ElasticSearchSettings>(configuration.GetSection(JwtSettings.SectionName));
        var elasticSearchSettings = configuration.GetSection(ElasticSearchSettings.SectionName)
                        .Get<ElasticSearchSettings>();

        var connectionSettings = new ConnectionSettings(new Uri(elasticSearchSettings!.BaseUrl))
                            .DefaultIndex(elasticSearchSettings!.DefaultIndex)
                            .ThrowExceptions();

        var client = new ElasticClient(connectionSettings);

        services.AddSingleton<IElasticClient>(client);

        CreateIndex(client, elasticSearchSettings.DefaultIndex);

        return services;
    }

    private static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();

        services.AddIdentity<IdentityUser, Role>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireDigit = false;
            options.User.RequireUniqueEmail = true;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateActor = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                RequireExpirationTime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings!.Issuer,
                ValidAudience = jwtSettings!.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings!.Secret))
            };
        });

        services.AddScoped<IPermissionService, PermissionService>();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthroizationPolicyProvider>();

        return services;
    }

    private static void CreateIndex(ElasticClient client, string indexName)
    {
        try
        {
            var existsResponse = client.Indices.Exists(indexName);
            if (!existsResponse.Exists)
            {

                var createIndexResponse = client
                    .Indices
                    .Create(indexName, c => c
                    .Map<ProductSnapshot>(m => m.AutoMap()));
            }
        }
        catch (Exception ex)
        {
            Log.Error($"{nameof(DependencyInjection)} failed with error {ex.Message}.");
            // throw;
        }
    }
}
