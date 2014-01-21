using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RanNanDoh.Commands;
using RanNanDoh.Domain.Messages;
using SimpleCQRS;

namespace RanNanDoh.Processes
{
    class UserNotificationProcess : Handles<PlayerWasChallenged>
    {
        private readonly ICommandSender _commandSender;

        public UserNotificationProcess(ICommandSender commandSender)
        {
            _commandSender = commandSender;
        }

        public void Handle(PlayerWasChallenged message)
        {
            _commandSender.Send(new NotifyPlayer(message.Username,message.ChallengerUsername, message.RoundId, message.AuthToken, message.ExternalId));
        }
    }
}
