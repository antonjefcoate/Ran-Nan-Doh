using System;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace RanNanDoh.ReadModels
{
    public abstract class MongoDtoAccessor<T> where T : IViewModel
    {
        private readonly string _mongoConn;
        protected readonly MongoCollection<T> Collection;

        protected MongoDtoAccessor(string mongoConn)
        {
            _mongoConn = mongoConn;
            Collection = GetDtoCollection();
        }

        private MongoCollection<T> GetDtoCollection()
        {
            var database = MongoDatabase.Create(_mongoConn);
            var collection = database.GetCollection<T>(typeof(T).ToString());

            return collection;
        }

        protected T FindOneByGuid(Guid id)
        {
            //var playerRounds = Collection.FindOneById(ObjectId.Parse(id.ToString()));
            return Collection.FindOne(Query<T>.EQ(x => x.Id, id));
        }

        protected void RemoveByGuid(Guid id)
        {
            Collection.Remove(Query<T>.EQ(x => x.Id, id));
        }
    }
}