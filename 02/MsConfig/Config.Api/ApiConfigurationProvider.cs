namespace Config.Api
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Net.Http;
    using System.Threading;

    internal class ApiConfigurationProvider : ConfigurationProvider, IDisposable
    {
        private readonly Timer _timer;

        private readonly ApiConfigurationSource _apiConfigurationSource;

        public ApiConfigurationProvider(ApiConfigurationSource apiConfigurationSource)
        {
            _apiConfigurationSource = apiConfigurationSource;

            _timer = new Timer(x => Load(), 
                null, 
                TimeSpan.FromSeconds(_apiConfigurationSource.Period), 
                TimeSpan.FromSeconds(_apiConfigurationSource.Period));
        }

        public void Dispose()
        {
            _timer?.Change(Timeout.Infinite, 0);
            _timer?.Dispose();
            Console.WriteLine("Dispose timer");
        }

        public override void Load()
        {
            try
            {
                var url = $"{_apiConfigurationSource.ReqUrl}?appName={_apiConfigurationSource.AppName}&env={_apiConfigurationSource.Env}";

                using (HttpClient client = new HttpClient())
                {
                    var resp = client.GetAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
                    var res = resp.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                    var config = Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigResult>(res);

                    if (config.Code == 0)
                    {
                        Data = config.Data;
                        OnReload();
                        Console.WriteLine($"update at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
                        Console.WriteLine($"{res}");
                    }
                    else
                    {
                        if (!_apiConfigurationSource.Optional)
                        {
                            throw new Exception($"can not load config from {_apiConfigurationSource.ReqUrl}");
                        }
                    }
                }
            }
            catch
            {
                if (!_apiConfigurationSource.Optional)
                {
                    throw new Exception($"can not load config from {_apiConfigurationSource.ReqUrl}");
                }
            }
        }
    }
}
