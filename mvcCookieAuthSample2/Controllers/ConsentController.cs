using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mvcCookieAuthSample.ViewModels;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using mvcCookieAuthSample.Services;

namespace mvcCookieAuthSample.Controllers
{
    public class ConsentController : Controller
    {
        private readonly ConsentService _consentService;
        public ConsentController(ConsentService consentService) {
            _consentService = consentService;
        }

        #region Action
        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnUrl"> 
        /// returnUrl 来自： account 控制器中的Login方法的returnUrl ,传输方式是通过get方式
        /// returnUrl 去向： 绑定到了 ConsentViewModel 模型上 最后通过post方式，发回来
        /// </param>
        /// 
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var model = await _consentService.BuildConsentViewModelAsync(returnUrl);
            if (model==null)
            {

            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(InputConsentViewModel inputConsent)
        {
          var result= await  _consentService.ProcessConsentAsync(inputConsent);
            if (result.IsRedirect)
            {
                return Redirect(result.RedirectUrl);
            }

            // 如果不跳转，则继续返回Index视图
            if (!string.IsNullOrEmpty(result.ValidationError))
            {
                ModelState.AddModelError("", result.ValidationError); // key没有传值
            }
            return View(result.consentViewModel); // 填充Index视图，需要ConsentViewModel的对象
        }



        #endregion

   


    }
}