using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using RanNanDoh.Domain.Messages;
using Facebook;
using SimpleCQRS;

namespace RanNanDoh.Domain.Services
{
    public interface IChallengeNotifier
    {
        void Notify(string opponentExternalId, string externalAccessToken, Guid roundId, string challengerName, string opponentName);
    }

    public class FacebookChallengeNotifier : IChallengeNotifier
    {
        private string _applicationId = "154100424727795";
        private string _redirectUrl = "http://localhost/rannandoh/player/FbCallback";
        private string _clientSecret = "bace8580ae3ad01edf87950bdff69248";
        private string _permissions = "read_friendlists,user_status";
        
        public void Notify(string opponentExternalId, string externalAccessToken, Guid roundId, string challengerName, string opponentName)
        {
            var client = new FacebookClient(externalAccessToken);
            dynamic parameters = new ExpandoObject();
            parameters.message = "You have been challenged in the art of 'RanNanDoh' by " + opponentName + "!";
            parameters.data = "roundId=" + roundId;

            dynamic result = client.Post(String.Format("{0}/apprequests", opponentExternalId), parameters);
            //return result.id;
        }
    }
}
