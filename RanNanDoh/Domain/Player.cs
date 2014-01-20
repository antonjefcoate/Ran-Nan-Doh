using System;
using Microsoft.Practices.ServiceLocation;
using RanNanDoh.Domain.Messages;
using RanNanDoh.Domain.Services;
using SimpleCQRS;

namespace RanNanDoh.Domain
{
    public class Player : AggregateRoot
    {
        private Guid _id;
        private PlayerSource _source;
        private string _userName;

        private string _authToken;
        // ExternalID is a string in order to provide compatibility for future providers
        private string _externalId;

        public Player()
        { }

        public Player(Guid id, string userName, PlayerSource source, string externalId)
        {
            ApplyChange(new PlayerCreated(id, userName, source, externalId));
        }

        #region Event Handling

        private void Apply(PlayerCreated e)
        {
            _id = e.Id;
            _userName = e.UserName;
            _source = e.Source;
            _externalId = e.ExternalId;
        }

        private void Apply(AuthTokenUpdated e)
        {
            _authToken = e.AuthToken;
        }

        private void Apply(PlayerChallenged e)
        {
            // I THINK we do nothing here?
        }

        #endregion

        public override Guid Id
        {
            get { return _id; }
        }

        public void UpdateAuthToken(string authToken)
        {
            if (string.IsNullOrWhiteSpace(authToken))
                throw new ArgumentOutOfRangeException("authToken", "an empty auth token has been passed");

            ApplyChange(new AuthTokenUpdated(_id, authToken));
        }

        public void IssueChallenge(Player opponent, Round round)
        {
            opponent.BeChallenged(this.Id, round.Id, this._userName);
        }
        
        public void BeChallenged(Guid challengerId, Guid roundId, string challengerName)
        {
            var notifier = ServiceLocator.Current.GetInstance<IChallengeNotifier>();

            // Notify opponent player
            notifier.Notify(this._externalId, _authToken, roundId, challengerName, this._userName);

            // Notify domain of event.
            ApplyChange(new PlayerChallenged(challengerId, this.Id, roundId));
        }
    }
}