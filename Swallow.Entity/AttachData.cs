using System;
using System.Linq;
using System.Reflection;

namespace Swallow.Entity {
    public enum AgeRange {
        [AttachData(Descrip.Chinese, "18岁及以下")]
        [AttachData(Descrip.English, "18 years old or less")]
        LessThan18,

        [AttachData(Descrip.Chinese, "19至29岁")]
        [AttachData(Descrip.English, "19 years old until 29 years old")]
        From19To29,

        [AttachData(Descrip.Chinese, "30岁及以上")]
        [AttachData(Descrip.English, "30 years old or more")]
        Above29
    }

    public enum Descrip {
        Chinese,
        English,
        Sort
    }

    //AgeRange.GetAttachedData<string>(DescripByLang.English);

    //部署N个网站分别支持N个语言，web.config设定global_lang
    public static class zGlobal {
        public static Descrip GlobalLang { get; set; }
        public static string GetText(this AgeRange range) {
            return range.GetAttach<string>(GlobalLang);
        }
    }

    //http://blog.zhaojie.me/2009/01/attachdataextensions.html
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class AttachDataAttribute : Attribute {
        public AttachDataAttribute(object key, object value) {
            this.Key = key;
            this.Value = value;
        }

        public object Key { get; private set; }

        public object Value { get; private set; }
    }

    public static class AttachDataExtensions {
        public static object GetAttach(this ICustomAttributeProvider provider, object key) {
            var attributes = (AttachDataAttribute[])provider.GetCustomAttributes(typeof(AttachDataAttribute), false);
            AttachDataAttribute result = attributes.FirstOrDefault(a => a.Key.Equals(key));
            return result != null ? result.Value : null;
        }

        public static T GetAttach<T>(this ICustomAttributeProvider provider, object key) {
            object result = provider.GetAttach(key);
            return result != null ? (T)result : default(T);
        }

        public static object GetAttach(this Enum value, object key) {
            return value.GetType().GetField(value.ToString()).GetAttach(key);
        }

        public static T GetAttach<T>(this Enum value, object key) {
            object result = value.GetAttach(key);
            return result != null ? (T)result : default(T);
        }

        public static object GetAttachFromObj(this object value, object key) {
            return value.GetType().GetField(value.ToString()).GetAttach(key);
        }

        public static T GetAttachFromObj<T>(this object value, object key) {
            object result = value.GetAttachFromObj(key);
            return result != null ? (T)result : default(T);
        }
    }
}
