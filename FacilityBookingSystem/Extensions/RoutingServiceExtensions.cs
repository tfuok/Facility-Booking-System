namespace FacilityBookingSystem.Extensions
{
    public static class RoutingServiceExtensions
    {
        public static IServiceCollection AddCustomRouting(this IServiceCollection services)
        {
            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.AppendTrailingSlash = false;
            });

            return services;
        }
    }
}
