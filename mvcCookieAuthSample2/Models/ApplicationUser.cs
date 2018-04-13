using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvcCookieAuthSample.Models
{
    public class ApplicationUser:IdentityUser<int>
    {
        public string Avatar{ get; set; }//社交软件头像常用Profile picture , Avatar比较用于视频, 游戏或是虚幻的头像
    }
}
