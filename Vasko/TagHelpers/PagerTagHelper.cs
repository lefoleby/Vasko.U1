using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Encodings.Web;

namespace Vasko.TagHelpers
{
    public class PagerTagHelper : TagHelper
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PagerTagHelper(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Номер текущей страницы
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Общее количество страниц
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Имя категории объектов
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Имя действия (action)
        /// </summary>
        public string? Action { get; set; }

        /// <summary>
        /// Имя контроллера
        /// </summary>
        public string? Controller { get; set; }

        /// <summary>
        /// Признак страниц администратора
        /// </summary>
        public bool Admin { get; set; } = false;

        // Номер предыдущей страницы
        private int Prev => CurrentPage == 1 ? 1 : CurrentPage - 1;

        // Номер следующей страницы
        private int Next => CurrentPage == TotalPages ? TotalPages : CurrentPage + 1;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Если всего одна страница - не выводим пейджер
            if (TotalPages <= 1)
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "div";
            output.AddClass("row", HtmlEncoder.Default);

            var nav = new TagBuilder("nav");
            nav.Attributes.Add("aria-label", "Page navigation");

            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");
            ul.AddCssClass("justify-content-center");

            // Кнопка "Предыдущая"
            ul.InnerHtml.AppendHtml(CreateListItem(Category, Prev, "<span aria-hidden=\"true\">&laquo;</span>"));

            // Кнопки страниц
            for (int i = 1; i <= TotalPages; i++)
            {
                ul.InnerHtml.AppendHtml(CreateListItem(Category, i, null));
            }

            // Кнопка "Следующая"
            ul.InnerHtml.AppendHtml(CreateListItem(Category, Next, "<span aria-hidden=\"true\">&raquo;</span>"));

            nav.InnerHtml.AppendHtml(ul);
            output.Content.AppendHtml(nav);
        }

        /// <summary>
        /// Создание одной кнопки пейджера
        /// </summary>
        private TagBuilder CreateListItem(string? category, int pageNo, string? innerText)
        {
            var li = new TagBuilder("li");
            li.AddCssClass("page-item");

            bool isDisabled = false;

            if (string.IsNullOrEmpty(innerText))
            {
                if (pageNo == CurrentPage)
                {
                    li.AddCssClass("active");
                }
            }
            else
            {
                if ((innerText.Contains("laquo") && CurrentPage == 1) ||
                    (innerText.Contains("raquo") && CurrentPage == TotalPages))
                {
                    li.AddCssClass("disabled");
                    isDisabled = true;
                }
            }

            var a = new TagBuilder("a");
            a.AddCssClass("page-link");

            string url;
            var routeData = new { pageNo = pageNo, category = category };

            if (Admin)
            {
                url = _linkGenerator.GetPathByPage(
                    _httpContextAccessor.HttpContext,
                    page: "./Index",
                    values: routeData) ?? "#";
            }
            else
            {
                url = _linkGenerator.GetPathByAction(
                    Action ?? "Index",
                    Controller ?? "Product",
                    routeData) ?? "#";
            }

            if (isDisabled)
            {
                a.Attributes.Add("href", "#");
                a.Attributes.Add("tabindex", "-1");
                a.Attributes.Add("aria-disabled", "true");
            }
            else
            {
                a.Attributes.Add("href", url);
            }

            var text = string.IsNullOrEmpty(innerText) ? pageNo.ToString() : innerText;
            a.InnerHtml.AppendHtml(text);

            li.InnerHtml.AppendHtml(a);
            return li;
        }
    }
}