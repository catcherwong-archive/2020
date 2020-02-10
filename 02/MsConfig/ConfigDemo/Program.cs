namespace ConfigDemo
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Configuration;

    public class Program
    {
        private static IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddApiConfiguration(x => 
                    {
                        x.AppName = "Demo";
                        x.Env = context.HostingEnvironment.EnvironmentName;
                        x.ReqUrl = configuration["ConfigUrl"];
                        x.Period = 60;
                        x.Optional = false;
                    });
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseUrls("http://*:9633");
                });
    }
}
