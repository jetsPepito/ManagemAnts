using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sentry;

namespace ManagemAntsServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (SentrySdk.Init(o =>
            {
                o.Dsn = "https://f5c071c350fb4987b6775a8bd0bbf022@o795773.ingest.sentry.io/5801894";
                o.Debug = true;
                o.TracesSampleRate = 1.0;
            }))
            {
                SentrySdk.CaptureMessage("Oops, Something went wrong");
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
