using Microsoft.Extensions.DependencyInjection;

namespace MiniGram.Client;

public static class MiniGramClientExtensions
{
    public static IServiceCollection AddMiniGramClient<T>(
        this IServiceCollection services,
        string baseUrl = null) where T : DelegatingHandler
    {
        services.AddScoped<T>();
        var url = baseUrl;
        services.AddHttpClient<IMiniGramClient, MiniGramClient>(client
                => client.BaseAddress = new Uri(url))
            .AddHttpMessageHandler<T>();

        return services;
    }
}
