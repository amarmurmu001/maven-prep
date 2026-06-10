using FluentValidation;
using LeaveManagementSystem.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace LeaveManagementSystem.Application.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ILeaveService, LeaveService>();
        services.AddScoped<IAdminService, AdminService>();

        return services;
    }
}
