using System;
using System.Linq;
using RanNanDoh.DataViews;
using SimpleCQRS;

namespace RanNanDoh.ReadModels
{
    public class BulshitDbUserRoundListDtoReader : IViewModelReader<UserRoundResultListDto>
    {
        public UserRoundResultListDto Get(Guid id)
        {
            var playerRounds = BullShitDatabase.UserRoundList.FirstOrDefault(x => x.Key == id);
            return playerRounds.Value;
        }
    }
}
