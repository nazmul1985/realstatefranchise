using System.Collections.Generic;
using Facebook;
using Newtonsoft.Json.Linq;

namespace RealStateFranchise.Utils
{
    public class FacebookUtil
    {
        private const string AppId = "1313744445371864";
        private const string AppSecret = "314f90eeb46aa22b9810192c24ff306d";

        public string RefreshTokenAndPostToFacebook(string currentAccessToken)
        {
            var newAccessToken = this.RefreshAccessToken(currentAccessToken);
            var pageAccessToken = this.GetPageAccessToken(newAccessToken);
            this.PostToFacebook(pageAccessToken);
            return newAccessToken; // replace current access token with this
        }

        public string GetPageAccessToken(string userAccessToken)
        {
            var fbClient = new FacebookClient
            {
                AppId = AppId,
                AppSecret = AppSecret,
                AccessToken = userAccessToken
            };

            var fbParams = new Dictionary<string, object>();
            var publishedResponse = fbClient.Get("/me/accounts", fbParams) as JsonObject;
            var data = JArray.Parse(publishedResponse["data"].ToString());

            foreach (var account in data)
            {
                if (account["name"].ToString().ToLower().Equals("your page name"))
                {
                    return account["access_token"].ToString();
                }
            }

            return string.Empty;
        }

        public string RefreshAccessToken(string currentAccessToken)
        {
            var fbClient = new FacebookClient();
            var fbParams = new Dictionary<string, object>
            {
                ["client_id"] = AppId,
                ["grant_type"] = "fb_exchange_token",
                ["client_secret"] = AppSecret,
                ["fb_exchange_token"] = currentAccessToken
            };
            var publishedResponse = fbClient.Get("/oauth/access_token", fbParams) as JsonObject;
            return publishedResponse["access_token"].ToString();
        }

        public void PostToFacebook(string pageAccessToken)
        {
            var fbClient = new FacebookClient(pageAccessToken)
            {
                AppId = AppId,
                AppSecret = AppSecret
            };
            var fbParams = new Dictionary<string, object>();
            fbParams["message"] = "Test message";
            var publishedResponse = fbClient.Post("/nazmul2009/feed", fbParams);
        }
    }
}