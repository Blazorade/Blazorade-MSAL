using Blazorade.Msal.Configuration;
using Blazorade.Msal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensionMethods
    {

        public static IServiceCollection AddBlazoradeMsal(this IServiceCollection services)
        {

            return services;
        }

        public static IServiceCollection AddBlazoradeMsal(this IServiceCollection services, Action<BlazoradeMsalOptions> config)
        {
            return services
                .AddBlazoradeMsal((sp, o) =>
                {
                    config?.Invoke(o);
                });
        }

        public static IServiceCollection AddBlazoradeMsal(this IServiceCollection services, Action<IServiceProvider, BlazoradeMsalOptions> config)
        {
            return services
                .AddScoped<BlazoradeMsalService>()
                .AddSingleton((p) =>
                {
                    var options = new BlazoradeMsalOptions();
                    config?.Invoke(p, options);
                    return options;
                });
        }
    }
}
