namespace Config.Api
{
    using Microsoft.Extensions.Configuration;

    public class ApiConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// Specifies the url of RESTful API.
        /// </summary>
        public string ReqUrl { get; set; }

        /// <summary>
        /// Specifies the polling period.
        /// </summary>
        public int Period { get; set; }

        /// <summary>
        /// Specifies whether this source is optional.
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        /// Specifies the name of the application.
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// Specifies the env of the application.
        /// </summary>
        public string Env { get; set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ApiConfigurationProvider(this);
        }
    }
}
