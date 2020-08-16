namespace PromDemo.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Prometheus;
    using System;

    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        private static readonly Counter OrderCreatedCount = Metrics
            .CreateCounter("yyyorder_created_total", "Number of created orders.", new CounterConfiguration
            {
                 LabelNames= new [] { "appkey", "opreator" }
            });

        private static readonly Counter OrderCanceledCount = Metrics
            .CreateCounter("yyyorder_canceled_total", "Number of canceled orders.", new CounterConfiguration
            {
                LabelNames = new[] { "appkey", "opreator" }
            });

        [HttpGet]
        public string Get()
        {
            var appKeys = new[] { "ali", "pdd", "mt" };
            var opreators = new[] { "cw", "pz" };

            var rd = new Random((int)DateTimeOffset.Now.ToUnixTimeMilliseconds()).Next(0, 2000);
            var appKeyidx = rd % 3;
            var opreatoidx = rd % 2;
            OrderCreatedCount.WithLabels(appKeys[appKeyidx], opreators[opreatoidx]).Inc();

            var cRd = new Random((int)DateTimeOffset.Now.ToUnixTimeMilliseconds()).NextDouble();

            if (cRd < 0.3d)
            {
                OrderCanceledCount.WithLabels(appKeys[appKeyidx], opreators[opreatoidx]).Inc();
            }

            return "ok";
        }
    }
}
