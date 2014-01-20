using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RanNanDoh.ReadModels;

using RanNanDohReadModel.Replay;

namespace RanNanDohUi.Controllers
{
    public class AdminController : Controller
    {
        private readonly TypeScanner<IViewModel> _viewModelScanner;

        private readonly ViewModelRebuilder _vmBuilder;

        public AdminController(TypeScanner<IViewModel> viewModelScanner, ViewModelRebuilder vmBuilder)
        {
            _viewModelScanner = viewModelScanner;
            _vmBuilder = vmBuilder;
        }

        //
        // GET: /Admin/

        public ActionResult ListViewModels()
        {
            var model = _viewModelScanner.Scan().Select(t => t.FullName);
            return View(model.ToList());
        }

        [HttpPost]
        public ActionResult RebuildViewModel(string viewModel)
        {
            _vmBuilder.Rebuild(viewModel);
            return new EmptyResult();
        }

    }
    
}
