using System;
using System.Collections.Generic;
using RanNanDoh.DataViews;
using SimpleCQRS;

namespace RanNanDoh.ReadModels
{
    public class BulshitDbPlayerDtoWriter : IViewModelWriter<PlayerDto>
    {
        public void Update(Guid id, PlayerDto viewModel)
        {
            BullShitDatabase.Users[id] = viewModel;
        }

        public void Add(Guid id, PlayerDto viewModel)
        {
            BullShitDatabase.Users.Add(id, viewModel);
        }

        public void Drop()
        {
            BullShitDatabase.Users = new Dictionary<Guid, PlayerDto>();
        }
    }
}