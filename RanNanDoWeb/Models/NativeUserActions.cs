using System;
using RanNanDoh.Commands;
using RanNanDoh.ReadModels;
using SimpleCQRS;
using System.Collections.Generic;

namespace RanNanDohUi.Models
{
    using System.Linq;

    public class NativeUserActions : IUserActions
    {
        private readonly IPlayerSession _session;
        private readonly ICommandSender _bus;
        private readonly IPlayerReadModel _playerReadModel;

        public NativeUserActions(IPlayerSession session, ICommandSender bus, IPlayerReadModel playerReadModel)
        {
            _session = session;
            _bus = bus;
            _playerReadModel = playerReadModel;
        }

        public bool Login(string code)
        {
            throw new NotImplementedException();
        }

        public Guid Create(string characterName)
        {
            var playerId = Guid.NewGuid();
            _bus.Send(new CreateNativePlayer(playerId, characterName));

            // 2. Get player
            var player = _playerReadModel.GetUser(playerId);

            // 3. Save user to session
            _session.Set(player);

            return playerId;
        }

        public IEnumerable<PotentialPlayer> GetFbFriends()
        {
            //No friends yet
            return Enumerable.Empty<PotentialPlayer>();
        }

    }
}