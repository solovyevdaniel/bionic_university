using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;
namespace LMS.HtmlHelpers
{
    public static class CKHelpers
    {
        /*
         * Добавляет определение js - скрипта на html разметку
         * (пример: @Html.AddCKSource("ckeditor/ckeditor.js")) - расположение файла со скриптом CKEditor'a
         */
        public static MvcHtmlString AddCKSource(this HtmlHelper helper, String src)
        {
            TagBuilder script = new TagBuilder("script");
            script.MergeAttribute("src", src);
            script.MergeAttribute("type", @"text/javascript");
            return new MvcHtmlString(script.ToString());
        }

        public static MvcHtmlString AddCKWithText(this HtmlHelper helper, String editorName, String text, String wrapTag = "p", String cssClass = null)
        {
            TagBuilder wrap = AddWrap(wrapTag, cssClass);

            wrap.InnerHtml += AddTextarea(editorName, text);
            wrap.InnerHtml += AddScript(editorName);

            return new MvcHtmlString(wrap.ToString());
        }

        public static MvcHtmlString AddCKWithText(this HtmlHelper helper, String editorName, String text, Int32 index, String wrapTag = "p", String cssClass = null)
        {
            TagBuilder wrap = AddWrap(wrapTag, cssClass);

            wrap.InnerHtml += AddTextarea(editorName, index, text);
            wrap.InnerHtml += AddScript(editorName + index.ToString());
            

            return new MvcHtmlString(wrap.ToString());
        }
        /*
         * Добавляет CKEditor на форму вместе с полем script и оболочкой:
         * Параметры:
         * editorName - параметр name тега textarea <textarea name="editorName"/>
         * wrapTag - тег оболочка (например, <p>) для script и textarea
         * cssClass - css class для оболочки
         */
        public static MvcHtmlString AddCKWithScript(this HtmlHelper helper, String editorName, String wrapTag = "p", String cssClass = null)
        {
            TagBuilder wrap = AddWrap(wrapTag, cssClass);

            wrap.InnerHtml += AddTextarea(editorName);
            wrap.InnerHtml += AddScript(editorName);

            return new MvcHtmlString(wrap.ToString());
        }
        //для циклов for
        public static MvcHtmlString AddCKWithScript(this HtmlHelper helper, String editorName, Int32 index, String wrapTag = "p", String cssClass = null)
        {
            TagBuilder wrap = AddWrap(wrapTag, cssClass);


            wrap.InnerHtml += AddTextarea(editorName, index);
            wrap.InnerHtml += AddScript(editorName + index.ToString());

            return new MvcHtmlString(wrap.ToString());
        }
        /*
         * Добавляет CKEditor на форму:
         * Параметры:
         * editorName - параметр name тега textarea <textarea name="editorName"/>
         * Для этого метода необходимо еще вызывать метод AddCKScript
         * @Html.AddCK(params)
         * @Html.AddCKScript(params)
         */
        public static MvcHtmlString AddCK(this HtmlHelper helper, String editorName, String wrapTag = "p", String cssClass = null)
        {
            TagBuilder wrap = AddWrap(wrapTag, cssClass);
            wrap.InnerHtml += AddTextarea(editorName);
            return new MvcHtmlString(wrap.ToString());
        }

        /*
         * Добавляет script тег на форму:
         * Параметры:
         * editorName - параметр name тега textarea <textarea name="editorName"/>
         */
        public static MvcHtmlString AddCKScript(this HtmlHelper helper, String editorName)
        {
            return new MvcHtmlString(AddScript(editorName).ToString());
        }

        /*
         * Добавляет определенное колличество CKEditor на форму вместе с полями script и оболочкой:
         * Параметры:
         * editorName - параметр name тега textarea <textarea name="editorName"/>
         * wrapTag - тег оболочка (например, <p>) для script и textarea
         * cssClass - css class для оболочки
         * startIndex - начальный индекс первого CKEditor
         * arrLength - колличество таких элементов
         * по сути что-то вроде
         * @for (Int32 i = startIndex; i <startIndex+arrLength; i++)
         * {
         *      @Html.AddCKWithScript(editorName + i, wrapTag, cssClass)
         * }
         */
        public static MvcHtmlString AddCKArray(this HtmlHelper helper, String editorName, Int32 startIndex, Int32 arrLength, String wrapTag = "p", String cssClass = null)
        {
            StringBuilder wrapInnerHtml = new StringBuilder();
            String idCKEditor = String.Empty;

            TagBuilder wrap = AddWrap(wrapTag, cssClass);
            TagBuilder textarea = new TagBuilder("textarea");
            TagBuilder scriptTag = new TagBuilder("script");

            textarea.MergeAttribute("name", editorName);
            scriptTag.MergeAttribute("type", @"text/javascript");

            for (Int32 i = startIndex; i < startIndex + arrLength; i++)
            {
                idCKEditor = editorName + i.ToString();
                textarea.MergeAttribute("id", idCKEditor, true);

                scriptTag.InnerHtml = String.Format("CKEDITOR.replace( '{0}' );", idCKEditor);

                wrapInnerHtml.Append(textarea + Environment.NewLine);
                wrapInnerHtml.Append(scriptTag + Environment.NewLine);
            }

            wrap.InnerHtml += wrapInnerHtml.ToString();

            return new MvcHtmlString(wrap.ToString());
        }
        /*
         * Скрипт типа "CKEDITOR.replace( '{0}' );" заменяет textarea с id editorName на CKEditor
         */
        private static TagBuilder AddScript(String editorName)
        {
            TagBuilder scriptTag = new TagBuilder("script");
            scriptTag.MergeAttribute("type", @"text/javascript");
            scriptTag.InnerHtml += String.Format("CKEDITOR.replace( '{0}' );", editorName);
            return scriptTag;
        }
        private static TagBuilder AddWrap(String tag, String cssClass = null)
        {
            TagBuilder builder = new TagBuilder(tag);
            if (cssClass != null)
                builder.AddCssClass(cssClass);
            return builder;
        }
        private static TagBuilder AddTextarea(String editorName)
        {
            TagBuilder textarea = new TagBuilder("textarea");
            textarea.MergeAttribute("name", editorName);
            textarea.MergeAttribute("id", editorName);
            return textarea;
        }
        private static TagBuilder AddTextarea(String editorName, String innerText = null)
        {
            TagBuilder textarea = new TagBuilder("textarea");
            textarea.MergeAttribute("name", editorName);
            textarea.MergeAttribute("id", editorName);
            if (innerText != null)
                textarea.InnerHtml += innerText;
            return textarea;
        }
        private static TagBuilder AddTextarea(String editorName, Int32 idIndex, String innerText = null)
        {
            TagBuilder textarea = new TagBuilder("textarea");
            textarea.MergeAttribute("name", editorName);
            textarea.MergeAttribute("id", editorName + idIndex.ToString());
            if (innerText != null)
                textarea.InnerHtml += innerText;
            return textarea;
        }
    }
}