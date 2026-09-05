using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Vasko.TagHelpers
{
    [HtmlTargetElement("img", Attributes = "img-action, img-controller")]
    public class ImageTagHelper : TagHelper
    {
        private readonly LinkGenerator _linkGenerator;

        public ImageTagHelper(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        public string ImgController { get; set; }
        public string ImgAction { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string path = _linkGenerator.GetPathByAction(ImgAction, ImgController);
            output.Attributes.Add("src", "/" + path);
        }
    }
}