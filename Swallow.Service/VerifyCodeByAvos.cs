using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swallow.Service {
    public class LeancloudVerifyCodeProvider : IVerifyCode {
        private const string VerifysmscodeCode = "/1.1/verifySmsCode/{code}";
        private const string Requestsmscode = "1.1/requestSmsCode";


        public string GetConfig(string key) {
            var setting = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(setting)) {
                throw new ConfigurationErrorsException("缺少配置" + key);
            }
            return setting;
        }

        private LeancloudVerifyRequest WrapperCodeRequest(string resource, object jsonData, Action<RestRequest> beginRequest = null) {
            var client = new RestClient("https://api.leancloud.cn/");
            var request = new RestRequest(resource, Method.POST);
            request.AddHeader("X-AVOSCloud-Application-Id", this.GetConfig("X-AVOSCloud-Application-Id"));
            request.AddHeader("X-AVOSCloud-Application-Key", this.GetConfig("X-AVOSCloud-Application-Key"));
            request.AddHeader("Content-Type", "application/json");
            if (beginRequest != null) {
                beginRequest(request);
            }
            if (jsonData != null) {
                request.AddJsonBody(jsonData);
            }

            return new LeancloudVerifyRequest(client, request);
        }

        private Action<RestRequest> VerifyCodeBeginRequest(string phone, string code) {
            return request => {
                request.AddParameter("code", code, ParameterType.UrlSegment);
                request.AddParameter("mobilePhoneNumber", phone, ParameterType.QueryString);
            };
        }

        public bool Verify(string phone, string code) {
            var content = this.WrapperCodeRequest(VerifysmscodeCode, null, VerifyCodeBeginRequest(phone, code)).GetResult();
            return content == "{}";
        }

        public bool SendCode(string phone) {
            var content = this.WrapperCodeRequest(Requestsmscode, new {
                mobilePhoneNumber = phone,
                ttl = 5
            }).GetResult();
            var success = content == "{}";
            if (!success)
                throw new VerifyCodeException(content);
            return success;
        }

        public async Task<bool> VerifyAsync(string phone, string code) {
            var content = await this.WrapperCodeRequest(VerifysmscodeCode, null, VerifyCodeBeginRequest(phone, code)).GetResultAsync();
            return content == "{}";
        }

        public async Task<bool> SendCodeAsync(string phone) {
            var content = await this.WrapperCodeRequest(Requestsmscode, new {
                mobilePhoneNumber = phone,
                ttl = 5
            }).GetResultAsync();
            var success = content == "{}";
            if (!success) throw new VerifyCodeException(content);
            return success;
        }

        private class LeancloudVerifyRequest {
            private IRestClient _client;
            private IRestRequest _request;
            public LeancloudVerifyRequest(IRestClient client, IRestRequest request) {
                _client = client;
                _request = request;
            }

            public string GetResult() {
                try {
                    var response = _client.Execute(_request);
                    return response.Content;
                } catch (Exception e) {
                    throw new VerifyCodeException(e.Message, e);
                }
            }

            public async Task<string> GetResultAsync() {
                try {
                    var response = await _client.ExecuteTaskAsync(_request);
                    return response.Content;
                } catch (Exception e) {
                    throw new VerifyCodeException(e.Message, e);
                }
            }
        }
    }
}
