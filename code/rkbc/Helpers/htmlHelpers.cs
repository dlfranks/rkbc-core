using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc.Rendering.Html
{
    public static class HtmlExtensions
    {
        public static void RenderPartialWithPrefix(this IHtmlHelper helper, string partialViewName, object model, string prefix, ViewDataDictionary viewData)
        {
            var tmp = viewData.TemplateInfo;
            viewData.TemplateInfo.HtmlFieldPrefix = prefix;
            helper.RenderPartialAsync(partialViewName,
                                 model,
                                 viewData);
            //viewData.TemplateInfo = tmp;
        }
    }
   
}
