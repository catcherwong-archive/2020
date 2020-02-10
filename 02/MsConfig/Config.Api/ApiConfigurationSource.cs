namespace Config.Api
{
    using Microsoft.Extensions.Configuration;

    public class ApiConfigurationSource : IConfigurationSource
    {
        public string ReqUrl { get; set; }

        public int Period { get; set; }

        public bool Optional { get; set; }

        public string AppName { get; set; }

        public string Env { get; set; }


        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ApiConfigurationProvider(this);
        }
    }
}
