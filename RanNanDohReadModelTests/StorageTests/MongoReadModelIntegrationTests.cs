using System;
using System.Collections.Generic;
using NUnit.Framework;
using RanNanDoh.DataViews;
using RanNanDoh.ReadModels;

namespace RanNanDohReadModelTests.StorageTests
{
    [TestFixture]
    public class MongoReadModelIntegrationTests
    {
        private MongoUserRoundListDtoReader _reader;
        private MongoUserRoundListDtoWriter _writer;

        private const string MongoConn = "mongodb://appharbor_12608d62-4286-4a15-beba-0e7bd8c913f3:p8516rv83di6no7k8t8uuqo40n@ds037657.mongolab.com:37657/appharbor_12608d62-4286-4a15-beba-0e7bd8c913f3";

        [SetUp]
        public void Setup()
        {
            _reader = new MongoUserRoundListDtoReader(MongoConn);
            _writer = new MongoUserRoundListDtoWriter(MongoConn);
        }

        [Ignore]
        [Test]
        public void StoreAndRetrieve()
        {
            var item = new UserRoundResultListDto { Id = Guid.NewGuid(), Items = new List<UserRoundResultDto>() };
            var innerItem = new UserRoundResultDto{ Id = Guid.NewGuid() };
            item.Items.Add(innerItem);

            _writer.Add(item.Id, item);
            var result = _reader.Get(item.Id);

            Assert.NotNull(result);
            CollectionAssert.IsNotEmpty(result.Items);
        }

        //[Test]
        //public void TestColl()
        //{
        //    var database = MongoDatabase.Create(MongoConn);
        //    var collection = database.GetCollection<TestItem>(typeof(TestItem).ToString());
        //    var item = new TestItem { Id = Guid.NewGuid(), TestData = "TESTING", Items = new List<InnerItem>() };
        //    item.Items.Add(new InnerItem{Data = "null"});
        //    collection.RemoveAll();

        //    collection.Insert(item);
        //    Thread.Sleep(600);
        //    var query = Query<TestItem>.EQ(x => x.Id, item.Id);
        //    var result = collection.FindOne(query);

        //    Assert.NotNull(result);
        //    CollectionAssert.IsNotEmpty(result.Items);
        //}

        //[Test]
        //public void TestColl2()
        //{
        //    var database = MongoDatabase.Create(MongoConn);
        //    var collection = database.GetCollection<UserRoundResultListDto>(typeof(UserRoundResultListDto) + "TEST");
        //    var item = new UserRoundResultListDto { Id = Guid.NewGuid(), Items = new List<UserRoundResultDto>() };
        //    var innerItem = new UserRoundResultDto
        //    {
        //        Id = Guid.NewGuid(),
        //        OpponentName = "as",
        //        PlayerName = "df",
        //        PlayerId = Guid.NewGuid(),
        //        Result = RoundResult.Draw
        //    };
        //    item.Items.Add(innerItem);
        //    collection.RemoveAll();

        //    collection.Insert(item);
        //    Thread.Sleep(600);
        //    var result = collection.FindOne();

        //    Assert.NotNull(result);
        //    CollectionAssert.IsNotEmpty(result.Items);
        //}

        //private class TestItem
        //{
        //    [BsonId]
        //    public Guid Id;
        //    public string TestData;
        //    public List<InnerItem> Items;
        //}

        //private class InnerItem
        //{
        //    public Guid Id;
        //    public string Data;
        //}
    }
}
