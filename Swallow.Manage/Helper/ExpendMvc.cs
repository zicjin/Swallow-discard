using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Swallow.Entity;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.ViewFeatures;

namespace Swallow.Manage {
    public static class ExpendMvc {

        public static IList<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items,
            Func<T, string> getText, Func<T, string> getValue, string selectValue) {
            return items.OrderBy(i => getText(i))
            .Select(i => new SelectListItem {
                Selected = (getValue(i) == selectValue),
                Text = getText(i),
                Value = getValue(i)
            }).ToList();
        }

        public static SelectList ToSelectList<TEnum>(this TEnum enumObj) {
            var values = from TEnum e in Enum.GetValues(typeof(TEnum))
                         select new { Id = e, Name = e.ToString() };
            return new SelectList(values, "Id", "Name", enumObj);
        }

        public static IEnumerable<SelectListItem> ToSelectListItems<TEnum>(this TEnum enumObj) {
            return from TEnum e in Enum.GetValues(typeof(TEnum))
                   select new SelectListItem() {
                       Selected = (Convert.ToInt32(e) == Convert.ToInt32(enumObj)), //使用DropDownListFor的话是不需要指定Selected的
                       Text = e.ToString(),
                       Value = Convert.ToInt32(e).ToString()
                   };
        }

        public static IEnumerable<SelectListItem> ToSelectListItemsWithNull<TEnum>(this TEnum enumObj) {
            //TEnum without 0 value.
            IList<SelectListItem> select_list = enumObj.ToSelectListItems().ToList();
            select_list.Insert(0, new SelectListItem {
                Selected = (Convert.ToInt32(enumObj) == 0),
                Text = "无",
                Value = "0"
            });
            return select_list;
        }

        public static IEnumerable<SelectListItem> ToSelectListItems<TEnum>(this TEnum enumObj, object attachEnum) {
            return from TEnum e in Enum.GetValues(typeof(TEnum))
                   select new SelectListItem() {
                       Selected = (Convert.ToInt32(e) == Convert.ToInt32(enumObj)),
                       Text = e.GetAttachFromObj<string>(attachEnum),
                       Value = Convert.ToInt32(e).ToString()
                   };
        }

        public static IEnumerable<GroupedSelectListItem> ToSelectGroupListItems<TEnum>(this Type enumObj, TEnum enumGroup, object attachEnum, IEnumerable<string> selvals) {
            return from Object e in Enum.GetValues(enumObj)
                   select new GroupedSelectListItem() {
                       Text = e.GetAttachFromObj<string>(attachEnum),
                       Value = e.ToString(),
                       GroupKey = enumGroup.ToString(),
                       GroupName = enumGroup.GetAttachFromObj<string>(attachEnum),
                       Selected = selvals == null ? false : selvals.Contains(e.ToString())
                   };
        }

        public static string IsSelected(this HtmlHelper helper, string controlName, string actionName) {
            var routeDatas = helper.ViewContext.RouteData.Values;
            if (routeDatas["controller"].ToString() == controlName) {
                if (string.IsNullOrEmpty(actionName))
                    return "selected";
                else if (routeDatas["action"].ToString() == actionName)
                    return "selected";
            }
            return "";
        }
    }
}