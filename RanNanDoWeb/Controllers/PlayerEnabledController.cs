using System.Web.Mvc;
using RanNanDoh.DataViews;
using RanNanDohUi.Models.ActionFilters;

namespace RanNanDohUi.Controllers
{
    [PlayerPopulate]
    public abstract class PlayerEnabledController : Controller
    {
        public PlayerDto Player { get; set; }
    }
}
