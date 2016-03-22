using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Swallow.Service {
    public class AliyunOssUpload {
        private readonly static string _appId;
        private readonly static string _appSecret;
        private readonly string _bucketName;
        private readonly string _city;

        public AliyunOssUpload(string bucketName, string city) {
            if (string.IsNullOrEmpty(bucketName)) {
                throw new Exception("bucketName不能为空");
            }
            _bucketName = bucketName;
            _city = city;
        }
        static AliyunOssUpload() {
            _appId = ConfigurationManager.AppSettings["OSSID"];
            _appSecret = ConfigurationManager.AppSettings["OSSSecret"];
            if (string.IsNullOrEmpty(_appId) || string.IsNullOrEmpty(_appSecret))
                throw new Exception("必须在AppSetting中配置阿里云的OSSID,OSSSecret");
        }

        public async Task<AliyunImage> UploadAsync(byte[] fileBytes, string fileName = null) {
            try {
                if (string.IsNullOrEmpty(fileName)) fileName = $"{Guid.NewGuid()}.jpg";
                var request = new AliyunRequest(_appId, _appSecret, _bucketName, _city);
                var response = await request.ExecutePutTaskAsync(fileName, restRequest => {
                    restRequest.AddHeader("Content-Disposition", $"attachment;filename={fileName}");
                    restRequest.AddParameter("image/jpeg", fileBytes, ParameterType.RequestBody);
                });
                return response.StatusCode == HttpStatusCode.OK ? new AliyunImage() { Url = $"http://{request.Host}/{fileName}", Name = fileName } : null;
            } catch (Exception e) {
                throw e;
            }
        }

        public async Task<AliyunImage> UploadAsync(Stream fileStream, string fileName = null) {
            using (MemoryStream stream = new MemoryStream()) {
                fileStream.CopyTo(stream);
                var buff = stream.ToArray();
                return await UploadAsync(buff, fileName);
            }
        }

        public async Task<bool> DeleteAsync(string name) {
            var request = new AliyunRequest(_appId, _appSecret, _bucketName, _city);
            var response = await request.ExecuteDeleteTaskAsync(name);
            return response.StatusCode == HttpStatusCode.NoContent;
        }
    }

    public class AliyunRequest {
        private readonly string _host = "{0}.oss-cn-{1}.aliyuncs.com";
        private string _appId;
        private string _secret;
        private string _bucketName;
        private RestClient _client;
        private RestRequest _request;
        public AliyunRequest(string appId, string secret, string bucketName, string city) {
            _host = string.Format(_host, bucketName, city);
            _appId = appId;
            _secret = secret;
            _bucketName = bucketName;
        }

        public string Host {
            get { return _host; }
        }

        public Task<IRestResponse> ExecutePutTaskAsync(string name, Action<RestRequest> beforeAction = null) {
            return ExecuteTaskAsync(name, Method.PUT, beforeAction);
        }

        public Task<IRestResponse> ExecuteDeleteTaskAsync(string name, Action<RestRequest> beforeAction = null) {
            return ExecuteTaskAsync(name, Method.DELETE, beforeAction);
        }

        private Task<IRestResponse> ExecuteTaskAsync(string name, Method method, Action<RestRequest> beforeAction) {
            InitRequestHeader(name, method);
            if (beforeAction != null) beforeAction(_request);
            return _client.ExecuteTaskAsync(_request);
        }

        private void InitRequestHeader(string name, Method method) {
            _client = new RestClient($"http://{_host}");
            _request = new RestRequest($"/{name}");
            _request.Method = method;
            _request.AddHeader("Content-Type", "image/jpeg");
            _request.AddHeader("Host", _host);
            _request.AddHeader("Date", DateTime.Now.AddHours(-8).ToString("R"));
            _request.AddHeader("Authorization", $"OSS {_appId}:{BuildSignature(_secret, _bucketName, name, method.ToString())}");
        }

        private static string BuildSignature(string secret, string bucket, string fileName, string method) {
            byte[] keyBytes = Encoding.UTF8.GetBytes(secret);
            string data = method + "\n\n" + "image/jpeg" + "\n" + DateTime.Now.AddHours(-8).ToString("R") + "\n" + "/" + bucket + "/" + fileName;
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            var sign = Convert.ToBase64String(new HMACSHA1(keyBytes).ComputeHash(dataBytes));
            return sign;
        }
    }

    public class AliyunImage {
        public string Url { get; set; }
        public string Name { get; set; }
    }
}
