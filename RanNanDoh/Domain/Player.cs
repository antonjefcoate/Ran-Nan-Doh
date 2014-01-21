using System;
using System.Collections.Generic;
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
        private readonly List<Guid> _ongoingOpponentsIds = new List<Guid>();

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

        private void Apply(PlayerWasChallenged e)
        {
            _ongoingOpponentsIds.Add(e.ChallengerId);
        }

        private void Apply(PlayerChallenged e)
        {
            _ongoingOpponentsIds.Add(e.OpponentId);
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
            GuardAgainstDisallowedGames(opponent);

            opponent.BeChallenged(this, round);

            ApplyChange(new PlayerChallenged(Id, opponent.Id, round.Id));
        }

        public void BeChallenged(Player challenger, Round round)
        {
            GuardAgainstDisallowedGames(challenger);

           // Notify domain of event.
            ApplyChange(new PlayerWasChallenged(Id, Username, _externalId, _authToken, round.Id, challenger.Username, challenger.Id));
        }

        private void GuardAgainstDisallowedGames(Player opponent)
        {
            if (_ongoingOpponentsIds.Contains(opponent.Id))
                throw new InvalidOperationException(string.Format("An existing game aginst {0} is in progress.", opponent));
        }

        protected string Username { get; private set; }

        public override string ToString()
        {
            return _userName;
        }
    }
}