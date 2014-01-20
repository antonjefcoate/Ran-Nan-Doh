using System;
using RanNanDoh.DataViews;
using RanNanDohUi.Infrastructure.WebUtils;

namespace RanNanDohUi.Models
{
    public class PlayerSession : IPlayerSession
    {
        private readonly ISessionState<PlayerDto> _session;
        public const string SessionKey = "player";

        public PlayerSession(ISessionState<PlayerDto> session)
        {
            _session = session;
        }

        public PlayerDto Get()
        {
            return _session.Get(SessionKey);
        }

        public void Set(PlayerDto player)
        {
            _session.Store(SessionKey, player);
        }
    }
}