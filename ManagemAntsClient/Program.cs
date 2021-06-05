using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sentry;

namespace ManagemAntsClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (SentrySdk.Init(o =>
            {
                o.Dsn = "https://6cc884f5828a4dbda5c34fa0f8e6230f@o795773.ingest.sentry.io/5801915";
                o.Debug = true;
                o.TracesSampleRate = 1.0;
            }))
            {
                CreateHostBuilder(args).Build().Run();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
