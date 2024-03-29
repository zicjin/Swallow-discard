﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using AspNet.Identity3.MongoDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swallow.Manage.Models;
using MongoDB.Driver;
using Swallow.Entity;
using Swallow.Core;
using Swallow.Service;

namespace Swallow.Manage {
    public class Startup {
        public Startup(IHostingEnvironment env) {
            // Set up configuration sources.

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment()) {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            string connection = Configuration["Data:DefaultConnection:ConnectionString"];
            services.AddSingleton<MongoDbContext>(provider => new MongoDbContext(connection));

            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);
            
            services.AddSingleton<IdentityContext>(provider => new IdentityContext(connection));
            services.AddIdentity<AppUser, IdentityRole>()
                .AddMongoStores<IdentityContext, AppUser, IdentityRole>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, MessageSender>();
            services.AddTransient<ISmsSender, MessageSender>();
            services.AddTransient<IEncryptorDecryptor, EncryptorDecryptor>();
            services.AddTransient<IVerifyCode, LeancloudVerifyCodeProvider>();
            services.AddTransient<ITranslater, YoudaoTranslater>();

            services.AddTransient<IUserDbForManage, UserDbByMongo>();
            services.AddTransient<IArticleDbForManage, ArticleDbByMongo>();
            services.AddTransient<ICaseDbForManage, CaseDbByMongo>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment()) {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            } else {
                app.UseExceptionHandler("/Home/Error");

                // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
                //try {
                //    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                //        .CreateScope()) {
                //        serviceScope.ServiceProvider.GetService<ApplicationDbContext>()
                //             .Database.Migrate();
                //    }
                //} catch { }
            }

            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseIdentity();

            // To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
