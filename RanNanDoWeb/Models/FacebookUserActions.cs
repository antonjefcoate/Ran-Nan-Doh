using System;
using RanNanDoh.Commands;
using RanNanDoh.Domain;
using RanNanDoh.ReadModels;
using SimpleCQRS;

namespace RanNanDohUi.Models
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    public class PotentialPlayer
    {
        public PotentialPlayer(string id, string name)
        {
            Id = id;
            Name = name;
        }
        public string Id { get; private set; }

        public string Name { get; private set; }

        public string PhotoHref
        {
            get
            {
                return string.Format("https://graph.facebook.com/{0}/picture", Id);
            }
        }
    }
    public class FacebookUserActions : IUserActions
    {
        private readonly IFacebookClientProvider _facebookClientProvider;
        private readonly IPlayerReadModel _playerReadModel;
        private readonly IPlayerSession _playerSession;
        private readonly ICommandSender _bus;

        private int _friendFetchedCount;

        private const int PageCount = 100;

        public FacebookUserActions(IFacebookClientProvider facebookClientProvider, IPlayerReadModel playerReadModel,
                                   IPlayerSession playerSession, ICommandSender bus)
        {
            _facebookClientProvider = facebookClientProvider;
            _playerReadModel = playerReadModel;
            _playerSession = playerSession;
            _bus = bus;
        }


        public bool Login(string code)
        {
            _facebookClientProvider.PopulateAccessToken(code);
            var player = _playerReadModel.GetFacebookPlayer(GetFbUid());

            if (player != null)
            {
                _playerSession.Set(player);
                return true;
            }
            return false;
        }

        public Guid Create(string characterName)
        {
            var playerId = Guid.NewGuid();
            var client = _facebookClientProvider.Get();
            dynamic user = client.Get("/me");

            // 1. Send command to create fb user
            _bus.Send(new CreateFacebookPlayer(playerId, characterName ?? user.name, PlayerSource.Facebook, GetFbUid(), client.AccessToken));

            // 2. Get player
            var player = _playerReadModel.GetUser(playerId);

            // 3. Save user to session
            _playerSession.Set(player);

            return playerId;
        }

        public IEnumerable<PotentialPlayer> GetFbFriends()
        {

            dynamic client = _facebookClientProvider.Get();

            var friends = client.Get("/me/friends");
            foreach (var friend in friends.data)
            {
                if(_friendFetchedCount > PageCount) 
                    yield break;
                string name = friend.name;
                if (name.StartsWith("P", true, CultureInfo.InvariantCulture) || name.StartsWith("d", true, CultureInfo.InvariantCulture))
                {
                    _friendFetchedCount++;
                    yield return new PotentialPlayer(friend.id, friend.name);
                }
            }
        }
        public IEnumerable<PotentialPlayer> GetRegisteredFbFriends()
        {
            dynamic client = _facebookClientProvider.Get();

            var friends = client.Get("/me/friends");
            
            foreach (var friend in friends.data)
            {
                if (_friendFetchedCount > PageCount)
                    yield break;
                string name = friend.name;
                if (_playerReadModel.GetFacebookPlayer(long.Parse(friend.id)) != null)
                {
                    _friendFetchedCount++;
                    yield return new PotentialPlayer(friend.id, friend.name);

                }
            }

        }

        private long GetFbUid()
        {
            var client = _facebookClientProvider.Get();
            dynamic user = client.Get("/me");

            long fbUid = 0;
            long.TryParse(user.id, out fbUid);

            return fbUid;
        }
    }
}