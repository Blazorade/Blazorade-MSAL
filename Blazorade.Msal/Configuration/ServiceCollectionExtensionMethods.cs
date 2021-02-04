using Blazorade.Msal.Configuration;
using Blazorade.Msal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for working with a <see cref="IServiceCollection"/> implementation.
    /// </summary>
    public static class ServiceCollectionExtensionMethods
    {

        public static IServiceCollection AddBlazoradeMsal(this IServiceCollection services)
        {
            return services
                .AddScoped<BlazoradeMsalService>()
                ;
        }

        public static IServiceCollection AddBlazoradeMsal(this IServiceCollection services, Action<BlazoradeMsalOptions> config)
        {
            return services
                .AddBlazoradeMsal()
                .AddBlazoradeMsal((sp, o) =>
                {
                    config?.Invoke(o);
                });
        }

        public static IServiceCollection AddBlazoradeMsal(this IServiceCollection services, Action<IServiceProvider, BlazoradeMsalOptions> config)
        {
            return services
                .AddBlazoradeMsal()
                .AddSingleton((p) =>
                {
                    var options = new BlazoradeMsalOptions();
                    config?.Invoke(p, options);
                    return options;
                });
        }
    }
}
