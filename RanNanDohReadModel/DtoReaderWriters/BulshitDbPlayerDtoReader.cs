using System;
using RanNanDoh.DataViews;
using SimpleCQRS;

namespace RanNanDoh.ReadModels
{
    public class BulshitDbPlayerDtoReader : IViewModelReader<PlayerDto>
    {
        public PlayerDto Get(Guid id)
        {
            return BullShitDatabase.Users[id];
        }

    }
}