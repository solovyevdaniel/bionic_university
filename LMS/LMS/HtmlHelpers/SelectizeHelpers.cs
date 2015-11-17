using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using LMS.Models;
namespace LMS.HtmlHelpers
{
    public static class SelectizeHelpers
    {
        public static MvcHtmlString AddSelectizeSource(this HtmlHelper helper, String src)
        {
            TagBuilder script = new TagBuilder("script");
            script.MergeAttribute("src", src);
            script.MergeAttribute("type", @"text/javascript");
            return new MvcHtmlString(script.ToString());
        }
        public static MvcHtmlString AddSelectizeScript(this HtmlHelper helper, String sId)
        {
            TagBuilder script = new TagBuilder("script");
            StringBuilder code = new StringBuilder();
            code.Append("$('#" +  sId + "').selectize({")
            .Append("plugins: ['remove_button'],")
            .Append("delimiter: ',', ")
            .Append("persist: false,")
            .Append("create: false")
            .Append("});");
            script.InnerHtml += code.ToString();
            return new MvcHtmlString(script.ToString());
        }
        public static MvcHtmlString AddSelectizeScript(this HtmlHelper helper, String sId, IEnumerable<CourseTag> tags)
        {
            StringBuilder options = new StringBuilder();
            StringBuilder selectizeCode = new StringBuilder();
            foreach (var i in tags)
                options.Append("{ id: '" + i.ID.ToString() + "', title: '" + i.Tag + "' },");
            TagBuilder script = new TagBuilder("script");
            selectizeCode.Append("$('#" + sId + "').selectize({")
                .Append("plugins: ['remove_button'],")
                .Append("delimiter: ',',")
                .Append("valueField: 'id',")
                .Append("labelField: 'title',")
                .Append("persist: false,")
                .Append("options: [" + options.ToString().Remove(options.Length - 1) + "],")
                .Append(" create: false")
                .Append("});");
            script.InnerHtml += selectizeCode.ToString();
            return new MvcHtmlString(script.ToString());
        }
        public static MvcHtmlString AddSelectizeTagsTextScript(this HtmlHelper helper, String sId, IEnumerable<CourseTag> tags)
        {
            StringBuilder options = new StringBuilder();
            StringBuilder selectizeCode = new StringBuilder();
            foreach (var i in tags)
                options.Append("{ id: '" + i.Tag + "', title: '" + i.Tag + "' },");
            TagBuilder script = new TagBuilder("script");
            selectizeCode.Append("$('#" + sId + "').selectize({")
                .Append("plugins: ['remove_button'],")
                .Append("delimiter: ',',")
                .Append("valueField: 'id',")
                .Append("labelField: 'title',")
                .Append("persist: false,")
                .Append("options: [" + options.ToString().Remove(options.Length - 1) + "],")
                .Append(" create: true")
                .Append("});");
            script.InnerHtml += selectizeCode.ToString();
            return new MvcHtmlString(script.ToString());
        }
        public static MvcHtmlString AddSelectizeScript(this HtmlHelper helper, String sId, IEnumerable<ApplicationUser> users)
        {
            StringBuilder options = new StringBuilder();
            StringBuilder selectizeCode = new StringBuilder();
            foreach (var i in users)
                options.Append("{ id: '" + i.Id + "' , title: '" + i.UserName + "' },");
            TagBuilder script = new TagBuilder("script");
            selectizeCode.Append("$('#" + sId + "').selectize({")
                .Append("plugins: ['remove_button'],")
                .Append("delimiter: ',',")
                .Append("valueField: 'id',")
                .Append("labelField: 'title',")
                .Append("persist: false,")
                .Append("options: [" + options.ToString().Remove(options.Length - 1) + "],")
                .Append(" create: false")
                .Append("});");
            script.InnerHtml += selectizeCode.ToString();
            return new MvcHtmlString(script.ToString());
        }
        public static MvcHtmlString AddSelectizeScript(this HtmlHelper helper, String sId, IEnumerable<Course> courses)
        {
            StringBuilder options = new StringBuilder();
            StringBuilder selectizeCode = new StringBuilder();
            foreach (var i in courses)
                options.Append("{ id: " + i.ID + " , title: '" + i.Title + "' },");
            TagBuilder script = new TagBuilder("script");
            selectizeCode.Append("$('#" + sId + "').selectize({")
                .Append("plugins: ['remove_button'],")
                .Append("delimiter: ',',")
                .Append("valueField: 'id',")
                .Append("labelField: 'title',")
                .Append("persist: false,")
                .Append("options: [" + options.ToString().Remove(options.Length - 1) + "],")
                .Append(" create: false")
                .Append("});");
            script.InnerHtml += selectizeCode.ToString();
            return new MvcHtmlString(script.ToString());
        }
    }
}