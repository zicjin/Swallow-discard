using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Swallow.Entity {
    public class CustomJsonSerializer : JsonSerializer {
        public CustomJsonSerializer() {
            this.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //this.NullValueHandling = NullValueHandling.Ignore; //影响angularjs scope绑定
            this.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
        }
    }
}
