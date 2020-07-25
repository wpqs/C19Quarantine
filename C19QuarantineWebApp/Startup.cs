using System;
using C19QCalcLib;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AppRequestCultureProvider = C19QuarantineWebApp.Pages.AppRequestCultureProvider;

namespace C19QuarantineWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var cultures = new AppSupportedCultures();
                options.DefaultRequestCulture = new RequestCulture(culture: cultures.GetCultureTabForNeutralCulture(), uiCulture: cultures.GetDefaultUiCultureTab());
                options.SupportedCultures = cultures.GetSupportedCulturesInfo();
                options.SupportedUICultures = cultures.GetSupportedCulturesInfo();
                //options.FallBackToParentCultures = true;    //default to true - if en-US isn't in list of SupportedCultures then fall-back to en
                //options.FallBackToParentUICultures = true;  //default to true - if en-US isn't in list of SupportedUICultures then fall-back to en
                options.AddInitialRequestCultureProvider(new AppRequestCultureProvider(options));
            });
            services.AddHttpContextAccessor();
            services.AddRazorPages();

            if (Env.IsDevelopment() == false)
            {
                services.AddHsts(options =>
                {
                    options.Preload = true;
                    options.IncludeSubDomains = true;
                    options.MaxAge = TimeSpan.FromDays(60);
                    //options.ExcludedHosts.Add("example.com");
                    //options.ExcludedHosts.Add("www.example.com");
                });

                services.AddHttpsRedirection(options =>
                {
                    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                    options.HttpsPort = 5001;
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseRequestLocalization();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
