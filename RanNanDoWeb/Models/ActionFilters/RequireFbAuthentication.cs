using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using RanNanDohUi.Infrastructure.WebUtils;
using System.Text;
using System.Web;

namespace RanNanDohUi.Models.ActionFilters
{
    using System.Web.Routing;

    public class RequireFbAuthentication : ActionFilterAttribute
    {
        private readonly IFacebookClientProvider _fbClientProvider;

        public RequireFbAuthentication()
        {
            _fbClientProvider = ServiceLocator.Current.GetInstance<IFacebookClientProvider>();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (!_fbClientProvider.IsAuthenticatedWithFb())
            {
                if (filterContext.ActionParameters.ContainsKey("request_ids"))
                {
                    var urlHelper = new UrlHelper(filterContext.RequestContext);
                    string redirectUrl = urlHelper.Action(
                        actionName: filterContext.ActionDescriptor.ActionName,
                        controllerName: filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                        routeValues: new RouteValueDictionary(filterContext.ActionParameters));

                    string url = urlHelper.Action(
                        actionName: "FacebookLogin",
                        controllerName: "Facebook",
                        routeValues:
                            new { state = HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(redirectUrl)) });

                    filterContext.Result = new RedirectResult(url);

                }
            }
        }
    }
}