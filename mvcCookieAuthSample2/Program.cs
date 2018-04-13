using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using mvcCookieAuthSample.Data;

namespace mvcCookieAuthSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args)
                 .MigrateDbContext<ApplicationDbContext>((context, services) =>
                 {
                     new ApplicationDbContextSeed().SeedAsync(context, services)
                     .Wait();
                 })
                .Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseEnvironment("Development")
                .UseUrls("http://localhost:5005")
                .UseStartup<Startup>()
                .Build();
    }
}
