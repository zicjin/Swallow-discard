﻿using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.AspNet.Routing;
using Microsoft.Extensions.WebEncoders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Web;

namespace Swallow.Manage {
    //http://stackoverflow.com/questions/607188/support-for-optgroup-in-dropdownlist-net-mvc
    public class GroupedSelectListItem : SelectListItem {
        public string GroupKey { get; set; }
        public string GroupName { get; set; }
    }

    public static partial class HtmlHelpers {

        public static HtmlString DropDownGropList(this HtmlHelper htmlHelper, string name, bool allowMultiple) {
            return DropDownListHelper(htmlHelper, name, null, null, allowMultiple, null);
        }

        public static HtmlString DropDownGropList(this HtmlHelper htmlHelper, string name, IEnumerable<GroupedSelectListItem> selectList, bool allowMultiple) {
            return DropDownListHelper(htmlHelper, name, selectList, null, allowMultiple, null);
        }

        public static HtmlString DropDownGropList(this HtmlHelper htmlHelper, string name, string optionLabel, bool allowMultiple) {
            return DropDownListHelper(htmlHelper, name, null, optionLabel, allowMultiple, null);
        }

        public static HtmlString DropDownGropList(this HtmlHelper htmlHelper, string name, IEnumerable<GroupedSelectListItem> selectList, bool allowMultiple, IDictionary<string, object> htmlAttributes) {
            return DropDownListHelper(htmlHelper, name, selectList, null, allowMultiple, htmlAttributes);
        }

        public static HtmlString DropDownGropList(this HtmlHelper htmlHelper, string name, IEnumerable<GroupedSelectListItem> selectList, bool allowMultiple, object htmlAttributes) {
            return DropDownListHelper(htmlHelper, name, selectList, null, allowMultiple, new RouteValueDictionary(htmlAttributes));
        }

        public static HtmlString DropDownGropList(this HtmlHelper htmlHelper, string name, IEnumerable<GroupedSelectListItem> selectList, string optionLabel, bool allowMultiple) {
            return DropDownListHelper(htmlHelper, name, selectList, optionLabel, allowMultiple, null);
        }

        public static HtmlString DropDownGropList(this HtmlHelper htmlHelper, string name, IEnumerable<GroupedSelectListItem> selectList, string optionLabel, bool allowMultiple, IDictionary<string, object> htmlAttributes) {
            return DropDownListHelper(htmlHelper, name, selectList, optionLabel, allowMultiple, htmlAttributes);
        }

        public static HtmlString DropDownGropList(this HtmlHelper htmlHelper, string name, IEnumerable<GroupedSelectListItem> selectList, string optionLabel, bool allowMultiple, object htmlAttributes) {
            return DropDownListHelper(htmlHelper, name, selectList, optionLabel, allowMultiple, new RouteValueDictionary(htmlAttributes));
        }

        public static HtmlString DropDownGroupListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<GroupedSelectListItem> selectList, bool allowMultiple) {
            return DropDownGroupListFor(htmlHelper, expression, selectList, null /* optionLabel */, allowMultiple, null /* htmlAttributes */);
        }

        public static HtmlString DropDownGroupListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<GroupedSelectListItem> selectList, bool allowMultiple, object htmlAttributes) {
            return DropDownGroupListFor(htmlHelper, expression, selectList, null /* optionLabel */, allowMultiple, new RouteValueDictionary(htmlAttributes));
        }

        public static HtmlString DropDownGroupListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<GroupedSelectListItem> selectList, bool allowMultiple, IDictionary<string, object> htmlAttributes) {
            return DropDownGroupListFor(htmlHelper, expression, selectList, null /* optionLabel */, allowMultiple, htmlAttributes);
        }

        public static HtmlString DropDownGroupListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<GroupedSelectListItem> selectList, string optionLabel, bool allowMultiple) {
            return DropDownGroupListFor(htmlHelper, expression, selectList, optionLabel, allowMultiple, null /* htmlAttributes */);
        }

        public static HtmlString DropDownGroupListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<GroupedSelectListItem> selectList, string optionLabel, bool allowMultiple, object htmlAttributes) {
            return DropDownGroupListFor(htmlHelper, expression, selectList, optionLabel, allowMultiple, new RouteValueDictionary(htmlAttributes));
        }

        public static HtmlString DropDownGroupListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<GroupedSelectListItem> selectList, string optionLabel, bool allowMultiple, IDictionary<string, object> htmlAttributes) {
            if (expression == null) {
                throw new ArgumentNullException("expression");
            }

            return DropDownListHelper(htmlHelper, ExpressionHelper.GetExpressionText(expression), selectList, optionLabel, allowMultiple, htmlAttributes);
        }

        private static HtmlString DropDownListHelper(HtmlHelper htmlHelper, string expression, IEnumerable<GroupedSelectListItem> selectList, string optionLabel, bool allowMultiple, IDictionary<string, object> htmlAttributes) {
            return SelectInternal(htmlHelper, optionLabel, expression, selectList, allowMultiple, htmlAttributes);
        }


        // Helper methods

        private static IEnumerable<GroupedSelectListItem> GetSelectData(this HtmlHelper htmlHelper, string name) {
            object o = null;
            if (htmlHelper.ViewData != null) {
                o = htmlHelper.ViewData.Eval(name);
            }
            if (o == null) {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        "Missing Select Data",
                        name,
                        "IEnumerable<GroupedSelectListItem>"));
            }
            IEnumerable<GroupedSelectListItem> selectList = o as IEnumerable<GroupedSelectListItem>;
            if (selectList == null) {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        "Wrong Select DataType",
                        name,
                        o.GetType().FullName,
                        "IEnumerable<GroupedSelectListItem>"));
            }
            return selectList;
        }

        internal static string ListItemToOption(GroupedSelectListItem item) {
            //TagBuilder builder = new TagBuilder("option") {
            //    InnerHtml = WebUtility.HtmlEncode(item.Text)
            //}; // fix for asp.net5
            TagBuilder builder = new TagBuilder("option");
            var writer = new StringWriter(new StringBuilder(item.Text));
            builder.WriteTo(writer, HtmlEncoder.Default);
            if (item.Value != null) {
                builder.Attributes["value"] = item.Value;
            }
            if (item.Selected) {
                builder.Attributes["selected"] = "selected";
            }
            //return builder.ToString(TagRenderMode.Normal);
            return builder.InnerHtml.ToString();
        }

        private static HtmlString SelectInternal(this HtmlHelper htmlHelper, string optionLabel, string name, IEnumerable<GroupedSelectListItem> selectList, bool allowMultiple, IDictionary<string, object> htmlAttributes) {
            name = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (String.IsNullOrEmpty(name)) {
                throw new ArgumentException("Null Or Empty", "name");
            }

            bool usedViewData = false;

            // If we got a null selectList, try to use ViewData to get the list of items.
            if (selectList == null) {
                selectList = htmlHelper.GetSelectData(name);
                usedViewData = true;
            }

            object defaultValue = (allowMultiple) ? htmlHelper.GetModelStateValue(name, typeof(string[])) : htmlHelper.GetModelStateValue(name, typeof(string));

            // If we haven't already used ViewData to get the entire list of items then we need to
            // use the ViewData-supplied value before using the parameter-supplied value.
            if (!usedViewData) {
                if (defaultValue == null) {
                    defaultValue = htmlHelper.ViewData.Eval(name);
                }
            }

            if (defaultValue != null) {
                IEnumerable defaultValues = (allowMultiple) ? defaultValue as IEnumerable : new[] { defaultValue };
                IEnumerable<string> values = from object value in defaultValues select Convert.ToString(value, CultureInfo.CurrentCulture);
                HashSet<string> selectedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
                List<GroupedSelectListItem> newSelectList = new List<GroupedSelectListItem>();

                foreach (GroupedSelectListItem item in selectList) {
                    item.Selected = (item.Value != null) ? selectedValues.Contains(item.Value) : selectedValues.Contains(item.Text);
                    newSelectList.Add(item);
                }
                selectList = newSelectList;
            }

            // Convert each ListItem to an <option> tag
            StringBuilder listItemBuilder = new StringBuilder();

            // Make optionLabel the first item that gets rendered.
            if (optionLabel != null) {
                listItemBuilder.AppendLine(ListItemToOption(new GroupedSelectListItem() { Text = optionLabel, Value = String.Empty, Selected = false }));
            }

            foreach (var group in selectList.GroupBy(i => i.GroupKey)) {
                string groupName = selectList.Where(i => i.GroupKey == group.Key).Select(it => it.GroupName).FirstOrDefault();
                listItemBuilder.AppendLine(string.Format("<optgroup label=\"{0}\" value=\"{1}\">", groupName, group.Key));
                foreach (GroupedSelectListItem item in group) {
                    listItemBuilder.AppendLine(ListItemToOption(item));
                }
                listItemBuilder.AppendLine("</optgroup>");
            }

            //TagBuilder tagBuilder = new TagBuilder("select") {
            //    InnerHtml = listItemBuilder.ToString()
            //};

            TagBuilder tagBuilder = new TagBuilder("option");
            var writer = new StringWriter(new StringBuilder(listItemBuilder.ToString()));
            tagBuilder.WriteTo(writer, HtmlEncoder.Default);

            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("name", name, true /* replaceExisting */);
            tagBuilder.GenerateId(name, "null");
            if (allowMultiple) {
                tagBuilder.MergeAttribute("multiple", "multiple");
            }

            //// If there are any errors for a named field, we add the css attribute.
            //ModelState modelState;
            //if (htmlHelper.ViewData.ModelState.TryGetValue(name, out modelState)) {
            //    if (modelState.Errors.Count > 0) {
            //        tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
            //    }
            //}

            return new HtmlString(tagBuilder.ToString());
        }

        internal static object GetModelStateValue(this HtmlHelper helper, string key, Type destinationType) {
            //ModelState modelState;
            //if (helper.ViewData.ModelState.TryGetValue(key, out modelState)) {
            //    if (modelState.Value != null) {
            //        return modelState.Value.ConvertTo(destinationType, null /* culture */);
            //    }
            //}
            return null;
        }

    }
}