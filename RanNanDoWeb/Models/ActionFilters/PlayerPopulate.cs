using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using RanNanDohUi.Controllers;

namespace RanNanDohUi.Models.ActionFilters
{
    public class PlayerPopulate : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.Controller as PlayerEnabledController;
            if (controller != null)
                controller.Player = ServiceLocator.Current.GetInstance<IPlayerSession>().Get();

            base.OnActionExecuting(filterContext);
        }
    }
}