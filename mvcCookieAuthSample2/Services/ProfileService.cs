using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using mvcCookieAuthSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace mvcCookieAuthSample.Services
{
    public class ProfileService : IProfileService
    {
        private UserManager<ApplicationUser> _userManager;
      

        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            //SubjectId对应数据库里的user表的Id（主键）
            var subjectId = context.Subject.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            //根据subjectId 拿到user信息
            var user = await _userManager.FindByIdAsync(subjectId);

            context.IssuedClaims = await GetClaimsFormUserAsync(user);
        }

        // Gets or sets a value indicating whether the subject is active and can recieve tokens
        public async Task IsActiveAsync(IsActiveContext context)
        {
            //SubjectId对应数据库里的user表的Id（主键）
            var subjectId = context.Subject.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            //根据subjectId 拿到user信息
            var user = await _userManager.FindByIdAsync(subjectId);
           
            context.IsActive = user != null; // user不为null,则给IsActive赋值true，否则false
        }

        private async Task<List<Claim>> GetClaimsFormUserAsync(ApplicationUser user)
        {
            //根据user实例化Claims
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject,user.Id.ToString()),//终端用户在发行方的唯一标识符(SubjectId)
                new Claim(JwtClaimTypes.PreferredUserName,user.UserName),//终端用户希望在RP中引用的缩写名
            };

            //根据给userManager传递user,获取到roles
           var roles = await _userManager.GetRolesAsync(user);
            // 添加roles
            foreach (var role in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role));
            }

            //添加头像
            if (!string.IsNullOrWhiteSpace(user.Avatar))
            {
                claims.Add(new Claim("avatar", user.Avatar));
            }

            return claims;
        }

    }
}
