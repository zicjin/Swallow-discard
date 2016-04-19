using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection;

namespace Swallow.Service {
    public class YoudaoTransModel {
        public int ErrorCode { get; set; }
        public string Query { get; set; }
        public IList<string> Translation { get; set; }
        public YoudaoTransModelBase Base { get; set; }
    }

    public class YoudaoTransModelBase {
        public string Phonetic { get; set; }
        public string UkPhonetic { get; set; }
        public string UsPhonetic { get; set; }
        public IList<string> Explains { get; set; }
    }

    public class YoudaoTranslater : ITranslater {
        public async Task<string> Trans(TranslateForm form) {

            YoudaoTransModel model = null;
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("http://fanyi.youdao.com/openapi.do?keyfrom=swallow&key=1811483803&type=data&doctype=json&version=1.1&q=" + form.Input);
            var jsonString = await response.Content.ReadAsStringAsync();

            try {
                model = JsonConvert.DeserializeObject<YoudaoTransModel>(jsonString);
            } catch (Exception) {
                LogService.SetLog(MethodBase.GetCurrentMethod(), Domain.Service, LogType.Warn, new string[] { form.Input });
                return string.Empty;
            }
            

            return model.Translation.First();
        }
    }
}
