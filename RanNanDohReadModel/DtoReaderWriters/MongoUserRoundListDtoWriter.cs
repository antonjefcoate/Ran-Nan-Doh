using System;
using MongoDB.Driver.Builders;
using RanNanDoh.DataViews;

namespace RanNanDoh.ReadModels
{
    public class MongoUserRoundListDtoWriter : MongoDtoAccessor<UserRoundResultListDto>, IViewModelWriter<UserRoundResultListDto>
    {
        public MongoUserRoundListDtoWriter(string mongoConn) : base(mongoConn)
        { }

        public void Update(Guid id, UserRoundResultListDto viewModel)
        {
            RemoveByGuid(viewModel.Id);
            Collection.Insert(viewModel);
        }

        public void Add(Guid id, UserRoundResultListDto viewModel)
        {
            Collection.Insert(viewModel);
        }

        public void Drop()
        {
            Collection.Drop();
        }
        
    }
}