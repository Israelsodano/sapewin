using DPA.Sapewin.CalculationWorkflow.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DPA.Sapewin.CalculationWorkflow.Application.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
            => services.InjectServices();

        private static IServiceCollection InjectServices(this IServiceCollection services)
            => services.AddScoped<IEmployeeCalendarService, EmployeeCalendarService>()
                        .AddScoped<IAppointmentsService, AppointmentsService>()
                        .AddScoped<IClearTablesService, ClearTablesService>();
    }
}