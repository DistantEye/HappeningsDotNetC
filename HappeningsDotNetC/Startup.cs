using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HappeningsDotNetC.Helpers;
using HappeningsDotNetC.Infrastructure;
using HappeningsDotNetC.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json.Linq;

namespace HappeningsDotNetC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication( CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                                options =>
                                {
                                    options.LoginPath = "/Login/Login";
                                    options.LogoutPath = "/Login/Logout";

                                });
            services.AddCors();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddSessionStateTempDataProvider();

            services.AddSession();

            DependencyInjectionMappings.Map(services); // Handles all registrations            

            var connection = @"Server=(localdb)\mssqllocaldb;Database=HappeningsDB;Trusted_Connection=True;ConnectRetryCount=0";
            services.AddDbContext<HappeningsContext>
                (options => options.UseSqlServer(connection));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(builder => builder
                                    .AllowAnyOrigin() // in production this should be configured to only target the intended frontend site
                                    .AllowAnyMethod()
                                    .AllowAnyHeader()
                                    .AllowCredentials());
            app.UseMiddleware<MaintainCorsHeadersMiddleware>();

            app.UseWhen(x => x.Request.Path.Value.StartsWith("/api"), builder =>
            {
                builder.UseExceptionHandler(new ExceptionHandlerOptions()
                {
                    ExceptionHandler = async (context) =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "application/json";
                        var ex = context.Features.Get<IExceptionHandlerFeature>();
                        if (ex != null)
                        {
                            context.Response.StatusCode = ErrorPackager.GetHttpCode(ex.Error);
                            var err = new JObject();
                            err.Add("message", JToken.FromObject(ex.Error.Message));
                            await context.Response.WriteAsync(err.ToString()).ConfigureAwait(false);
                        }
                    }
                });
            });
            app.UseWhen(x => !x.Request.Path.Value.StartsWith("/api"), builder =>
            {
                if (env.IsDevelopment())
                {
                    builder.UseDeveloperExceptionPage();
                }
                else
                {

                    builder.UseExceptionHandler("/Home/Error");
                    builder.UseHsts();
                }
            });

            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication(); 
            app.UseCookiePolicy();
            app.UseSession();            

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
