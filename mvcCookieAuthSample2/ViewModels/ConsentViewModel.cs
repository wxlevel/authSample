using System.Collections.Generic;

namespace mvcCookieAuthSample.ViewModels
{
    public class ConsentViewModel
    {
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientLogoUrl { get; set; }
        public string ClientUrl { get; set; }
        public bool RememberConsent { get; set; }

        public IEnumerable<ScopeViewModel> IdentityScopes { get; set; }
        public IEnumerable<ScopeViewModel> ApiResourceScopes { get; set; }

        //  来自： account 控制器中的Login方法的returnUrl ,传输方式是通过get方式 去向： 绑定到了 ConsentViewModel 模型上 最后通过post方式，发回来
        public string ReturnUrl { get; set; } 
    }
}
