using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Facebook;
using RanNanDohUi.Infrastructure.WebUtils;

namespace RanNanDohUi.Models
{
    public interface IFacebookClientProvider
    {
        string GetLoginUrl(string state);
        void PopulateAccessToken(string code);
        bool IsAuthenticatedWithFb();
        FacebookClient Get();
    }

    public class FacebookClientProvider : IFacebookClientProvider
    {
        private readonly ISessionState<string> _storage;

        const string StoreKey = "FacebookAccessToken";

        // ToDo: Get these from app settings etc.
        private string _applicationId = "154100424727795";
        private string _redirectUrl = "http://localhost/rannandoh/facebook/FbCallback";
        private string _clientSecret = "bace8580ae3ad01edf87950bdff69248";
        private string _permissions = "read_friendlists,user_status";

        public FacebookClientProvider(ISessionState<string> storage)
        {
            _storage = storage;
        }

        public string GetLoginUrl(string state)
        {
            var loginUrl = string.Format("https://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri={1}&scope={2}",
                _applicationId, _redirectUrl, _permissions);
            if(!string.IsNullOrWhiteSpace(state))
            {
                loginUrl = string.Format("{0}&state={1}", loginUrl, state);
            }

            return loginUrl;

            // ToDo: This is prob a better method...
            //dynamic parameters = new ExpandoObject();
            //parameters.client_id = _applicationId;
            //parameters.redirect_uri = _redirectUrl;

            //// The requested response: an access token (token), an authorization code (code), or both (code token).
            //parameters.response_type = "code token";

            //// list of additional display modes can be found at http://developers.facebook.com/docs/reference/dialogs/#display
            //parameters.display = "popup";

            //// add the 'scope' parameter only if we have extendedPermissions.
            //if (!string.IsNullOrWhiteSpace(_permissions))
            //    parameters.scope = _permissions;

            //// generate the login url
            //var fb = new FacebookClient();
            //return fb.GetLoginUrl(parameters).ToString();
        }

        public void PopulateAccessToken(string code)
        {
            if (!TokenExists())
            {
                string accessToken = null;

                var fb = new FacebookClient();
                FacebookOAuthResult oauthResult;
                if (fb.TryParseOAuthCallbackUrl(HttpContext.Current.Request.Url, out oauthResult))
                {
                    // The url is the result of OAuth 2.0 authentication
                    if (oauthResult.IsSuccess)
                    {
                        accessToken = oauthResult.AccessToken;
                    }
                    else
                    {
                        var errorDescription = oauthResult.ErrorDescription;
                        var errorReason = oauthResult.ErrorReason;
                    }
                }
                else
                {
                    // The url is NOT the result of OAuth 2.0 authentication.
                }
                if (string.IsNullOrWhiteSpace(accessToken))
                    accessToken = GetAccessToken(code);

                _storage.Store(StoreKey, accessToken);
            }
        }

        public bool IsAuthenticatedWithFb()
        {
            return this.TokenExists();
        }

        private bool TokenExists()
        {
            return _storage.Get(StoreKey) != null &&
                !string.IsNullOrWhiteSpace(_storage.Get(StoreKey).ToString());
        }

        public FacebookClient Get()
        {
            if (TokenExists())
                return new FacebookClient(_storage.Get(StoreKey).ToString());
            
            throw new InvalidOperationException("Cannot generate FacebookClient with no AccessToken.");
        }

        private string GetAccessToken(string code)
        {
            string accessToken;

            var fbAccessUrl =
                string.Format(
                    "https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}",
                    _applicationId, _redirectUrl, _clientSecret, code);

            // Send the request to exchange the code for access_token
            var accessTokenRequest = WebRequest.Create(fbAccessUrl);
            var response = (HttpWebResponse)accessTokenRequest.GetResponse();
                
            var encoding = Encoding.GetEncoding(response.CharacterSet);
            using (var sr = new StreamReader(response.GetResponseStream(), encoding))
            {
                accessToken = HttpUtility.ParseQueryString(sr.ReadToEnd()).Get("access_token");
            }

            return accessToken;
        }
    }
}