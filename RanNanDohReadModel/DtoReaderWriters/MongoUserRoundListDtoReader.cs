using System;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using RanNanDoh.DataViews;

namespace RanNanDoh.ReadModels
{
    public class MongoUserRoundListDtoReader : MongoDtoAccessor<UserRoundResultListDto>, IViewModelReader<UserRoundResultListDto>
    {
        public MongoUserRoundListDtoReader(string mongoConn) : base(mongoConn)
        { }

        public UserRoundResultListDto Get(Guid id)
        {
            return FindOneByGuid(id);
        }
    }
}