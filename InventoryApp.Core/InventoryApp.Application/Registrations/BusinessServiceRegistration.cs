using System.Text;
using FluentValidation;
using InventoryApp.Application.Providers.Service;
using InventoryApp.Application.Providers.Validator;
using InventoryApp.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace InventoryApp.Application.Registrations;

public static class BusinessServiceRegistration
{
    public static IServiceCollection AddAuthService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
            {

                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)
                    )
                };
            });

            return services;
    }
    public static IServiceCollection AddBusinessService(this IServiceCollection services)
    {   
        ServiceRegistrationProvider.RegisterServices(services);

        return services;
    }
    public static IServiceCollection AddValidatorService(this IServiceCollection services)
    {
        var validatorAssemblies = ValidatorAssemblyProvider.GetValidatorAssemblies();

        var createDtoValidatorAssemblies = CreateDTOValidatorAssemblyProvider.GetValidatorAssemblies();

        var updateDtoValidatorAssemblies = UpdateDTOAssemblyProvider.GetValidatorAssemblies();

        var authDtoValidatorAssemblies = AuthDTOValidatorAssemblyProvider.GetValidatorAssemblies();

        foreach (var assemblytype in authDtoValidatorAssemblies)
            services.AddValidatorsFromAssemblyContaining(assemblytype);

        foreach (var assemblyType in createDtoValidatorAssemblies)
            services.AddValidatorsFromAssemblyContaining(assemblyType);

        foreach (var assemblyType in updateDtoValidatorAssemblies)
            services.AddValidatorsFromAssemblyContaining(assemblyType);

        foreach (var assemblyType in validatorAssemblies)
            services.AddValidatorsFromAssemblyContaining(assemblyType);

        services.AddHostedService<TokenCleanupService>();


        return services;
    }
}