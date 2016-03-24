//using System;
//using System.Data.Entity;
//using System.Globalization;
//using AutoMapper;
//using Eagle.Entity;
//using ServiceStack;
//using ServiceStack.Text;

namespace Swallow.Core {
    public class CoreBootstrapper {
        public static void ApplicationStartup() {
            //Licensing.RegisterLicense(@"2523-e1JlZjoyNTIzLE5hbWU6YW52ZSxUeXBlOkluZGllLEhhc2g6R3NxU2o5emJzVWZUaENTNlFFSCtUQTdLTEV2aFcyRml3WERndWl0ZVNtcTMrVjN3MnAwZzhLMEpuUUhINWxDR1Vxb2k3VWR2S1UzOHQxSFpMa2lISE9zemxBRCtDdi9TcWM1cGEwaXpyMVR6TG1zOXhvUWRQS2FabGc2cWhUMEIwOGJYQ3E2VXM4MERjN1EyTzlOOCtKZDVTMk56U2dvbGJMQW5JVFhVaEFNPSxFeHBpcnk6MjAxNi0wNC0xN30=");

            ////http://stackoverflow.com/a/19708432/346701
            //JsConfig<DateTime>.SerializeFn = time => new DateTime(time.Ticks, DateTimeKind.Utc).ToString();
            //JsConfig<DateTime>.DeSerializeFn = time => DateTime.Parse(time, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);

            //DbConfiguration.SetConfiguration(new MySqlConfiguration());
            //Database.SetInitializer(new EagleInitializer());

            //Mapper.CreateMap<LanceServiceInfo, LanceService>();
            //Mapper.CreateMap<LanceServiceInfo, LanceServiceSnap>();
            //Mapper.CreateMap<LanceService, LanceServiceSnap>();
            //Mapper.CreateMap<LanceServiceCreateForms, LanceService>();
            //UserProxy.CreateMap();
        }
    }
}
