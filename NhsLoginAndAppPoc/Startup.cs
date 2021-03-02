using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using NhsLoginAndAppPoc.AutoMapper;
using NhsLoginAndAppPoc.Repositories;
using NhsLoginAndAppPoc.Services;
using NhsUk.HeaderFooterApiClient;
using NhsUk.HeaderFooterApiClient.Interfaces;
using NhsUk.HeaderFooterApiClient.Models;

namespace NhsLoginAndAppPoc
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddProfile(new AppProfile(Configuration)));

            services.AddTransient<IImmunisationStatusService, ImmunisationStatusService>();
            services.AddTransient<IImmunisationStatusRepository, ImmunisationStatusRepository>();
            services.AddTransient<IUserInfoService, UserInfoService>();

            services.AddScoped<IHeaderFooterApiClientReader, HeaderFooterApi>(sp =>
            {
                IConfigurationSection configuration = Configuration.GetSection("HeaderFooterApi");

                var apiReaderOptions = new ApiReaderOptions(
                    sp.GetService<IHttpClientFactory>(),
                    Guid.Parse(configuration["SubscriptionKey"]),
                    configuration["EndpointBaseUrl"]);

                return new HeaderFooterApi(
                    apiReaderOptions, sp.GetService<IMemoryCache>(),
                    int.Parse(configuration["CacheExpiryTimeInMinutes"]),
                    sp.GetService<ILogger<HeaderFooterApi>>());
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = _ => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie()
                .AddOpenIdConnect(options =>
                {
                    options.ClientId = "nhsloginandapppoc";
                    options.Authority = "https://auth.sandpit.signin.nhs.uk/";
                    options.ResponseType = "code";
                    options.ResponseMode = "form_post";
                    options.CallbackPath = "/home";
                    options.SaveTokens = true;
                    options.Scope.Add("email");
                    options.Scope.Add("phone");
                    options.Scope.Add("gp_registration_details");
                    options.Scope.Add("gp_integration_credentials");
                    options.Scope.Add("client_metadata");
                    options.Events = new OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProvider = context =>
                        {
                            if (context.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication)
                            {
                                context.ProtocolMessage.Parameters.Add("vtr", "[\"P9.Cp.Cd\", \"P9.Cp.Ck\", \"P9.Cm\"]");
                            }

                            return Task.CompletedTask;
                        },

                        OnAuthorizationCodeReceived = (context) =>
                        {
                            if (context.TokenEndpointRequest?.GrantType == OpenIdConnectGrantTypes.AuthorizationCode)
                            {
                                context.TokenEndpointRequest.ClientAssertionType =
                                    "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
                                context.TokenEndpointRequest.ClientAssertion = TokenHelper.CreateClientAuthJwt();
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddControllersWithViews();
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
