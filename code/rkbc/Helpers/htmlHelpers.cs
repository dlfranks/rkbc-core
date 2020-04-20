using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rkbc.web.helpers
{
    public enum DataMessageTypes
    {
        Standard = 1,
        Error
    }
}
namespace Microsoft.AspNetCore.Mvc.Rendering
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

        public static IHtmlContent jsTempDataMessage<TModel>(this IHtmlHelper<TModel> htmlHelper, bool isMobile)
        {
            object msg = null, type = null, duration = null;

            if (htmlHelper.ViewContext.TempData.TryGetValue("message", out msg))
            {
                htmlHelper.ViewContext.TempData.TryGetValue("messageType", out type);
                htmlHelper.ViewContext.TempData.TryGetValue("duration", out duration);

                if (type == null)
                    type = rkbc.web.helpers.DataMessageTypes.Standard;

                if (isMobile)
                {
                    return (new HtmlString("$.mobileGrowl('" + msg.ToString() + "');"));
                }
                else
                {
                    if ((rkbc.web.helpers.DataMessageTypes)type == rkbc.web.helpers.DataMessageTypes.Error)
                        return (new HtmlString("$.jGrowl('" + msg.ToString() + "', { theme: 'jgrowl-error-notification', sticky: true, header: 'Error' });"));
                    else
                    {
                        if (!String.IsNullOrEmpty((string)duration))
                        {
                            if ((string)duration == "sticky")
                            {
                                return (new HtmlString("$.jGrowl('" + msg.ToString() + "', { sticky: true });"));
                            }
                            else
                            {
                                return (new HtmlString("$.jGrowl('" + msg.ToString() + "', { life: " + duration + " });"));
                            }
                        }
                        else
                        {
                            return (new HtmlString("$.jGrowl('" + msg.ToString() + "');"));
                        }
                    }
                }

            }
            return (null);
        }
    }
   
}
