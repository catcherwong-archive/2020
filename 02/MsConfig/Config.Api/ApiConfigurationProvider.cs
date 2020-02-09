namespace Config.Api
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Net.Http;
    using System.Threading;

    public class ApiConfigurationProvider : ConfigurationProvider, IDisposable
    {
        private Timer _timer;

        public string ReqUrl { get; private set; }

        public int Period { get; private set; }

        public bool Optional { get; private set; }

        public string AppName { get; private set; }

        public string Env { get; private set; }

        public ApiConfigurationProvider(string appName, string env, string url, int second, bool optional = false)
        {
            AppName = appName;
            Env = env;
            ReqUrl = url;
            Period = second;
            Optional = optional;

            _timer = new Timer(x => Load(), null, TimeSpan.Zero, TimeSpan.FromSeconds(Period));
        }


        public void Dispose()
        {
            _timer.Dispose();

            Console.WriteLine("Dispose timer");
        }

        public override void Load()
        {
            try
            {
                var url = $"{ReqUrl}?appName={AppName}&env={Env}";

                using (HttpClient client = new HttpClient())
                {
                    var resp = client.GetAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
                    var res = resp.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                    var config = Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigResult>(res);

                    if (config.Code == 0)
                    {
                        Data = config.Data;
                        Console.WriteLine($"update at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
                        Console.WriteLine($"{res}");
                    }
                    else
                    {
                        if (!Optional)
                        {
                            throw new Exception($"can not load config from {ReqUrl}");
                        }
                    }
                }
            }
            catch
            {
                if (!Optional)
                {
                    throw new Exception($"can not load config from {ReqUrl}");
                }
            }
        }
    }
}
