using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcCookieAuthSample.ViewModels
{
    public class ProcessConsentResult
    {
        public string RedirectUrl { get; set; }
        public bool IsRedirect => RedirectUrl != null; // Lambda expression: if RedirectUrl != null, then return true

        // 如果consentResponse=null,即什么权限都没有选中，那么继续返回Index视图，需要一个ConsentViewModel的对象
        public ConsentViewModel consentViewModel { get; set; }

        public string ValidationError { get; set; }
    }
}
