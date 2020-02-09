namespace Config.Api
{
    using Microsoft.Extensions.Configuration;

    public class ApiConfigurationSource : IConfigurationSource
    {
        public string ReqUrl { get; private set; }

        public int Period { get; private set; }

        public bool Optional { get; private set; }

        public string AppName { get; private set; }

        public string Env { get; private set; }

        public ApiConfigurationSource(string appName, string env, string url, int second, bool optional = false)
        {
            AppName = appName;
            Env = env;
            ReqUrl = url;
            Period = second;
            Optional = optional;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ApiConfigurationProvider(AppName, Env, ReqUrl, Period, Optional);
        }
    }
}
