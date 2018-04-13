using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using mvcCookieAuthSample.Data;
using Microsoft.EntityFrameworkCore;
using mvcCookieAuthSample.Models;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Services;
using IdentityServer4.EntityFramework;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;

namespace mvcCookieAuthSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            const string connectString = @"Data Source=.\GCPACSWS;database=IdentityServer4.Quickstart.EntityFramework-2.0.0;trusted_connection=yes;";
            var migrationAssemblyName = typeof(Startup).Assembly.GetName().Name;

            //Enable asp.net core Identity
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<ApplicationUser, ApplicationUserRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //     .AddCookie(options => {
            //         options.LoginPath = "/Account/Login";
            //     });

            // services.Configure<IdentityOptions>(options =>
            // {
            //     options.Password.RequireLowercase = true;
            //     options.Password.RequireNonAlphanumeric = true;
            //     options.Password.RequireUppercase = true;
            //     options.Password.RequiredLength = 12;
            // });

            services.AddIdentityServer()
            .AddDeveloperSigningCredential()
            // change from memory to DB for clients,apiresources and IdentityResources info 
            //.AddInMemoryClients(Config.GetClients())
            //.AddInMemoryApiResources(Config.GetApiResources())
            //.AddInMemoryIdentityResources(Config.GetIdentityResources())
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder =>
                {
                    builder.UseSqlServer(connectString, sql => sql.MigrationsAssembly(migrationAssemblyName));
                };
            })
            // this adds the operational data from DB (codes, tokens, consents)
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder =>
                {
                    builder.UseSqlServer(connectString, sql => sql.MigrationsAssembly(migrationAssemblyName));
                };
            })


            //.AddTestUsers(Config.GetTestUsers()); 去除TestUser,Enable AspNetIdentity
            .AddAspNetIdentity<ApplicationUser>()
            .Services.AddScoped<IProfileService, Services.ProfileService>();

            services.AddScoped<Services.ConsentService>();//添加ConsentService的依赖注入
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            //app.UseAuthentication();
            InitializeIdentityServerDatabase(app);
            app.UseIdentityServer();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }

        private void InitializeIdentityServerDatabase(IApplicationBuilder app)
        {
            //app.ApplicationServices 是 IServiceProvider 
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var configDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                configDbContext.Database.Migrate();
                if (!configDbContext.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        configDbContext.Clients.Add(client.ToEntity());
                    }
                    configDbContext.SaveChanges();
                }
                if (!configDbContext.ApiResources.Any())
                {
                    foreach (var api in Config.GetApiResources())
                    {
                        configDbContext.ApiResources.Add(api.ToEntity());
                    }
                    configDbContext.SaveChanges();
                }
                if (!configDbContext.IdentityResources.Any())
                {
                    foreach (var identity in Config.GetIdentityResources())
                    {
                        configDbContext.IdentityResources.Add(identity.ToEntity());
                    }
                    configDbContext.SaveChanges();
                }
            }
            
        }
    }
}
