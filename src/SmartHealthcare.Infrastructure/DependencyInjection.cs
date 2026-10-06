using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.Repositories;
using SmartHealthcare.Domain.Entities;
using SmartHealthcare.Infrastructure.Identity;
using SmartHealthcare.Infrastructure.Persistence.UnitOfWork;
using SmartHealthcare.Infrastructure.Persistence.DbContext;
using SmartHealthcare.Infrastructure.Persistence.Repositories;

namespace SmartHealthcare.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Server=localhost;Database=SmartHealthcareDb;Trusted_Connection=True;TrustServerCertificate=True;";

        // EF Core Registration
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // ASP.NET Core Identity Registration
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ISignInService, SignInService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Email & Notification Services
        services.Configure<SmartHealthcare.Infrastructure.Services.EmailSettings>(
            configuration.GetSection("EmailSettings"));
        services.AddScoped<IEmailSender, SmartHealthcare.Infrastructure.Services.EmailSender>();
        services.AddScoped<INotificationSender>(sp => sp.GetRequiredService<IEmailSender>());

        // Repositories & Unit Of Work
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IDoctorRepository, DoctorRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();

        return services;
    }
}
