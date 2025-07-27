using System.Net;
using Wishlist.Services.Scrapers;

namespace Wishlist.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProductScraper<TScraper>(this IServiceCollection services)
        where TScraper : class, IProductScraper
    {
        services.AddHttpClient<TScraper>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli
            });

        services.AddScoped<IProductScraper, TScraper>();

        return services;
    }
}