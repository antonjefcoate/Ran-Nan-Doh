using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RanNanDoh.Domain;
using RanNanDoh.ReadModels;
using SimpleCQRS;

namespace RanNanDohUi.Models
{
    public interface IUserActionProvider
    {
        IUserActions Get(PlayerSource type);
    }

    public class UserActionProvider : IUserActionProvider
    {
        private readonly IFacebookClientProvider _facebookClientProvider;
        private readonly IPlayerReadModel _playerReadModel;
        private readonly IPlayerSession _playerSession;
        private readonly ICommandSender _bus;

        public UserActionProvider(IFacebookClientProvider facebookClientProvider, IPlayerReadModel playerReadModel, IPlayerSession playerSession, ICommandSender bus)
        {
            _facebookClientProvider = facebookClientProvider;
            _playerReadModel = playerReadModel;
            _playerSession = playerSession;
            _bus = bus;
        }

        public IUserActions Get(PlayerSource type)
        {
            switch (type)
            {
                case PlayerSource.Facebook:
                    return new FacebookUserActions(_facebookClientProvider, _playerReadModel, _playerSession, _bus);
                case PlayerSource.Native:
                    return new NativeUserActions(_playerSession, _bus, _playerReadModel);
                default:
                    throw new ArgumentOutOfRangeException("type", "Not a supported player source type.");
            }
        }
    }
}