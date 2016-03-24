using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Ninject;
using Nancy.Conventions;
using Ninject;
using Swallow.Core;
using Swallow.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using Newtonsoft.Json;
using Swallow.Entity;
using Nancy.Authentication.Token;
using System.Reflection;
using ServiceStack.Redis;

namespace Swallow.Api {
    public class Bootstrapper : NinjectNancyBootstrapper {
        protected override void ApplicationStartup(IKernel container, IPipelines pipelines) {
            base.ApplicationStartup(container, pipelines);

            //在开发阶段使用Initializer模式而不是Migrations模式
            //Database.SetInitializer<BumblebeeContext>(new DataInitializer());

            StaticConfiguration.DisableErrorTraces = false; // Just for IIS host

            CoreBootstrapper.ApplicationStartup();
            var mailTo = ConfigurationManager.AppSettings["log_mailto"];
            LogService.Instance.Config(
                new string[] { Domain.Api.ToString(), Domain.Core.ToString(), Domain.Service.ToString() },
                new IZicEmailConfig[] { new SwallowConfig(mailTo) });
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions) {
            base.ConfigureConventions(nancyConventions);
            //this.Conventions.AcceptHeaderCoercionConventions.Add((acceptHeaders, ctx) => {
            //    // http://goo.gl/hiGlFT query headers

            //    IEnumerable<Tuple<string, decimal>> jsonAccept = new[] { Tuple.Create("application/json", 1.0m), Tuple.Create("*/*", 0.9m) };
            //    return jsonAccept;
            //});
        }

        //https://github.com/NancyFx/Nancy/wiki/Container-Support
        protected override void ConfigureApplicationContainer(IKernel container) {
            base.ConfigureApplicationContainer(container);
            var redisConnection = ConfigurationManager.ConnectionStrings["RedisServer"].ConnectionString;

            // https://github.com/NancyFx/Nancy.Serialization.JsonNet
            container.Bind<JsonSerializer>().To<CustomJsonSerializer>();

            container.Bind<ITokenizer>().To<Tokenizer>().WithConstructorArgument("configuration", (Action<Tokenizer.TokenizerConfigurator>)(configurator => {
                configurator.KeyExpiration(() => TimeSpan.FromDays(60));
                configurator.TokenExpiration(() => TimeSpan.FromDays(30));
                configurator.WithKeyCache(new RedisTokenKeyStore(redisConnection).WithKeyStoreName("keyStoreForApi"));
            }));

            container.Bind<MongoDbContext>().To<MongoDbContext>().WithConstructorArgument("connection", ConfigurationManager.ConnectionStrings["MongoServer"].ConnectionString); ;
            container.Bind<IUserDbForManage>().To<UserDbByMongo>();
            container.Bind<IArticleDbForManage>().To<ArticleDbByMongo>();
            container.Bind<ICaseDbForManage>().To<CaseDbByMongo>();
            container.Bind<IEncryptorDecryptor>().To<EncryptorDecryptor>();
            container.Bind<IVerifyCode>().To<LeancloudVerifyCodeProvider>();
            container.Bind<IEmailSender>().To<MessageSender>();
            container.Bind<ISmsSender>().To<MessageSender>();

            container.Bind<IRedisClientsManager>().To<RedisManagerPool>().WithConstructorArgument("hosts", new string[] { redisConnection });
            Container = container;
        }

        protected override void ConfigureRequestContainer(IKernel container, NancyContext context) {
            base.ConfigureRequestContainer(container, context);
            //container.Bind<IPerRequest>().To<PerRequest>().InSingletonScope();
        }
        public static IKernel Container { get; set; }
        protected override void RequestStartup(IKernel container, IPipelines pipelines, NancyContext context) {
            TokenAuthentication.Enable(pipelines, new TokenAuthenticationConfiguration(container.Get<ITokenizer>()));
            pipelines.OnError.AddItemToEndOfPipeline((z, ex) => {
                LogService.SetLog(MethodBase.GetCurrentMethod(), Domain.Api, LogType.Error, 
                    new List<string>() { z.Request.Headers.Accept.ToString(), z.Request.Url.ToString()}, ex.ToString());
                return null;
            });

            pipelines.AfterRequest.AddItemToEndOfPipeline((ctx) => {
                ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
                    .WithHeader("Access-Control-Allow-Methods", "GET, POST, DELETE, PUT, OPTIONS")
                    .WithHeader("Access-Control-Allow-Headers", "Authorization, X-Requested-With, Accept, Origin, Content-Type");
            }); // http://stackoverflow.com/a/23554350/346701 or use Extensions
        }
    }
}