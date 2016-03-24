using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Nancy.Authentication.Token.Storage;
using ServiceStack.Redis;

namespace Swallow.Api {
    public class RedisTokenKeyStore : ITokenKeyStore {
        public RedisTokenKeyStore()
            : this(new RedisClient()) {
        }

        public RedisTokenKeyStore(string redisConnectionString)
            : this(new RedisConfiguration(redisConnectionString).GetClient()) {
        }

        public RedisTokenKeyStore(IRedisClient client) {
            _client = client;
        }

        public IDictionary<DateTime, byte[]> Retrieve() {
            return this._client.Get<IDictionary<DateTime, byte[]>>(_keyStoreName) ?? new ConcurrentDictionary<DateTime, byte[]>();
        }

        public void Store(IDictionary<DateTime, byte[]> keys) {
            this._client.Set(_keyStoreName, keys);
        }

        public void Purge() {
            this._client.ExpireEntryIn(_keyStoreName, TimeSpan.Zero);
        }

        public RedisTokenKeyStore WithKeyStoreName(string name) {
            this._keyStoreName = name;
            return this;
        }

        #region Private Members

        private readonly IRedisClient _client;
        private string _keyStoreName = "keyStore";

        private class RedisConfiguration {
            public RedisConfiguration(string redisConnectionString) {
                Parse(redisConnectionString);
            }

            public RedisClient GetClient() {
                var redisClient = new RedisClient(host, port);
                if (!string.IsNullOrEmpty(password))
                    redisClient.Password = password;

                return redisClient;
            }

            private void Parse(string redisConStr) {
                string[] redisConsAt = redisConStr.Split('@');
                string[] redisConsColon;
                password = string.Empty;

                if (redisConsAt.Length > 1) {
                    password = redisConsAt[0];
                    redisConsColon = redisConsAt[1].Split(':');
                } else {
                    redisConsColon = redisConStr.Split(':');
                }

                host = redisConsColon[0];
                port = Convert.ToInt32(redisConsColon[1]);
            }

            private string host;
            private int port;
            private string password;
        }

        #endregion
    }
}