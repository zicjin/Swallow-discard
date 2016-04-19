using Nancy;
using Swallow.Core;
using System;
using System.Collections.Generic;
using Nancy.ModelBinding;
using System.Linq;
using System.Threading.Tasks;
using Nancy.Authentication.Token;
using Nancy.Responses;
using Swallow.Entity;
using Swallow.Api.Security;
using Nancy.Security;
using Swallow.Service;

namespace Swallow.Api {
    public class TranslateModule : NancyModule {
        public TranslateModule(ITranslater Translater, ITokenizer tokenizer) :
            base("/translate") {

            Post["/", runAsync:true] = async (p, ct) => {
                var forms = this.Bind<TranslateForm>();
                string result = await Translater.Trans(forms);
                if (string.IsNullOrEmpty(result))
                    return new TextResponse(HttpStatusCode.UnprocessableEntity, "failure");
                return result;
            };

        }
    }
}