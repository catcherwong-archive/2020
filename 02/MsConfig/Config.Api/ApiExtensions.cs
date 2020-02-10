namespace Microsoft.Extensions.Configuration
{
    using Config.Api;
    using System;

    public static class ApiExtensions
    {
        public static IConfigurationBuilder AddApiConfiguration(
            this IConfigurationBuilder builder, Action<ApiConfigurationSource> action)
        {
            var source = new ApiConfigurationSource();

            action(source);

            return builder.Add(source);
        }
    }
}
