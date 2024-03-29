using Blazorade.Msal.Configuration;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWasmSample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services
                .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
                .AddBlazoradeMsal((sp, o) =>
                {
                    var root = sp.GetService<IConfiguration>();
                    var config = root.GetSection("app");
                    config.Bind(o);

                    o.PostLogoutUrl = "/loggedout";
                    o.InteractiveLoginMode = InteractiveLoginMode.Redirect;
                })
                ;

            await builder.Build().RunAsync();
        }
    }
}
