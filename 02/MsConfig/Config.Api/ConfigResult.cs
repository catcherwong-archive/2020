namespace Config.Api
{
    using System.Collections.Generic;

    public class ConfigResult
    {
        public int Code { get; set; }

        public string Msg { get; set; }

        public Dictionary<string, string> Data { get; set; }
    }
}
