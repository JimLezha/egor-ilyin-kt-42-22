using egorIlyinKT_42_22.Services.DepartmentServices;
using egorIlyinKT_42_22.Services.DisciplineServices;
using egorIlyinKT_42_22.Services.TeacherServices;
using egorIlyinKT_42_22.Services.LoadServices;



namespace egorIlyinKT_42_22.ServiceExtensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            
            services.AddScoped<TeacherService>();
            
            services.AddScoped<DisciplineService>();
            services.AddScoped<LoadService>();
            services.AddScoped<DepartmentService>();

            return services;
        }
    }
}
