using System;

namespace RanNanDohUi.Models
{
    using System.Collections.Generic;

    public interface IUserActions
    {
        bool Login(string code);
        Guid Create(string characterName);

        IEnumerable<PotentialPlayer> GetFbFriends();
    }
}