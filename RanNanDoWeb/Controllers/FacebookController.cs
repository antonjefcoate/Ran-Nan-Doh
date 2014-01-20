using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RanNanDohUi.Controllers
{
    using System.Text;

    using RanNanDoh.Domain;

    using RanNanDohUi.Models;
    using RanNanDohUi.Models.ActionFilters;

    public class FacebookController : PlayerEnabledController
    {
        private readonly IFacebookClientProvider _facebookClientProvider;
        private readonly IUserActionProvider _playerActionProvider;

        public FacebookController(IFacebookClientProvider facebookClientProvider, IUserActionProvider playerActionProvider)
        {
            _facebookClientProvider = facebookClientProvider;
            this._playerActionProvider = playerActionProvider;
        }

        public ActionResult FacebookLogin(string state)
        {
            return Redirect(_facebookClientProvider.GetLoginUrl(state));
        }

        public ActionResult FbCallBack(string code, string state)
        {
            if (Player != null)
                RedirectToAction("Index", "Player");

            var playerActions = _playerActionProvider.Get(PlayerSource.Facebook);
            if (!playerActions.Login(code))
                playerActions.Create(null);

            if(!string.IsNullOrWhiteSpace(state))
            {
                string redirectUrl = Encoding.UTF8.GetString(HttpServerUtility.UrlTokenDecode(state));
                redirectUrl = HttpUtility.UrlDecode(redirectUrl);
                this.Index(redirectUrl.Substring(redirectUrl.IndexOf("?request_ids=") + 1));
            }

            return RedirectToAction("Index","Player");
        }

        //
        // GET: /Facebook/
        [RequireFbAuthentication]
        public ActionResult Index(string request_ids)
        {
            var requestIds = request_ids.Split(new[]{','});

            return this.View(requestIds);
        }

        [HttpPost]
        public ActionResult Request(string requestId)
        {

            dynamic fbRequestId = requestId;
            var fb = _facebookClientProvider.Get();

            var fbRequest = fb.Get(fbRequestId);
            //todo:Delete fbRequest once it is processed
            var token = fbRequest.data.Split(new[] { ':' });

            return RedirectToAction("PlayAgainstFriend", "Game", new { roundId = token[0], opponentId = token[1] });
        }


        public ActionResult Friends_callback(string request, long[] to, string error_msg)
        {
            var fb = _facebookClientProvider.Get();

            var fbRequest = fb.Get(request as dynamic);
            string data = fbRequest.data;
            var token = data.Split(new[] { ':' });
            
            if (!string.IsNullOrWhiteSpace(error_msg))
                ModelState.AddModelError("FbError", error_msg);

            //return string.Format("You've successfully sent a request to {0} with request id {1}", to[0], request);
            return RedirectToAction("PlayAgainstFriend", "Game", new { roundId = token[0], opponentId = to[0].ToString() });
        }
    }
}
