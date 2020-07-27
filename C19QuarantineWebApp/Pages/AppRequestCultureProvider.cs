﻿using System;
using System.Threading.Tasks;
using C19QCalcLib;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace C19QuarantineWebApp.Pages
{
    public class AppRequestCultureProvider : RequestCultureProvider
    {
        private  readonly string CookieProviderTypeName = "Microsoft.AspNetCore.Localization.CookieRequestCultureProvider";

        public string CultureKey { get; set; } = "culture";

        public string UiCultureKey { get; set; } = "ui-culture";

        private readonly AppSupportedCultures _supportedCultures;

        public AppRequestCultureProvider(RequestLocalizationOptions options)
        {
            Options = options;
            _supportedCultures = new AppSupportedCultures();
        }

        //Create an extension method for the User that returns the Culture that the User has stored in his Profile
        //public static string GetCulture(this ClaimsPrincipal claimsPrincipal)
        //{
        //    if (claimsPrincipal == null)
        //        throw new ArgumentNullException(nameof(claimsPrincipal));
        //    return claimsPrincipal.FindFirstValue(Constants.ClaimTypes.Culture);  //or Constants.ClaimTypes.locale - see https://openid.net/specs/openid-connect-core-1_0.html#StandardClaims
        //}

        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException();

            var updateCookie = true;
            //var configuration = httpContext.RequestServices.GetService<IConfigurationRoot>(); //Configuration providers read configuration data from key-value pairs using a variety of configuration sources https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1
            var culture = AppSupportedCultures.DefaultTab;      //configuration[CultureKey];
            var uiCulture = AppSupportedCultures.DefaultUiTab;  //configuration[UiCultureKey];


            if (httpContext.User.Identity.IsAuthenticated)
            {           //https://ml-software.ch/posts/writing-a-custom-request-culture-provider-in-asp-net-core-2-1
                culture = AppSupportedCultures.DefaultTab; //GetCultureTabFromValue(httpContext.User.GetCulture());
                uiCulture = AppSupportedCultures.DefaultUiTab; //GetUiCultureTabFromValue(httpContext.User.GetUiCulture());
            }
            else
            {
                if (Options != null)
                {
                    foreach (var provider in Options.RequestCultureProviders)   //0: this, 1: query string, 2: cookie, 3: Accept-Language header
                    {
                        var providerTypeName = provider.GetType().ToString();
                        if (providerTypeName.Equals(this.GetType().ToString()) == false)
                        {
                            var val = await provider.DetermineProviderCultureResult(httpContext);
                            if (val?.Cultures?.Count > 0)
                                culture = val.Cultures[0].ToString();
                            if (val?.UICultures?.Count > 0)
                                uiCulture = val.UICultures[0].ToString();
                            if (val?.Cultures?.Count > 0)
                            {
                                if (providerTypeName.Equals(CookieProviderTypeName))
                                    updateCookie = false;
                                break;
                            }
                        }
                    }
                }
            }
            var cookies = new MxCookies(httpContext.RequestServices.GetService<IHttpContextAccessor>());
            if (_supportedCultures.IsSupported(culture) == false)
            {
                culture = _supportedCultures.GetNearestMatch(culture);
                updateCookie = true;
            }
            if (_supportedCultures.IsSupported(uiCulture) == false)
            {
                uiCulture = _supportedCultures.GetNearestMatch(uiCulture, true);
                updateCookie = true;
            }
            if (updateCookie)
                cookies.SetValue(MxSupportedCultures.CookieName, _supportedCultures.GetCulturesEncodedValue(culture, uiCulture));

            return new ProviderCultureResult(culture, uiCulture);
        }
    }
}
