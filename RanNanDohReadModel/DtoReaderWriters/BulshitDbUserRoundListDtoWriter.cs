using System;
using System.Collections.Generic;
using RanNanDoh.DataViews;
using SimpleCQRS;

namespace RanNanDoh.ReadModels
{
    public class BulshitDbUserRoundListDtoWriter : IViewModelWriter<UserRoundResultListDto>
    {
        public void Update(Guid id, UserRoundResultListDto viewModel)
        {
            BullShitDatabase.UserRoundList[id] = viewModel;
        }

        public void Add(Guid id, UserRoundResultListDto viewModel)
        {
            BullShitDatabase.UserRoundList.Add(id, viewModel);
        }

        public void Drop()
        {
            BullShitDatabase.UserRoundList = new Dictionary<Guid, UserRoundResultListDto>();
        }
    }
}