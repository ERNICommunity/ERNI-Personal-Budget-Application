using ERNI.PBA.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("WebAPI",
        client => client.BaseAddress = new Uri("http://localhost:5001/api")).AddHttpMessageHandler(sp =>
        {
            var handler = sp.GetService<AuthorizationMessageHandler>()
            .ConfigureHandler(
                 authorizedUrls: new[] { "http://localhost:5001" },
                 scopes: new[] { "SCOPE" }
             );
            return handler;
        });

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("WebAPI"));

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
});

await builder.Build().RunAsync();
