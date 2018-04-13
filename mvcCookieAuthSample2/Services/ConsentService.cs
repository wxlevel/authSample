using IdentityServer4.Services;
using IdentityServer4.Stores;
using mvcCookieAuthSample.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace mvcCookieAuthSample.Services
{
    public class ConsentService
    {
        private readonly IClientStore _clientStore;
        private readonly IResourceStore _resourceStore;
        private readonly IIdentityServerInteractionService _identityServerInteractionService;

        // 显示依赖注入
        public ConsentService(IClientStore clientStore, IResourceStore resourceStore, IIdentityServerInteractionService identityServerInteractionService)
        {
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _identityServerInteractionService = identityServerInteractionService;

        }


        public async Task<ConsentViewModel> BuildConsentViewModelAsync(string returnUrl, InputConsentViewModel inputConsent = null)
        {

            // 通过 identityServerInteractionService 获取 authorization context 信息   
            var requestContext = await _identityServerInteractionService.GetAuthorizationContextAsync(returnUrl);
            if (requestContext == null)
                return null;

            // 通过 clientStore 获取 client 信息 
            var client = await _clientStore.FindEnabledClientByIdAsync(requestContext.ClientId);
            if (client == null)
                return null;

            // 通过 resourceStore 获取 apiResource 和 identityResource 信息
            var resource = await _resourceStore.FindEnabledResourcesByScopeAsync(requestContext.ScopesRequested);
            if (resource == null)
                return null;

            var consentViewModel = CreateConsentViewModel(requestContext, client, resource, inputConsent);
            //将 returnUrl 绑定到 consentViewModel对象上
            consentViewModel.ReturnUrl = returnUrl;
            return consentViewModel;
        }

        public async Task<ProcessConsentResult> ProcessConsentAsync(InputConsentViewModel inputConsent)
        {
            ProcessConsentResult result = new ProcessConsentResult();

            ConsentResponse consentResponse = null;
            if (inputConsent.Button == "no")
            {
                consentResponse = ConsentResponse.Denied;
            }
            else if (inputConsent.Button == "yes")
            {
                if (inputConsent.ScopesConsented != null && inputConsent.ScopesConsented.Any())
                {
                    consentResponse = new ConsentResponse
                    {
                        RememberConsent = inputConsent.RememberConsent,
                        ScopesConsented = inputConsent.ScopesConsented
                    };
                }
                else
                { // 一个权限都没有选，给校验错误提示
                    result.ValidationError = "Please select at least one permission!";
                }
            }

            if (consentResponse != null)
            { //构造 Redirect return url 

                var requestContext = await _identityServerInteractionService.GetAuthorizationContextAsync(inputConsent.ReturnUrl);
                await _identityServerInteractionService.GrantConsentAsync(requestContext, consentResponse); //告诉identityServer 用户做了哪些授权（同意 / 取消）

                //return Redirect(inputConsent.ReturnUrl);
                result.RedirectUrl = inputConsent.ReturnUrl;
            }
            else
            { // 如果consentResponse=null,即什么权限都没有选中，那么继续返回Index视图，这时实例化ConsentViewModel的对象

                var consentViewModel = await BuildConsentViewModelAsync(inputConsent.ReturnUrl, inputConsent);
                result.consentViewModel = consentViewModel;
            }

            return result;
        }

        #region private Method
        private ConsentViewModel CreateConsentViewModel(AuthorizationRequest request, Client client, Resources resources, InputConsentViewModel inputConsent)
        {
            var ScopesSelected = inputConsent?.ScopesConsented ?? Enumerable.Empty<string>();

            return new ConsentViewModel
            {
                ClientName = client.ClientName,
                ClientLogoUrl = client.LogoUri,
                ClientUrl = client.ClientUri,
                RememberConsent = inputConsent?.RememberConsent ?? true, //client.AllowRememberConsent,

                IdentityScopes = resources.IdentityResources.Select(identity => CreateScopeViewModel(identity, ScopesSelected.Contains(identity.Name) || inputConsent == null)), //是否选中了name，如果inputConsent=null,给ischecked赋值为true
                // these place use selectMany() instead of if use select(), because select() will return IEnumable<Icollection<Scope>> 
                ApiResourceScopes = resources.ApiResources.SelectMany(apiresource => apiresource.Scopes)
                .Select(scope => CreateScopeViewModel(scope, ScopesSelected.Contains(scope.Name) || inputConsent == null))

            };

        }
        // parameters: identityResource
        private ScopeViewModel CreateScopeViewModel(IdentityResource identityResource, bool isChecked)
        {
            return new ScopeViewModel
            {
                Name = identityResource.Name,
                DisplayName = identityResource.DisplayName,
                Emphasize = identityResource.Emphasize,
                Description = identityResource.Description,
                Checked = isChecked || identityResource.Required, // 你是已经 checked 或者 Required，那么就给你 checked
                Required = identityResource.Required

            };
        }
        // parameters: apiResource 
        private ScopeViewModel CreateScopeViewModel(Scope scope, bool isChecked)
        {
            return new ScopeViewModel
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Emphasize = scope.Emphasize,
                Description = scope.Description,
                Checked = isChecked || scope.Required,
                Required = scope.Required

            };
        }
        #endregion
    }
}
