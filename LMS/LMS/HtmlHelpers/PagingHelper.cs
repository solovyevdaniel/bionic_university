using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using LMS.Models;
namespace LMS.HtmlHelpers
{
    public static class PagingHelpers
    {
        public static MvcHtmlString AddPageLinks(this HtmlHelper hHelper, PagesInfo pInfo, String cssClass, Func<Int32, String> toHtml)
        {
            StringBuilder result = new StringBuilder();
            if (pInfo != null)
            {
                for (int i = 1; i <= pInfo.TotalPages; i++)
                {
                    TagBuilder tag = new TagBuilder("a");
                    tag.MergeAttribute("href", toHtml(i));
                    tag.InnerHtml = i.ToString();
                   // if (i == pInfo.CurrentPage)
                        tag.AddCssClass(cssClass);
                    result.Append(tag.ToString());
                }
                return MvcHtmlString.Create(result.ToString());
            }
            return MvcHtmlString.Empty;
        }
        public static MvcHtmlString AddCourses(this HtmlHelper hHelper, String route, params String[] cssClasses)
        {
            StringBuilder result = new StringBuilder();
            TagBuilder refA = new TagBuilder("a");
            refA.MergeAttribute("href", route);
            refA.SetInnerText("course");
            /*тут будет логика для внутренностей тега а*/
            result.Append(refA);
            return MvcHtmlString.Create(result.ToString());
        }
    }
}