using GraphClient;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddBlazoradeMsal((sp, options) =>
    {
        var root = sp.GetService<IConfiguration>() ?? throw new Exception($"Cannot get service instance for {typeof(IConfiguration).FullName}.");
        var config = root.GetSection("app");
        config.Bind(options);
    })
    ;

await builder.Build().RunAsync();
