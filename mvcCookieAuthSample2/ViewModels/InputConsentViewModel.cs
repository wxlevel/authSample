using System.Collections.Generic;

namespace mvcCookieAuthSample.ViewModels
{
    public class InputConsentViewModel
    {
        public string Button { get; set; } // 同意 取消 按钮
        public IEnumerable<string> ScopesConsented{ get; set; }

        public bool RememberConsent { get; set; }
        public string ReturnUrl { get; set; }


    }
}
