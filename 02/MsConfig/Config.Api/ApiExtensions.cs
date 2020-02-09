namespace Config.Api
{
    using Microsoft.Extensions.Configuration;

    public static class ApiExtensions
    {
        public static IConfigurationBuilder AddApiConfiguration(
            this IConfigurationBuilder builder, string appName, string env, string url, int period = 10, bool optional = false)
        {
            return builder.Add(new ApiConfigurationSource(appName, env, url, period, optional)) ;
        }
    }
}
