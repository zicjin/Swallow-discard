using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Swallow.Service {
    public interface IVerifyCode {
        bool Verify(string phone, string code);
        bool SendCode(string phone);
        Task<bool> VerifyAsync(string phone, string code);
        Task<bool> SendCodeAsync(string phone);
    }

    [Serializable]
    public class VerifyCodeException : Exception {
        public VerifyCodeException() {
        }

        public VerifyCodeException(string message) : base(message) {
        }

        public VerifyCodeException(string message, Exception inner) : base(message, inner) {
        }

        protected VerifyCodeException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}
