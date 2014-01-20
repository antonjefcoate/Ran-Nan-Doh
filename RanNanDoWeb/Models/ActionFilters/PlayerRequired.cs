using System.Security.Authentication;
using System.Web.Mvc;
using RanNanDohUi.Controllers;

namespace RanNanDohUi.Models.ActionFilters
{
    using System.Web.Routing;

    public class PlayerRequired : PlayerPopulate
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var controller = filterContext.Controller as PlayerEnabledController;
            if (controller != null && controller.Player == null)
                throw new AuthenticationException("This action requires an authenticated player.");
        }
    }
}