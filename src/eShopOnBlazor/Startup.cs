using eShopOnBlazor.Models;
using eShopOnBlazor.Models.Infrastructure;
using eShopOnBlazor.Services;
using log4net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Data.Entity;

namespace eShopOnBlazor
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
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            if (Configuration.GetValue<bool>("UseMockData"))
            {
                services.AddSingleton<ICatalogService, CatalogServiceMock>();
            }
            else
            {
                services.AddScoped<ICatalogService, CatalogService>();
                services.AddScoped<IDatabaseInitializer<CatalogDBContext>, CatalogDBInitializer>();
                services.AddSingleton<CatalogItemHiLoGenerator>();
                services.AddScoped(_ => new CatalogDBContext(Configuration.GetConnectionString("CatalogDBContext")));
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLog4Net("log4Net.xml");

            if (Env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Middlware for Application_BeginRequest
            app.Use((ctx, next) =>
            {
                LogicalThreadContext.Properties["activityid"] = new ActivityIdHelper(ctx);
                LogicalThreadContext.Properties["requestinfo"] = new WebRequestInfo(ctx);
                return next();
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            ConfigDataBase(app);
        }

        private void ConfigDataBase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetService<IDatabaseInitializer<CatalogDBContext>>();

                if (initializer != null)
                {
                    Database.SetInitializer(initializer);
                }
            }
        }

        public class ActivityIdHelper
        {
            private readonly string _activityId;

            public ActivityIdHelper(HttpContext ctx)
            {
                _activityId = ctx.TraceIdentifier;
            }

            public override string ToString() => _activityId;
        }

        public class WebRequestInfo
        {
            private readonly string _message;

            public WebRequestInfo(HttpContext context)
            {
                var userAgent = context.Request.Headers["User-Agent"];
                _message = $"{context.Request.Path}, {userAgent}";
            }

            public override string ToString() => _message;
        }
    }
}
