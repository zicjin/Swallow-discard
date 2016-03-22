using System;
using System.Configuration;

namespace Swallow.Service {
    public class AliyunOssUploadBuild {
        public static AliyunOssUpload BuildProfileOssUpload() {
            var bucket = ConfigurationManager.AppSettings["OSSBucketName"];
            if (string.IsNullOrEmpty(bucket))
                throw new Exception("必须配置OSSBucketName");
            return new AliyunOssUpload(bucket, "shanghai");
        }

        public static AliyunOssUpload BuildServiceOssUpload() {
            var bucket = ConfigurationManager.AppSettings["OSSBucketName_Service"];
            if (string.IsNullOrEmpty(bucket))
                throw new Exception("必须配置OSSBucketName");
            return new AliyunOssUpload(bucket, "shanghai");
        }

        public static AliyunOssUpload BuildCaseOssUpload() {
            var bucket = ConfigurationManager.AppSettings["OSSBucketName_Case"];
            if (string.IsNullOrEmpty(bucket))
                throw new Exception("必须配置OSSBucketName");
            return new AliyunOssUpload(bucket, "shanghai");
        }
    }
}
