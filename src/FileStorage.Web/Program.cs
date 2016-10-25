using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace FileStorage.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel(s=>s.MaxRequestBufferSize = Int64.MaxValue)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
