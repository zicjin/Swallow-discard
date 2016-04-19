using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swallow.Service {
    public enum Language {
        Unknow = 0,
        En = 1,
        Cn = 2
    }

    public class TranslateForm {
        public string Input { get; set; }
        public Language inputLanguage { get; set; }
        public Language outputLanguage { get; set; }
    }

    public interface ITranslater {
        Task<string> Trans(TranslateForm form);
    }
}
