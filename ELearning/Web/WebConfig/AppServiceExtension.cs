using Web.Services;

namespace Web.WebConfig
{
    public static class AppServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
        {
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddAppService(config, env);
            //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddServicesDependencies();

            return services;
        }
    }
}
