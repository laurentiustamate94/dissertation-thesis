using CloudApp.DbModels;
using CloudApp.Interfaces;
using CloudApp.Services;
using Communication.Common;
using Communication.Common.Interfaces;
using Communication.Common.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CloudApp
{
    public class Startup
    {
        public const string CookieScheme = "DissertationThesis";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddDbContext<DissertationThesisContext>(options => options.UseSqlServer(Configuration.GetConnectionString("UsersDatabase")));

            services.AddAuthentication(CookieScheme) // Sets the default scheme to cookies
               .AddCookie(CookieScheme, options =>
               {
                   options.AccessDeniedPath = "/account/denied";
                   options.LoginPath = "/account/login";
               });

            services.AddHttpContextAccessor();

            services.AddSingleton<IUniqueIdGenerationService, UniqueIdGenerationService>();
            services.AddSingleton<IDataProtector, DataProtector>();

            services.AddSingleton<IDataPersistor, DataPersistor>();
            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<IKeyPairManagementService, KeyPairManagementService>();
            services.AddSingleton<IFitbitService, FitbitService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
