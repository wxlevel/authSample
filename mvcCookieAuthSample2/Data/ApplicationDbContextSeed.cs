using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using mvcCookieAuthSample.Models;
using Microsoft.Extensions.DependencyInjection;

namespace mvcCookieAuthSample.Data
{
    public class ApplicationDbContextSeed
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationUserRole> _roleManager;
        public async Task SeedAsync(ApplicationDbContext context, IServiceProvider services)
        {
            if (!context.Roles.Any())
            {
                _roleManager = services.GetRequiredService<RoleManager<ApplicationUserRole>>();
                var role = new ApplicationUserRole()
                {
                    Name = "Administracor_roleName",
                    NormalizedName = "Administracor_roleNormalizedName"
                };
                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    throw new Exception("初始默认角色失败:" + result.Errors.SelectMany(e =>e.Description));
                }
            }
            if (!context.Users.Any())
            {
                _userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                var defaultUser = new ApplicationUser
                {
                    UserName = "level",
                    Email = "wxlevel@163.com",
                    NormalizedUserName = "admin",
                    Avatar = "https://stackify.com/wp-content/uploads/2017/10/NET-core-2.1-1-793x397.png",
                };

                var result = await _userManager.CreateAsync(defaultUser, "Password123.");
                try
                {
                    await _userManager.AddToRoleAsync(defaultUser, "Administracor_roleName");
                }
                catch (Exception ex)
                {

                    throw ex;
                }  
               
                if (!result.Succeeded)
                {
                    throw new Exception("初始默认用户失败:" + result.Errors.SelectMany(e => e.Description));
                }
            }
        }
    }
}
